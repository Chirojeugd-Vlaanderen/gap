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
        private const string GROEPSWERKJAARCACHEKEY = "gwj{0}";
        private const string NATIONALEFUNCTIESCACHEKEY = "natfun";
        private const string GROEPIDCACHEKEY = "gid_{0}";

        private readonly Cache _cache = HttpRuntime.Cache;

        /// <summary>
        /// Verwijdert het recentste groepswerkjaar van groep met ID <paramref name="groepID"/>
        /// uit de cache.
        /// </summary>
        /// <param name="groepID">
        /// ID van de groep waarvan groepswerkjaarcache te resetten
        /// </param>
        public void GroepsWerkJaarResetten(int groepID)
        {
            _cache.Remove(string.Format(GROEPSWERKJAARCACHEKEY, groepID));
        }

        /// <summary>
        /// Haalt van de groep met gegeven <paramref name="groepID"/> het ID van het recentste groepswerkjaar op.
        /// </summary>
        /// <param name="groepID">
        ///     ID van de groep waarvan groepswerkjaarID gevraagd
        /// </param>
        /// <param name="groepenRepo">groepenrepository, via dewelke het groepswerkjaar indien nodig opgehaald
        /// kan worden.</param>
        /// <returns>
        /// Het ID van het recentste groespwerkjaar van de groep
        /// </returns>
        public int GroepsWerkJaarIDOphalen(int groepID, IRepository<Groep> groepenRepo)
        {
            int? gwjID = (int?)_cache.Get(string.Format(GROEPSWERKJAARCACHEKEY, groepID));

            if (gwjID == null)
            {
                gwjID = groepenRepo.ByID(groepID).GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First().ID;

                _cache.Add(
                    string.Format(GROEPSWERKJAARCACHEKEY, groepID),
                    gwjID,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(2, 0, 0),
                    CacheItemPriority.Normal,
                    null);
            }

            return gwjID.Value;
        }

        /// <summary>
        /// Haalt alle nationale functies op
        /// </summary>
        /// <param name="functieRepo">Repository die deze worker kan gebruiken om functies
        ///     op te vragen.</param>
        /// <returns>
        /// Lijstje nationale functies
        /// </returns>
        /// <remarks>
        /// De repository wordt bewust niet geregeld door de constructor van deze klasse,
        /// omdat we moeten vermijden dat de IOC-container hiervoor een nieuwe context aanmaakt.
        /// </remarks>
        public List<Functie> NationaleFunctiesOphalen(IRepository<Functie> functieRepo)
        {
            if (_cache[NATIONALEFUNCTIESCACHEKEY] == null)
            {
                var nationaleFuncties = from fn in functieRepo.Select()
                                        where fn.IsNationaal
                                        select fn;

                _cache.Add(
                    NATIONALEFUNCTIESCACHEKEY,
                    nationaleFuncties.ToList(),
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(1, 0, 0, 0) /* bewaar 1 dag */,
                    CacheItemPriority.Low,
                    null);
            }

            return _cache[NATIONALEFUNCTIESCACHEKEY] as List<Functie>;
        }

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