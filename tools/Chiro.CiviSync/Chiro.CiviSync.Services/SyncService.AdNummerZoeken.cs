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
using System.Linq;
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
        /// Zoek het AD-nummer van een persoon die lijkt op een persoon met gegeven <paramref name="details"/>.
        /// De bedoeling is dat dit enkel wordt aangeroepen als het AD-nummer nog niet gekend of mogelijk
        /// ongeldig is.
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
                result.AssertValid();
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
                result.AssertValid();
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
                Gender = (Gender)(3 - (int)details.Persoon.Geslacht),
                AddressGetRequest = new AddressRequest(),
                EmailGetRequest = new BaseRequest(),
                PhoneGetRequest = new BaseRequest(),
                WebsiteGetRequest = new BaseRequest(),
                ImGetRequest = new BaseRequest()
            };

            var contactResult =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                    svc => svc.ContactGet(_apiKey, _siteKey, nameGenderRequest));

            // Zoek op telefoonnummer of fax.
            var gevondenViaTelefoonNr =
                (from c in
                     contactResult.Values
                 where
                     c.PhoneResult.Values.Any(nr =>
                         CommunicatieLogic.GeldigNummer(nr.PhoneNumber) &&
                         CommunicatieLogic.StandaardNummer(
                             details.Communicatie.Where(
                                 cm => cm.Type == CommunicatieType.TelefoonNummer || cm.Type == CommunicatieType.Fax)
                                 .Select(cm => cm.Waarde))
                             .Contains(CommunicatieLogic.StandaardNummer(nr.PhoneNumber)))
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
                                                  CommunicatieLogic.StandaardUrl(details.Communicatie.Where(
                                                      cm =>
                                                          cm.Type == CommunicatieType.WebSite || cm.Type == CommunicatieType.Twitter ||
                                                          cm.Type == CommunicatieType.StatusNet)
                                                      .Select(cm => cm.Waarde))
                                                      .Contains(CommunicatieLogic.StandaardUrl(ws.Url)))
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

        /// <summary>
        /// Zoek in CiviCRM het AD-nummer van een persoon die lijkt op de gegeven 
        /// <paramref name="persoon"/>.
        /// </summary>
        /// <param name="persoon">Persoon waarvoor ad-nummer gezocht moet worden</param>
        /// <returns>AD-nummer van een persoon die lijkt op de gegeven
        /// <paramref name="persoon"/>, of <c>null</c> als er zo geen is 
        /// gevonden.</returns>
        /// <remarks>Dit is een beperkte versie van de overload die met
        /// PersoonDetails werkt.</remarks>
        private int? AdNummerZoeken(Persoon persoon)
        {
            if (persoon.AdNummer != null)
            {
                // Het lijkt misschien een beetje gek als je het AD-nummer zoekt van
                // iemand waarvan je het AD-nummer al hebt, maar dit vangt het geval op
                // dat het AD-nummer niet (meer) bestaat in Civi.

                var request = new ContactRequest
                {
                    ExternalIdentifier = persoon.AdNummer.ToString(),
                    ReturnFields = "external_identifier"
                };
                var result =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                        svc => svc.ContactGet(_apiKey, _siteKey, request));
                result.AssertValid();
                if (result.Count >= 1)
                {
                    return int.Parse(result.Values.First().ExternalIdentifier);
                }

                // AD-nummer niet gevonden in Civi. Doe er iets mee.
                _log.Loggen(Niveau.Error,
                    String.Format("AD-nummer {0} van {1} {2} niet gevonden.", persoon.AdNummer,
                        persoon.VoorNaam, persoon.Naam), null, persoon.AdNummer,
                    persoon.ID);
            }

            // Probeer op GAP-ID

            if (persoon.ID > 0)
            {
                var request = new ContactRequest
                {
                    GapId = persoon.ID,
                    ReturnFields = "external_identifier"
                };
                var result =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                        svc => svc.ContactGet(_apiKey, _siteKey, request));
                result.AssertValid();
                if (result.Count >= 1)
                {
                    return int.Parse(result.Values.First().ExternalIdentifier);
                }
            }

            // Haal eens iedereen op met dezelfde details

            var nameGenderRequest = new ContactRequest
            {
                FirstName = persoon.VoorNaam,
                LastName = persoon.Naam,
                ContactType = ContactType.Individual,
                Gender = (Gender)(3 - (int)persoon.Geslacht),
                BirthDate = persoon.GeboorteDatum
            };

            var contactResult =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                    svc => svc.ContactGet(_apiKey, _siteKey, nameGenderRequest));

            contactResult.AssertValid();
            if (contactResult.Count == 0)
            {
                return null;
            }
            
            int adNr = int.Parse(contactResult.Values.First().ExternalIdentifier);

            if (contactResult.Count > 1)
            {
                _log.Loggen(Niveau.Warning,
                    String.Format("Geen uniek AD-nummer gevonden voor {1} {2}, gebdat {3}. {0} gekozen.", adNr,
                        persoon.VoorNaam, persoon.Naam, persoon.GeboorteDatum), null, persoon.AdNummer,
                    persoon.ID);
            }

            // We vermoeden dat de persoon nog niet bestaat.
            return adNr == 0 ? (int?)null: adNr;            
        }
    }
}