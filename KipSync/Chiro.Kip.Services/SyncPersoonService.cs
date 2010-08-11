using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
		public void PersoonUpdated(Persoon persoon)
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
		/// <param name="adNummers">AD-nummers van personen de dat adres moeten krijgen</param>
		public void VoorkeurAdresUpdated(Adres adres, IEnumerable<int> adNummers)
		{
			// TODO: transactie, want er gebeurt hier nogal wat.

			using (var dc = new kipadminEntities())
			{
				// Vind adrestype

				int adresTypeID = (int)adres.AdresType;

				var adresType = (from at in dc.AdresTypeSet
				                 where at.ID == adresTypeID
				                 select at).FirstOrDefault();

				// Vind personen met gegeven adnummers.

				var personen = from p in dc.PersoonSet.Include("kipWoont.kipAdres")
					       .Where(Utility.BuildContainsExpression<Chiro.Kip.Data.Persoon, int>(prs=>prs.AdNr, adNummers))
				               select p;

				// Vind of maak adres

				// Als dat linq to sql is, dan gebeurt het zoeken sowieso hoofdletterongevoelig.

				string huisNr = adres.HuisNr.ToString();
				string postNr = adres.Postnummer.ToString();

				var adresInDb = (from adr in dc.AdresSet.Include("kipWoont.kipPersoon").Include("kipWoont.kipAdresType")
				                 where adr.Straat == adres.Straat
				                       && adr.Nr == huisNr
				                       && adr.PostNr == postNr
				                       && adr.Gemeente == adres.Woonplaats
				                 select adr).FirstOrDefault();

				if (adresInDb == null)
				{
					adresInDb = new Chiro.Kip.Data.Adres
					            	{
					            		ID = 0,
					            		Straat = adres.Straat,
					            		Nr = adres.HuisNr == null ? null : adres.HuisNr.ToString(),
								PostNr = adres.Postnummer.ToString(),
					            		Gemeente = adres.Woonplaats
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

				foreach (var wnt in goeieAdressen)
				{
					wnt.VolgNr = 1;
					wnt.kipAdresType = adresType;
					Console.WriteLine("Update voorkeuradres: AD{0}", wnt.kipPersoon.AdNr);
				}

				var overigePersonen = from p in personen
				                      where !goeieAdressen.Any(wnt => wnt.kipPersoon.AdNr == p.AdNr)
				                      select p;

				foreach (var p in overigePersonen)
				{
					dc.AddToWoontSet(new Woont {kipAdres = adresInDb, kipPersoon = p, VolgNr = 1, kipAdresType = adresType});
					Console.WriteLine("Update voorkeuradres: AD{0}", p.AdNr);
				}

				// fingers crossed:

				dc.SaveChanges();
			}
		}

		public void CommunicatieUpdated(Persoon persoon, IEnumerable<Communicatiemiddel> communicatiemiddelen)
		{
			Debug.WriteLine(string.Format("You entered: {0}", persoon.ID));
			Console.WriteLine("You entered: {0}", persoon.ID);
		}
	}
}
