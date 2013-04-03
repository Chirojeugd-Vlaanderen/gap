/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
using System.Collections.Generic;
using System.Data;
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
                var ledenMgr = new LedenManager();

                // Vind de groep, zodat we met groepID kunnen werken ipv stamnummer.

                groep = (from g in db.Groep.OfType<ChiroGroep>()
                         where g.STAMNR == gedoe.StamNummer
                         select g).FirstOrDefault();

                if (groep == null)
                {
                    _log.FoutLoggen(0, String.Format("Inschrijving voor onbestaande groep {0}", gedoe.StamNummer));
                    return;
                }

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

                if (lid != null)
                {
                    _log.BerichtLoggen(groep.GroepID, String.Format("Bewaren van bestaand lid, met AD-nr {0}", adNummer));
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
                            groep.GroepID,
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

                    if (aansluiting == null || // er was nog geen aansluiting
                        IsTeOud(aansluiting, gedoe) ||
                        // vorige aansluiting te oud
                        (aansluiting.REKENING != null && aansluiting.REKENING.DOORGEBOE != "N"))
                        // vorige aansluiting heeft doorgeboekte factuur
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
                                               WERKJAAR = (short) gedoe.WerkJaar,
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

                    // Lid maken is eigenlijk niet meer zo veel werk, als alle administratie
                    // van die aansluitingen achter de rug is.

                    lid = new Lid
                              {
                                  AANSL_NR = (short) aansluiting.VolgNummer,
                                  AANTAL_JA = (short) aantalJaren,
                                  ACTIEF = "J",
                                  AFDELING1 = null,
                                  AFDELING2 = null,
                                  Groep = groep,
                                  HeeftFunctie = null,
                                  MAILING_TOEVOEG = null,
                                  Persoon = persoon,
                                  STATUS = null,
                                  STEMPEL = DateTime.Now,
                                  VERZ_NR = 0,
                                  WEB_TOEVOEG = null,
                                  werkjaar = gedoe.WerkJaar
                              };
                    db.AddToLid(lid);
                }

                lid.EindeInstapPeriode = gedoe.EindeInstapPeriode;

                // LidType, Afdelingen, Functies

                ledenMgr.LidTypeInstellen(lid, gedoe.LidType);
                ledenMgr.AfdelingenZetten(lid, gedoe.OfficieleAfdelingen.ToArray(), db);
                ledenMgr.FunctiesVervangen(lid, gedoe.NationaleFuncties.ToArray(), db);

                // In de aansluitingslijn zit ook telkens een telling: het aantal leden wordt geteld per afdeling en geslacht,
                // het aantal leiding enkel per geslacht, met uitzondering van proost en VB.  Vroeger gebeurde die telling
                // in KipSync.  Maar ik ga nu Kipadmin aanpassen, zodat die telling gebeurt net voordat de factuur wordt
                // gemaakt.

                try
                {
                    db.SaveChanges();
                }
                catch (UpdateException)
                {
                    _log.FoutLoggen(groep.GroepID,
                                    String.Format(Properties.Resources.LidMetAansl0, adNummer, gedoe.StamNummer,
                                                  gedoe.WerkJaar));
                }


                feedback = String.Format("Persoon met AD-nr. {0} ingeschreven als lid voor {1} in {2}", adNummer,
                                         gedoe.StamNummer, gedoe.WerkJaar);
            }
		    _log.BerichtLoggen(groep == null ? 0 : groep.GroepID, feedback);
		}

        /// <summary>
        /// Levert <c>true</c> op als de gegeven <paramref name="aansluiting"/> te oud is om het lid bepaald door
        /// <paramref name="gedoe"/> er aan toe te voegen.
        /// </summary>
        /// <param name="aansluiting">Te controleren aansluiting</param>
        /// <param name="gedoe">Kandidaatlid om toe te voegen aan <paramref name="aansluiting"/></param>
        /// <returns><c>true</c> als de gegeven <paramref name="aansluiting"/> te oud is om het lid bepaald door
        /// <paramref name="gedoe"/> er aan toe te voegen.</returns>
        /// <remarks>Als de probeerperiode van het lid niet later is dan 15/10 van het huidige werkjaar, is de
        /// aansluiting nooit te oud.</remarks>
	    private static bool IsTeOud(Aansluiting aansluiting, LidGedoe gedoe)
        {
            var eindePpJaarOvergang = new DateTime(gedoe.WerkJaar,
                                                        Properties.Settings.Default.EindeProbeerperiodeJaarOvergang.
                                                            Month,
                                                        Properties.Settings.Default.EindeProbeerperiodeJaarOvergang.Day);
            return aansluiting.Datum.AddDays(Properties.Settings.Default.ToevoegTermijnAansluiting) < DateTime.Now &&
                   gedoe.EindeInstapPeriode > eindePpJaarOvergang;
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
		    string feedback;

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
			    var ledenMgr = new LedenManager();
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
				feedback = ledenMgr.FunctiesVervangen(lid, functies.ToArray(), db);
			    db.SaveChanges();
			}
			_log.BerichtLoggen(0, feedback);
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
			    var ledenMgr = new LedenManager();
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

			    ledenMgr.LidTypeInstellen(lid,lidType);

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
			    var ledenMgr = new LedenManager();
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

				ledenMgr.AfdelingenZetten(lid, afdelingen.ToArray(), db);

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
	    /// <param name="uitschrijfDatum">uitschrijfdatum zoals geregistreerd in GAP</param>
	    /// <remarks>Lid wordt hoe dan ook verwijderd.  De check op probeerperiode gebeurt
	    /// in GAP.</remarks>
	    [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void LidVerwijderen(int adNummer, string stamNummer, int werkjaar, DateTime uitschrijfDatum)
        {
            using (var db = new kipadminEntities())
            {
                var lid = (from l in db.Lid.Include(ld => ld.HeeftFunctie).Include(ld => ld.Groep)
                           where
                               l.Persoon.AdNummer == adNummer && (l.Groep is ChiroGroep) &&
                               String.Compare((l.Groep as ChiroGroep).STAMNR, stamNummer, StringComparison.OrdinalIgnoreCase) == 0 &&
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
                    int? aansluitNr = lid.AANSL_NR;
                    int groepID = lid.Groep.GroepID;

                    // verwijder functies van lid
                    foreach (var f in lid.HeeftFunctie.ToArray())
                    {
                        db.DeleteObject(f);
                    }

                    // verwijder lid zelf & bewaar
                    db.DeleteObject(lid);
                    db.SaveChanges();

                    // als er geen andere leden meer zijn met hetzelfde 'aansluitnr', verwijder dan
                    // die aansluiting

                    var allesOp = (from l in db.Lid
                                   where l.AANSL_NR == aansluitNr && l.Groep.GroepID == groepID && l.werkjaar == werkjaar
                                   select l).FirstOrDefault() == null;

                    if (allesOp)
                    {
                        var aansluiting = (from a in db.Aansluiting
                                           where
                                               a.WerkJaar == werkjaar && a.Groep.GroepID == groepID &&
                                               a.VolgNummer == aansluitNr
                                           select a).FirstOrDefault();
                        db.DeleteObject(aansluiting);
                        db.SaveChanges();

                        _log.BerichtLoggen(0, String.Format(
                            "{2} - Aansluiting verwijderd. Nr {0} wj {1} uitschrijfdatum {3}",
                            aansluitNr,
                            werkjaar,
                            stamNummer, uitschrijfDatum));
                    }

                    _log.BerichtLoggen(0, String.Format(
                        "{2} - Lid verwijderd. AD{0} wj{1} uitschrijfdatum {3}",
                        adNummer,
                        werkjaar,
                        stamNummer, uitschrijfDatum));
                }
            }
        }

	    /// <summary>
	    /// Verwijdert een lid als het ad-nummer om een of andere reden niet bekend is.
	    /// </summary>
	    /// <param name="details">Gegevens die hopelijk toelaten het lid te identificeren</param>
	    /// <param name="stamNummer">Stamnummer van het lid</param>
	    /// <param name="werkjaar">Werkjaar van het lid</param>
	    /// <param name="uitschrijfDatum">Uitschrijfdatum zoals geregistreerd in GAP </param>
	    /// <remarks>Lid wordt hoe dan ook verwijderd.  De check op probeerperiode gebeurt
	    /// in GAP.</remarks>
	    [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void NieuwLidVerwijderen(PersoonDetails details, string stamNummer, int werkjaar, DateTime uitschrijfDatum)
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

            LidVerwijderen(adnr, stamNummer, werkjaar, uitschrijfDatum);
        }
	}
}
