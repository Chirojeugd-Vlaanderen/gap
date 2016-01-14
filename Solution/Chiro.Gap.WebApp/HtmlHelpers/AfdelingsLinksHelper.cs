/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
    public static class AfdelingsLinksHelper
    {
        /// <summary>
        /// Maakt voor de gegeven <paramref name="afdelingen"/> links naar de pagina met de leden uit
        /// die afdelingen.
        /// </summary>
        /// <param name="html">HtmlHelper waarvoor dit een extension method is</param>
        /// <param name="afdelingen">Informatie over de afdelingen waarnaar gelinkt moet worden</param>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar voor de afdelingspagina's</param>
        /// <param name="groepID">ID van de groep van de afdeling</param>
        /// <returns>Links naar de afdelingspagina's voor de <paramref name="afdelingen"/>, gescheiden
        /// door whitespace</returns>
        // TODO (#1025): Kan dit niet met minder parameters?
        public static MvcHtmlString AfdelingsLinks(
            this HtmlHelper html,
            IEnumerable<AfdelingInfo> afdelingen,
            int groepsWerkJaarID,
            int groepID)
        {
            var builder = new StringBuilder();

            foreach (var afd in afdelingen)
            {
                builder.Append(html.ActionLink(
                    html.Encode(String.IsNullOrWhiteSpace(afd.Afkorting) ? "(nvt)" : afd.Afkorting),
                    "Afdeling",
                    new { Controller = "Leden", groepsWerkJaarID, groepID, afd.ID },
                    new { title = String.Format(Properties.Resources.AfdelingsLinkTitel, afd.Naam) }).ToString());
                builder.Append(' ');
            }

            return MvcHtmlString.Create(builder.ToString());
        }
    }
}