/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
    /// Synchronisatie van abonnementen naar Mailchimp
    /// </summary>
    public class AbonnementenSync : BaseSync, IAbonnementenSync
    {
        public AbonnementenSync(ServiceHelper serviceHelper) : base(serviceHelper)
        {
        }

        public void AbonnementBewaren(Abonnement teSyncenAbonnement)
        {
            var info = Mapper.Map<Abonnement, AbonnementInfo>(teSyncenAbonnement);

            if (info.Adres.Land == null)
            {
                // Mailchimp wil altijd een land.
                info.Adres.Land = "België";
            }

            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.AbonnementNaarMailchimp(info));
        }

        public void AbonnementVerwijderen(Abonnement teSyncenAbonnement)
        {
            var info = Mapper.Map<Abonnement, AbonnementInfo>(teSyncenAbonnement);
            info.AbonnementType = 0;
            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.AbonnementNaarMailchimp(info));
        }
    }
}
