/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
    public static class PersoonsLinkHelper
    {
        /// <summary>
        /// Maakt een link met als tekst <paramref name="voornaam"/> en <paramref name="naam"/>, die linkt naar de gelieerde
        /// persoon met gegeven <paramref name="gelieerdePersoonID"/>
        /// </summary>
        /// <param name="helper">HtmlHelper waarvoor dit een extension method is</param>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon waarnaar te linken</param>
        /// <param name="voornaam">Te tonen voornaam</param>
        /// <param name="naam">Te tonen naam</param>
        /// <returns>Link naar een gelieerde persoon</returns>
        public static MvcHtmlString PersoonsLink(this HtmlHelper helper, int gelieerdePersoonID, string voornaam, string naam)
        {
            // Een persoonslink komt typisch voor in sorteerbare lijsten, dus is het beter als de familienaam vooraan staat.
            return helper.ActionLink(String.Format("{0} {1}", naam, voornaam),
                                     "EditRest",
                                     new { Controller = "Personen", id = gelieerdePersoonID });
        }
    }
}