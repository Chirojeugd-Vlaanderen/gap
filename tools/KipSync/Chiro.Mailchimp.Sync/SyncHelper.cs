/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.Mailchimp.Sync.Models;
using Chiro.Mailchimp.Sync.Properties;
using RestSharp;

namespace Chiro.Mailchimp.Sync
{
    public class SyncHelper
    {
        public void AbonnementSyncen(AbonnementInfo abonnementInfo)
        {
            var member = new Member {email_address = abonnementInfo.EmailAdres};

            var client = new RestClient(Settings.Default.ApiServer)
            {
                Authenticator = new HttpBasicAuthenticator("apikey", Settings.Default.ApiKey)
            };
            var request = new RestRequest("3.0/lists/{listid}/members/{memberid}");
            request.AddUrlSegment("listid", Settings.Default.ListId);
            request.AddUrlSegment("memberid", member.id);

            IRestResponse<Member> response = client.Execute<Member>(request);
            var bestaande = response.Data;

            if (bestaande == null && abonnementInfo.AbonnementType != 0)
            {
                member.merge_fields = new MergeFields
                {
                    FNAME = abonnementInfo.VoorNaam,
                    LNAME = abonnementInfo.Naam,
                    STAMNUMMER = abonnementInfo.StamNr,
                    //STRAAT_NUM = abonnementInfo.Adres.ToString(),
                    //POSTCODE = abonnementInfo.Adres.PostNr.ToString(),
                    //GEMEENTE = abonnementInfo.Adres.WoonPlaats,
                    //LAND = abonnementInfo.Adres.Land
                };

                var request2 = new RestRequest("3.0/lists/{listid}/members/", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                request2.AddUrlSegment("listid", Settings.Default.ListId);
                request2.AddBody(member);
                var result = client.Execute(request2);

                // Hier krijg je nu een foutmelding van verplichte velden die niet goed zijn, zoals 
                // bijv. die keuzevakjes waar je er eentje uit moet kiezen.
            }
        }
    }
}
