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
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using Chiro.ChiroCivi.ServiceContracts.DataContracts;
using Chiro.ChiroCivi.ServiceContracts.DataContracts.Requests;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Services.Properties;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        private static readonly Regex GeldigTelefoonNummer = new Regex(Settings.Default.TelefoonRegex);
        private static readonly Regex Alfanumeriek = new Regex(@"[^\d]");
        private static readonly Regex Protocol = new Regex(@"^https?://");

        /// <summary>
        /// Creeer lidrelatie (geen membership) voor een persoon waarvan we geen AD-nummer kennen.
        /// </summary>
        /// <param name="details">Details van de persoon waarvoor een lidrelatie gemaakt moet worden.</param>
        /// <param name="lidGedoe">Informatie over de lidrelatie.</param>
        public void NieuwLidBewaren(PersoonDetails details, LidGedoe lidGedoe)
        {
            if (details.Persoon.AdNummer != null)
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "NieuwLidBewaren aangeroepen voor persoon {0} {1} (gid {3}) met bestaand AD-Nummer {2}."
                        , details.Persoon.VoorNaam, details.Persoon.Naam, details.Persoon.AdNummer, details.Persoon.ID),
                    lidGedoe.StamNummer, details.Persoon.AdNummer, details.Persoon.ID);
            }
            if (String.IsNullOrEmpty(details.Persoon.VoorNaam))
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "NieuwLidBewaren voor persoon zonder voornaam: {0} (gid {1} ad {2})."
                        , details.Persoon.Naam, details.Persoon.ID, details.Persoon.AdNummer),
                    lidGedoe.StamNummer, details.Persoon.AdNummer, details.Persoon.ID);
            }

            // Update of maak de persoon, en vind zijn AD-nummer
            int adNr = UpdatenOfMaken(details);

            // Bewaar de lidrelatie op basis van het AD-nummer.
            LidBewaren(adNr, lidGedoe);
        }

        /// <summary>
        /// Probeert een persoon te vinden op basis van persoonsgegevens, adressen en communicatie.
        /// Als dat lukt, worden de meegegeven persoonsgegevens, adressen en communicatie overgenomen 
        /// in de CiviCRM. Als er niemand gevonden is, dan wordt een nieuwe persoon aangemaakt.
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        private int UpdatenOfMaken(PersoonDetails details)
        {
            int? adNummer = AdNummerZoeken(details);

            if (adNummer != null)
            {
                return adNummer.Value;
            }

            throw new NotImplementedException();

            // TODO: aanmaken van nieuw contact, en AD-nummer opleveren.
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

                var request = new ExternalIdentifierRequest
                {
                    ExternalIdentifier = details.Persoon.AdNummer.ToString(),
                    ReturnFields = "external_identifier"
                };
                var result =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResultValue<Contact>>(
                        svc => svc.ContactGet(_apiKey, _siteKey, request));
                if (result.Count >= 1)
                {
                    return int.Parse(result.Values.First().ExternalIdentifier);
                }
            }

            // Probeer op GAP-ID

            if (details.Persoon.ID > 0)
            {
                var request = new GapIdRequest
                {
                    GapId = details.Persoon.ID,
                    ReturnFields = "external_identifier"
                };
                var result =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResultValue<Contact>>(
                        svc => svc.ContactGet(_apiKey, _siteKey, request));
                if (result.Count >= 1)
                {
                    return int.Parse(result.Values.First().ExternalIdentifier);
                }
            }

            // Haal eens iedereen op met hetzelfde geslacht en dezelfde naam.

            var nameGenderRequest = new NameGenderRequest
            {
                FirstName = details.Persoon.VoorNaam,
                LastName = details.Persoon.Naam,
                Gender = (Gender) (2 - (int) details.Persoon.Geslacht),
                ChainedEntities = new[] {CiviEntity.Address, CiviEntity.Email, CiviEntity.Phone, CiviEntity.Im}
            };

            var contactResult  =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValue<Contact>>(
                    svc => svc.ContactGet(_apiKey, _siteKey, nameGenderRequest));

            // Zoek op telefoonnummer of fax.
            var gevondenViaTelefoonNr =
                (from c in
                    contactResult.Values
                    where
                        c.ChainedPhones.Values.Any(nr =>
                            GeldigNummer(nr.PhoneNumber) &&
                            StandaardNummer(
                                details.Communicatie.Where(
                                    cm => cm.Type == CommunicatieType.TelefoonNummer || cm.Type == CommunicatieType.Fax)
                                    .Select(cm => cm.Waarde))
                                .Contains(StandaardNummer(nr.PhoneNumber)))
                    select c).FirstOrDefault();
            if (gevondenViaTelefoonNr != null)
            {
                return int.Parse(gevondenViaTelefoonNr.ExternalIdentifier);
            }

            // Zoek op e-mail
            var gevondenViaEmail = (from c in contactResult.Values
                where
                    c.ChainedEmails.Values.Any(
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
                    c.ChainedWebsites.Values.Any(
                        ws =>
                            StandaardUrl(details.Communicatie.Where(
                                cm =>
                                    cm.Type == CommunicatieType.WebSite || cm.Type == CommunicatieType.Twitter ||
                                    cm.Type == CommunicatieType.StatusNet)
                                .Select(cm => cm.Waarde))
                                .Contains(StandaardUrl(ws.Url)))
                select c).FirstOrDefault();
            if (gevondenViaWebsite != null)
            {
                return int.Parse(gevondenViaWebsite.ExternalIdentifier);
            }

            // Zoek op IM
            var gevondenViaIm = (from c in contactResult.Values
                where
                    c.ChainedIms.Values.Any(
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
        /// Bekijkt <paramref name="url"/>. Stript eventueel http(s)://-prefix, en zet zaken die beginnen met @ om naar
        /// een twitter-url.
        /// </summary>
        /// <param name="url">url of twitter handle</param>
        /// <returns>Url's zonder http(s).</returns>
        private string StandaardUrl(string url)
        {
            return StandaardUrl(new[] {url}).First();
        }

        /// <summary>
        /// Bekijkt <paramref name="urls"/>. Stript eventuele http(s)://-prefixes, en zet zaken die beginnen met @ om naar
        /// een twitter-url.
        /// </summary>
        /// <param name="urls">Te behandelen lijst url's en twitter handles</param>
        /// <returns>Url's zonder http(s).</returns>
        private List<string> StandaardUrl(IEnumerable<string> urls)
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
        private string StandaardNummer(string telefoonNummer)
        {
            return StandaardNummer(new [] {telefoonNummer}).First();
        }

        /// <summary>
        /// Converteert de gegeven <paramref name="telefoonNummers" /> naar internationaal formaat
        /// beginnend met + en zonder spaties. Bijv +3232310795.
        /// </summary>
        /// <param name="telefoonNummers">Om te zetten telefoonnummers</param>
        /// <returns>Een lijst omgezette telefoonnummers</returns>
        private List<string> StandaardNummer(IEnumerable<string> telefoonNummers)
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
        private bool GeldigNummer(string nr)
        {
            return GeldigTelefoonNummer.IsMatch(nr);
        }
    }
}