using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Transactions;
using AutoMapper;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.KipUpdate;
using Chiro.Kip.Data;
using Chiro.Kip.Services.DataContracts;
using System.Linq;

using Adres = Chiro.Kip.Services.DataContracts.Adres;
using Persoon = Chiro.Kip.Services.DataContracts.Persoon;
using KipPersoon = Chiro.Kip.Data.Persoon;
using KipAdres = Chiro.Kip.Data.Adres;

namespace Chiro.Kip.Services
{
	/// <summary>
	/// Klasse die persoons- en lidgegevens overzet van GAP naar Kipadmin.
	/// TODO: Deze klasse is veel te groot.
	/// </summary>
	public class SyncPersoonService : ISyncPersoonService
	{
		private static readonly Object _lidMakenToken = new object();
		private static readonly Object _adresManipulerenToken = new object();
		private static readonly Object _nieuwePersoonToken = new object();
		private static readonly Object _communicatieToken = new object();

		private readonly IPersoonUpdater _persoonUpdater;

		/// <summary>
		/// Standaardconstructor
		/// </summary>
		public SyncPersoonService()
		{
			// TODO (#736): Inversion of control
			_persoonUpdater = new PersoonUpdater();
		}

		/// <summary>
		/// Updatet een persoon in Kipadmin op basis van de gegevens in.  Als er geen AD-nummer is, dan doen we
		/// een schamele poging om de persoon al te vinden.   Als ook dat niet lukt, maken we een nieuwe persoon aan,
		/// en wordt het AD-nummer van <paramref name="persoon"/> ingevuld.
		/// </summary>
		/// <param name="persoon">Informatie over een geupdatete persoon in GAP</param>
		/// <remarks>Als AD-nummer ontbreekt, wordt er sowieso een nieuwe persoon gemaakt.</remarks>
		public void PersoonUpdaten(Persoon persoon)
		{
			Mapper.CreateMap<Persoon, KipPersoon>()
			    .ForMember(dst => dst.AdNummer, opt => opt.Ignore())
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var dc = new kipadminEntities())
			{
				if (persoon.AdNummer.HasValue)
				{
					var q = (from p in dc.PersoonSet
						 where p.AdNummer == persoon.AdNummer.Value
						 select p).FirstOrDefault();
					if (q != null)
					{
						Mapper.Map<Persoon, KipPersoon>(persoon, q);
						q.Stempel = DateTime.Now;
						dc.SaveChanges();

						Console.WriteLine("You saved: ID{0} {1} {2} AD{3}", persoon.ID, persoon.VoorNaam, persoon.Naam, persoon.AdNummer);
					}
					else
					{
						// AdNr niet terug gevonden
						// TODO: Handle niet teruggevonden AdNr
						Debug.Assert(false);
					}

				}
				else
				{
					// new persoon, geen AdNr
					var q = new KipPersoon();
					Mapper.Map<Persoon, KipPersoon>(persoon, q);

					q.Stempel = DateTime.Now;
					q.BurgerlijkeStandId = 6;

					dc.AddToPersoonSet(q);
					dc.SaveChanges();

					persoon.AdNummer = q.AdNummer;

					Console.WriteLine("You saved: GapID{0} {1} {2} AD{3}", persoon.ID, q.VoorNaam, q.Naam, q.AdNummer);

					_persoonUpdater.AdNummerZetten(persoon.ID, q.AdNummer);

				}
			}
		}

		/// <summary>
		/// Aan te roepen als een voorkeursadres gewijzigd moet worden.
		/// </summary>
		/// <param name="adres">Nieuw voorkeursadres</param>
		/// <param name="bewoners">AD-nummers en adrestypes voor personen de dat adres moeten krijgen</param>
		public void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners)
		{
			// 1 keer tegelijk, anders krijgen we concurrencyproblemen als er 2 personen tegelijk hetzelfde
			// adres krijgen.

			lock (_adresManipulerenToken)
			{
				// We gebruiken een transactie, want hier gebeurt nogal wat.

#if KIPDORP
				using (var tx = new TransactionScope())
				{
#endif
					using (var dc = new kipadminEntities())
					{
						var adNummers = from b in bewoners select b.AdNummer;

						// Vind personen met gegeven adnummers.

						var personen = from p in dc.PersoonSet.Include("kipWoont.kipAdres")
						               	.Where(Utility.BuildContainsExpression<Chiro.Kip.Data.Persoon, int>(prs => prs.AdNummer, adNummers))
						               select p;

						// Vind of maak adres

						// Als dat linq to sql is, dan gebeurt het zoeken sowieso hoofdletterongevoelig.

						string huisNr = adres.HuisNr.ToString();
						string postNr = adres.PostNr.ToString();

						var adresInDb = (from adr in dc.AdresSet.Include("kipWoont.kipPersoon").Include("kipWoont.kipAdresType")
						                 where adr.Straat == adres.Straat
						                       && adr.Nr == huisNr
						                       && adr.PostNr == postNr
						                       && adr.Gemeente == adres.WoonPlaats
						                 select adr).FirstOrDefault();

						if (adresInDb == null)
						{
							adresInDb = new Chiro.Kip.Data.Adres
							            	{
							            		ID = 0,
							            		Straat = adres.Straat,
							            		Nr = adres.HuisNr == null ? null : adres.HuisNr.ToString(),
							            		PostNr = adres.PostNr.ToString(),
							            		Gemeente = adres.WoonPlaats
							            		// TODO (#238) Buitenlandse adressen.
							            	};
							dc.AddToAdresSet(adresInDb);
						}

						// Bewaar hier de changes al eens, zodat het nieuwe adres een ID krijgt.

						dc.SaveChanges();

						// We zitten met het gedoe dat in Kipadmin de adressen een volgnummer hebben.  De voorkeurs-
						// adressen moeten bewaard worden met volgnummer 1.
						//
						// We gaan dat pragmatisch oplossen :-)
						//  - verwijder van alle personen het adres met volgnummer 1
						//  - als er nog personen zijn die al aan het doeladres gekoppeld zijn, dan moeten die
						//    adressen volgnummer 1 krijgen
						//  - personen die het adres nog niet hebben moeten het krijgen met volgnummer 1

						var eersteAdressen = personen.SelectMany(prs => prs.kipWoont).Where(kw => kw.VolgNr == 1);

						foreach (var wnt in eersteAdressen.ToArray())
						{
							dc.DeleteObject(wnt);
						}

						// Oude objecten met volgnr 1 al verwijderen, om duplicates (adnr,volgnr) in woont
						// te vermijden.

						dc.SaveChanges();

						// Dat ID hebben we nu hier nodig:

						var goeieAdressen = personen.SelectMany(prs => prs.kipWoont).Where(kw => kw.kipAdres.ID == adresInDb.ID);

						// TODO: Het adrestype bepalen is iedere keer een linq-expressie, en dus iedere
						// keer een loop.  Kan dat niet efficienter?

						foreach (var wnt in goeieAdressen)
						{
							wnt.VolgNr = 1;

							int adresTypeID = (int) (from b in bewoners
							                         where b.AdNummer == wnt.kipPersoon.AdNummer
							                         select b.AdresType).FirstOrDefault();

							wnt.kipAdresType = (from at in dc.AdresTypeSet
							                    where at.ID == adresTypeID
							                    select at).FirstOrDefault();

							Console.WriteLine("Update voorkeuradres: AD{0}", wnt.kipPersoon.AdNummer);
						}

						var overigePersonen = from p in personen
						                      where !goeieAdressen.Any(wnt => wnt.kipPersoon.AdNummer == p.AdNummer)
						                      select p;

						foreach (var p in overigePersonen)
						{
							int adresTypeID = (int) (from b in bewoners
							                         where b.AdNummer == p.AdNummer
							                         select b.AdresType).FirstOrDefault();

							var adresType = (from at in dc.AdresTypeSet
							                 where at.ID == adresTypeID
							                 select at).FirstOrDefault();

							dc.AddToWoontSet(new Woont
							                 	{
							                 		kipAdres = adresInDb,
							                 		kipPersoon = p,
							                 		VolgNr = 1,
							                 		kipAdresType = adresType,
							                 		Geldig = true
							                 	});
							Console.WriteLine("Update voorkeuradres: AD{0}", p.AdNummer);
						}

						// fingers crossed:

						dc.SaveChanges();
					}
#if KIPDORP
					tx.Complete();
				}
#endif
			}
		}


		/// <summary>
		/// Verwijdert alle bestaande contactinfo, en vervangt door de contactinfo meegegeven in 
		/// <paramref name="communicatieMiddelen"/>.
		/// </summary>
		/// <param name="adNr">AD-nummer van persoon waarvoor contactinfo toe te voegen</param>
		/// <param name="communicatieMiddelen">te bewaren contactinfo</param>
		public void CommunicatieBewaren(
			int adNr,
			IEnumerable<CommunicatieMiddel> communicatieMiddelen)
		{

			lock (_communicatieToken)
			{
#if KIPDORP
				using (var tx = new TransactionScope())
				{
#endif
					using (var db = new kipadminEntities())
					{
						// Haal eerst persoon op met alle communicatie-info gekend in Kipadmin.

						var persoon = (from p in db.PersoonSet.Include(p => p.kipContactInfo)
						               where p.AdNummer == adNr
						               select p).FirstOrDefault();

						// Verwijder gewoon alle bestaande communicatie

						var teVerwijderen = (from cv in persoon.kipContactInfo
						                     select cv).ToArray();

						foreach (var cv in teVerwijderen)
						{
							db.DeleteObject(cv);
						}

						// Bewaar tussentijds om key voiolations te vermijden
						db.SaveChanges();

						// Voeg nu de meegeleverde communicatie opnieuw toe.

						var nieuweComm = (from cm in communicatieMiddelen
						                  select new ContactInfo
						                         	{
						                         		ContactInfoId = 0,
						                         		ContactTypeId = (int) cm.Type,
						                         		GeenMailings = cm.GeenMailings,
						                         		Info = cm.Waarde,
						                         		kipPersoon = persoon
						                         	}).OrderBy(nc => nc.ContactTypeId).ToList();

						// Nummeren per type.

						int teller = 0;
						int vorigType = -1;

						foreach (var comm in nieuweComm)
						{
							if (comm.ContactTypeId != vorigType)
							{
								teller = 0;
								vorigType = comm.ContactTypeId;
							}
							comm.VolgNr = ++teller;
							db.AddToContactInfoSet(comm);
						}

						db.SaveChanges();
					}
#if KIPDORP
					tx.Complete();
				}
#endif
			}
		}

		/// <summary>
		/// Verwijdert een communicatiemiddel uit Kipadmin.
		/// </summary>
		/// <param name="adNr">AD-nummer van persoon die communicatiemiddel moet verliezen</param>
		/// <param name="communicatie">Gegevens over het te verwijderen communicatiemiddel</param>
		public void CommunicatieVerwijderen(int adNr, CommunicatieMiddel communicatie)
		{
			using (var db = new kipadminEntities())
			{
				// We gaan ervan uit dat er geen dubbele communicatievormen in de database
				// zitten; we verwijderen enkel de FirstOrDefault.

				int communicatieTypeID = (int)communicatie.Type;

				var teVerwijderen = (from ci in db.ContactInfoSet
						     where ci.kipPersoon.AdNummer == adNr
									   && ci.ContactTypeId == communicatieTypeID
									   && ci.Info == communicatie.Waarde
						     select ci).FirstOrDefault();

				if (teVerwijderen != null)
				{
					Console.WriteLine("Verwijderen communicatie: AD{0} {1}", adNr, communicatie.Waarde);
					db.DeleteObject(teVerwijderen);
					db.SaveChanges();
				}
			}
		}

		/// <summary>
		/// Maakt een persoon met gekend ad-nummer lid, of updatet een bestaand lid
		/// </summary>
		/// <param name="adNummer">AD-nummer van de persoon</param>
		/// <param name="gedoe">De nodige info om de persoon lid te kunnen maken</param>
		public void LidBewaren(
			int adNummer,
			LidGedoe gedoe)
		{
			lock (_lidMakenToken)
			{
				// Aangezien 1 'savechanges' van entity framework ook een transaction is, moet ik geen
				// distributed transaction opzetten.  Misschien... En de lock zorgt ervoor dat dit stuk
				// code maar 1 keer tegelijk loopt.

				// TODO: Het gelockte stuk is aan de grote kant; waarschijnlijk volstaat het om een aantal
				// kleinere stukken code te locken.


				using (var db = new kipadminEntities())
				{
					// Vind de groep, zodat we met groepID kunnen werken ipv stamnummer.

					Groep groep = (from g in db.Groep.OfType<ChiroGroep>()
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

					int aantalJaren = (from l in db.Lid
							   where l.AANSL_NR > 0 &&
					 l.Persoon.AdNummer == adNummer &&
								 l.Groep.GroepID == groep.GroepID &&
								 l.werkjaar < gedoe.WerkJaar
							   select l.werkjaar).Distinct().Count() + 1;

					if (lid != null)
					{
						// In praktijk zullen we nooit een bestaand lid met functies en afdelingen
						// tegelijk updateten.  Dus het is niet erg dat dit stuk nog niet geimplementeerd
						// is :-)

						throw new NotImplementedException();
					}
					else
					{
						int volgNummer = 1;

						// Nieuw lid.

						// Zoek persoon op.

						var persoon = (from p in db.PersoonSet
							       where p.AdNummer == adNummer
							       select p).FirstOrDefault();

						// Zoek eerst naar een geschikte aansluiting om het lid aan
						// toe te voegen.

						// TODO: Als we kader gaan aansluiten, dan zijn er geen rekeningen
						// gekoppeld aan de aansluiting!  Dan loopt het dus anders.

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

						// Als de laatste aansluiting nog niet doorgeboekt is, dan gaat het
						// nieuwe lid gewoon bij die laatste aansluiting.

						// Is er nog geen laatste aansluiting, of was de laatste wel
						// doorgeboekt, dan maken we er een nieuwe.

						if (aansluiting == null || aansluiting.REKENING.DOORGEBOE != "N")
						{
							// Creeer nieuwe aansluiting, en meteen ook een rekening.
							// Die rekening mag nog leeg zijn; kipadmin berekent de
							// bedragen bij het overzetten van de factuur.

							volgNummer = (aansluiting == null ? 1 : aansluiting.VolgNummer + 1);

							var rekening = new Rekening
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
										WerkJaar = gedoe.WerkJaar
									};

							db.AddToRekeningSet(rekening);
							db.AddToAansluiting(aansluiting);

						}

						// aansluiting bevat nu het aansluitingsrecord waaraan het lid
						// toegevoegd kan worden.

						aansluiting.Datum = DateTime.Now; // aansluitingsdatum is datum recentste lid.
						aansluiting.Stempel = DateTime.Now;
						aansluiting.Wijze = "G";

						lid = new Lid
							{
								AANSL_NR = (short)volgNummer,
								AANTAL_JA = (short)aantalJaren,
								ACTIEF = "J",
								AFDELING1 = null,
								AFDELING2 = null,
								Groep = groep,
								HeeftFunctie = null,
								MAILING_TOEVOEG = null,
								Persoon = persoon,
								SOORT = gedoe.LidType == LidTypeEnum.Kind ? "LI" : "LE",
								STATUS = null,
								STEMPEL = DateTime.Now,
								VERZ_NR = 0,
								WEB_TOEVOEG = null,
								werkjaar = gedoe.WerkJaar
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

						// Domme telling in aansluitingslijn

						if (gedoe.LidType == LidTypeEnum.Kind && persoon.Geslacht == (int)GeslachtsEnum.Man)
						{
							switch (gedoe.OfficieleAfdelingen.First())
							{
								case AfdelingEnum.Ribbels:
									++aansluiting.RibbelsJ;
									break;
								case AfdelingEnum.Speelclub:
									++aansluiting.SpeelClubJ;
									break;
								case AfdelingEnum.Rakwis:
									++aansluiting.RakwisJ;
									break;
								case AfdelingEnum.Titos:
									++aansluiting.TitosJ;
									break;
								case AfdelingEnum.Ketis:
									++aansluiting.KetisJ;
									break;
								case AfdelingEnum.Aspis:
									++aansluiting.AspisJ;
									break;
								default:
									break;
							}

						}
						else if (gedoe.LidType == LidTypeEnum.Kind && persoon.Geslacht == (int)GeslachtsEnum.Vrouw)
						{
							switch (gedoe.OfficieleAfdelingen.First())
							{
								case AfdelingEnum.Ribbels:
									++aansluiting.RibbelsM;
									break;
								case AfdelingEnum.Speelclub:
									++aansluiting.SpeelClubM;
									break;
								case AfdelingEnum.Rakwis:
									++aansluiting.RakwisM;
									break;
								case AfdelingEnum.Titos:
									++aansluiting.TitosM;
									break;
								case AfdelingEnum.Ketis:
									++aansluiting.KetisM;
									break;
								case AfdelingEnum.Aspis:
									++aansluiting.AspisM;
									break;
								default:
									break;
							}

						}
						else if (gedoe.LidType == LidTypeEnum.Leiding)
						{
							if (gedoe.NationaleFuncties.Contains(FunctieEnum.Vb)) ++aansluiting.Vb;
							else if (gedoe.NationaleFuncties.Contains(FunctieEnum.Proost)) ++aansluiting.Proost;
							else if (persoon.Geslacht == (int)GeslachtsEnum.Man) ++aansluiting.LeidingJ;
							else if (persoon.Geslacht == (int)GeslachtsEnum.Vrouw) ++aansluiting.LeidingM;
						}

						// Lid toevoegen aan datacontext, en bewaren.

						db.AddToLid(lid);

						db.SaveChanges();

						Console.WriteLine(String.Format("Persoon met AD-nr. {0} ingeschreven als lid voor {1} in {2}", adNummer,
										gedoe.StamNummer, gedoe.WerkJaar));
					}

				}

			}
		}

		/// <summary>
		/// Maakt een persoon zonder ad-nummer lid.  Dit is een dure operatie, omdat er gezocht zal worden of de persoon
		/// al bestaat.  Zeker de eerste keer op 16 oktober, gaat dit zwaar zijn.  Vanaf volgend jaar, zal het merendeel
		/// van de leden al een ad-nummer hebben.
		/// </summary>
		/// <param name="persoon">Persoonsgegevens van de lid te maken persoon</param>
		/// <param name="adres">Voorkeursadres voor de persoon</param>
		/// <param name="adresType">Adrestype van dat voorkeursadres</param>
		/// <param name="communicatieMiddelen">Lijst met communicatiemiddelen van de persoon</param>
		/// <param name="lidGedoe">nodige info om lid te kunnen maken</param>
		/// <remarks>We gaan sowieso op zoek naar een bestaande persoon</remarks>
		public void NieuwLidBewaren(
			Persoon persoon,
			Adres adres,
			AdresTypeEnum adresType,
			IEnumerable<CommunicatieMiddel> communicatieMiddelen,
			LidGedoe lidGedoe)
		{
			// Als het AD-nummer al gekend is, moet (gewoon) 'LidBewaren' gebruikt worden.

			Debug.Assert(persoon.AdNummer == null);

			Chiro.Kip.Data.Persoon poging; // poging om persoon al te vinden in database.

			// maar 1 keer tegelijk een nieuwe persoon aanmaken, omdat er gezocht wordt
			// naar een bestaande.  Wat lastig is op het moment dat er een andere aangemaakt
			// wordt...

			lock (_nieuwePersoonToken)
			{
				using (var db = new kipadminEntities())
				{
					// Probeer eerst de gevraagde persoon te vinden.

					int geslacht = (int) persoon.Geslacht;

					var naamgenoten = (from p in db.PersoonSet
					                   	.Include(prs => prs.kipContactInfo)
					                   	.Include(prs => prs.kipWoont.First().kipAdres)
					                   where p.Naam == persoon.Naam && p.VoorNaam == persoon.VoorNaam
					                         && p.Geslacht == geslacht
					                   select p);

					// Als we er eentje vinden met een overeenkomstige contactinfo, dan nemen we gewoon die.
					// (Na!)

					var alleCommunicatie = from cm in communicatieMiddelen
					                       select cm.Waarde;

					poging = naamgenoten.SelectMany(ng => ng.kipContactInfo)
						.Where(Utility.BuildContainsExpression<ContactInfo, string>(ci => ci.Info, alleCommunicatie))
						.Select(ci => ci.kipPersoon).FirstOrDefault();

					if (poging == null)
					{
						// Niet gevonden.  Probeer nog eens op geboortedatum en postnummer

						string postNummerString = adres == null ? String.Empty: adres.PostNr.ToString();

						poging = (from ng in naamgenoten
						          where ng.GeboorteDatum == persoon.GeboorteDatum
						                && ng.kipWoont.Any(wnt => wnt.kipAdres.PostNr == postNummerString)
						          select ng).FirstOrDefault();
					}
				}
			}

			// Als er iemand gevonden is: AD-nummer overnemen

			if (poging != null)
			{
				persoon.AdNummer = poging.AdNummer;
			}

			// En nu komt het.

			PersoonUpdaten(persoon); // updatet persoon en stuurt AD-nummer naar GAP

			// Als er nog geen AD-nummer was, dan heeft PersoonUpdaten er nu eentje gemaakt.

			Debug.Assert(persoon.AdNummer != null);

			LidBewaren((int) persoon.AdNummer, lidGedoe);

			if (adres != null)
			{
				StandaardAdresBewaren(
					adres,
					new Bewoner[] {new Bewoner {AdNummer = (int) persoon.AdNummer, AdresType = adresType}});
			}

			CommunicatieBewaren((int) persoon.AdNummer, communicatieMiddelen);
			Console.WriteLine("Nieuw lid bewaard: ID{0} AD{1}", persoon.ID, persoon.AdNummer);
		}
		
	}
}
