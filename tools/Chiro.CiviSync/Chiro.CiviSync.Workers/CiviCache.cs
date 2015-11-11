/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Runtime.Caching;

namespace Chiro.CiviSync.Workers
{
    /// <summary>
    /// Cache voor een aantal zeken die we geregeld nodig hebben van CiviCRM, maar die
    /// we niet iedere keer opnieuw willen ophalen.
    /// </summary>
    public class CiviCache : ICiviCache
    {
        private const string ContactIdCacheKey = "cid{0}";
        private const string LandCodeCacheKey = "iso{0}";

        private static readonly ObjectCache Cache = new MemoryCache("CiviSyncCache");

        /// <summary>
        /// Haalt het contactID op van het contact met gegeven
        /// <paramref name="externalIdentifier"/>, aangenomen dat het gecachet is.
        /// </summary>
        /// <param name="externalIdentifier">External identifier van een contact.</param>
        /// <returns>Het gecachete contactID van dat contact, of <c>null</c> als het
        /// niet gecachet is.</returns>
        public int? ContactIdGet(string externalIdentifier)
        {
            return (int?)Cache[string.Format(ContactIdCacheKey, externalIdentifier)];
        }

        /// <summary>
        /// Bewaart het contactID van een contact in de cache.
        /// </summary>
        /// <param name="externalIdentifier">External identifier van het contact met
        /// te bewaren contactID.</param>
        /// <param name="cid">Te bewaren contactID.</param>
        public void ContactIdSet(string externalIdentifier, int cid)
        {
            Cache.Set(string.Format(ContactIdCacheKey, externalIdentifier), cid,
                new CacheItemPolicy {SlidingExpiration = new TimeSpan(2, 0, 0, 0)});
        }

        /// <summary>
        /// Invalideer alle gecachete data.
        /// </summary>
        public void Invalideren()
        {
            // Dit is tamelijk omslachtig

            foreach (var element in Cache)
            {
                Cache.Remove(element.Key);
            }
        }

        /// <summary>
        /// Haalt de ISO-code op van het land met gegeven
        /// <paramref name="countryId"/>, aangenomen dat het gecachet is.
        /// </summary>
        /// <param name="countryId">Civi-identifier van een land.</param>
        /// <returns>De gecachete ISO-code van dat contact, of <c>null</c> als ze
        /// niet gecachet is.</returns>
        public string LandCodeGet(int? countryId)
        {
            return (string)Cache[string.Format(LandCodeCacheKey, countryId)];
        }

        /// <summary>
        /// Bewaart de ISO-code van een land in de cache.
        /// </summary>
        /// <param name="countryId">Civi-ID van het land waarvan je de ISO-code wilt bewaren.</param>
        /// <param name="isoCode">Te bewaren ISO-code.</param>
        public void LandCodeSet(int? countryId, string isoCode)
        {
            Cache.Set(string.Format(LandCodeCacheKey, countryId), isoCode,
                new CacheItemPolicy {SlidingExpiration = new TimeSpan(2, 0, 0, 0)});
        }
    }
}
