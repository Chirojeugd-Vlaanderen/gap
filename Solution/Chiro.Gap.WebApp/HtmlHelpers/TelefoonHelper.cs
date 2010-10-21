using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
	public static class TelefoonHelper
	{
		/// <summary>
		/// HTML-helper voor telefoonnummer, die niets meer doet dan spaties vervangen
		/// door &nbsp; (zodat telefoonnrs niet gewordrapt worden)
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="telefoonNr">string met telefoonnummer</param>
		/// <returns>html voor telefoonnr</returns>
		public static string Telefoon(this HtmlHelper htmlHelper, string telefoonNr)
		{
			if (String.IsNullOrEmpty(telefoonNr))
			{
				return String.Empty;
			}
			else
			{
				return telefoonNr.Replace(" ", "&nbsp;").Replace("-", "&#8209;");
			}
		}
	}
}
