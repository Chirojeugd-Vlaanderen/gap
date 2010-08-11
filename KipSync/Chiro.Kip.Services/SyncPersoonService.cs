using System;
using System.Collections.Generic;
using System.Diagnostics;
using AutoMapper;
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

		public void AdresUpdated(Persoon persoon, IEnumerable<Adres> adressen)
		{

			using (var dc = new kipadminEntities())
			{
				if (persoon.AdNr.HasValue)
				{
					var q = (from p in dc.PersoonSet.Include("kipWoont.kipAdres")
						 where p.AdNr == persoon.AdNr.Value
						 select p).FirstOrDefault();

					if (q != null)
					{
						// Pickup adressen
						foreach (var w in q.kipWoont)
						{
							var a = w.kipAdres;
							var z = (from adres in adressen
								 where
								     (a.Land == adres.Land || (a.Land == "" && adres.Land == "België")) &&
								     a.Gemeente == adres.Woonplaats &&
								     a.PostNr == (adres.Postnummer + " " + adres.Postcode) &&
								     a.Straat == adres.Straat &&
								     a.Nr == adres.HuisNr.ToString() + adres.Bus
								 select adres);



						}

						q.Stempel = DateTime.Now;

						dc.SaveChanges();

						Console.WriteLine("You saved: GapID{0} {1} {2} AD{3} with his adresses", persoon.ID, persoon.Voornaam, persoon.Naam, persoon.AdNr);
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


			Console.WriteLine("You updates adres of: AD{0}", persoon.AdNr);
		}

		public void CommunicatieUpdated(Persoon persoon, IEnumerable<Communicatiemiddel> communicatiemiddelen)
		{
			Debug.WriteLine(string.Format("You entered: {0}", persoon.ID));
			Console.WriteLine("You entered: {0}", persoon.ID);
		}
	}
}
