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

namespace Chiro.CiviSync.Workers
{
    /// <summary>
    /// Cache voor een aantal zeken die we geregeld nodig hebben van CiviCRM, maar die
    /// we niet iedere keer opnieuw willen ophalen.
    /// </summary>
    public interface ICiviCache
    {
        /// <summary>
        /// Haalt het contactID op van het contact met gegeven
        /// <paramref name="externalIdentifier"/>, aangenomen dat het gecachet is.
        /// </summary>
        /// <param name="externalIdentifier">External identifier van een contact.</param>
        /// <returns>Het gecachete contactID van dat contact, of <c>null</c> als het
        /// niet gecachet is.</returns>
        int? ContactIdGet(string externalIdentifier);

        /// <summary>
        /// Bewaart het contactID van een contact in de cache.
        /// </summary>
        /// <param name="externalIdentifier">External identifier van het contact met
        /// te bewaren contactID.</param>
        /// <param name="cid">Te bewaren contactID.</param>
        void ContactIdSet(string externalIdentifier, int cid);

        /// <summary>
        /// Invalideer alle gecachete data.
        /// </summary>
        void Invalideren();

        /// <summary>
        /// Haalt de ISO-code op van het land met gegeven
        /// <paramref name="countryId"/>, aangenomen dat het gecachet is.
        /// </summary>
        /// <param name="countryId">Civi-identifier van een land.</param>
        /// <returns>De gecachete ISO-code van dat contact, of <c>null</c> als ze
        /// niet gecachet is.</returns>
        string LandCodeGet(int? countryId);

        /// <summary>
        /// Bewaart de ISO-code van een land in de cache.
        /// </summary>
        /// <param name="countryId">Civi-ID van het land waarvan je de ISO-code wilt bewaren.</param>
        /// <param name="isoCode">Te bewaren ISO-code.</param>
        void LandCodeSet(int? countryId, string isoCode);
    }
}