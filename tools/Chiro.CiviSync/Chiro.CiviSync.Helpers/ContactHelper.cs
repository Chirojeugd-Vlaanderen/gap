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
using System.Linq;
using System.Runtime.Caching;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;

namespace Chiro.CiviSync.Helpers
{
    /// <summary>
    /// Contactgerelateerde methods die hopelijk interessant zijn voor de gebruikers
    /// van de CiviCRM-API.
    /// </summary>
    public class ContactHelper
    {
        private const string ContactIdCacheKey = "cid{0}";

        private readonly ObjectCache _cache = new MemoryCache("HelperCache");
        private readonly ServiceHelper _serviceHelper;
        private readonly string _apiKey;
        private readonly string _siteKey;

        protected ServiceHelper ServiceHelper
        {
            get { return _serviceHelper; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceHelper">Helper to be used for WCF service calls</param>
        /// <param name="apiKey">Key of the API-user</param>
        /// <param name="siteKey">Key of the CiviCRM-instance you want to access</param>
        public ContactHelper(ServiceHelper serviceHelper, string apiKey, string siteKey)
        {
            _serviceHelper = serviceHelper;
            _apiKey = apiKey;
            _siteKey = siteKey;
        }

        /// <summary>
        /// Returns the contact-ID of the contact with given <paramref name="externalIdentifier"/>.
        /// </summary>
        /// <param name="externalIdentifier">Value to look for.</param>
        /// <returns>The contact-ID of the contact with given <paramref name="externalIdentifier"/>, or
        /// <c>null</c> if no such contact is found.</returns>
        /// <remarks>This method uses caching to speed up things.</remarks>
        public int? ContactIdGet(string externalIdentifier)
        {
            int? cid = (int?) _cache[String.Format(ContactIdCacheKey, externalIdentifier)];

            if (cid == null)
            {
                var result =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                        svc =>
                            svc.ContactGet(_apiKey, _siteKey,
                                new ContactRequest {ExternalIdentifier = externalIdentifier, ReturnFields = "id"}));

                if (result == null || result.Count != 1)
                {
                    return null;
                }

                cid = result.Values.First().Id;
                if (cid == null)
                {
                    return null;
                }

                _cache.Set(String.Format(ContactIdCacheKey, externalIdentifier), cid,
                    new CacheItemPolicy {SlidingExpiration = new TimeSpan(2, 0, 0, 0)});
            }
            return cid;
        }
    }
}
