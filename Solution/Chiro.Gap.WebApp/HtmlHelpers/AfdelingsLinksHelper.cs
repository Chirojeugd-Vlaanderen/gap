﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
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
		/// <param name="html">htmlhelper</param>
		/// <param name="afdelingen">Informatie over de afdelingen waarnaar gelinkt moet worden</param>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar voor de afdelingspagina's</param>
		/// <param name="groepID">ID van de groep van de afdeling</param>
		/// <returns>Links naar de afdelingspagina's voor de <paramref name="afdelingen"/>, gescheiden
		/// door whitespace</returns>
		// TODO: Kan dit niet met minder parameters?
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
					html.Encode(afd.Afkorting),
					"Afdeling",
					new {Controller = "Leden", groepsWerkJaarID, groepID, afd.ID},
					new {title = String.Format(Properties.Resources.AfdelingsLinkTitel, afd.Naam)}
				               	).ToString());
				builder.Append(' ');
			}

			return MvcHtmlString.Create(builder.ToString());
		}
	}
}