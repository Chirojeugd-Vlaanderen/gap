/*
 * Copyright 2017 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
using System.Linq;
using Chiro.Gap.Api.Models;

namespace Chiro.Gap.Api
{
    public class ApiAccountManager
    {
        private static Random random = new Random();

        /// <summary>
        /// Maakt een API-account met gegeven <paramref name="username"/> voor 
        /// de persoon met gegeven <paramref name="adnr"/>, en levert het
        /// wachtwoord op.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="adnr"></param>
        /// <returns>Het wachtwoord van de nieuwe account</returns>
        public string Register(string username, int adnr, bool force)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz._-";
            string passsword = new string(Enumerable.Repeat(chars, Properties.Settings.Default.PwLength)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            var usermodel = new UserModel
            {
                AdNummer = adnr,
                Password = passsword,
                ConfirmPassword = passsword,
                UserName = username
            };
            using (var repo = new AuthRepository())
            {
                var user = repo.FindUser(username).GetAwaiter().GetResult();
                if (user != null)
                {
                    if (!force)
                    {
                        return null;
                    }
                    repo.DeleteUser(username).Wait();
                }
                repo.RegisterUser(usermodel).Wait();
            }
            return passsword;
        }
    }
}