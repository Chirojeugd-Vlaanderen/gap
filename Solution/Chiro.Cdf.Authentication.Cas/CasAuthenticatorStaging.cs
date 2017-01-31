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

using System.Diagnostics;
using System.Linq;
using DotNetCasClient.Security;

namespace Chiro.Cdf.Authentication.Cas
{
    /// <summary>
    /// Haalt het AD-nummer op uit de CasPrincipal, en geeft sowieso developerrechten.
    /// DIT IS ENKEL TE GEBRUIKEN IN DEV!
    /// </summary>
    public class CasAuthenticatorStaging: IAuthenticator
    {
        public UserInfo WieBenIk()
        {
            var principal = System.Web.HttpContext.Current.User as CasPrincipal;
            Debug.Assert(principal != null);
            bool isDev = principal.Assertion.Attributes["cas:drupal_roles"].Any(role => role == "Toegang tot GAP-staging");
            return new UserInfo()
            {
                AdNr = int.Parse(principal.Assertion.Attributes["cas:ad_nummer"].First()),
                DeveloperMode = isDev
            };
        }
    }
}
