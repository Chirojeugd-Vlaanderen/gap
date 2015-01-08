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
using Chiro.CiviSync.Services.Properties;

namespace Chiro.CiviSync.Services.Helpers
{
    public static class CommunicatieHelper
    {
        private static readonly Regex GeldigTelefoonNummer = new Regex(Settings.Default.TelefoonRegex);
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
    }
}