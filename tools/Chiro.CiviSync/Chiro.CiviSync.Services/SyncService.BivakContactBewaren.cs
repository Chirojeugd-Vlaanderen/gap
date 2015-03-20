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
using System.ServiceModel;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
	public partial class SyncService
	{
        /// <summary>
        /// Stelt de persoon met gegeven <paramref name="adNummer"/> in als contactpersoon voor
        /// het bivak met gegeven <paramref name="uitstapId"/>
        /// </summary>
        /// <param name="uitstapId">
        /// UitstapID (GAP) voor het bivak
        /// </param>
        /// <param name="adNummer">
        /// AD-nummer contactpersoon bivak
        /// </param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public async void BivakContactBewaren(int uitstapId, int adNummer)
        {
            var apiResult = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Event>>(
                svc =>
                    svc.EventGet(_apiKey, _siteKey,
                        new EventRequest
                        {
                            GapUitstapId = uitstapId,
                            ReturnFields = "id,custom_53,custom_56,custom_61",
							// Haal ook organiserende ploeg op, zodat we het stamnummer
							// hebben om te loggen.
                            ContactGetRequest = new ContactRequest
                            {
                                IdValueExpression = "$value.custom_56_id",
								ReturnFields = "external_identifier"
                            }
                        }));
            AssertValid(apiResult);
            if (apiResult.Count == 0)
            {
                _log.Loggen(Niveau.Error,
                    String.Format(
                        "Geen evenement gevonden met GAP-uitstapID {0}.",
                        uitstapId), null, null, null);
                return;
            }
            var bivak = apiResult.Values.First();
            
            if (apiResult.Count > 1)
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "Meerdere evenement gevonden met GAP-uitstapID {0}. We kiezen {1}.",
                        uitstapId, bivak.Id), null, null, null);                
            }

            int? contactIdPersoon = _contactHelper.ContactIdGet(adNummer);
            string stamNr;

            if (bivak.ContactResult.Count == 0)
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "Geen organiserende ploeg voor GAP-uitstapID {0}.",
                        uitstapId), null, null, null);
                stamNr = String.Empty;
            }
            else
            {
                stamNr = bivak.ContactResult.Values.First().ExternalIdentifier;
            }

			if (contactIdPersoon == null)
			{
			    _log.Loggen(Niveau.Error,
			        String.Format(
			            "Kan contact voor bivak met GAP-uitstapID {1} van {2} niet vastleggen: onbestaand AD-nummer {0} terug naar GAP.",
			            adNummer, uitstapId, stamNr),
			        stamNr, null, null);
                await _gapUpdateHelper.OngeldigAdNaarGap(adNummer);
                return;
            }

            if (contactIdPersoon != bivak.OrganiserendePersoon1Id)
            {
                var request = new EventRequest
                {
                    Id = bivak.Id,
                    OrganiserendePersoon1Id = contactIdPersoon
                };
                var saveResult = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Event>>(
                    svc => svc.EventSave(_apiKey, _siteKey, request));
                AssertValid(saveResult);

                _log.Loggen(Niveau.Info,
                    String.Format("Contact voor bivak van {0}: AD {1} bewaard. Gap-ID {2}, Civi-ID {3}.", stamNr,
                        adNummer,
                        uitstapId, bivak.Id), stamNr,
                    adNummer, null);
            }
            else
            {
                _log.Loggen(Niveau.Info,
                    String.Format("Contact voor bivak van {0} (AD {1}) bleef ongewijzigd. Gap-ID {2}, Civi-ID {3}.",
                        stamNr,
                        adNummer,
                        uitstapId, bivak.Id), stamNr,
                    adNummer, null);
            }
        }

        /// <summary>
        /// Stelt de persoon met gegeven <paramref name="details"/> in als contactpersoon voor
        /// het bivak met gegeven <paramref name="uitstapId"/>
        /// </summary>
        /// <param name="uitstapId">
        /// UitstapID (GAP) voor het bivak
        /// </param>
        /// <param name="details">
        /// Gegevens van de persoon
        /// </param>
        /// <remarks>
        /// Deze method mag enkel gebruikt worden als het ad-nummer van de
        /// persoon onbestaand of onbekend is.
        /// </remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void BivakContactBewarenAdOnbekend(int uitstapId, PersoonDetails details)
        {
            BivakContactBewaren(uitstapId, UpdatenOfMaken(details));
        }
	}
}