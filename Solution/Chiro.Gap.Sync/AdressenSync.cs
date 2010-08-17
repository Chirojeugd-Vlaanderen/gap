using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Sync.SyncService;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Regelt de synchronisatie van adresgegevens naar Kipadmin
	/// </summary>
	public class AdressenSync : IAdressenSync
	{
		private readonly ISyncPersoonService _svc;

		/// <summary>
		/// Constructor voor AdressenSync
		/// </summary>
		/// <param name="svc">Te gebruiken service</param>
		public AdressenSync(ISyncPersoonService svc)
		{
			_svc = svc;	
		}

		/// <summary>
		/// Stelt de gegeven persoonsadressen in als standaardadressen in Kipadmin
		/// </summary>
		/// <param name="persoonsAdressen">Persoonsadressen die als standaardadressen (adres 1) naar
		/// Kipadmin moeten.  Personen moeten gekoppeld zijn, net zoals adressen met straatnaam en gemeente</param>
		public void StandaardAdressenBewaren(IEnumerable<PersoonsAdres> persoonsAdressen)
		{
			// Voorlopig afhandelen per adres.  Zou die distinct werken?

			foreach (var adr in persoonsAdressen.Select(pa => pa.Adres).Distinct())
			{
				var bewoners = from pa in adr.PersoonsAdres
					       select new SyncService.Bewoner
					       {
						       AdNummer = pa.Persoon.AdNummer ?? 0,
						       AdresType = (SyncService.AdresTypeEnum)pa.AdresType
					       };

				// TODO (#238): Buitenlandse adressen!

				var adres = new SyncService.Adres
				{
					Bus = adr.Bus,
					HuisNr = adr.HuisNr,
					Land = "",
					PostNr = adr.StraatNaam.PostNummer,
					Straat = adr.StraatNaam.Naam,
					WoonPlaats = adr.WoonPlaats.Naam
				};

				_svc.StandaardAdresBewaren(adres, bewoners.ToList());
			}
		}
	}
}
