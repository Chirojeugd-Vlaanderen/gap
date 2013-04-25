using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
    ///<summary>
    /// Helper ter vervanging van '<a style="cursor : pointer" id="clInfo" class="ui-icon ui-icon-help"></a>' 
    /// of 'Html.ActionLink'
    /// In het bestand 'Handleiding/trefwoorden.aspx' heeft elke dd-tag een id gekregen overeenkomstig met de informatie die in
    /// die tag te vinden is. In de helper geef je dit id mee.
    /// Deze retourneert html, het oproepen van de info uit de dd-tag gebeurt dmv een Ajax call in JQuery
    /// Zoiets:
    ///$('#Ad-info').click(function () {
    ///    toonInfo('#ADINFO', "AD-nummer", "#extraInfoDialog");
    ///});
    /// de opgeroepen functie zit in het bestand 'algemeneFuncties.js'
    /// </summary>

    public static class InfoLinkHelper
    {   
        public static String InfoLink(this HtmlHelper html, string id)
        {
            return String.Format("<a id=\""+id +"\" class=\"ui-icon ui-icon-info\" style=\"cursor:pointer\"></a>");
        }
    }
}
