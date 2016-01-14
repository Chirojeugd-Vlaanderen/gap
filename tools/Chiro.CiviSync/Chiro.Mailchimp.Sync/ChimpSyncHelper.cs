/*
 * Copyright 2015,2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using System;
using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.Mailchimp.Sync.Models;
using Chiro.Mailchimp.Sync.Properties;
using RestSharp;

namespace Chiro.Mailchimp.Sync
{
    /// <summary>
    /// Deze klasse zorgt voor de communicatie met Mailchimp.
    /// </summary>
    public class ChimpSyncHelper : IChimpSyncHelper
    {
        /// <summary>
        /// Maakt of updatet het mailchimpabonnement met gegeven <paramref name="abonnementInfo"/>.
        /// </summary>
        /// <param name="abonnementInfo">Info over te maken/updaten abonnement.</param>
        public void AbonnementSyncen(AbonnementInfo abonnementInfo)
        {
            var member = new Member
            {
                email_address = abonnementInfo.MailChimpAdres
            };

            if (abonnementInfo.AbonnementType == 0)
            {
                AbonnementVerwijderen(abonnementInfo.EmailAdres);
                return;
            }

            var client = new RestClient(Settings.Default.ApiServer)
            {
                Authenticator = new HttpBasicAuthenticator("apikey", Settings.Default.ApiKey)
            };
            var request = new RestRequest("3.0/lists/{listid}/members/{memberid}");
            request.AddUrlSegment("listid", Settings.Default.ListId);
            request.AddUrlSegment("memberid", member.id);

            var response = client.Execute<Member>(request);
            var bestaande = response.Data;

            member.merge_fields = new MergeFields
            {
                FNAME = abonnementInfo.VoorNaam,
                LNAME = abonnementInfo.Naam,
                STAMNUMMER = abonnementInfo.StamNr,
                HOE = abonnementInfo.AbonnementType == 1 ? "Digitaal graag" : "Papier hier",
                STRAAT_NUM = string.Empty,
                POSTCODE = string.Empty,
                GEMEENTE = string.Empty,
                LAND = string.Empty
            };

            if (abonnementInfo.Adres != null)
            {
                member.merge_fields.STRAAT_NUM = string.IsNullOrEmpty(abonnementInfo.Adres.Bus)
                    ? String.Format("{0} {1}", abonnementInfo.Adres.Straat, abonnementInfo.Adres.HuisNr)
                    : String.Format("{0} {1} bus {2}", abonnementInfo.Adres.Straat, abonnementInfo.Adres.HuisNr,
                        abonnementInfo.Adres.Bus);

                member.merge_fields.POSTCODE = abonnementInfo.Adres.PostNr;
                member.merge_fields.GEMEENTE = abonnementInfo.Adres.WoonPlaats;
                member.merge_fields.LAND = string.IsNullOrEmpty(abonnementInfo.Adres.Land)
                    ? "België"
                    : abonnementInfo.Adres.Land;
            }

            member.status = "subscribed";
            if (bestaande == null)
            {
                var request2 = new RestRequest("3.0/lists/{listid}/members/")
                {
                    RequestFormat = DataFormat.Json,
                    Method = Method.POST
                };
                request2.AddUrlSegment("listid", Settings.Default.ListId);
                request2.AddBody(member);
                var result = client.Execute(request2);
            }
            else
            {
                var request2 = new RestRequest("3.0/lists/{listid}/members/{memberid}")
                {
                    RequestFormat = DataFormat.Json,
                    Method = Method.PATCH
                };
                request2.AddUrlSegment("listid", Settings.Default.ListId);
                request2.AddUrlSegment("memberid", member.id);
                request2.AddBody(member);
                var result = client.Execute(request2);
            }
        }

        /// <summary>
        /// Verwijdert het mailchimpabonnement voor het gegeven <paramref name="eMailAdres"/>
        /// </summary>
        /// <param name="eMailAdres">E-mailadres dat van Mailchimp verwijderd moet worden.</param>
        public void AbonnementVerwijderen(string eMailAdres)
        {
            var member = new Member
            {
                email_address = eMailAdres
            };
            // ID wordt berekend op basis van e-mailadres.
            var request = new RestRequest("3.0/lists/{listid}/members/{memberid}")
            {
                RequestFormat = DataFormat.Json,
                Method = Method.DELETE
            };
            request.AddUrlSegment("listid", Settings.Default.ListId);
            request.AddUrlSegment("memberid", member.id);

            var client = new RestClient(Settings.Default.ApiServer)
            {
                Authenticator = new HttpBasicAuthenticator("apikey", Settings.Default.ApiKey)
            };
            var result = client.Execute(request);
        }
    }
}
