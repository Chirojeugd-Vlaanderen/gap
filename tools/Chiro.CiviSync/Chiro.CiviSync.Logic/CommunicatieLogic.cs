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
using System.Text.RegularExpressions;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic.Properties;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Logic
{
    public static class CommunicatieLogic
    {
        private static readonly Regex GeldigTelefoonNummer = new Regex(Settings.Default.TelefoonRegex);
        private static readonly Regex GsmNummerExpression = new Regex(Settings.Default.GsmRegex);
        private static readonly Regex Alfanumeriek = new Regex(@"[^\d]");
        private static readonly Regex Protocol = new Regex(@"^https?://");

        /// <summary>
        /// Bekijkt <paramref name="url"/>. Stript eventueel http(s)://-prefix, en zet zaken die beginnen met @ om naar
        /// een twitter-url.
        /// </summary>
        /// <param name="url">url of twitter handle</param>
        /// <returns>Url's zonder http(s).</returns>
        public static string StandaardUrl(string url)
        {
            return StandaardUrl(new[] { url }).First();
        }

        /// <summary>
        /// Bekijkt <paramref name="urls"/>. Stript eventuele http(s)://-prefixes, en zet zaken die beginnen met @ om naar
        /// een twitter-url.
        /// </summary>
        /// <param name="urls">Te behandelen lijst url's en twitter handles</param>
        /// <returns>Url's zonder http(s).</returns>
        public static List<string> StandaardUrl(IEnumerable<string> urls)
        {
            var results = new List<string>();

            foreach (var url in urls)
            {
                if (url.StartsWith("@"))
                {
                    results.Add("twitter.com/" + url.Substring(1));
                }
                else
                {
                    results.Add(Protocol.Replace(url, String.Empty));
                }
            }

            return results;
        }

        /// <summary>
        /// Converteert gegeven <paramref name="telefoonNummer" /> naar internationaal formaat
        /// beginnend met + en zonder spaties. Bijv +3232310795.
        /// </summary>
        /// <param name="telefoonNummer">Om te zetten telefoonnummer</param>
        /// <returns>`Het omgezette telefoonnummer</returns>
        public static string StandaardNummer(string telefoonNummer)
        {
            return StandaardNummer(new[] { telefoonNummer }).First();
        }

        /// <summary>
        /// Converteert de gegeven <paramref name="telefoonNummers" /> naar internationaal formaat
        /// beginnend met + en zonder spaties. Bijv +3232310795.
        /// </summary>
        /// <param name="telefoonNummers">Om te zetten telefoonnummers</param>
        /// <returns>Een lijst omgezette telefoonnummers</returns>
        public static List<string> StandaardNummer(IEnumerable<string> telefoonNummers)
        {
            var result = new List<string>();

            foreach (string nr in telefoonNummers)
            {
                string omgezet = Alfanumeriek.Replace(nr, String.Empty);
                if (omgezet.StartsWith("0") && !omgezet.StartsWith("00"))
                {
                    omgezet = "32" + omgezet.Substring(1);
                }
                else if (omgezet.StartsWith("00"))
                {
                    omgezet = omgezet.Substring(2);
                }
                result.Add("+" + omgezet);
            }
            return result;
        }

        /// <summary>
        /// Controleert of een telefoonnr <paramref name="nr"/> een geldig telefoonnummer is.
        /// </summary>
        /// <param name="nr">Te controleren telefoonnummer</param>
        /// <returns><c>true</c> als <paramref name="nr"/> geldig is, <c>false</c> als <paramref name="nr"/> ongeldig is.</returns>
        public static bool GeldigNummer(string nr)
        {
            return GeldigTelefoonNummer.IsMatch(nr);
        }

        /// <summary>
        /// Zet een lijst communicatievormen van GAP om naar een lijst entities voor CiviCRM
        /// (van de types Email, Phone, Website, Im).
        /// </summary>
        /// <param name="communicatie">Lijst communicatievormen</param>
        /// <param name="contactId">Te gebruiken contact-ID voor civi-entities. Mag <c>null</c> zijn.</param>
        /// <param name="enkelZoeken">Als deze <c>true</c> is, worden enkel de relevante velden meegenomen om de
        /// communicatievorm te kunnen zoeken (dus geen IsBulk).</param>
        /// <returns>Lijst met e-mailadressen, telefoonnummers, websites, im (als Object).</returns>
        public static BaseRequest[] RequestMaken(IList<CommunicatieMiddel> communicatie, int? contactId, bool enkelZoeken)
        {
            var telefoonNummers = from c in communicatie
                                  where c.Type == CommunicatieType.TelefoonNummer || c.Type == CommunicatieType.Fax
                                  select new PhoneRequest
                                  {
                                      ContactId = contactId,
                                      PhoneNumber = c.Waarde,
                                      PhoneType =
                                          c.Type == CommunicatieType.Fax
                                              ? PhoneType.Fax
                                              : GsmNummerExpression.IsMatch(c.Waarde) ? PhoneType.Mobile : PhoneType.Phone
                                  };
            var eMailAdressen = from c in communicatie
                                where c.Type == CommunicatieType.Email
                                select new EmailRequest
                                {
                                    ContactId = contactId,
                                    EmailAddress = c.Waarde,
                                    IsBulkMail = enkelZoeken ? (bool?)null : c.IsBulk
                                };
            // wat betreft websites ondersteunt GAP enkel twitter en 'iets anders'
            var websites = from c in communicatie
                           where
                               c.Type == CommunicatieType.WebSite || c.Type == CommunicatieType.Twitter ||
                               c.Type == CommunicatieType.StatusNet
                           select new WebsiteRequest
                           {
                               ContactId = contactId,
                               Url = c.Type == CommunicatieType.Twitter ? c.Waarde.Replace("@", "https://twitter.com/") : c.Waarde,
                               WebsiteType = c.Type == CommunicatieType.Twitter ? WebsiteType.Twitter : WebsiteType.Main
                           };
            // wat betreft IM kent GAP enkel MSN en XMPP.
            var im = from c in communicatie
                     where c.Type == CommunicatieType.Msn || c.Type == CommunicatieType.Xmpp
                     select new ImRequest
                     {
                         ContactId = contactId,
                         Name = c.Waarde,
                         Provider = c.Type == CommunicatieType.Msn ? Provider.Msn : Provider.Jabber
                     };
            return telefoonNummers.Union<BaseRequest>(eMailAdressen).Union(websites).Union(im).ToArray();
        }

        /// <summary>
        /// Zet een communicatievorm van GAP om naar een communicatierequest voor CiviCRM
        /// (van het type Email, Phone, Website of Im).
        /// </summary>
        /// <param name="communicatie">Communicatievorm</param>
        /// <param name="contactId">Te gebruiken contact-ID voor de requests. Mag <c>null</c> zijn.</param>
        /// <param name="enkelZoeken">Als deze <c>true</c> is, worden enkel de relevante velden meegenomen om de
        /// communicatievorm te kunnen zoeken (dus geen IsBulk of zo).</param>
        /// <returns>het gevraagde request als Object.</returns>
        public static BaseRequest RequestMaken(CommunicatieMiddel communicatie, int? contactId, bool enkelZoeken)
        {
            return RequestMaken(new[] { communicatie }, contactId, enkelZoeken).First();
        }

        /// <summary>
        /// Zet een lijst communicatievormen van GAP om naar een lijst entities voor CiviCRM
        /// (van de types Email, Phone, Website, Im).
        /// </summary>
        /// <param name="communicatie">Lijst communicatievormen</param>
        /// <param name="contactId">Te gebruiken contact-ID voor civi-entities. Mag <c>null</c> zijn.</param>
        /// <returns>Lijst met e-mailadressen, telefoonnummers, websites, im (als Object).</returns>
        public static BaseRequest[] RequestMaken(List<CommunicatieMiddel> communicatie, int? contactId)
        {
            return RequestMaken(communicatie, contactId, false);
        }

        /// <summary>
        /// Tamelijk hacky functie om een id te zetten van een communicatierequest.
        /// </summary>
        /// <param name="communicatieRequest">Request om ID van te zetten</param>
        /// <param name="id">nieuw ID</param>
        public static void RequestIdZetten(BaseRequest communicatieRequest, int id)
        {
            if (communicatieRequest.GetType() == typeof (EmailRequest))
            {
                ((EmailRequest)communicatieRequest).Id = id;
            }
            else if (communicatieRequest.GetType() == typeof (PhoneRequest))
            {
                ((PhoneRequest) communicatieRequest).Id = id;
            }
            else if (communicatieRequest.GetType() == typeof (WebsiteRequest))
            {
                ((WebsiteRequest)communicatieRequest).Id = id;
            }
            else if (communicatieRequest.GetType() == typeof (ImRequest))
            {
                ((WebsiteRequest) communicatieRequest).Id = id;
            }
            else
            {
                throw new NotSupportedException("Onbekend communicatierequest.");
            }
        }


        /// <summary>
        /// Maakt save-requests voor de gegeven <paramref name="communicatieMiddelen"/>, en chaint ze aan het
        /// gegeven <paramref name="contactRequest"/>.
        /// </summary>
        /// <param name="contactRequest"></param>
        /// <param name="communicatieMiddelen"></param>
        public static void RequestsChainen(ContactRequest contactRequest, IEnumerable<CommunicatieMiddel> communicatieMiddelen)
        {
            if (communicatieMiddelen != null)
            {
                // Doe enkel iets als er communicatiemiddelen gegeven zijn.
                var civiCommunicatie = RequestMaken(communicatieMiddelen.ToList(), null);
                contactRequest.PhoneSaveRequest = civiCommunicatie.OfType<PhoneRequest>();
                contactRequest.EmailSaveRequest = civiCommunicatie.OfType<EmailRequest>();
                contactRequest.WebsiteSaveRequest = civiCommunicatie.OfType<WebsiteRequest>();
                contactRequest.ImSaveRequest = civiCommunicatie.OfType<ImRequest>();
            }
        }
    }
}