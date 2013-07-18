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

using System;
using AutoMapper;

using Chiro.Adf.ServiceModel;
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
    public class BivakSync : IBivakSync
    {
        /// <summary>
        /// Bewaart de uitstap <paramref name="uitstap"/> in Kipadmin als bivak.  Zonder contactpersoon
        /// of plaats.
        /// </summary>
        /// <param name="uitstap">Te bewaren uitstap</param>
        public void Bewaren(Uitstap uitstap)
        {
            // TODO (#1057): Dit zijn waarschijnlijk te veel databasecalls

            var teSyncen = Mapper.Map<Uitstap, Bivak>(uitstap);
            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.BivakBewaren(teSyncen));

            var contactPersoon = uitstap.ContactDeelnemer != null ? uitstap.ContactDeelnemer.GelieerdePersoon : null;

            if (uitstap.Plaats != null && uitstap.Plaats.Adres != null)
            {
                ServiceHelper.CallService<ISyncPersoonService>(
                    svc =>
                    svc.BivakPlaatsBewaren(uitstap.ID, uitstap.Plaats.Naam,
                                           Mapper.Map<Adres, Kip.ServiceContracts.DataContracts.Adres>(uitstap.Plaats.Adres)));
            }

            if (contactPersoon != null)
            {
                if (contactPersoon.Persoon.AdNummer != null)
                {
                    // AD-nummer gekend: gewoon koppelen via AD-nummer
                    ServiceHelper.CallService<ISyncPersoonService>(svc => svc.BivakContactBewaren(
                        uitstap.ID,
                        (int) contactPersoon.Persoon.AdNummer));
                }
                else
                {
                    throw new NotImplementedException();
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
