using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Transactions;
using AutoMapper;
using Chiro.Cdf.Data.Entity;
using Chiro.Kip.Data;
using Chiro.Kip.Services.DataContracts;
using System.Linq;

using Adres = Chiro.Kip.Services.DataContracts.Adres;
using Persoon = Chiro.Kip.Services.DataContracts.Persoon;
using KipPersoon = Chiro.Kip.Data.Persoon;
using KipAdres = Chiro.Kip.Data.Adres;

namespace Chiro.Kip.Services
{
	public class SyncPersoonService : ISyncPersoonService
	{

		/// <summary>
		/// Updatet een persoon in Kipadmin op basis van de gegevens in 
		/// <paramref name="persoon"/>
		/// </summary>
		/// <param name="persoon">Informatie over een geupdatete persoon in GAP</param>
		public void PersoonUpdaten(Persoon persoon)
		{
			Mapper.CreateMap<Persoon, KipPersoon>()
			    .ForMember(dst => dst.AdNr, opt => opt.Ignore())
			    .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => (int)src.Geslacht))
			    .ForMember(dst => dst.GapID, opt => opt.MapFrom(src => src.ID));

			using (var dc = new kipadminEntities())
			{
				if (persoon.AdNr.HasValue)
				{
					var q = (from p in dc.PersoonSet
						 where p.AdNr == persoon.AdNr.Value
						 select p).FirstOrDefault();
					if (q != null)
					{
						Mapper.Map<Persoon, KipPersoon>(persoon, q);

						q.Stempel = DateTime.Now;

						dc.SaveChanges();

						Console.WriteLine("You saved: ID{0} {1} {2} AD{3}", persoon.ID, persoon.Voornaam, persoon.Naam, persoon.AdNr);
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

					Console.WriteLine("You saved: GapID{0} {1} {2} AD{3}", persoon.ID, q.VoorNaam, q.Naam, q.AdNr);

					// TODO: AdNr terug sturen

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
					               	.Where(Utility.BuildContainsExpression<Chiro.Kip.Data.Persoon, int>(prs => prs.AdNr, adNummers))
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

						int adresTypeID = (int)(from b in bewoners
						                   where b.AdNummer == wnt.kipPersoon.AdNr
						                   select b.AdresType).FirstOrDefault();

						wnt.kipAdresType = (from at in dc.AdresTypeSet
								    where at.ID == adresTypeID
								    select at).FirstOrDefault();

						Console.WriteLine("Update voorkeuradres: AD{0}", wnt.kipPersoon.AdNr);
					}

					var overigePersonen = from p in personen
					                      where !goeieAdressen.Any(wnt => wnt.kipPersoon.AdNr == p.AdNr)
					                      select p;

					foreach (var p in overigePersonen)
					{
						int adresTypeID = (int)(from b in bewoners
									where b.AdNummer == p.AdNr
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
						Console.WriteLine("Update voorkeuradres: AD{0}", p.AdNr);
					}

					// fingers crossed:

					dc.SaveChanges();
				}
#if KIPDORP
				tx.Complete();
			}
#endif
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


#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
				using (var db = new kipadminEntities())
				{
					// Haal eerst persoon op met alle communicatie-info gekend in Kipadmin.

					var persoon = (from p in db.PersoonSet.Include(p => p.kipContactInfo)
					               where p.AdNr == adNr
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
				                     where ci.kipPersoon.AdNr == adNr
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
		/// <param name="stamNummer">Stamnummer van groep waarvan lid te maken</param>
		/// <param name="werkJaar">Werkjaar waarin ld te maken</param>
		/// <param name="lidType">Lidtype: kind, leiding, of kader</param>
		/// <param name="nationaalBepaaldeFuncties">Alle nationaal bepaalde functies die toegekend moeten zijn
		/// aan dit lid.</param>
		/// <param name="officieleAfdelingen">Alle officiele afdelingen die toegekend moeten zijn aan dit lid.
		/// </param>
		public void LidBewaren(
			int adNummer, 
			string stamNummer, 
			int werkJaar, 
			LidTypeEnum lidType, 
			IEnumerable<FunctieEnum> nationaalBepaaldeFuncties, 
			IEnumerable<AfdelingEnum> officieleAfdelingen)
		{
			throw new NotImplementedException();
		}
	}
}
