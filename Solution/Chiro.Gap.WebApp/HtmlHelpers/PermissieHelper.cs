/*
 * Copyright 2015, Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using System.Web.Mvc;
using Chiro.Gap.Domain;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
    public static class PermissieHelper
    {
        public static string Permissie(this HtmlHelper htmlHelper, Permissies permissies)
        {
            switch (permissies)
            {
                case Permissies.Geen:
                    return "&#10005;";
                case Permissies.Lezen:
                    return "&#55357;&#56384;";
                case Permissies.Bewerken:
                    return "&#9998;";
                default:
                    return "WTF";
            }
        }
    }
}