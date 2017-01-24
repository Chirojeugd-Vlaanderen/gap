/*
 * Copyright 2016, 2017 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using Chiro.Gap.ServiceContracts;
using Chiro.Gap.WorkerInterfaces;
using System.Diagnostics;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Service  met hacks voor dev- en testomgeving.
    /// </summary>
    /// <remarks>
    /// DEZE FILE MAG NIET AANWEZIG ZIJN IN DE CODE VOOR DE LIVE-OMGEVING!
    /// </remarks>
    public class DbHacksService : BaseService, IDbHacksService
    {
        public DbHacksService(
            ILedenManager ledenManager,
            IGroepsWerkJarenManager groepsWerkJarenManager,
            IAuthenticatieManager authenticatieManager,
            IAutorisatieManager autorisatieManager,
            IAbonnementenManager abonnementenManager): base(ledenManager, groepsWerkJarenManager, authenticatieManager, autorisatieManager, abonnementenManager)
        { }

        /// <summary>
        /// Erg lelijke hack die direct in de database schrijft om de aangelogde gebruiker
        /// toegang te geven tot een testgroep.
        /// </summary>
        public void WillekeurigeGroepToekennen()
        {
            Debug.Assert(_authenticatieMgr.AdNummerGet().HasValue);
            // Eigenlijk moet de autorisatiemanager de rechten controleren.
            Debug.Assert(_authenticatieMgr.IsDeveloper());
            TestHacks.TestHacks.TestGroepToevoegen(_authenticatieMgr.AdNummerGet().Value);
        }
    }
}
