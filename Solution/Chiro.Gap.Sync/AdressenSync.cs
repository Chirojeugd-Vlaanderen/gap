// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Linq;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Sync.SyncService;

using Adres = Chiro.Gap.Sync.SyncService.Adres;

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
					       select new Bewoner
					       {
						       AdNummer = pa.Persoon.AdNummer ?? 0,
						       AdresType = (AdresTypeEnum)pa.AdresType
					       };

				// TODO (#238): Buitenlandse adressen!

				var adres = new Adres
				{
					Bus = adr.Bus,
					HuisNr = adr.HuisNr,
					Land = string.Empty,
					PostNr = adr.StraatNaam.PostNummer,
					Straat = adr.StraatNaam.Naam,
					WoonPlaats = adr.WoonPlaats.Naam
				};

				_svc.StandaardAdresBewaren(adres, bewoners.ToList());
			}
		}
	}
}
