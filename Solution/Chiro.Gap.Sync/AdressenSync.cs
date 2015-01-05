/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Adres = Chiro.Gap.Poco.Model.Adres;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Regelt de synchronisatie van adresgegevens naar Kipadmin
    /// </summary>
    public class AdressenSync : BaseSync, IAdressenSync
    {
        /// <summary>
        /// Constructor.
        /// 
        /// De ServiceHelper wordt geïnjecteerd door de dependency injection container. Wat de
        /// ServiceHelper precies zal opleveren, hangt af van welke IChannelProvider geregistreerd
        /// is bij de container.
        /// </summary>
        /// <param name="serviceHelper">ServiceHelper, nodig voor service calls.</param>
        public AdressenSync(ServiceHelper serviceHelper) : base(serviceHelper) { }

        /// <summary>
        /// Stelt de gegeven persoonsadressen in als standaardadressen in Kipadmin
        /// </summary>
        /// <param name="persoonsAdressen">Persoonsadressen die als standaardadressen (adres 1) naar
        /// Kipadmin moeten.  Personen moeten gekoppeld zijn, net zoals adressen met straatnaam en gemeente</param>
        public void StandaardAdressenBewaren(IList<PersoonsAdres> persoonsAdressen)
        {
            // Voorlopig afhandelen per adres.

            var gegroepeerdOpAdres = persoonsAdressen.GroupBy(pa => pa.Adres, pa => pa);

            foreach (var persoonsAdressenVoorAdres in gegroepeerdOpAdres)
            {
                var adres = Mapper.Map<Adres, Kip.ServiceContracts.DataContracts.Adres>(persoonsAdressenVoorAdres.Key);
                var bewoners = from pa in persoonsAdressenVoorAdres
                               select new Bewoner
                               {
                                   Persoon = Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(pa.Persoon),
                                   AdresType = (AdresTypeEnum)pa.AdresType
                               };

                ServiceHelper.CallService<ISyncPersoonService>(svc => svc.StandaardAdresBewaren(adres, bewoners.ToList()));
            }
        }
    }
}
