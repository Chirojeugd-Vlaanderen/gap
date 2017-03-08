/*
 * Copyright 2017 the GAP developers. See the NOTICE file at the 
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
using System.Web;
using System.Web.Caching;
using Chiro.Ad.ServiceContracts;

namespace Chiro.Cdf.Authentication.Ad
{
    /// <summary>
    /// Authenticatie tegen Active Directory, via de AD-service.
    /// </summary>
    public class AdAuthenticator: IAuthenticator
    {
        private readonly ServiceHelper.ServiceHelper _serviceHelper;
        protected ServiceHelper.ServiceHelper ServiceHelper
        {
            get { return _serviceHelper; }
        }

        public AdAuthenticator(ServiceHelper.ServiceHelper serviceHelper)
        {
            _serviceHelper = serviceHelper;
        }

        public UserInfo WieBenIk()
        {
            var cache = HttpRuntime.Cache;
            string gebruikersNaam = HttpContext.Current.User.Identity.Name;
            string cacheKey = string.Format("AUTH_{0}", gebruikersNaam);
            int ? adNummer = (int?)cache.Get(cacheKey);

            if (adNummer == null)
            {
                adNummer = ServiceHelper.CallService<IAdService, int?>(svc => svc.AdNummerOphalen(gebruikersNaam));

                cache.Add(
                    cacheKey,
                    adNummer,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(8, 0, 0),
                    CacheItemPriority.Normal,
                    null);
            }
            return new UserInfo {AdNr = adNummer.Value};
        }
    }
}
