/*
   Copyright 2013-2015 Chirojeugd-Vlaanderen vzw

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
using AutoMapper;
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
        /// Probeert een persoon te vinden op basis van persoonsgegevens, adressen en communicatie.
        /// Als dat lukt, worden de meegegeven persoonsgegevens, adressen en communicatie overgenomen 
        /// in de CiviCRM. Als er niemand gevonden is, dan wordt een nieuwe persoon aangemaakt.
        /// </summary>
        /// <param name="details">details voor te updaten/maken persoon</param>
        /// <returns>AD-nummer van die persoon</returns>
        /// <remarks>
        /// UpdatenOfMaken logt rariteiten zoals een AD-nummer dat al bestaat
        /// of een persoon zonder voornaam.
        /// </remarks>
        private int UpdatenOfMaken(PersoonDetails details)
        {
            if (details.Persoon.AdNummer != null)
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "UpdatenOfMaken aangeroepen voor persoon {0} {1} (gid {3}) met bestaand AD-Nummer {2}."
                        , details.Persoon.VoorNaam, details.Persoon.Naam, details.Persoon.AdNummer, details.Persoon.ID),
                    null, details.Persoon.AdNummer, details.Persoon.ID);
            }
            if (String.IsNullOrEmpty(details.Persoon.VoorNaam))
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "NieuwLidBewaren voor persoon zonder voornaam: {0} (gid {1} ad {2})."
                        , details.Persoon.Naam, details.Persoon.ID, details.Persoon.AdNummer),
                    null, details.Persoon.AdNummer, details.Persoon.ID);
            }

            int? adNummer = AdNummerZoeken(details);

            if (adNummer != null)
            {
                _log.Loggen(Niveau.Debug,
                    String.Format("Persoonsgegevens {0} {1} (gid {2}) matchten met bestaande persoon (ad {3})",
                        details.Persoon.VoorNaam, details.Persoon.Naam, details.Persoon.ID, adNummer), null, adNummer,
                    details.Persoon.ID);

                // Bewaar GAP-ID
                var request = new ContactRequest
                {
                    ContactType = ContactType.Individual,
                    ExternalIdentifier = adNummer.ToString(),
                    GapId = details.Persoon.ID,
                    ApiOptions = new ApiOptions {Match = "external_identifier"}
                };

                var saveResult = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                    svc => svc.ContactSave(_apiKey, _siteKey, request));
                saveResult.AssertValid();
                return adNummer.Value;
            }

            var contactRequest = Mapper.Map<Persoon, ContactRequest>(details.Persoon);

            CommunicatieLogic.RequestsChainen(contactRequest, details.Communicatie);

            var address = Mapper.Map<Adres, AddressRequest>(details.Adres);
            address.LocationTypeId = AdresLogic.CiviLocationTypeId(details.AdresType);
            contactRequest.AddressSaveRequest = new List<AddressRequest> { address };

            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                    svc => svc.ContactSave(_apiKey, _siteKey, contactRequest));

            if (result.IsError == 0)
            {
                // Normaalgezien krijgen we het AD-nummer mee terug als external identifier.
                adNummer = int.Parse(result.Values.First().ExternalIdentifier);
            }
            else
            {
                // Workaround CRM-15815, zie #3405.
                ServiceHelper.CallService<ICiviCrmApi, EmptyResult>(
                    svc => svc.ContactSaveWorkaroundCrm15815(_apiKey, _siteKey, contactRequest));

                // Haal AD-nummer op door nieuwe call.
                var result2 = ServiceHelper.CallService<ICiviCrmApi, Contact>(
                    svc =>
                        svc.ContactGetSingle(_apiKey, _siteKey,
                            new ContactRequest
                            {
                                ContactType = ContactType.Individual,
                                GapId = details.Persoon.ID,
                                ReturnFields = "external_identifier"
                            }));
                adNummer = int.Parse(result2.ExternalIdentifier);
            }

            _log.Loggen(Niveau.Debug,
                String.Format("Nieuwe persoon gemaakt: {0} {1} (gid {2} ad {3})",
                    details.Persoon.VoorNaam, details.Persoon.Naam, details.Persoon.ID, adNummer), null, adNummer,
                details.Persoon.ID);

            return adNummer.Value;
        }
    }
}