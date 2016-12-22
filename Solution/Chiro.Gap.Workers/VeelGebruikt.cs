/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkte authenticatie Copyright 2014 Johan Vervloet
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
using System.Linq;
using System.Web;
using System.Web.Caching;
using Chiro.Ad.ServiceContracts;
using Chiro.Cdf.ServiceHelper;
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
        private const string WerkJaarCacheKey = "{0}_wj";
        private const string AdNrCacheKey = "{0}_ad";
        private const string UserNameCacheKey = "{0}_user";

        private readonly Cache _cache = HttpRuntime.Cache;
        private readonly ServiceHelper _serviceHelper;

        protected ServiceHelper ServiceHelper
        {
            get { return _serviceHelper; }
        }

        /// <summary>
        /// Constructor voor dependency injection.
        /// </summary>
        /// <param name="serviceHelper">Interface om webservices aan te roepen</param>
        public VeelGebruikt(ServiceHelper serviceHelper)
        {
            _serviceHelper = serviceHelper;
        }

        /// <summary>
        /// Haalt het beginjaar van het huidig werkjaar van de gegeven <paramref name="groep"/> op.
        /// (Bijv. 2012 voor 2012-2013)
        /// </summary>
        /// <param name="groep">groep waarvan het werkjaar opgezocht moet worden</param>
        /// <returns>
        /// het beginjaar van het huidig werkjaar van de <paramref name="groep"/>.
        /// (Bijv. 2012 voor 2012-2013)
        /// </returns>
        public int WerkJaarOphalen(Groep groep)
        {
            int? werkJaar = (int?)_cache.Get(string.Format(WerkJaarCacheKey, groep.ID));

            if (werkJaar == null)
            {
                werkJaar = (from gwj in groep.GroepsWerkJaar
                            orderby gwj.WerkJaar descending
                            select gwj.WerkJaar).First();

                _cache.Add(
                    string.Format(WerkJaarCacheKey, groep.ID),
                    werkJaar,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(2, 0, 0),
                    CacheItemPriority.Normal,
                    null);
            }

            return werkJaar.Value;
        }

        /// <summary>
        /// Invalideert het gecachete huidige werkjaar van de gegeven <paramref name="groep"/>
        /// </summary>
        /// <param name="groep">Groep waarvan gecachete werkjaar moet worden geinvalideerd.</param>
        public void WerkJaarInvalideren(Groep groep)
        {
            _cache.Remove(string.Format(WerkJaarCacheKey, groep.ID));
        }

        /// <summary>
        /// Haalt het AD-nummer op van de user met gegeven <paramref name="gebruikersNaam"/>.
        /// </summary>
        /// <param name="gebruikersNaam">Een gebruikersnaam.</param>
        /// <returns>Het AD-nummer van de user met die gebruikersnaam.</returns>
        public int? AdNummerOphalen(string gebruikersNaam)
        {
            int? adNummer = (int?)_cache.Get(string.Format(AdNrCacheKey, gebruikersNaam));

            if (adNummer == null)
            {
                adNummer = ServiceHelper.CallService<IAdService, int?>(svc => svc.AdNummerOpHalen(gebruikersNaam));

                _cache.Add(
                    string.Format(AdNrCacheKey, gebruikersNaam),
                    adNummer,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(8, 0, 0),
                    CacheItemPriority.Normal,
                    null);
            }

            return adNummer.Value;
        }

        /// <summary>
        /// Invalideert het gecachete AD-nummer voor de gebruiker met gegeven <paramref name="gebruikersNaam"/>.
        /// </summary>
        /// <param name="gebruikersNaam">Gebruikersnaam van gebruiker waarvan gecachete AD-nummer
        /// geinvalideerd moet worden.</param>
        public void AdNummerInvalideren(string gebruikersNaam)
        {
            _cache.Remove(string.Format(AdNrCacheKey, gebruikersNaam));
        }

        /// <summary>
        /// Vraagt de gebruikersnaam op van de persoon met gegeven <paramref name="adNummer"/>.
        /// </summary>
        /// <param name="adNummer">AD-nummer van persoon wiens gebruikersnaam gezocht is.</param>
        /// <returns>De gebruikersnaam van de persoon met gegeven AD-nummer.</returns>
        public string GebruikersNaamOphalen(int adNummer)
        {
            string gebruikersNaam = (string)_cache.Get(string.Format(UserNameCacheKey, adNummer));

            if (String.IsNullOrEmpty(gebruikersNaam))
            {
                gebruikersNaam = ServiceHelper.CallService<IAdService, string>(svc => svc.gebruikersNaamOphalen(adNummer));

                _cache.Add(
                    string.Format(UserNameCacheKey, adNummer),
                    gebruikersNaam,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(4, 0, 0),
                    CacheItemPriority.Normal,
                    null);
            }

            return gebruikersNaam;
        }

        public void GebruikersNaamInvalideren(int? adNummer)
        {
            _cache.Remove(string.Format(UserNameCacheKey, adNummer));
        }
    }
}