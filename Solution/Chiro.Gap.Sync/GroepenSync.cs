/*
 * Copyright 2008-2013, 2016 the GAP developers. See the NOTICE file at the 
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

using System;
using AutoMapper;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;

namespace Chiro.Gap.Sync
{
    public class GroepenSync: BaseSync, IGroepenSync
    {
        /// <summary>
        /// Constructor.
        /// 
        /// De ServiceHelper wordt geïnjecteerd door de dependency injection container. Wat de
        /// ServiceHelper precies zal opleveren, hangt af van welke IChannelProvider geregistreerd
        /// is bij de container.
        /// </summary>
        /// <param name="serviceHelper">ServiceHelper, nodig voor service calls.</param>
        public GroepenSync(ServiceHelper serviceHelper) : base(serviceHelper) { }

        /// <summary>
        /// Updatet de gegevens van groep <paramref name="g"/> in Kipadmin. Het stamnummer van <paramref name="g"/>
        /// bepaalt de groep waarover het gaat.
        /// </summary>
        /// <param name="g">Te updaten groep in Kipadmin</param>
        public void Bewaren(Groep g)
        {
            Kip.ServiceContracts.DataContracts.Groep syncGroep =
                Mapper.Map<Groep, Kip.ServiceContracts.DataContracts.Groep>(g);

            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.GroepUpdaten(syncGroep));
        }

        /// <summary>
        /// Sluit het huidige werkjaar van de gegeven <param name="groep"> af in Civi. Dat komt erop neer dat
        /// alle lidrelaties worden beeindigd.</param>
        /// </summary>
        /// <param name="groep">Groep waarvan het werkjaar afgesloten moet worden.</param>
        public void WerkjaarAfsluiten(Groep groep)
        {
            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.GroepsWerkjaarAfsluiten(groep.Code));
        }

        /// <summary>
        /// Herstelt de lidrelaties van de gegeven <paramref name="groep"/> naar de toestand op de gegeven
        /// <paramref name="datum"/>. In praktijk wordt dit gebruikt om een werkjaar terug te draaien.
        /// </summary>
        /// <param name="groep"></param>
        /// <param name="datum"></param>
        public void WerkjaarTerugDraaien(Groep groep, DateTime datum)
        {
            ServiceHelper.CallService<ISyncPersoonService>(
                svc => svc.GroepsWerkjaarTerugDraaien(groep.Code, datum));
        }
    }
}
