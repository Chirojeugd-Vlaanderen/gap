/*
 * Copyright 2017 the GAP developers. See the NOTICE file at the 
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
using System.Web;

namespace Chiro.Cdf.Authentication.Dev
{
    /// <summary>
    /// Dummy-authenticator voor dev, zodat het ook werkt als CAS niet beschikbaar is.
    /// </summary>
    public class DummyAuthenticator: IAuthenticator
    {
        /// <summary>
        /// Levert een dummy-AD-nummer op, voor dev.
        /// </summary>
        /// <returns></returns>
        public UserInfo WieBenIk()
        {
            // Voor dev 'berekenen' we een dummy-AD-nummer.
            string userName;
            userName = HttpContext.Current == null ? String.Empty : HttpContext.Current.User.Identity.Name;
            if (String.IsNullOrEmpty(userName))
            {
                // Als we geen username vinden, geven we een dummy ad-nummer.
                return new UserInfo()
                {
                    AdNr = 42,
                    DeveloperMode = true
                };
            }
            int result = 0;
            foreach (char c in userName)
            {
                result += (int)c;
            }
            return new UserInfo
            {
                AdNr = result,
                DeveloperMode = true
            };
        }
    }
}
