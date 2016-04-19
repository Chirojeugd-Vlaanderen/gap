/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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

using System.Diagnostics;
using AutoMapper;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Adres = Chiro.Gap.Poco.Model.Adres;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Synchronisatie van bivakaangifte naar Kipadmin
    /// </summary>
    public class BivakSync : BaseSync, IBivakSync
    {
        /// <summary>
        /// Constructor.
        /// 
        /// De ServiceHelper wordt geïnjecteerd door de dependency injection container. Wat de
        /// ServiceHelper precies zal opleveren, hangt af van welke IChannelProvider geregistreerd
        /// is bij de container.
        /// </summary>
        /// <param name="serviceHelper">ServiceHelper, nodig voor service calls.</param>
        public BivakSync(ServiceHelper serviceHelper) : base(serviceHelper) { }

        /// <summary>
        /// Bewaart de uitstap <paramref name="uitstap"/> in Kipadmin als bivak.  Met contactpersoon
        /// en plaats.
        /// </summary>
        /// <param name="uitstap">Te bewaren uitstap</param>
        public void Bewaren(Uitstap uitstap)
        {
            // TODO (#1057): Verfijnen van sync bivakgegevens van GAP naar Kipadmin/CiviCRM

            var teSyncen = Mapper.Map<Uitstap, Bivak>(uitstap);
            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.BivakBewaren(teSyncen));

            var contactPersoon = uitstap.ContactDeelnemer != null ? uitstap.ContactDeelnemer.GelieerdePersoon : null;

            if (uitstap.Plaats != null && uitstap.Plaats.Adres != null)
            {
                // Als we het syncen van het adres ooit lostrekken van het syncen van het bivak (#1057), moeten we
                // eraan denken dat de naam van het bivak in Civi moet veranderen als het bivakadres verandert (#5030).
                ServiceHelper.CallService<ISyncPersoonService>(
                    svc =>
                    svc.BivakPlaatsBewaren(uitstap.ID, uitstap.Plaats.Naam,
                                           Mapper.Map<Adres, Kip.ServiceContracts.DataContracts.Adres>(uitstap.Plaats.Adres)));
            }

            if (contactPersoon != null)
            {
                Debug.Assert(contactPersoon.Persoon.InSync);
                if (contactPersoon.Persoon.AdNummer != null)
                {
                    // AD-nummer gekend: gewoon koppelen via AD-nummer
                    ServiceHelper.CallService<ISyncPersoonService>(svc => svc.BivakContactBewaren(
                        uitstap.ID,
                        (int) contactPersoon.Persoon.AdNummer));
                }
                else
                {                   
                    ServiceHelper.CallService<ISyncPersoonService>(
                        svc =>
                        svc.BivakContactBewarenAdOnbekend(uitstap.ID,
                                                          Mapper.Map<GelieerdePersoon, PersoonDetails>(contactPersoon)));
                }
            }
        }

        /// <summary>
        /// Verwijdert uitstap met <paramref name="uitstapID"/> uit kipadmin
        /// </summary>
        /// <param name="uitstapID">ID te verwijderen uitstap</param>
        public void Verwijderen(int uitstapID)
        {
            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.BivakVerwijderen(uitstapID));
        }
    }
}
