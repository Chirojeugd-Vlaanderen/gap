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

using System.Security.Principal;
using System.Threading;
using Chiro.Gap.Services.Test.Properties;

namespace Chiro.Gap.Services.Test
{
    public static class PermissionHelper
    {
        public static void FixPermissions()
        {
            // In staging heeft de BaseService een PrincipalPermission-attribuut
            // zodat enkel de users uit de juiste Windows-groepen de service mogen
            // gebruiken. 
            // Als je unit tests runt met MSTest, dan werkt dat niet, ook al zit
            // de user die de tests uitvoert (typisch jenkins) in de juiste
            // security groepen. Daarom werken we daarrond door te foefelen
            // met Thread.CurrentPrincipal.

            var identity = new GenericIdentity(Settings.Default.TestUser);
            var roles = new[] { Settings.Default.TestSecurityGroep };
            var principal = new GenericPrincipal(identity, roles);
            Thread.CurrentPrincipal = principal;
        }
    }
}
