// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Web.Mvc;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
	public static class TelefoonHelper
	{
		/// <summary>
		/// HTML-helper voor telefoonnummer, die niets meer doet dan spaties vervangen
		/// door non-breaking spaces (zodat telefoon-nrs niet gewordwrapt worden)
		/// </summary>
		/// <param name="htmlHelper">Deze htmlhelper zelf</param>
		/// <param name="telefoonNr">String met telefoonnummer</param>
		/// <returns>Html voor telefoonnr</returns>
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
