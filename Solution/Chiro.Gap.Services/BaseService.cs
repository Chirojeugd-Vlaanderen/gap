/*
 * Copyright 2014, 2017 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Verfijnen gebruikersrechten Copyright 2015 Chirojeugd-Vlaanderen vzw
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

using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Cdf.AdnrWcfExtension;
using System.ServiceModel;
using System.Linq;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Base class voor onze services.
    /// 
    /// Deze doet in principe niet veel anders dan AutoMapper initializeren.
    /// </summary>
    [AdnrInspectorBehavior]
    public class BaseService
    {
        protected readonly ILedenManager _ledenMgr;
        protected readonly IGroepsWerkJarenManager _groepsWerkJarenMgr;
        protected readonly IAuthenticatieManager _authenticatieMgr;
        protected readonly IAbonnementenManager _abonnementenMgr;
        protected readonly IAutorisatieManager _autorisatieMgr;

        protected int AdnrCaller
        {
            get
            {
                var adnrHeader = OperationContext.Current.IncomingMessageProperties.FirstOrDefault(f => f.Key == "AdnrHeader").Value as AdnrHeader;
                return adnrHeader.AdNr;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ledenManager">LedenManager.</param>
        /// <param name="groepsWerkJarenManager">GroepsWerkJarenManager.</param>
        /// <param name="authenticatieManager">AuthenticatieManager.</param>
        /// <param name="autorisatieManager">AutorisatieManager.</param>
        /// <param name="abonnementenManager">AbonnementenManager.</param>
        public BaseService(
            ILedenManager ledenManager, 
            IGroepsWerkJarenManager groepsWerkJarenManager,
            IAuthenticatieManager authenticatieManager,
            IAutorisatieManager autorisatieManager,
            IAbonnementenManager abonnementenManager)
        {
            _ledenMgr = ledenManager;
            _groepsWerkJarenMgr = groepsWerkJarenManager;
            _abonnementenMgr = abonnementenManager;
            _authenticatieMgr = authenticatieManager;
            _autorisatieMgr = autorisatieManager;
            MappingsDefinieren();

            int a = AdnrCaller;
        }

        /// <summary>
        /// Definieert meteen alle nodige mappings.
        /// </summary>
        private void MappingsDefinieren()
        {
            var helper = new MappingHelper(_ledenMgr, _groepsWerkJarenMgr, _abonnementenMgr, _authenticatieMgr, _autorisatieMgr);
            helper.MappingsDefinieren();
        }

    }
}
