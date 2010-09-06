using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using AutoMapper;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.KipUpdate;
using Chiro.Kip.Data;
using Chiro.Kip.Services.DataContracts;
using System.Linq;
using Chiro.Kip.Workers;
using Adres = Chiro.Kip.Services.DataContracts.Adres;
using Persoon = Chiro.Kip.Services.DataContracts.Persoon;
using KipPersoon = Chiro.Kip.Data.Persoon;
using KipAdres = Chiro.Kip.Data.Adres;

namespace Chiro.Kip.Services
{
	/// <summary>
	/// Klasse die persoons- en lidgegevens overzet van GAP naar Kipadmin.
	/// 
	/// BELANGRIJK: Oorspronkelijk werden voor de meeste methods geen personen over de lijn gestuurd, maar enkel
	/// AD-nummers.  Het idee daarachter was dat toch enkel gegevens van personen met AD-nummer naar kipadmin
	/// gesynct moeten worden.
	/// 
	/// Maar met het AD-nummer alleen kom je er niet.  Het kan namelijk goed zijn dat een persoon gewijzigd wordt
	/// tussen het moment dat hij voor het eerst lid wordt, en het moment dat hij zijn AD-nummer krijgt.  Deze
	/// wijzigingen willen we niet verliezen.
	/// 
	/// Het PersoonID van GAP meesturen helpt in de meeste gevallen.  Maar dat kan mis gaan op het moment dat een persoon
	/// uit kipadmin nog dubbel in GAP zit.  Vooraleer deze persoon zijn AD-nummer krijgt, weten we dat immers niet.
	/// 
	/// Vandaar dat nu alle methods volledige persoonsobjecten gebruiken, zodat het opzoeken van een persoon zo optimaal
	/// mogelijk kan gebeuren.  Het persoonsobject een AD-nummer heeft, wordt er niet naar de rest gekeken.
	/// 
	/// TODO: Deze klasse is veel te groot.
	/// TODO: De mapping van Persoon naar PersoonZoekInfo zou beter ergens op 1 plaats gedefinieerd worden, ipv in 
	/// elke method apart.
	/// </summary>
	public class SyncPersoonService : ISyncPersoonService
	{
		private static readonly Object _ledenToken = new object();
		private static readonly Object _adresManipulerenToken = new object();
		private static readonly Object _persoonManipulerenToken = new object();
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
		/// Constructor voor IOC (TODO #736)
		/// </summary>
		/// <param name="persoonUpdater">updater voor gap</param>
		public SyncPersoonService(IPersoonUpdater persoonUpdater)
		{
			_persoonUpdater = persoonUpdater;
		}

		/// <summary>
		/// Updatet een persoon in Kipadmin op basis van de gegevens in GAP.  Als er geen AD-nummer is, dan doen we
		/// een schamele poging om de persoon al te vinden.   Als ook dat niet lukt, maken we een nieuwe persoon aan.
		/// Bij ontbrekend AD-nummer, en wordt het achteraf ingevuld bij <paramref name="persoon"/>.  (Niet interessant
		/// voor service, maar wel voor andere methods die deze aanroepen).
		/// </summary>
		/// <param name="persoon">Informatie over een geupdatete persoon in GAP</param>
		/// <remarks>Als AD-nummer ontbreekt, wordt er sowieso een nieuwe persoon gemaakt.</remarks>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PersoonUpdaten(Persoon persoon)
		{
			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			Mapper.CreateMap<Persoon, KipPersoon>()
			    .ForMember(dst => dst.AdNummer, opt => opt.Ignore())
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
				KipPersoon kipPersoon;

				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(persoon);

				lock (_persoonManipulerenToken)
				{
					kipPersoon = mgr.Zoeken(zoekInfo, true, db);
					Mapper.Map(persoon, kipPersoon);
					kipPersoon.Stempel = DateTime.Now;
					db.SaveChanges();
				}
				if (persoon.AdNummer != kipPersoon.AdNummer)
				{
					persoon.AdNummer = kipPersoon.AdNummer;
					_persoonUpdater.AdNummerZetten(persoon.ID, kipPersoon.AdNummer);				
				}
			}

			Console.WriteLine("Persoon geupdatet: ID{0} {1} {2} AD{3}", persoon.ID, persoon.VoorNaam, persoon.Naam, persoon.AdNummer);
		}

		/// <summary>
		/// Aan te roepen als een voorkeursadres gewijzigd moet worden.
		/// </summary>
		/// <param name="adres">Nieuw voorkeursadres</param>
		/// <param name="bewoners">AD-nummers en adrestypes voor personen de dat adres moeten krijgen</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners)
		{
			StringBuilder feedback = new StringBuilder();

			var pMgr = new PersonenManager();

			Debug.Assert(adres != null);	// We gaan niet belachelijk doen he

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			// 1 keer tegelijk, anders krijgen we concurrencyproblemen als er 2 personen tegelijk hetzelfde
			// adres krijgen.

			lock (_adresManipulerenToken)
			{
				using (var db = new kipadminEntities())
				{
					var personen = new List<KipPersoon>();

					foreach (var b in bewoners)
					{
						var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(b.Persoon);
						zoekInfo.PostNr = adres.PostNr;

						var gevonden = pMgr.Zoeken(zoekInfo, false, db);
						
						if (gevonden == null)
						{
							throw new InvalidOperationException(String.Format(
								Properties.Resources.PersoonNietGevonden,
								b.Persoon.VoorNaam,
								b.Persoon.Naam));
						}
				
						personen.Add(gevonden);

						// In bewoners ad-nummer aanpassen, zoda we straks het juiste adrestype kunnen vinden
						b.Persoon.AdNummer = gevonden.AdNummer;
					}


					// Vind of maak adres

					// Als dat linq to sql is, dan gebeurt het zoeken sowieso hoofdletterongevoelig.

					string huisNr = adres.HuisNr.ToString();
					string postNr = adres.PostNr.ToString();

					var adresInDb = (from adr in db.AdresSet.Include("kipWoont.kipPersoon").Include("kipWoont.kipAdresType")
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
						db.AddToAdresSet(adresInDb);

						// Bewaar hier de changes al eens, zodat het nieuwe adres een ID krijgt.
						db.SaveChanges();

					}


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
						db.DeleteObject(wnt);
					}

					// Oude objecten met volgnr 1 al verwijderen, om duplicates (adnr,volgnr) in woont
					// te vermijden.

					db.SaveChanges();

					// Dat ID hebben we nu hier nodig:

					var goeieAdressen = personen.SelectMany(prs => prs.kipWoont).Where(kw => kw.kipAdres.ID == adresInDb.ID);

					// TODO: Het adrestype bepalen is iedere keer een linq-expressie, en dus iedere
					// keer een loop.  Kan dat niet efficienter?

					foreach (var wnt in goeieAdressen)
					{
						wnt.VolgNr = 1;

						int adresTypeID = (int) (from b in bewoners
						                         where b.Persoon.AdNummer == wnt.kipPersoon.AdNummer
						                         select b.AdresType).FirstOrDefault();

						wnt.kipAdresType = (from at in db.AdresTypeSet
						                    where at.ID == adresTypeID
						                    select at).FirstOrDefault();

						feedback.AppendLine(String.Format("Update voorkeuradres: AD{0}", wnt.kipPersoon.AdNummer));
					}

					var overigePersonen = from p in personen
					                      where !goeieAdressen.Any(wnt => wnt.kipPersoon.AdNummer == p.AdNummer)
					                      select p;

					foreach (var p in overigePersonen)
					{
						int adresTypeID = (int) (from b in bewoners
						                         where b.Persoon.AdNummer == p.AdNummer
						                         select b.AdresType).FirstOrDefault();

						var adresType = (from at in db.AdresTypeSet
						                 where at.ID == adresTypeID
						                 select at).FirstOrDefault();

						db.AddToWoontSet(new Woont
						                 	{
						                 		kipAdres = adresInDb,
						                 		kipPersoon = p,
						                 		VolgNr = 1,
						                 		kipAdresType = adresType,
						                 		Geldig = true
						                 	});
						feedback.Append(String.Format("Update voorkeuradres: AD{0}", p.AdNummer));
					}

					// fingers crossed:

					db.SaveChanges();
				}
			}
			Console.WriteLine(feedback.ToString());
		}


		/// <summary>
		/// Verwijdert alle bestaande contactinfo, en vervangt door de contactinfo meegegeven in 
		/// <paramref name="communicatieMiddelen"/>.
		/// </summary>
		/// <param name="pers">persoon waarvoor contactinfo te updaten</param>
		/// <param name="communicatieMiddelen">te updaten contactinfo</param>
		/// <remarks>Van de persoon zelf blijven we af!</remarks>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AlleCommunicatieBewaren(
			Persoon pers,
			IEnumerable<CommunicatieMiddel> communicatieMiddelen)
		{
			string feedback;

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
				KipPersoon persoon;

				// Haal eerst persoon op met alle communicatie-info gekend in Kipadmin.

				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(pers);
				zoekInfo.Communicatie = (from cm in communicatieMiddelen select cm.Waarde).ToArray();

				lock (_persoonManipulerenToken)
				{
					persoon = mgr.Zoeken(zoekInfo, false, db);
				}

				if (persoon == null)
				{
					throw new InvalidOperationException(String.Format(
						Properties.Resources.PersoonNietGevonden,
						pers.VoorNaam,
						pers.Naam));
				}


				lock (_communicatieToken)
				{
					// Hieronder quick and dirty gepruts, 
					// o.a. omdat de communicatievormen in kipadmin genummerd moeten zijn
					// TODO: Toch wat properder proberen

					// Verwijder gewoon alle bestaande communicatie

					var teVerwijderen = (from cv in persoon.kipContactInfo
					                     select cv).ToArray();

					foreach (var cv in teVerwijderen)
					{
						db.DeleteObject(cv);
					}

					// Bewaar tussentijds om key violations te vermijden
					db.SaveChanges();

					// Voeg nu de meegeleverde communicatie opnieuw toe.

					var nieuweComm = (from cm in communicatieMiddelen.Distinct()
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
					feedback = String.Format("Communicatie bewaard voor ID{0} {1} {2} AD{3}", pers.ID, persoon.VoorNaam, persoon.Naam, persoon.AdNummer);

				}
			}
			Console.WriteLine(feedback);
		}

		/// <summary>
		/// Voegt communicatevorm <paramref name="communicatie"/> toe aan de communicatievormen van <paramref name="pers"/>,
		/// op voorwaarde dat die er nog niet was.
		/// </summary>
		/// <param name="pers">Persoon die nieuwe communicatievorm moet krijgen</param>
		/// <param name="communicatie">Toe te voegen communicatievorm</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void CommunicatieToevoegen(Persoon pers, CommunicatieMiddel communicatie)
		{
			string feedback = String.Empty;

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
				KipPersoon persoon;

				// Haal eerst persoon op met alle communicatie-info gekend in Kipadmin.

				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(pers);
				zoekInfo.Communicatie = new string[] {communicatie.Waarde};

				lock (_persoonManipulerenToken)
				{
					persoon = mgr.Zoeken(zoekInfo, false, db);
				}

				if (persoon == null)
				{
					throw new InvalidOperationException(String.Format(
						Properties.Resources.PersoonNietGevonden,
						pers.VoorNaam,
						pers.Naam));
				}

				lock (_communicatieToken)
				{
					// Zoek bestaande communicatie van zelfde type op

					var bestaande = from ci in persoon.kipContactInfo
					                where ci.ContactTypeId == (int) communicatie.Type
					                select ci;

					// Voeg enkel toe als nog niet bestaat.

					var gevonden = (from ci in bestaande
					                where String.Compare(ci.Info, communicatie.Waarde, true) == 0
					                select ci.ContactInfoId).FirstOrDefault();
                                                                          
					if (gevonden == 0)
					{
						// volgnummer bepalen
						int volgnr;

						if (bestaande.FirstOrDefault() == null)
						{
							// Er bestaan er nog geen: volgnr = 1
							volgnr = 1;
						}
						else
						{
							volgnr = (from ci in bestaande select ci.VolgNr).Max() + 1;
						}

						var contactinfo = new ContactInfo
						                  	{
						                  		ContactInfoId = 0,
						                  		ContactTypeId = (int) communicatie.Type,
						                  		GeenMailings = communicatie.GeenMailings,
						                  		Info = communicatie.Waarde,
						                  		kipPersoon = persoon,
						                  		VolgNr = volgnr
						                  	};
						db.AddToContactInfoSet(contactinfo);
						db.SaveChanges();

						feedback = String.Format(
							"Communicate toegevoegd voor ID{0} {1} {2} AD{3}: {4}",
							persoon.GapID, persoon.VoorNaam, persoon.Naam, persoon.AdNummer, communicatie.Waarde);
					}

					
				}
			}
			Console.WriteLine(feedback);

		}

		/// <summary>
		/// Verwijdert een communicatiemiddel uit Kipadmin.
		/// </summary>
		/// <param name="pers">Persoonsgegevens van de persoon waarvan het communicatiemiddel moet verdwijnen.</param>
		/// <param name="communicatie">Gegevens over het te verwijderen communicatiemiddel</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void CommunicatieVerwijderen(Persoon pers, CommunicatieMiddel communicatie)
		{
			string feedback =String.Empty;

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var db = new kipadminEntities())
			{
				var mgr = new PersonenManager();
				var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(pers);
				var persoon = mgr.Zoeken(zoekInfo, false, db);

				// We gaan ervan uit dat er geen dubbele communicatievormen in de database
				// zitten; we verwijderen enkel de FirstOrDefault.

				int communicatieTypeID = (int)communicatie.Type;

				var teVerwijderen = (from ci in db.ContactInfoSet
						     where ci.kipPersoon.AdNummer == persoon.AdNummer
									   && ci.ContactTypeId == communicatieTypeID
									   && ci.Info == communicatie.Waarde
						     select ci).FirstOrDefault();

				if (teVerwijderen != null)
				{
					feedback = String.Format("Verwijderen communicatie: AD{0} {1}", persoon.AdNummer, communicatie.Waarde);
					db.DeleteObject(teVerwijderen);
					db.SaveChanges();
				}
			}

			Console.WriteLine(feedback);
		}

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
			string feedback;
			lock (_ledenToken)
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

						feedback=String.Format("Persoon met AD-nr. {0} ingeschreven als lid voor {1} in {2}", adNummer,
										gedoe.StamNummer, gedoe.WerkJaar);
					}

				}

			}
			Console.WriteLine(feedback);
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
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void NieuwLidBewaren(
			Persoon persoon,
			Adres adres,
			AdresTypeEnum adresType,
			IEnumerable<CommunicatieMiddel> communicatieMiddelen,
			LidGedoe lidGedoe)
		{
			string feedback;

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			Mapper.CreateMap<Persoon, KipPersoon>()
			    .ForMember(dst => dst.AdNummer, opt => opt.Ignore())
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			// Als het AD-nummer al gekend is, moet (gewoon) 'LidBewaren' gebruikt worden.

			Debug.Assert(persoon.AdNummer == null);

			Chiro.Kip.Data.Persoon gevonden; // poging om persoon al te vinden in database.

			// Doe eigenlijk hetzelfde als bij PersoonUpdaten, maar in dit geval hebben we meer info
			// om bestaande personen op te zoeken.

			lock (_persoonManipulerenToken)
			{
				using (var db = new kipadminEntities())
				{
					var mgr = new PersonenManager();
					var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(persoon);
					if (communicatieMiddelen != null)
					{
						zoekInfo.Communicatie = (from cm in communicatieMiddelen select cm.Waarde).ToArray();
					}
				
					if (adres != null)
					{
						zoekInfo.PostNr = adres.PostNr;
					}

					// Zoek of maak gevraagde persoon
					gevonden = mgr.Zoeken(zoekInfo, true, db);

					// Neem nieuwe gegevens over
					Mapper.Map(persoon, gevonden);
					db.SaveChanges();
				}
			}
			feedback = String.Format("Nieuw lid bewaard: ID{0} {1} {2} AD{3}", persoon.ID, persoon.VoorNaam, persoon.Naam, persoon.AdNummer);

			// Als er geen AD-nummer was, dan heeft de SaveChanges er voor ons eentje gemaakt.
			Debug.Assert(gevonden.AdNummer > 0);

			// AD-nummer overnemen in persoon
			persoon.AdNummer = gevonden.AdNummer;
			_persoonUpdater.AdNummerZetten(persoon.ID, gevonden.AdNummer);

			LidBewaren((int) persoon.AdNummer, lidGedoe);

			if (adres != null)
			{
				StandaardAdresBewaren(
					adres,
					new Bewoner[] {new Bewoner {Persoon = persoon, AdresType = adresType}});
			}

			AlleCommunicatieBewaren(persoon, communicatieMiddelen);
			Console.WriteLine(feedback);
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

				lock (_persoonManipulerenToken)
				{
					lid = mgr.LidZoeken(zoekInfo, stamNummer, werkJaar, db);
				}

				if (lid == null)
				{
					throw new InvalidOperationException(String.Format(
						Properties.Resources.LidNietGevonden,
						pers.VoorNaam,
						pers.Naam,
						stamNummer,
						werkJaar));
				}

				lock (_ledenToken)
				{
					// pragmatisch: eerst bestaande functies verwijderen.

					foreach (var hf in lid.HeeftFunctie)
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
					}
					db.SaveChanges();

				}
			}
			Console.WriteLine(feedback);
		}

		/// <summary>
		/// Updatet de afdelingen van een lid.
		/// </summary>
		/// <param name="pers">Persoon waarvan de afdelingen geupdatet moeten worden</param>
		/// <param name="stamNummer">Stamnummer van de groep waarin de persoon lid is</param>
		/// <param name="werkJaar">Werkjaar waarin de persoon lid is</param>
		/// <param name="afdelingen">Toe te kennen afdelingen.  Eventuele andere reeds toegekende functies worden verwijderd.</param>
		/// <remarks>Er is in Kipadmin maar plaats voor 2 afdelingen/lid</remarks>
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

				// locking gebeurt niet helemaal juist.  Maar uiteindelijk ga ik toch geen
				// meerdere threads gebruiken.

				lock (_persoonManipulerenToken)
				{
					lid = mgr.LidZoeken(zoekInfo, stamNummer, werkJaar, db);
				}

				if (lid == null)
				{
					throw new InvalidOperationException(String.Format(
						Properties.Resources.LidNietGevonden,
						pers.VoorNaam,
						pers.Naam,
						stamNummer,
						werkJaar));
				}

				lock (_ledenToken)
				{
					if (afdelingen.Count() >= 1)
					{
						int afdid = (int) afdelingen.First();
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
						int afdid = (int) afdelingen.Skip(1).First();
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

			}
			Console.WriteLine(feedback);
		}

		/// <summary>
		/// Bestelt dubbelpunt voor de gegeven persoon in het gegeven groepswerkjaar, gegeven dat de persoon
		/// een AD-nummer heeft
		/// </summary>
		/// <param name="adNummer">AD-nummer van persoon die Dubbelpunt wil</param>
		/// <param name="stamNummer">Groep die Dubbelpunt betaalt</param>
		/// <param name="werkJaar">Werkjaar waarvoor Dubbelpuntabonnement</param>
		public void DubbelpuntBestellen(int adNummer, string stamNummer, int werkJaar)
		{
			throw new NotImplementedException();
		}
	}
}
