// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Sync.SyncService;

using Persoon = Chiro.Gap.Orm.Persoon;

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
						       Persoon = Mapper.Map<Persoon, SyncService.Persoon>(pa.Persoon),
						       AdresType = (AdresTypeEnum)pa.AdresType
					       };

				var adres = Mapper.Map<Chiro.Gap.Orm.Adres, Chiro.Gap.Sync.SyncService.Adres>(adr);

				_svc.StandaardAdresBewaren(adres, bewoners.ToList());
			}
		}
	}
}
