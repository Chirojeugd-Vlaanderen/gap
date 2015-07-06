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

using System.Linq;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Workers
{
    public class CommunicatieWorker: BaseWorker
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceHelper">Helper to be used for WCF service calls</param>
        /// <param name="log">logger object</param>
        public CommunicatieWorker(ServiceHelper serviceHelper, IMiniLog log) : base(serviceHelper, log)
        {
        }

        /// <summary>
        /// Verwijdert alle e-mail van contact met gegeven <paramref name="contactId"/>.
        /// </summary>
        /// <param name="contactId"></param>
        public void AlleEmailVerwijderen(int contactId)
        {
            var alleEmail =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Email>>(
                    svc => svc.EmailGet(ApiKey, SiteKey, new EmailRequest {ContactId = contactId}));
            alleEmail.AssertValid();

            // Verwijderen moet één voor één, denk ik.
            foreach (var deleteResult in alleEmail.Values.Select(email => email.Id).Select(emailId => ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                svc => svc.EmailDelete(ApiKey, SiteKey, new IdRequest(emailId)))))
            {
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
                    svc => svc.PhoneGet(ApiKey, SiteKey, new PhoneRequest { ContactId = contactId }));
            alles.AssertValid();

            // Verwijderen moet één voor één, denk ik.
            foreach (var deleteResult in alles.Values.Select(teVerwijderen => teVerwijderen.Id).Select(id => ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                svc => svc.PhoneDelete(ApiKey, SiteKey, new IdRequest(id)))))
            {
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
                    svc => svc.WebsiteGet(ApiKey, SiteKey, new WebsiteRequest { ContactId = contactId }));
            alles.AssertValid();

            // Verwijderen moet één voor één, denk ik.
            foreach (var deleteResult in alles.Values.Select(teVerwijderen => teVerwijderen.Id).Select(id => ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                svc => svc.WebsiteDelete(ApiKey, SiteKey, new IdRequest(id)))))
            {
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
                    svc => svc.ImGet(ApiKey, SiteKey, new ImRequest { ContactId = contactId }));
            alles.AssertValid();

            // Verwijderen moet één voor één, denk ik.
            foreach (var deleteResult in alles.Values.Select(teVerwijderen => teVerwijderen.Id).Select(id => ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                svc => svc.ImDelete(ApiKey, SiteKey, new IdRequest(id)))))
            {
                deleteResult.AssertValid();
            }
        }
    }
}
