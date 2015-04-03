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
using Chiro.CiviSync.Helpers;
using Chiro.CiviSync.Logic;
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
            Event bivak;
            string stamNr;
            
            ValideerBivak(uitstapId, out bivak, out stamNr);
            if (bivak == null)
            {
                return;
            }

            int? contactIdPersoon = _contactWorker.ContactIdGet(adNummer);
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
                saveResult.AssertValid();;

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
        /// Controleert of het bivak met gegeven <paramref name="uitstapId"/> bestaat,
        /// of het uniek is, en of het een organiserende ploeg heeft. Logt fouten
        /// als dat niet het geval is.
        /// Als er een bivak is gevonden dan wordt dat opgeleverd in
        /// <paramref name="bivak"/>, en bevat <paramref name="stamNr"/> het stamnummer
        /// van de organiserende ploeg.
        /// </summary>
        /// <param name="uitstapId">Een GAP-uitstapID</param>
        /// <param name="bivak">out-parameter voor gevonden bivak, met gekoppelde
        /// organiserende ploeg en adres.</param>
        /// <param name="stamNr">out-parameter voor stamnummer van organisator</param>
	    private void ValideerBivak(int uitstapId, out Event bivak, out string stamNr)
        {
            bivak = _bivakWorker.BivakDetailsOphalen(uitstapId);

            if (bivak == null)
	        {
	            _log.Loggen(Niveau.Error,
	                String.Format(
	                    "Geen evenement gevonden met GAP-uitstapID {0}.",
	                    uitstapId), null, null, null);
	            stamNr = String.Empty;
	            return;
	        }

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