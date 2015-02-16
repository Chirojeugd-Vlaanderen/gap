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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Chiro.CiviSync.Helpers.Properties;
using Chiro.Gap.UpdateApi.Models;

namespace Chiro.CiviSync.Helpers
{
    public class GapUpdateHelper
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _url;

        public GapUpdateHelper(string url, string username, string password)
        {
            _url = url;
            _username = username;
            _password = password;
        }

        /// <summary>
        /// Rapporteer het gegeven <paramref name="adNummer"/> als ongeldig bij GAP.
        /// </summary>
        /// <param name="adNummer">Als ongeldig te rapporteren AD-nummer</param>
        public async Task OngeldigAdNaarGap(int adNummer)
        {
            // TODO: ServiceHelper gebruiken.
            using (var client = new HttpClient())
            {
                if (!String.IsNullOrEmpty(_username) && !String.IsNullOrEmpty(_password))
                {
                    // TODO: Dit kan ook properder met een HttpClientHandler class.
                    var byteArray = Encoding.ASCII.GetBytes(String.Join(":", _username, _password));
                    var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    client.DefaultRequestHeaders.Authorization = header;
                }
                client.BaseAddress = new Uri(_url);
                await client.PostAsJsonAsync("foutad",
                    new FoutAd { AdNummer = adNummer });
            }
        }
    }
}