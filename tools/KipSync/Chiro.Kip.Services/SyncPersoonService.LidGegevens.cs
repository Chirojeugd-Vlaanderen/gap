using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using AutoMapper;
using Chiro.Cdf.Data.Entity;
using Chiro.Kip.Data;
using System.Linq;
using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.Kip.Workers;
using Persoon = Chiro.Kip.ServiceContracts.DataContracts.Persoon;

namespace Chiro.Kip.Services
{
	public partial class SyncPersoonService
	{

		/// <summary>
		/// Maakt een persoon met gekend ad-nummer lid, of updatet een bestaand lid
		/// </summary>
		/// <param name="adNummer">AD-nummer van de persoon</param>
		/// <param name="gedoe">De nodige info om de persoon lid te kunnen maken</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LidBewaren(
			int adNummer,
			LidGedoe gedoe)
		{
			string feedback = String.Empty;
			ChiroGroep groep;

			using (var db = new kipadminEntities())
			{
				// Vind de groep, zodat we met groepID kunnen werken ipv stamnummer.

				groep = (from g in db.Groep.OfType<ChiroGroep>()
					 where g.STAMNR == gedoe.StamNummer
					 select g).FirstOrDefault();

				// Bestaat het lid al?
				// De moeilijkheid is dat bij het begin van het nieuwe werkjaar standaard
				// alle leden van het vorige werkjaar in kipadmin zitten, met 'aansl_nr' = 0.
				// Negeer dus die records.  Op het moment dat het eerste lid van het nieuwe
				// werkjaar wordt overgezet, verdwijnen de leden met aansl_jr = 0

				Lid lid = (from l in db.Lid.Include(ld => ld.HeeftFunctie.First().Functie)
					   where

						l.AANSL_NR > 0 &&
						l.Persoon.AdNummer == adNummer &&
						l.Groep.GroepID == groep.GroepID &&
						l.werkjaar == gedoe.WerkJaar
					   select l).FirstOrDefault();

				// Aan het hoeveelste jaar van dit lid zijn we?
				// Jaren worden geteld als 'aantal jaar als kind', 
				// 'aantal jaar als leider' en 'aantal jaar als kadermedewerker'

				string soort;

				if (gedoe.LidType == LidTypeEnum.Kind) soort = "LI";
				else if (gedoe.LidType == LidTypeEnum.Leiding) soort = "LE";
				else if (gedoe.LidType == LidTypeEnum.Kader) soort = "KA";
				else throw new NotSupportedException("Ongeldig lidtype");

				int aantalJaren = (from l in db.Lid
						   where l.AANSL_NR > 0 &&
							 l.Persoon.AdNummer == adNummer &&
							 l.werkjaar < gedoe.WerkJaar &&
							 l.SOORT.ToUpper() == soort
						   select l.werkjaar).Distinct().Count() + 1;

				if (lid != null)
				{
					// TODO (#817): Updaten van lid mogelijk maken!

					// In de huidige implementatie van GAP hebben we dit niet nodig.
					// Dus NotImplemented throwen was een goede oplossing.  Maar we
					// komen hier ook terecht als er per ongeluk iemand 2 keer lid 
					// gemaakt wordt van dezelfde groep.  (Wat dan weer mogelijk is als
					// een persoon dubbel in GAP zit.)

					// Vandaar dat er hier niets gebeurt, en er enkel een fout gelogd
					// wordt.

					_log.FoutLoggen(groep.GroepID, String.Format(
						"Dubbel toevoegen van lid genegeerd.  Ad-nr {0}",
						adNummer));
					// throw new NotImplementedException();
				}
				else
				{
					// Nieuw lid.

					// Zoek persoon op.

					var persoon = (from p in db.PersoonSet
						       where p.AdNummer == adNummer
						       select p).FirstOrDefault();

					if (persoon == null)
					{
						_log.FoutLoggen(
							groep == null ? 0 : groep.GroepID,
							String.Format(
								"genegeerd: Lid met onbekend AD-nr. {0}",
								adNummer));
						return;
					}


					// Zoek eerst naar een geschikte aansluiting om het lid aan
					// toe te voegen.

					var aansluitingenDitWerkjaar = (from a in db.Aansluiting.Include(asl => asl.REKENING)
									where a.WerkJaar == gedoe.WerkJaar
									      && a.Groep.GroepID == groep.GroepID
									select a).OrderByDescending(aa => aa.VolgNummer);

					// Laatste aansluiting opzoeken

					var aansluiting = aansluitingenDitWerkjaar.FirstOrDefault();

					if (aansluiting == null)
					{
						// Eerste lid voor dit werkjaar.  Verwijder alle huidige
						// leden, die er nog in zitten als kopietje van vorig jaar.

						var teVerwijderenLeden = (from l in db.Lid.Include(ld => ld.HeeftFunctie)
									  where l.Groep.GroepID == groep.GroepID
										&& l.werkjaar == gedoe.WerkJaar
									  select l).ToList();

						var teVerwijderenFuncties = teVerwijderenLeden.SelectMany(ld => ld.HeeftFunctie).ToList();

						foreach (var hf in teVerwijderenFuncties)
						{
							db.DeleteObject(hf);
						}

						foreach (var l in teVerwijderenLeden)
						{
							db.DeleteObject(l);
						}

						// Om zodadelijk geen conflicten te krijgen, gaan we dat
						// al eens bewaren.

						db.SaveChanges();
					}

                    // Er moet (voor plaatselijke groepen) een nieuwe aansluiting gemaakt worden als:
                    //    - er nog geen aansluiting was
                    //    - (of) de vorige aansluiting gefactureerd was, met doorgeboekte factuur
                    //    - (of) de vorige aansluiting te oud was.
                    //
                    // In de andere gevallen wordt het nieuwe lid toegevoegd aan de recentste aansluiting.

					// (Voor kaderploegen
					// is er geen factuur gekoppeld aan een aansluiting.  Daar gaat
					// dus alles op aansluiting 1.)

					if (aansluiting == null ||      // er was nog geen aansluiting
                        aansluiting.Datum.AddDays(Properties.Settings.Default.ToevoegTermijnAansluiting) < DateTime.Now ||  // vorige aansluiting te oud
                        (aansluiting.REKENING != null && aansluiting.REKENING.DOORGEBOE != "N" ))   // vorige aansluiting heeft doorgeboekte factuur
					{
						// Creeer nieuwe aansluiting, en meteen ook een rekening.
						// Die rekening mag nog leeg zijn; kipadmin berekent de
						// bedragen bij het overzetten van de factuur.

						int volgNummer = (aansluiting == null ? 1 : aansluiting.VolgNummer + 1);

						Rekening rekening = null;

						if (!groep.IsGewestVerbond)
						{
							// Factuur enkel maken als het geen gewest/verbond
							// is.

							rekening = new Rekening
							{
								WERKJAAR = (short)gedoe.WerkJaar,
								TYPE = "F",
								REK_BRON = "AANSLUIT",
								STAMNR = gedoe.StamNummer,
								VERWIJSNR = volgNummer,
								FACTUUR = "N",
								FACTUUR2 = "N",
								DOORGEBOE = "N",
								DAT_REK = DateTime.Now
							};
							db.AddToRekeningSet(rekening);
						}

						aansluiting = new Aansluiting
						{
							RibbelsJ = 0,
							RibbelsM = 0,
							SpeelClubJ = 0,
							SpeelClubM = 0,
							RakwisJ = 0,
							RakwisM = 0,
							TitosJ = 0,
							TitosM = 0,
							KetisJ = 0,
							KetisM = 0,
							AspisJ = 0,
							AspisM = 0,
							LeidingJ = 0,
							LeidingM = 0,
							Proost = 0,
							Vb = 0,
							Freelance = 0,
							AansluitingID = 0,
							Groep = groep,
							Noot = null,
							REKENING = rekening,
							SolidariteitsBijdrage = 0,
							VolgNummer = volgNummer,
							Datum = DateTime.Now,
							WerkJaar = gedoe.WerkJaar
						};

						db.AddToAansluiting(aansluiting);
					}

					// aansluiting bevat nu het aansluitingsrecord waaraan het lid
					// toegevoegd kan worden.

					// aansluitingsdatum is datum aansluiting eerste lid dat binnen komt.
					// (De groep kan er niet aan doen dat er niet constant gefactureerd wordt)

					aansluiting.Stempel = DateTime.Now;
					aansluiting.Wijze = "G";

                    if (aansluiting.REKENING != null)
                    {
                        // Rekening mag pas overgezet worden (en bedrag pas berekend) nadat
                        // de instapperiode van dit lid voorbij is.
                        aansluiting.REKENING.FacturerenVanaf = gedoe.EindeInstapPeriode;
                    }

					lid = new Lid
					{
						AANSL_NR = (short)aansluiting.VolgNummer,
						AANTAL_JA = (short)aantalJaren,
						ACTIEF = "J",
						AFDELING1 = null,
						AFDELING2 = null,
						Groep = groep,
						HeeftFunctie = null,
						MAILING_TOEVOEG = null,
						Persoon = persoon,
						SOORT = groep.IsGewestVerbond ? "KA" : (gedoe.LidType == LidTypeEnum.Kind ? "LI" : "LE"),
						STATUS = null,
						STEMPEL = DateTime.Now,
						VERZ_NR = 0,
						WEB_TOEVOEG = null,
						werkjaar = gedoe.WerkJaar,
                        EindeInstapPeriode = gedoe.EindeInstapPeriode
					};

					// 2 afdelingen kunnen we overnemen.

					if (gedoe.OfficieleAfdelingen.Count() >= 1)
					{
						int afdid = (int)gedoe.OfficieleAfdelingen.First();
						lid.AFDELING1 = (from a in db.AfdelingSet
								 where a.AFD_ID == afdid
								 select a.AFD_NAAM).FirstOrDefault();
					}

					if (gedoe.OfficieleAfdelingen.Count() >= 2)
					{
						int afdid = (int)gedoe.OfficieleAfdelingen.Skip(1).First();
						lid.AFDELING2 = (from a in db.AfdelingSet
								 where a.AFD_ID == afdid
								 select a.AFD_NAAM).FirstOrDefault();
					}

					// Functies

					var toeTeKennen =
						db.FunctieSet.Where(Utility.BuildContainsExpression<Functie, int>(
							f => f.id,
							gedoe.NationaleFuncties.Cast<int>()));

					foreach (var functie in toeTeKennen)
					{
						var hf = new HeeftFunctie
						{
							Lid = lid,
							Functie = functie
						};
						db.AddToHeeftFunctieSet(hf);
					}

					// In de aansluitingslijn zit ook telkens een telling: het aantal leden wordt geteld per afdeling en geslacht,
                    // het aantal leiding enkel per geslacht, met uitzondering van proost en VB.  Vroeger gebeurde die telling
                    // in KipSync.  Maar ik ga nu Kipadmin aanpassen, zodat die telling gebeurt net voordat de factuur wordt
                    // gemaakt.

					// Lid toevoegen aan datacontext, en bewaren.

					db.AddToLid(lid);

					db.SaveChanges();

					feedback = String.Format("Persoon met AD-nr. {0} ingeschreven als lid voor {1} in {2}", adNummer,
								 gedoe.StamNummer, gedoe.WerkJaar);
				}
			}
			_log.BerichtLoggen(groep == null ? 0 : groep.GroepID, feedback);
		}

	    /// <summary>
		/// Maakt een persoon zonder ad-nummer lid.  Dit is een dure operatie, omdat er gezocht zal 
		/// worden of de persoon al bestaat.  Zeker de eerste keer op 16 oktober, gaat dit zwaar 
		/// zijn.  Vanaf volgend jaar, zal het merendeel van de leden al een ad-nummer hebben.
		/// </summary>
		/// <param name="details">Details van de persoon die lid moet kunnen worden</param>
		/// <param name="lidGedoe">nodige info om lid te kunnen maken</param>
		/// <remarks>We gaan sowieso op zoek naar een bestaande persoon</remarks>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void NieuwLidBewaren(
			PersoonDetails details,
			LidGedoe lidGedoe)
		{
			// Als het AD-nummer al gekend is, moet (gewoon) 'LidBewaren' gebruikt worden.
			Debug.Assert(details.Persoon.AdNummer == null);

			if (String.IsNullOrEmpty(details.Persoon.VoorNaam))
			{
				_log.FoutLoggen(0, String.Format(
					"Lid zonder voornaam genegeerd; persoonID {0}",
					details.Persoon.ID));
				return;
			}
			int adnr = UpdatenOfMaken(details);

			LidBewaren(adnr, lidGedoe);
		}

		/// <summary>
		/// Updatet de functies van een lid.
		/// </summary>
		/// <param name="persoon">Persoon waarvan de lidfuncties geupdatet moeten worden</param>
		/// <param name="stamNummer">Stamnummer van de groep waarin de persoon lid is</param>
		/// <param name="werkJaar">Werkjaar waarin de persoon lid is</param>
		/// <param name="functies">Toe te kennen functies.  Eventuele andere reeds toegekende functies worden 
		/// verwijderd.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void FunctiesUpdaten(
			Persoon pers,
			string stamNummer,
			int werkJaar,
			IEnumerable<FunctieEnum> functies)
		{
			var feedback = new StringBuilder();

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
				Lid lid;

				// Eens kijken of we het lid waarvan sprake kunnen vinden.

				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(pers);

				// locking gebeurt niet helemaal juist.  Maar uiteindelijk ga ik toch geen
				// meerdere threads gebruiken.

				lid = mgr.LidZoeken(zoekInfo, stamNummer, werkJaar, db);

				if (lid == null)
				{
					_log.FoutLoggen(0, String.Format(
						Properties.Resources.LidNietGevonden,
						pers.VoorNaam,
						pers.Naam,
						stamNummer,
						werkJaar));
					return;
				}
				// pragmatisch: eerst bestaande functies verwijderen.

				var teVerwijderen = lid.HeeftFunctie.ToList();

				foreach (var hf in teVerwijderen)
				{
					db.DeleteObject(hf);
				}
				db.SaveChanges();
				feedback.AppendLine(String.Format(
					"Functies verwijderd van ID{0} {1} {2} AD{3}",
					lid.Persoon.GapID,
					lid.Persoon.VoorNaam,
					lid.Persoon.Naam,
					lid.Persoon.AdNummer));


				var toeTeKennen = db.FunctieSet.Where(Utility.BuildContainsExpression<Functie, int>(
					f => f.id,
					functies.Cast<int>()));

				foreach (var functie in toeTeKennen)
				{
					var hf = new HeeftFunctie
					{
						Lid = lid,
						Functie = functie
					};
					db.AddToHeeftFunctieSet(hf);
					feedback.AppendLine(String.Format(
						"Functie toegekend aan ID{0} {1} {2} AD{3}: {4}",
						lid.Persoon.GapID,
						lid.Persoon.VoorNaam,
						lid.Persoon.Naam,
						lid.Persoon.AdNummer,
						functie.CODE));

					// Als functie fin. ver. is, pas dan ook betaler in groepsrecord
					// aan.

					if (functie.id == (int)FunctieEnum.FinancieelVerantwoordelijke)
					{
						lid.GroepReference.Load();

						// FIXME (#555): oud-leidingsploegen! 

						var cg = lid.Groep as ChiroGroep;

						if (cg != null)
						{
							// OH NEE, dat is geen foreign key :-(

							cg.BET_ADNR = lid.Persoon.AdNummer;
							cg.STEMPEL = DateTime.Now;
						}

						feedback.AppendLine("'BET_ADNR' bijgewerkt");
					}
				}
				db.SaveChanges();
			}
			_log.BerichtLoggen(0, feedback.ToString());
		}

		/// <summary>
		/// Stelt het lidtype van het lid in bepaald door <paramref name="persoon"/>, <paramref name="stamNummer"/>
		/// en <paramref name="werkJaar"/>.
		/// </summary>
		/// <param name="persoon">Persoon waarvan het lidtype aangepast moet worden</param>
		/// <param name="stamNummer">Stamnummer van groep waarin de persoon lid is</param>
		/// <param name="werkJaar">Werkjaar waarvoor het lidtype moet aangepast worden</param>
		/// <param name="lidType">nieuw lidtype</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LidTypeUpdaten(Persoon persoon, string stamNummer, int werkJaar, LidTypeEnum lidType)
		{
			string feedback = String.Empty;

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
				Lid lid;

				// Eens kijken of we het lid waarvan sprake kunnen vinden.

				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(persoon);

				// locking gebeurt niet helemaal juist.  Maar uiteindelijk ga ik toch geen
				// meerdere threads gebruiken.

				lid = mgr.LidZoeken(zoekInfo, stamNummer, werkJaar, db);

				if (lid == null)
				{
					throw new InvalidOperationException(String.Format(
						Properties.Resources.LidNietGevonden,
						persoon.VoorNaam,
						persoon.Naam,
						stamNummer,
						werkJaar));
				}

				lid.SOORT = lidType == LidTypeEnum.Kind ? "LI" : "LE";

				// Niet helemaal juist, want in Kipadmin behouden we afelingen en functies, waar
				// die in GAP verdwijnen.  #toobad

				feedback = String.Format(
					"Lidtype veranderd naar {0}: ID{1} {2} {3} AD{4}",
					lid.SOORT,
					persoon.ID,
					persoon.VoorNaam,
					persoon.Naam,
					lid.Persoon.AdNummer);

				db.SaveChanges();
			}
			_log.BerichtLoggen(0, feedback);
		}

		/// <summary>
		/// Updatet de afdelingen van een lid.
		/// </summary>
		/// <param name="pers">Persoon waarvan de afdelingen geupdatet moeten worden</param>
		/// <param name="stamNummer">Stamnummer van de groep waarin de persoon lid is</param>
		/// <param name="werkJaar">Werkjaar waarin de persoon lid is</param>
		/// <param name="afdelingen">Toe te kennen afdelingen.  Eventuele andere reeds toegekende functies worden verwijderd.</param>
		/// <remarks>
		/// Er is in Kipadmin maar plaats voor 2 afdelingen/lid
		/// <para/>
		/// In theorie moet hier het aansluitingsrecord ook aangepast worden.  Maar voorlopig laten
		/// we dat maar even zo.  (Als de aantallen al kloppen, is het voor mij ook al goed ;))
		/// </remarks>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AfdelingenUpdaten(Persoon pers, string stamNummer, int werkJaar, IEnumerable<AfdelingEnum> afdelingen)
		{
			var feedback = new StringBuilder();

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
				Lid lid;

				// Eens kijken of we het lid waarvan sprake kunnen vinden.

				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(pers);

				lid = mgr.LidZoeken(zoekInfo, stamNummer, werkJaar, db);

				if (lid == null)
				{
					_log.FoutLoggen(0, String.Format(
					    Properties.Resources.LidNietGevonden,
					    pers.VoorNaam,
					    pers.Naam,
					    stamNummer,
					    werkJaar));
					return;
				}

				if (afdelingen.Count() >= 1)
				{
					int afdid = (int)afdelingen.First();
					lid.AFDELING1 = (from a in db.AfdelingSet
							 where a.AFD_ID == afdid
							 select a.AFD_NAAM).FirstOrDefault();
				}
				else
				{
					lid.AFDELING1 = null;
				}

				if (afdelingen.Count() >= 2)
				{
					int afdid = (int)afdelingen.Skip(1).First();
					lid.AFDELING2 = (from a in db.AfdelingSet
							 where a.AFD_ID == afdid
							 select a.AFD_NAAM).FirstOrDefault();
				}
				else
				{
					lid.AFDELING2 = null;
				}

				db.SaveChanges();
				feedback.AppendLine(String.Format(
					"Afdelingen van ID{0} {1} {2} AD{3}: {4} {5}",
					lid.Persoon.GapID,
					lid.Persoon.VoorNaam,
					lid.Persoon.Naam,
					lid.Persoon.AdNummer, lid.AFDELING1, lid.AFDELING2));
			}
			_log.BerichtLoggen(0, feedback.ToString());
		}

        /// <summary>
        /// Verwijdert een persoon met gekend AD-nummer als lid
        /// </summary>
        /// <param name="adNummer">AD-nummer te verwijderen lid</param>
        /// <param name="stamNummer">Stamnummer te verwijderen lid</param>
        /// <param name="werkjaar">Werkjaar te verwijderen lid</param>
        /// <remarks>Lid wordt hoe dan ook verwijderd.  De check op probeerperiode gebeurt
        /// in GAP.</remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void LidVerwijderen(int adNummer, string stamNummer, int werkjaar)
        {
            using (var db = new kipadminEntities())
            {
                var lid = (from l in db.Lid.Include(ld => ld.HeeftFunctie)
                           where
                               l.Persoon.AdNummer == adNummer && (l.Groep is ChiroGroep) &&
                               String.Compare((l.Groep as ChiroGroep).STAMNR, stamNummer, true) == 0 &&
                               l.werkjaar == werkjaar
                           select l).FirstOrDefault();

                if (lid == null)
                {
                    _log.FoutLoggen(0, String.Format(
                        "{2} - Te verwijderen lid niet gevonden. AD{0} wj{1}", 
                        adNummer, 
                        werkjaar, 
                        stamNummer));
                }
                else
                {
                    foreach (var f in lid.HeeftFunctie)
                    {
                        db.DeleteObject(f);
                    }
                    db.DeleteObject(lid);
                    db.SaveChanges();

                    _log.BerichtLoggen(0, String.Format(
                        "{2} - Lid verwijderd. AD{0} wj{1}",
                        adNummer,
                        werkjaar,
                        stamNummer));

                }
            }
        }

        /// <summary>
        /// Verwijdert een lid als het ad-nummer om een of andere reden niet bekend is.
        /// </summary>
        /// <param name="details">Gegevens die hopelijk toelaten het lid te identificeren</param>
        /// <param name="stamNummer">Stamnummer van het lid</param>
        /// <param name="werkjaar">Werkjaar van het lid</param>
        /// <remarks>Lid wordt hoe dan ook verwijderd.  De check op probeerperiode gebeurt
        /// in GAP.</remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void NieuwLidVerwijderen(PersoonDetails details, string stamNummer, int werkjaar)
        {
            // Als het AD-nummer al gekend is, moet (gewoon) 'LidBewaren' gebruikt worden.
            Debug.Assert(details.Persoon.AdNummer == null);

            if (String.IsNullOrEmpty(details.Persoon.VoorNaam))
            {
                _log.FoutLoggen(0, String.Format(
                    "Te verwijderen lid zonder voornaam genegeerd; persoonID {0}",
                    details.Persoon.ID));
                return;
            }
            int adnr = UpdatenOfMaken(details);

            LidVerwijderen(adnr, stamNummer, werkjaar);
        }

	}
}
