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
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Workers
{
    public class BaseWorker
    {
        protected string ApiKey { get; private set; }
        protected string SiteKey { get; private set; }

        private readonly ServiceHelper _serviceHelper;
        private readonly IMiniLog _log;

        private const string ContactIdCacheKey = "cid{0}";
        private static readonly ObjectCache Cache = new MemoryCache("ContactWorkerCache");

        protected IMiniLog Log
        {
            get { return _log; }
        }

        protected ServiceHelper ServiceHelper
        {
            get { return _serviceHelper; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceHelper">Helper to be used for WCF service calls</param>
        /// <param name="log">Logger</param>
        public BaseWorker(ServiceHelper serviceHelper, IMiniLog log)
        {
            _serviceHelper = serviceHelper;
            _log = log;            
        }

        /// <summary>
        /// Configureer de keys voor API access.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="siteKey"></param>
        public void Configureren(string apiKey, string siteKey)
        {
            ApiKey = apiKey;
            SiteKey = siteKey;
        }

        // Contact-ID's omzetten van GAP naar Civi is overal nuttig, dus
        // dat zetten we in de base worker.

        /// <summary>
        /// Returns the contact-ID of the contact with given <paramref name="externalIdentifier"/>.
        /// </summary>
        /// <param name="externalIdentifier">Value to look for.</param>
        /// <returns>The contact-ID of the contact with given <paramref name="externalIdentifier"/>, or
        /// <c>null</c> if no such contact is found.</returns>
        /// <remarks>This method uses caching to speed up things.</remarks>
        public int? ContactIdGet(int externalIdentifier)
        {
            return ContactIdGet(externalIdentifier.ToString());
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
            int? cid = (int?)Cache[String.Format(ContactIdCacheKey, externalIdentifier)];

            if (cid != null) return cid;
            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                    svc =>
                        svc.ContactGet(ApiKey, SiteKey,
                            new ContactRequest { ExternalIdentifier = externalIdentifier, ReturnFields = "id" }));
            result.AssertValid();

            if (result.Count == 0)
            {
                return null;
            }
            var contact = result.Values.First();
            cid = contact.Id;

            Cache.Set(String.Format(ContactIdCacheKey, externalIdentifier), cid,
                new CacheItemPolicy { SlidingExpiration = new TimeSpan(2, 0, 0, 0) });
            return cid;
        }

        /// <summary>
        /// Invalideer alle gecachete data.
        /// </summary>
        public void CacheInvalideren()
        {
            // Dit is tamelijk omslachtig

            foreach (var element in Cache)
            {
                Cache.Remove(element.Key);
            }
        }
    }
}
