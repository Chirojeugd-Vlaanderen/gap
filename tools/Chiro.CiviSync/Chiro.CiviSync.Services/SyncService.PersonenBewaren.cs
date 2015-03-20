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
using Chiro.CiviCrm.Api.DataContracts.EntityRequests;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Helpers;
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

                var request = new ContactRequest
                {
                    ContactType = ContactType.Individual,
                    ExternalIdentifier = adNummer.ToString(),
                    GapId = details.Persoon.ID,
                    ApiOptions = new ApiOptions {Match = "external_identifier"}
                };

                var saveResult = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                    svc => svc.ContactSave(_apiKey, _siteKey, request));
                AssertValid(saveResult);

                return adNummer.Value;
            }

            var contact = Mapper.Map<Persoon, ContactRequest>(details.Persoon);
            var address = Mapper.Map<Adres, AddressRequest>(details.Adres);
            address.LocationTypeId = MappingHelper.CiviLocationTypeId(details.AdresType);

            var civiCommunicatie = MappingHelper.CiviCommunicatie(details.Communicatie.ToList(), null);

            contact.AddressSaveRequest = new List<AddressRequest> {address};
            contact.PhoneSaveRequest = civiCommunicatie.OfType<Phone>();
            contact.EmailSaveRequest = civiCommunicatie.OfType<Email>();
            contact.WebsiteSaveRequest = civiCommunicatie.OfType<Website>();
            contact.ImSaveRequest = civiCommunicatie.OfType<Im>();

            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                    svc => svc.ContactSave(_apiKey, _siteKey, contact));

            if (result.IsError == 0)
            {
                // Normaalgezien krijgen we het AD-nummer mee terug als external identifier.
                adNummer = int.Parse(result.Values.First().ExternalIdentifier);
            }
            else
            {
                // Workaround CRM-15815, zie #3405.
                ServiceHelper.CallService<ICiviCrmApi, EmptyResult>(
                    svc => svc.ContactSaveWorkaroundCrm15815(_apiKey, _siteKey, contact));

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

        /// <summary>
        /// Zoek het AD-nummer van een persoon die lijkt op een persoon met gegeven <paramref name="details"/>.
        /// </summary>
        /// <param name="details">Informatie die moet toelaten het AD-nummer te vinden.</param>
        /// <returns>Het gevraagde AD-nummer, of <c>null</c> als er niemand gevonden werd die voldoende
        /// overeen kwam met <paramref name="details"/>.</returns>
        /// <remarks>Matchen gebeurt als volgt:
        /// 1. op basis van AD-nummer
        /// 2. op basis van GAP-ID
        /// 3. op basis van naam, geslacht en communicatievorm
        /// 4. op basis van naam, geslacht en geboortedatum
        /// </remarks>
        private int? AdNummerZoeken(PersoonDetails details)
        {
            if (details.Persoon.AdNummer != null)
            {
                // Het lijkt misschien een beetje gek als je het AD-nummer zoekt van
                // iemand waarvan je het AD-nummer al hebt, maar dit vangt het geval op
                // dat het AD-nummer niet (meer) bestaat in Civi.

                var request = new ContactRequest
                {
                    ExternalIdentifier = details.Persoon.AdNummer.ToString(),
                    ReturnFields = "external_identifier"
                };
                var result =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                        svc => svc.ContactGet(_apiKey, _siteKey, request));
                AssertValid(result);
                if (result.Count >= 1)
                {
                    return int.Parse(result.Values.First().ExternalIdentifier);
                }
                
                // AD-nummer niet gevonden in Civi. Doe er iets mee.
                _log.Loggen(Niveau.Error,
                    String.Format("AD-nummer {0} van {1} {2} niet gevonden.", details.Persoon.AdNummer,
                        details.Persoon.VoorNaam, details.Persoon.Naam), null, details.Persoon.AdNummer,
                    details.Persoon.ID);
            }

            // Probeer op GAP-ID

            if (details.Persoon.ID > 0)
            {
                var request = new ContactRequest
                {
                    GapId = details.Persoon.ID,
                    ReturnFields = "external_identifier"
                };
                var result =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                        svc => svc.ContactGet(_apiKey, _siteKey, request));
                AssertValid(result);
                if (result.Count >= 1)
                {
                    return int.Parse(result.Values.First().ExternalIdentifier);
                }
            }

            // Haal eens iedereen op met hetzelfde geslacht en dezelfde naam.

            var nameGenderRequest = new ContactRequest
            {
                FirstName = details.Persoon.VoorNaam,
                LastName = details.Persoon.Naam,
                ContactType = ContactType.Individual,
                Gender = (Gender) (3 - (int) details.Persoon.Geslacht),
                AddressGetRequest = new AddressRequest(),
                EmailGetRequest = new BaseRequest(),
                PhoneGetRequest = new BaseRequest(),
                WebsiteGetRequest = new BaseRequest(),
                ImGetRequest = new BaseRequest()
            };

            var contactResult  =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                    svc => svc.ContactGet(_apiKey, _siteKey, nameGenderRequest));

            // Zoek op telefoonnummer of fax.
            var gevondenViaTelefoonNr =
                (from c in
                    contactResult.Values
                    where
                        c.PhoneResult.Values.Any(nr =>
                            CommunicatieHelper.GeldigNummer(nr.PhoneNumber) &&
                            CommunicatieHelper.StandaardNummer(
                                details.Communicatie.Where(
                                    cm => cm.Type == CommunicatieType.TelefoonNummer || cm.Type == CommunicatieType.Fax)
                                    .Select(cm => cm.Waarde))
                                .Contains(CommunicatieHelper.StandaardNummer(nr.PhoneNumber)))
                    select c).FirstOrDefault();
            if (gevondenViaTelefoonNr != null)
            {
                return int.Parse(gevondenViaTelefoonNr.ExternalIdentifier);
            }

            // Zoek op e-mail
            var gevondenViaEmail = (from c in contactResult.Values
                where
                    c.EmailResult.Values.Any(
                        em =>
                            details.Communicatie.Where(cm => cm.Type == CommunicatieType.Email)
                                .Select(cm => cm.Waarde.ToLower())
                                .Contains(em.EmailAddress.ToLower()))
                select c).FirstOrDefault();
            if (gevondenViaEmail != null)
            {
                return int.Parse(gevondenViaEmail.ExternalIdentifier);
            }

            // Zoek op website
            var gevondenViaWebsite = (from c in contactResult.Values
                where
                    c.WebsiteResult.Values.Any(
                        ws =>
                            CommunicatieHelper.StandaardUrl(details.Communicatie.Where(
                                cm =>
                                    cm.Type == CommunicatieType.WebSite || cm.Type == CommunicatieType.Twitter ||
                                    cm.Type == CommunicatieType.StatusNet)
                                .Select(cm => cm.Waarde))
                                .Contains(CommunicatieHelper.StandaardUrl(ws.Url)))
                select c).FirstOrDefault();
            if (gevondenViaWebsite != null)
            {
                return int.Parse(gevondenViaWebsite.ExternalIdentifier);
            }

            // Zoek op IM
            var gevondenViaIm = (from c in contactResult.Values
                where
                    c.ImResult.Values.Any(
                        im =>
                            details.Communicatie.Where(
                                cm => cm.Type == CommunicatieType.Msn || cm.Type == CommunicatieType.Xmpp)
                                .Select(cm => cm.Waarde)
                                .Contains(im.Name))
                select c).FirstOrDefault();
            if (gevondenViaIm != null)
            {
                return int.Parse(gevondenViaIm.ExternalIdentifier);
            }

            // Probeer ten slotte de geboortedatum.
            var gevondenViaGeboortedatum = (from c in contactResult.Values
                where c.BirthDate == details.Persoon.GeboorteDatum && c.BirthDate != null
                select c).FirstOrDefault();
            if (gevondenViaGeboortedatum != null)
            {
                return int.Parse(gevondenViaGeboortedatum.ExternalIdentifier);
            }

            // We vermoeden dat de persoon nog niet bestaat.
            return null;
        }
    }
}