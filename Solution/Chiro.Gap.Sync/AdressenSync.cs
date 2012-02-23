// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

using Persoon = Chiro.Gap.Orm.Persoon;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Regelt de synchronisatie van adresgegevens naar Kipadmin
    /// </summary>
    public class AdressenSync : IAdressenSync
    {
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
                                   Persoon = Mapper.Map<Persoon, Chiro.Kip.ServiceContracts.DataContracts.Persoon>(pa.Persoon),
                                   AdresType = (AdresTypeEnum)pa.AdresType
                               };

                var adres = Mapper.Map<Orm.Adres, Chiro.Kip.ServiceContracts.DataContracts.Adres>(adr);

                ServiceHelper.CallService<ISyncPersoonService>(svc => svc.StandaardAdresBewaren(adres, bewoners.ToList()));
            }
        }
    }
}
