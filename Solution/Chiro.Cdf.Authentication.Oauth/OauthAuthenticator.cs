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

using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Chiro.Cdf.Authentication.Oauth
{
    public class OauthAuthenticator: IAuthenticator
    {
        public UserInfo WieBenIk()
        {
            var claims = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims;
            int adnr = int.Parse((from c in claims where c.Type == "adnr" select c.Value).First());
            return new UserInfo
            {
                AdNr = adnr
            };
        }
    }
}
