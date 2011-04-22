using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using AutoMapper;
using Chiro.Cdf.Data.Entity;
using Chiro.Kip.Data;
using System.Linq;
using Chiro.Kip.Log;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.Kip.Services.UpdateService;
using Chiro.Kip.Workers;
using Adres = Chiro.Kip.ServiceContracts.DataContracts.Adres;
using KipPersoon = Chiro.Kip.Data.Persoon;
using Persoon = Chiro.Kip.ServiceContracts.DataContracts.Persoon;

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

		private readonly IUpdateService _svc;
		private readonly IMiniLog _log;

		/// <summary>
		/// Kipadmin kent het onderscheid postnummer/postcode niet.  Deze
		/// domme functie plakt de twee aan elkaar.
		/// </summary>
		/// <param name="adres">Adres</param>
		/// <returns>combinatie postnummer/postcode van adres</returns>
		private static string KipPostNr(Adres adres)
		{
			return String.IsNullOrEmpty(adres.PostCode)
			       	? adres.PostNr.ToString()
			       	: String.Format(
			       		"{0} {1}",
			       		adres.PostNr,
			       		adres.PostCode);
		}

		/// <summary>
		/// Standaardconstructor
		/// </summary>
		/// <param name="updateService">Service die gebruikt moet worden om updates terug te sturen naar GAP</param>
		public SyncPersoonService(IUpdateService updateService)
		{
			_svc = updateService;
			_log = new MiniLog();
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
			if (String.IsNullOrEmpty(persoon.VoorNaam))
			{
				_log.FoutLoggen(0, String.Format(
					"Persoon zonder voornaam niet geupdatet: {0}",
					persoon.Naam));
				return;
			}

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
				kipPersoon = mgr.Zoeken(zoekInfo, true, db);
				Mapper.Map(persoon, kipPersoon);
				kipPersoon.Stempel = DateTime.Now;
				db.SaveChanges();
				if (persoon.AdNummer != kipPersoon.AdNummer)
				{
					persoon.AdNummer = kipPersoon.AdNummer;
					_svc.AdNummerToekennen(persoon.ID, kipPersoon.AdNummer);
				}
			}

			_log.BerichtLoggen(0, String.Format(
				"Persoon geupdatet: ID{0} {1} {2} AD{3}", 
				persoon.ID, 
				persoon.VoorNaam, 
				persoon.Naam, 
				persoon.AdNummer));
		}

		/// <summary>
		/// Aan te roepen als een voorkeursadres gewijzigd moet worden.  Deze method vervangt in kipadmin
		/// gewoon het adres met volgnummer 1.  De andere adressen blijven onaangeroerd.
		/// </summary>
		/// <param name="adres">Nieuw voorkeursadres</param>
		/// <param name="bewoners">AD-nummers en adrestypes voor personen de dat adres moeten krijgen</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners)
		{
			StringBuilder feedback = new StringBuilder();

			var pMgr = new PersonenManager();

			Debug.Assert(adres != null); // We gaan niet belachelijk doen he

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
				.ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int) src.Geslacht))
				.ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			// Kipadmin kent het onderscheid postnummer-postcode niet.  Als er een postcode is, wordt die
			// met een spatie aan de postnummer geplakt.

			string postNr = KipPostNr(adres);
			
			using (var db = new kipadminEntities())
			{
				var personen = new List<KipPersoon>();

				// Zoek alle 'bewoners' op.  De gevonden bewoners worden bewaard
				// in 'personen'.

				foreach (var b in bewoners)
				{
					// NOOT: Adressen van bewoners zonder AD-nummer mogen wel bewaard worden.  Het
					// kan namelijk zijn dat een persoon al wel aangesloten is, maar dat
					// zijn AD-nummer nog niet teruggesynct is.

					if (b.Persoon == null)
					{
						_log.FoutLoggen(0, String.Format(Properties.Resources.AdresZonderPersoon, adres.Straat, adres.HuisNr, adres.Bus, adres.PostNr, adres.WoonPlaats));
					}
					else if (String.IsNullOrEmpty(b.Persoon.Naam) || String.IsNullOrEmpty(b.Persoon.VoorNaam))
					{
						_log.FoutLoggen(0, String.Format(Properties.Resources.NegeerPersoonOnvolledigeNaam, b.Persoon.VoorNaam, b.Persoon.Naam, b.Persoon.AdNummer));
					}
					else
					{
						var zoekInfo = Mapper.Map<Persoon, PersoonZoekInfo>(b.Persoon);
						zoekInfo.PostNr = postNr;

						var gevonden = pMgr.Zoeken(zoekInfo, false, db);

						if (gevonden == null)
						{
							// Er worden soms wel eens huisgenoten meegegeven die niet in kipadmin zitten.
							// Als we ze tegen komen, negeren we het adres.

							_log.BerichtLoggen(0, String.Format(
								"Adreswijziging: {0} {1} niet gevonden",
								b.Persoon.VoorNaam, b.Persoon.Naam));
							return;
						}
						else
						{
							personen.Add(gevonden);

							// In bewoners ad-nummer aanpassen, zoda we straks het juiste adrestype kunnen vinden
							b.Persoon.AdNummer = gevonden.AdNummer;
						}
					}
				}


				// Vind of maak adres

				// Als dat entities op sql server zijn, dan gebeurt het zoeken sowieso 
				// hoofdletterongevoelig.

				string huisNr = null;
				
				if (adres.HuisNr != null)
				{
					if (String.IsNullOrEmpty(adres.Bus))
					{
						huisNr = adres.HuisNr.ToString();
					}
					else if (adres.Bus[0] >= '0' && adres.Bus[0] <= '9')
					{
						// Als de bus numeriek is, dan zetten we 'bus' tussen
						// huisnummer en bus

						huisNr = String.Format("{0} bus {1}", adres.HuisNr, adres.Bus);
					}
					else
					{
						// zo niet: een spatie

						huisNr = String.Format("{0} {1}", adres.HuisNr, adres.Bus);
					}
				}


				var adresInDb = (from adr in db.AdresSet.Include("kipWoont.kipPersoon").Include("kipWoont.kipAdresType")
				                 where adr.Straat == adres.Straat
				                       && adr.Nr == huisNr
				                       && adr.PostNr == postNr
				                       && adr.Gemeente == adres.WoonPlaats
						       && (string.IsNullOrEmpty(adr.Land) && string.IsNullOrEmpty(adres.Land)
						       || adr.Land == adres.Land)
				                 select adr).FirstOrDefault();

				if (adresInDb == null)
				{
					adresInDb = new Chiro.Kip.Data.Adres
					            	{
					            		ID = 0,
					            		Straat = adres.Straat,
					            		Nr = huisNr,
					            		PostNr = postNr,
					            		Gemeente = adres.WoonPlaats,
					            		Land = adres.Land
					            	};
					db.AddToAdresSet(adresInDb);

					_log.BerichtLoggen(0, String.Format(
						"Nieuw adres gemaakt ({0}): {1} {2}, {3} {4} * {5}",
						adresInDb.ID, adresInDb.Straat, adresInDb.Nr, adresInDb.PostNr, adresInDb.Gemeente, adresInDb.Land));

				}
				else
				{
					_log.BerichtLoggen(0, String.Format(
						"Bestaand adres gevonden ({0}): {1} {2}, {3} {4} * {5}",
						adresInDb.ID, adresInDb.Straat, adresInDb.Nr, adresInDb.PostNr, adresInDb.Gemeente, adresInDb.Land));
				}


				// We zitten met het gedoe dat in Kipadmin de adressen een volgnummer hebben.  De voorkeurs-
				// adressen moeten bewaard worden met volgnummer 1.
				//
				// Strategie:
				//	verwijder koppelingen met to-adres waar volgnummer verschilt van 1
				//	vind of maak 'woont'-object met volgnummer 1
				//	koppel 'woont'-object met volgnummer 1 aan to-adres 


				var teVerwijderen =
					personen.SelectMany(prs => prs.kipWoont).Where(kw => kw.kipAdres.ID == adresInDb.ID && kw.VolgNr != 1);

				if (teVerwijderen.Count() > 0)
				{
					foreach (var wnt in teVerwijderen.ToArray())
					{
						db.DeleteObject(wnt);
					}

					// Als het nieuwe adres al een adres was met een ander volgnummer,
					// moet dat hier toch al verwijderd worden, om te vermijden dat een persoon
					// bij het saven tussentijds 2 keer op hetzelfde adres woont
					// (-> unique index violation)

					db.SaveChanges();
				}



				// TODO: Het adrestype bepalen is iedere keer een linq-expressie, en dus iedere
				// keer een loop.  Kan dat niet efficienter?

				foreach (var p in personen)
				{
					var eersteWnt = (from wnt in p.kipWoont
					                 where wnt.VolgNr == 1
					                 select wnt).FirstOrDefault();

					int adresTypeID = (int)(from b in bewoners
								where b.Persoon != null && b.Persoon.AdNummer == p.AdNummer
								select b.AdresType).FirstOrDefault();

					var kAdrType = (from at in db.AdresTypeSet
							    where at.ID == adresTypeID
							    select at).FirstOrDefault();

					if (eersteWnt == null)
					{
						eersteWnt = new Woont
						            	{
						            		kipPersoon = p,
						            		kipAdres = adresInDb,
						            		Geldig = true,
						            		VolgNr = 1,
						            		kipAdresType = kAdrType
						            	};
						db.AddToWoontSet(eersteWnt);
					}
					else
					{
						eersteWnt.kipAdres = adresInDb;
						eersteWnt.kipAdresType = kAdrType;
						eersteWnt.Geldig = true;
					}


					feedback.AppendLine(String.Format("Update voorkeuradres: AD{0}", p.AdNummer));
				}

				// fingers crossed:

				db.SaveChanges();
			}
			_log.BerichtLoggen(0, feedback.ToString());
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
				persoon = mgr.Zoeken(zoekInfo, false, db);

				if (persoon == null)
				{
					_log.FoutLoggen(0, String.Format(
						Properties.Resources.CommunicatieOnbekendePersoon,
						zoekInfo.VoorNaam,
						zoekInfo.Naam,
						String.Concat(
							(from cm in communicatieMiddelen select cm.Waarde + ';').ToArray())));

					//throw new InvalidOperationException(String.Format(
					//        Properties.Resources.PersoonNietGevonden,
					//        pers.VoorNaam,
					//        pers.Naam));
					return;
				}
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
				feedback = String.Format("Communicatie bewaard voor ID{0} {1} {2} AD{3}", pers.ID, persoon.VoorNaam, persoon.Naam,
				                         persoon.AdNummer);
			}
			_log.BerichtLoggen(0, feedback);
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
				persoon = mgr.Zoeken(zoekInfo, false, db);

				if (persoon == null)
				{
					//throw new InvalidOperationException(String.Format(
					//        Properties.Resources.PersoonNietGevonden,
					//        pers.VoorNaam,
					//        pers.Naam));
					_log.FoutLoggen(0, 
						String.Format(Properties.Resources.CommunicatieOnbekendePersoon,
						pers.VoorNaam,
						pers.Naam,
						communicatie.Waarde));
					return;
				}
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
			_log.BerichtLoggen(0, feedback);

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

				if (persoon == null)
				{
					_log.FoutLoggen(0, String.Format(
						Properties.Resources.CommVerwijderenOnbekendePersoon,
						pers.VoorNaam,
						pers.Naam,
						communicatie.Waarde));
					return;
				}

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

			_log.BerichtLoggen(0, feedback);
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

					// Als de laatste aansluiting nog niet doorgeboekt is, dan gaat het
					// nieuwe lid gewoon bij die laatste aansluiting.  (Voor kaderploegen
					// is er geen factuur gekoppeld aan een aansluiting.  Daar gaat
					// dus alles op aansluiting 1.)

					// Is er nog geen laatste aansluiting, of was de laatste wel
					// doorgeboekt, dan maken we er een nieuwe.

					if (aansluiting == null || (aansluiting.REKENING != null && aansluiting.REKENING.DOORGEBOE != "N"))
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
					      		SOORT = groep.IsGewestVerbond ? "KA" : (gedoe.LidType == LidTypeEnum.Kind ? "LI" : "LE"),
					      		STATUS = null,
					      		STEMPEL = DateTime.Now,
					      		VERZ_NR = 0,
					      		WEB_TOEVOEG = null,
					      		werkjaar = gedoe.WerkJaar
					      	};

					// 2 afdelingen kunnen we overnemen.

					if (gedoe.OfficieleAfdelingen.Count() >= 1)
					{
						int afdid = (int) gedoe.OfficieleAfdelingen.First();
						lid.AFDELING1 = (from a in db.AfdelingSet
						                 where a.AFD_ID == afdid
						                 select a.AFD_NAAM).FirstOrDefault();
					}

					if (gedoe.OfficieleAfdelingen.Count() >= 2)
					{
						int afdid = (int) gedoe.OfficieleAfdelingen.Skip(1).First();
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

					if (gedoe.LidType == LidTypeEnum.Kind && persoon.Geslacht == (int) GeslachtsEnum.Man)
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
								++aansluiting.SpeciaalJ;
								break;
						}
					}
					else if (gedoe.LidType == LidTypeEnum.Kind && persoon.Geslacht == (int) GeslachtsEnum.Vrouw)
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
								++aansluiting.SpeciaalM;
								break;
						}
					}
					else if (gedoe.LidType == LidTypeEnum.Leiding)
					{
						if (gedoe.NationaleFuncties.Contains(FunctieEnum.Vb)) ++aansluiting.Vb;
						else if (gedoe.NationaleFuncties.Contains(FunctieEnum.Proost)) ++aansluiting.Proost;
						else if (persoon.Geslacht == (int) GeslachtsEnum.Man) ++aansluiting.LeidingJ;
						else if (persoon.Geslacht == (int) GeslachtsEnum.Vrouw) ++aansluiting.LeidingM;
					}

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
					_log.FoutLoggen(0,String.Format(
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
			_log.BerichtLoggen(0, feedback.ToString());
		}

		/// <summary>
		/// Bestelt dubbelpunt voor de gegeven persoon in het gegeven groepswerkjaar, gegeven dat de persoon
		/// een AD-nummer heeft
		/// </summary>
		/// <param name="adNummer">AD-nummer van persoon die Dubbelpunt wil</param>
		/// <param name="stamNummer">Groep die Dubbelpunt betaalt</param>
		/// <param name="werkJaar">Werkjaar waarvoor Dubbelpuntabonnement</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DubbelpuntBestellen(int adNummer, string stamNummer, int werkJaar)
		{
			ChiroGroep groep = null;
			string feedback = String.Empty;
			using (var db = new kipadminEntities())
			{
				// Heeft de persoon toevallig al een abonnement voor het gegeven werkjaar?

				var abonnement = (from ab in db.Abonnement
				                  where ab.kipPersoon.AdNummer == adNummer && ab.werkjaar == werkJaar
				                  select ab).FirstOrDefault();

				if (abonnement == null)
				{
					// We doen enkel verder als er nog geen abonnement is.

					// Haal groep en persoon op.

					groep = (from g in db.Groep.OfType<ChiroGroep>()
					         where g.STAMNR == stamNummer
					         select g).FirstOrDefault();

					var persoon = (from p in db.PersoonSet
					               where p.AdNummer == adNummer
					               select p).FirstOrDefault();

                    if (persoon == null)
                    {
                        _log.FoutLoggen(groep.GroepID, String.Format(
                            "Dubbelpunt voor onbestaand ad-nummer {0}", adNummer));
                        return;
                    }

					// Bestaat er al een niet-doorgeboekte rekening voor Dubbelpunt voor het gegeven
					// groepswerkjaar?

					var rekening = (from f in db.RekeningSet
					                where f.WERKJAAR == werkJaar && f.REK_BRON == "DP" && f.DOORGEBOE == "N"
					                      && f.STAMNR == stamNummer
					                select f).FirstOrDefault();

					if (rekening == null)
					{
						// Nog geen rekening; maak er een nieuwe
						rekening = new Rekening
						           	{
						           		WERKJAAR = (short) werkJaar,
						           		TYPE = "F",
						           		REK_BRON = "DP",
						           		STAMNR = stamNummer,
						           		VERWIJSNR = 0,
						           		FACTUUR = "N",
						           		FACTUUR2 = "N",
						           		DOORGEBOE = "N",
						           		DAT_REK = DateTime.Now,
						           		STEMPEL = DateTime.Now
						           	};
						db.AddToRekeningSet(rekening);
					}

					abonnement = new Abonnement
					             	{
					             		werkjaar = werkJaar,
					             		UITG_CODE = "DP",
					             		Groep = groep,
					             		GRATIS = "N",
					             		REKENING = rekening,
					             		AANVR_DAT = DateTime.Now,
					             		STEMPEL = DateTime.Now,
					             		kipPersoon = persoon,
					             		EXEMPLAAR = 1,
					             		AANT_EXEM = 1,
					             		BESTELD1 = "J",
					             		BESTELD2 = "J",
					             		BESTELD3 = "J",
					             		BESTELD4 = "J",
					             		BESTELD5 = "J",
					             		BESTELD6 = "J",
					             		BESTELD7 = "J",
					             		BESTELD8 = "J",
					             		BESTELD9 = "J",
					             		BESTELD10 = "J",
					             		BESTELD11 = "J",
					             		BESTELD12 = "J",
					             		BESTELD13 = "J",
					             		BESTELD14 = "J",
					             		BESTELD15 = "J"
					             	};

					db.AddToAbonnement(abonnement);
					db.SaveChanges();

					feedback = String.Format(
						"Dubbelpuntabonnement voor {0} {1} AD{2}, rekening {3}",
						persoon.VoorNaam, persoon.Naam, persoon.AdNummer, rekening.NR);
				}
			}
			_log.BerichtLoggen((groep == null ? 0 : groep.GroepID), feedback);
		}

		/// <summary>
		/// Bestelt dubbelpunt voor een 'onbekende' persoon in het gegeven groepswerkjaar
		/// </summary>
		/// <param name="details">details voor de persoon die Dubbelpunt wil bestellen</param>
		/// <param name="stamNummer">Groep die Dubbelpunt betaalt</param>
		/// <param name="werkJaar">Werkjaar waarvoor Dubbelpuntabonnement</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DubbelpuntBestellenNieuwePersoon(PersoonDetails details, string stamNummer, int werkJaar)
		{
			// Als het AD-nummer al gekend is, moet (gewoon) 'LidBewaren' gebruikt worden.
			Debug.Assert(details.Persoon.AdNummer == null);

			if (String.IsNullOrEmpty(details.Persoon.VoorNaam))
			{
				_log.FoutLoggen(0, String.Format(
					"Persoon zonder voornaam niet geupdatet: {0}",
					details.Persoon.Naam));
				return;
			}

			int adnr = UpdatenOfMaken(details);
			DubbelpuntBestellen(adnr, stamNummer, werkJaar);
		}

		/// <summary>
		/// Verzekert de persoon met AD-nummer <paramref name="adNummer"/> tegen loonverlies voor werkjaar
		/// <paramref name="werkJaar"/>.  De groep met stamnummer <paramref name="stamNummer"/> betaalt.
		/// </summary>
		/// <param name="adNummer">AD-nummer van te verzekeren persoon</param>
		/// <param name="stamNummer">Stamnummer van betalende groep</param>
		/// <param name="werkJaar">Werkjaar waarin te verzekeren voor loonverlies</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LoonVerliesVerzekeren(int adNummer, string stamNummer, int werkJaar)
		{
			ChiroGroep groep = null;
			string feedback = String.Empty;
			using (var db = new kipadminEntities())
			{
				// Haal groep op.

				groep = (from g in db.Groep.OfType<ChiroGroep>()
					 where g.STAMNR == stamNummer
					 select g).FirstOrDefault();

				// Nakijken of de persoon in kwestie nog niet verzekerd is.

				var bestaande = (from v in db.PersoonsVerzekering
				                 where v.kipPersoon.AdNummer == adNummer && v.ExtraVerzekering.WerkJaar == werkJaar
				                 select v).FirstOrDefault();

				if (bestaande != null)
				{
					_log.BerichtLoggen(groep.GroepID, String.Format(
						"Dubbele verzekering loonverlies van persoon genegeerd.  Ad-nr {0}",
						adNummer));
					return;
				}

				// Opzoeken persoon

				var persoon = (from p in db.PersoonSet
				               where p.AdNummer == adNummer
				               select p).FirstOrDefault();

				if (persoon == null)
				{
					_log.FoutLoggen(
						groep == null ? 0 : groep.GroepID,
						String.Format(
							"genegeerd: loonverlies onbekend AD-nr. {0}", 
							adNummer));
					return;
				}

				// Zoek eerst naar een geschikte lijn in ExtraVerzekering die we
				// kunnen recupereren voor deze verzekering.

				var extraVerzDitWerkjaar = (from v in db.ExtraVerzekering.Include(verz => verz.REKENING)
				                            where v.WerkJaar == werkJaar
				                                  && v.Groep.GroepID == groep.GroepID
				                            select v).OrderByDescending(vv => vv.VolgNummer);

				// Laatste opzoeken

				var verzekering = extraVerzDitWerkjaar.FirstOrDefault();

				// Als de laatste verzekering een niet-doorgeboekte rekening heeft,
				// komt deze persoon er gewoon bij.  Is er nog geen laatste verzekering,
				// of is de rekening al wel doorgeboekt, dan maken we er een nieuwe.

				if (verzekering == null || (verzekering.REKENING != null && verzekering.REKENING.DOORGEBOE != "N"))
				{
					int volgNummer = (verzekering == null ? 1 : verzekering.VolgNummer + 1);
					// Bedragen rekening mogen leeg zijn; worden aangevuld bij factuur
					// overzetten in Kipadmin.

					// Kaderploegen krijgen geen factuur

					var rekening = (groep.TYPE.ToUpper() != "L")
					               	? null
					               	: new Rekening
					               	  	{
					               	  		WERKJAAR = (short) werkJaar,
					               	  		TYPE = "F",
					               	  		REK_BRON = "U_VERZEK",
					               	  		STAMNR = stamNummer,
					               	  		VERWIJSNR = volgNummer,
					               	  		FACTUUR = "N",
					               	  		FACTUUR2 = "N",
					               	  		DOORGEBOE = "N",
					               	  		DAT_REK = DateTime.Now
					               	  	};

					verzekering = new ExtraVerzekering
					              	{
					              		Datum = DateTime.Now,
					              		DoodInvaliditeit = null,
					              		ExtraVerzekeringID = 0,
					              		Groep = groep,
					              		LoonVerlies = 0,
					              		Noot = String.Empty,
					              		REKENING = rekening,
					              		VolgNummer = volgNummer,
					              		WerkJaar = werkJaar
					              	};

					if (rekening != null)
					{
						db.AddToRekeningSet(rekening);
					}
					db.AddToExtraVerzekering(verzekering);
				}

				// verzekering bevat nu het verzekeringsrecord waarbij de nieuwe
				// persoon opgeteld kan worden.

				// In de 'noot' van het verzekeringsrecord bewaren we
				// comma-separated de AD-nummers van verzekerde personen.
				// Dit is 'legacy'; we connecteren het verzekeringsrecord nu met
				// de verzekerde persoon.

				var persoonsVerzekering = new PersoonsVerzekering
				                          	{
				                          		ExtraVerzekering = verzekering,
				                          		kipPersoon = persoon
				                          	};

				db.AddToPersoonsVerzekering(persoonsVerzekering);

				verzekering.Noot = String.Format(
					"{0},{1}",
					verzekering.Noot,
					adNummer);
				verzekering.Stempel = DateTime.Now;
				verzekering.Wijze = "G";
				++verzekering.LoonVerlies;

				db.SaveChanges();

				feedback = String.Format(
					"Persoon met AD-nr. {0} verzekerd tegen loonverlies voor {1} in {2}",
					adNummer,
					stamNummer,
					werkJaar);
			}
			_log.BerichtLoggen(groep == null ? 0 : groep.GroepID, feedback);
		}

		/// <summary>
		/// Verzekert een persoon zonder AD-nummer tegen loonverlies voor werkjaar
		/// <paramref name="werkJaar"/>.  De groep met stamnummer <paramref name="stamNummer"/> betaalt.
		/// </summary>
		/// <param name="details">details van te verzekeren persoon</param>
		/// <param name="stamNummer">Stamnummer van betalende groep</param>
		/// <param name="werkJaar">Werkjaar waarin te verzekeren voor loonverlies</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LoonVerliesVerzekerenAdOnbekend(PersoonDetails details, string stamNummer, int werkJaar)
		{
			// Als het AD-nummer al gekend is, moet (gewoon) 'LidBewaren' gebruikt worden.
			Debug.Assert(details.Persoon.AdNummer == null);

			if (String.IsNullOrEmpty(details.Persoon.VoorNaam))
			{
				_log.FoutLoggen(0, String.Format(
					"Persoon zonder voornaam niet geupdatet: {0}",
					details.Persoon.Naam));
				return;
			}

			int adnr = UpdatenOfMaken(details);
			LoonVerliesVerzekeren(adnr, stamNummer, werkJaar);
		}

		/// <summary>
		/// Probeert een persoon te vinden op basis van persoonsgegevens, adressen en communicatie.
		/// Als dat lukt, worden de meegegeven persoonsgegevens, adressen, communicatie overgenomen in de
		/// database.  Als dat niet lukt, wordt een nieuwe persoon aangemaakt.
		/// </summary>
		/// <param name="details">Details van de te vinden persoon</param>
		/// <returns>AD-nummer van gevonden/aangemaakte persoon.</returns>
		private int UpdatenOfMaken(PersoonDetails details)
		{
			var persoon = details.Persoon;
			var adres = details.Adres;
			var adresType = details.AdresType;
			var communicatieMiddelen = details.Communicatie;

			Mapper.CreateMap<Persoon, PersoonZoekInfo>()
				.ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int) src.Geslacht))
				.ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			Mapper.CreateMap<Persoon, KipPersoon>()
				.ForMember(dst => dst.AdNummer, opt => opt.Ignore())
				.ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int) src.Geslacht))
				.ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			Chiro.Kip.Data.Persoon gevonden;

			// Doe eigenlijk hetzelfde als bij PersoonUpdaten, maar in dit geval hebben we meer info
			// om bestaande personen op te zoeken.

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
					zoekInfo.PostNr = KipPostNr(adres);
				}

				// Zoek of maak gevraagde persoon
				gevonden = mgr.Zoeken(zoekInfo, true, db);

				// Neem nieuwe gegevens over
				Mapper.Map(persoon, gevonden);
				db.SaveChanges();
			}
			// Als er geen AD-nummer was, dan heeft de SaveChanges er voor ons eentje gemaakt.
			Debug.Assert(gevonden.AdNummer > 0);

			string feedback = String.Format(
				"Nieuwe persoon bewaard: ID{0} {1} {2} AD{3}",
				persoon.ID,
				persoon.VoorNaam,
				persoon.Naam,
				gevonden.AdNummer);

			// AD-nummer overnemen in persoon en GAP
			persoon.AdNummer = gevonden.AdNummer;
			_svc.AdNummerToekennen(persoon.ID, gevonden.AdNummer);

			if (adres != null)
			{
				StandaardAdresBewaren(
					adres,
					new Bewoner[] {new Bewoner {Persoon = persoon, AdresType = adresType}});
			}

			AlleCommunicatieBewaren(persoon, communicatieMiddelen);
			_log.BerichtLoggen(0, feedback);

			return gevonden.AdNummer;
		}
	}
}
