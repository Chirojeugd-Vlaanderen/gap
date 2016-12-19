/*
 * Copyright 2015, 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using AutoMapper;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

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

        public void Bewaren(Abonnement teSyncenAbonnement)
        {
            var type = (AbonnementTypeEnum)(int)teSyncenAbonnement.Type;
            if (teSyncenAbonnement.GelieerdePersoon.Persoon.AdNummer.HasValue)
            {
                ServiceHelper.CallService<ISyncPersoonService>(
                    svc =>
                        svc.AbonnementBewaren(teSyncenAbonnement.GelieerdePersoon.Persoon.AdNummer.Value, teSyncenAbonnement.GroepsWerkJaar.WerkJaar, type));
            }
            else
            {
                var details = Mapper.Map<GelieerdePersoon, PersoonDetails>(teSyncenAbonnement.GelieerdePersoon);
                ServiceHelper.CallService<ISyncPersoonService>(
                    svc => svc.AbonnementNieuwePersoonBewaren(details, teSyncenAbonnement.GroepsWerkJaar.WerkJaar, type));
            }            
        }

        public void AlleAbonnementenVerwijderen(GelieerdePersoon gelieerdePersoon)
        {
            if (gelieerdePersoon.Persoon.AdNummer.HasValue)
            {
                ServiceHelper.CallService<ISyncPersoonService>(
                    svc =>
                        svc.AbonnementStopzetten(gelieerdePersoon.Persoon.AdNummer.Value));
            }
            else
            {
                var details = Mapper.Map<GelieerdePersoon, PersoonDetails>(gelieerdePersoon);
                ServiceHelper.CallService<ISyncPersoonService>(
                    svc => svc.AbonnementStopzettenNieuwePersoon(details));
            }
        }
    }
}
