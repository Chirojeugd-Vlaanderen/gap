﻿/*
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
using System.ServiceModel;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Zet naam en adres van groep <paramref name="g"/> over naar CiviCRM.
        /// </summary>
        /// <param name="g"></param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void GroepUpdaten(Groep g)
        {
            // Haal groep op met alle adressen. Die zijn in theorie enkel nodig als we een adres moeten vervangen.
            // Maar omdat het wijzigen van groepsgegevens niet zo frequent voorkomt, kan dat niet zo'n kwaad.

            var getRequest = new ContactRequest
            {
                ExternalIdentifier = g.Code,
                ContactType = ContactType.Organization,
                AddressGetRequest = new AddressRequest()
            };
            var getResult =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                    svc => svc.ContactGet(_apiKey, _siteKey, getRequest));

            int? civiContactId = getResult.Id;

            if (civiContactId == null)
            {
                _log.Loggen(Niveau.Error, string.Format("Onbestaande groep {0} - gegevens niet aangepast.", g.Code),
                    g.Code, null, null);
                return;
            }
            var request = new ContactRequest
            {
                Id = civiContactId,
                OrganizationName = g.Naam,
                // Vroeger gebruikten we in Chirocivi vooral legal_name, maar dat was fout. Intussen gebruiken
                // we overal organization_name, maar voor de veiligheid laat ik onderstaande lijn nog staan.
                LegalName = g.Naam
            };
            if (g.Adres != null)
            {
                foreach (var addres in getResult.Values.First().AddressResult.Values)
                {
                    // Ik had dit liever met 1 chained call gedaan, maar dat ondersteunt Chiro.CiviCrm.Wcf niet.
                    ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                        svc => svc.AddressDelete(_apiKey, _siteKey, new DeleteRequest(addres.Id)));
                }
                request.AddressSaveRequest = new[] {MappingHelper.Map<Adres, AddressRequest>(g.Adres)};
            }
            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResult>(svc => svc.ContactSave(_apiKey, _siteKey, request));
            result.AssertValid();

            if (g.Adres != null)
            {
                _log.Loggen(Niveau.Info,
                    string.Format("Groep {0} bijgwerkt - naam: {1}, adres {2}, {3} {4}", g.Code, g.Naam,
                        AdresLogic.StraatNrBus(g.Adres), g.Adres.PostNr, g.Adres.WoonPlaats), g.Code, null, null);
            }
            else
            {
                _log.Loggen(Niveau.Info,
                    string.Format("Groep {0} bijgwerkt - naam: {1}", g.Code, g.Naam), g.Code, null, null);
            }
        }
    }
}