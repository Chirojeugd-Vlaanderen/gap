// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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