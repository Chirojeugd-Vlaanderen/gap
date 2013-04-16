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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Klasse om veel gebruikte zaken mee op te vragen, die dan ook gecachet kunnen worden.
    /// Enkel als iets niet gecachet is, wordt de data access aangeroepen
    /// </summary>
    public class VeelGebruikt : IVeelGebruikt
    {
        private const string GROEPIDCACHEKEY = "gid_{0}";

        private readonly Cache _cache = HttpRuntime.Cache;

        /// <summary>
        /// Haalt het groepID van de groep met gegeven stamnummer op uit de cache.
        /// </summary>
        /// <param name="code">
        /// Stamnummer van groep waarvan groepID te bepalen is
        /// </param>
        /// <returns>
        /// GroepID van de groep met stamnummer <paramref name="code"/>.
        /// </returns>
        public int CodeNaarGroepID(string code)
        {
            var groepID = (int?)_cache.Get(string.Format(GROEPIDCACHEKEY, code));

            if (groepID == null || groepID == 0)
            {
                throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
                //groepID = _groepenDao.Ophalen(code).ID;

                //_cache.Add(
                //    string.Format(GROEPIDCACHEKEY, code),
                //    groepID,
                //    null,
                //    Cache.NoAbsoluteExpiration,
                //    new TimeSpan(2, 0, 0),
                //    CacheItemPriority.Normal,
                //    null);
            }

            return groepID ?? 0;
        }

        /// <summary>
        /// Haalt het beginjaar van het huidig werkjaar van de groep met gegeven <paramref name="groepID"/> op.
        /// (Bijv. 2012 voor 2012-2013)
        /// </summary>
        /// <param name="groepID">ID van de groep, waarvan het werkjaar opgezocht moet worden</param>
        /// <returns>
        /// het beginjaar van het huidig werkjaar van de groep met gegeven <paramref name="groepID"/>.
        /// (Bijv. 2012 voor 2012-2013)
        /// </returns>
        public int WerkJaarOphalen(int groepID)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }
    }
}