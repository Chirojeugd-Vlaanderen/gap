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
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Workers
{
    /// <summary>
    /// Contactgerelateerde methods die hopelijk interessant zijn voor de gebruikers
    /// van de CiviCRM-API.
    /// </summary>
    public class ContactWorker: BaseWorker
    {
        private const string ContactIdCacheKey = "cid{0}";
        private static readonly ObjectCache Cache = new MemoryCache("ContactWorkerCache");

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceHelper">Helper to be used for WCF service calls</param>
        /// <param name="log">Logger</param>
        public ContactWorker(ServiceHelper serviceHelper, IMiniLog log): base(serviceHelper, log)
        {
        }

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
            int? cid = (int?) Cache[String.Format(ContactIdCacheKey, externalIdentifier)];

            if (cid != null) return cid;
            var result =
                ServiceHelper.CallService<ICiviCrmApi, Contact>(
                    svc =>
                        svc.ContactGetSingle(ApiKey, SiteKey,
                            new ContactRequest {ExternalIdentifier = externalIdentifier, ReturnFields = "id"}));

            // GetSingle levert een 'leeg' contact-object als er niemand
            // wordt gevonden. In dat geval (en normaal gezien enkel dan)
            // is het ID 0.
            if (result == null || result.Id == 0)
            {
                return null;
            }

            cid = result.Id;

            Cache.Set(String.Format(ContactIdCacheKey, externalIdentifier), cid,
                new CacheItemPolicy {SlidingExpiration = new TimeSpan(2, 0, 0, 0)});
            return cid;
        }

        /// <summary>
        /// Haalt de persoon met gegeven <paramref name="adNummer"/> op, samen met zijn recentste
        /// lidrelatie in de groep met <paramref name="civiGroepId"/>.
        /// </summary>
        /// <param name="adNummer"></param>
        /// <param name="civiGroepId"></param>
        /// <returns>De persoon met gegeven <paramref name="adNummer"/> op, samen met zijn recentste
        /// lidrelatie in de groep met <paramref name="civiGroepId"/>. Als de persoon niet werd gevonden,
        /// wordt <c>null</c> opgeleverd.</returns>
        /// <remarks>Dit zou beter werken op contact-ID ipv op adNummer. Zie #3717.</remarks>
        public Contact PersoonMetRecentsteLid(int adNummer, int? civiGroepId)
        {
            // Haal de persoon op met gegeven AD-nummer en zijn recentste lidrelatie in de gevraagde groep.
            var contactRequest = new ContactRequest
            {
                ExternalIdentifier = adNummer.ToString(),
                RelationshipGetRequest = new RelationshipRequest
                {
                    RelationshipTypeId = (int)RelatieType.LidVan,
                    ContactIdAValueExpression = "$value.id",
                    ContactIdB = civiGroepId,
                    ApiOptions = new ApiOptions { Sort = "start_date DESC", Limit = 1 }
                }
            };

            var contact =
                ServiceHelper.CallService<ICiviCrmApi, Contact>(
                    svc => svc.ContactGetSingle(ApiKey, SiteKey, contactRequest));

            // Elk contact heeft een ID verschillend van 0. 
            // Als het opgeleverd ID 0 is, wil dat zeggen
            // dat het contact niet gevonden is.
            return contact.Id == 0 ? null : contact;
        }

        /// <summary>
        /// Haalt de persoon met gegeven <paramref name="adNummer"/> op, samen met zijn recentste
        /// membership van het gegeven <paramref name="type"/>.
        /// </summary>
        /// <param name="adNummer">AD-nummer van op te halen persoon</param>
        /// <param name="type">gevraagde membership type</param>
        /// <returns>De persoon met gegeven <paramref name="adNummer"/> op, samen met zijn recentste
        /// membership van het gegeven <paramref name="type"/>.</returns>
        /// <remarks>Dit zou beter werken op contact-ID ipv op adNummer. Zie #3717.</remarks>
        public Contact PersoonMetRecentsteMembership(int adNummer, MembershipType type)
        {
            // Haal de persoon op met gegeven AD-nummer en zijn recentste lidrelatie in de gevraagde groep.
            var contactRequest = new ContactRequest
            {
                ExternalIdentifier = adNummer.ToString(),
                MembershipGetRequest = new MembershipRequest
                {
                    MembershipTypeId = (int)type,
                    ApiOptions = new ApiOptions { Sort = "start_date DESC", Limit = 1 }
                }
            };

            var contact =
                ServiceHelper.CallService<ICiviCrmApi, Contact>(
                    svc => svc.ContactGetSingle(ApiKey, SiteKey, contactRequest));

            return (contact == null || contact.Id == 0) ? null : contact;
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
        public int? AdNummerZoeken(PersoonDetails details)
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
                        svc => svc.ContactGet(ApiKey, SiteKey, request));
                result.AssertValid();
                if (result.Count >= 1)
                {
                    return int.Parse(result.Values.First().ExternalIdentifier);
                }

                // AD-nummer niet gevonden in Civi. Doe er iets mee.
                Log.Loggen(Niveau.Error,
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
                        svc => svc.ContactGet(ApiKey, SiteKey, request));
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
                EmailGetRequest = new EmailRequest(),
                PhoneGetRequest = new PhoneRequest(),
                WebsiteGetRequest = new WebsiteRequest(),
                ImGetRequest = new ImRequest()
            };

            var contactResult =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                    svc => svc.ContactGet(ApiKey, SiteKey, nameGenderRequest));

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
        public int? AdNummerZoeken(Persoon persoon)
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
                        svc => svc.ContactGet(ApiKey, SiteKey, request));
                result.AssertValid();
                if (result.Count >= 1)
                {
                    return int.Parse(result.Values.First().ExternalIdentifier);
                }

                // AD-nummer niet gevonden in Civi. Doe er iets mee.
                Log.Loggen(Niveau.Error,
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
                        svc => svc.ContactGet(ApiKey, SiteKey, request));
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
                    svc => svc.ContactGet(ApiKey, SiteKey, nameGenderRequest));

            contactResult.AssertValid();
            if (contactResult.Count == 0)
            {
                return null;
            }

            int adNr = int.Parse(contactResult.Values.First().ExternalIdentifier);

            if (contactResult.Count > 1)
            {
                Log.Loggen(Niveau.Warning,
                    String.Format("Geen uniek AD-nummer gevonden voor {1} {2}, gebdat {3}. {0} gekozen.", adNr,
                        persoon.VoorNaam, persoon.Naam, persoon.GeboorteDatum), null, persoon.AdNummer,
                    persoon.ID);
            }

            // We vermoeden dat de persoon nog niet bestaat.
            return adNr == 0 ? (int?)null : adNr;
        }
    }
}