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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;

namespace Chiro.CiviSync.Workers
{
    public class CommunicatieWorker
    {
        private readonly ServiceHelper _serviceHelper;
        private string _apiKey;
        private string _siteKey;

        protected ServiceHelper ServiceHelper
        {
            get { return _serviceHelper; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceHelper">Helper to be used for WCF service calls</param>
        public CommunicatieWorker(ServiceHelper serviceHelper)
        {
            _serviceHelper = serviceHelper;
        }

        /// <summary>
        /// Configureer de keys voor API access.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="siteKey"></param>
        public void Configureren(string apiKey, string siteKey)
        {
            _apiKey = apiKey;
            _siteKey = siteKey;
        }

        /// <summary>
        /// Verwijdert alle e-mail van contact met gegeven <paramref name="contactId"/>.
        /// </summary>
        /// <param name="contactId"></param>
        public void AlleEmailVerwijderen(int contactId)
        {
            var alleEmail =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Email>>(
                    svc => svc.EmailGet(_apiKey, _siteKey, new EmailRequest {ContactId = contactId}));
            alleEmail.AssertValid();

            // Verwijderen moet één voor één, denk ik.
            foreach (var email in alleEmail.Values)
            {
                int emailId = email.Id;
                var deleteResult =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                        svc => svc.EmailDelete(_apiKey, _siteKey, new IdRequest(emailId)));
                deleteResult.AssertValid();
            }
        }

        /// <summary>
        /// Verwijdert alle telefoon- en faxnummers van contact met gegeven <paramref name="contactId"/>.
        /// </summary>
        /// <param name="contactId"></param>
        public void AlleTelefoonEnFaxVerwijderen(int contactId)
        {
            var alles =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Phone>>(
                    svc => svc.PhoneGet(_apiKey, _siteKey, new PhoneRequest { ContactId = contactId }));
            alles.AssertValid();

            // Verwijderen moet één voor één, denk ik.
            foreach (var teVerwijderen in alles.Values)
            {
                int id = teVerwijderen.Id;
                var deleteResult =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                        svc => svc.PhoneDelete(_apiKey, _siteKey, new IdRequest(id)));
                deleteResult.AssertValid();
            }
        }

        /// <summary>
        /// Verwijdert alle websites van contact met gegeven <paramref name="contactId"/>.
        /// </summary>
        /// <param name="contactId"></param>
        public void AlleWebsitesVerwijderen(int contactId)
        {
            var alles =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Website>>(
                    svc => svc.WebsiteGet(_apiKey, _siteKey, new WebsiteRequest { ContactId = contactId }));
            alles.AssertValid();

            // Verwijderen moet één voor één, denk ik.
            foreach (var teVerwijderen in alles.Values)
            {
                int id = teVerwijderen.Id;
                var deleteResult =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                        svc => svc.WebsiteDelete(_apiKey, _siteKey, new IdRequest(id)));
                deleteResult.AssertValid();
            }
        }

        /// <summary>
        /// Verwijdert alle IM van contact met gegeven <paramref name="contactId"/>.
        /// </summary>
        /// <param name="contactId"></param>
        public void AlleImVerwijderen(int contactId)
        {
            var alles =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Im>>(
                    svc => svc.ImGet(_apiKey, _siteKey, new ImRequest { ContactId = contactId }));
            alles.AssertValid();

            // Verwijderen moet één voor één, denk ik.
            foreach (var teVerwijderen in alles.Values)
            {
                int id = teVerwijderen.Id;
                var deleteResult =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                        svc => svc.ImDelete(_apiKey, _siteKey, new IdRequest(id)));
                deleteResult.AssertValid();
            }
        }
    }
}
