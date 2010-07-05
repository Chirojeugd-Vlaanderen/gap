// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

// dank aan http://blogs.msdn.com/miah/archive/2008/11/10/checkboxlist-helper-for-mvc.aspx

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
	/// <summary>
	/// HTML-helper die een lijst van elementen uitschrijft in een nederlandse zin (overal "de" voorzetten, juiste komma's en "en").
	/// </summary>
	public static class PrintLijstHelper
	{
		/// <summary>
		/// Print een lijst van afdelingsnamen met de juiste spaties, "de" en "een" tussenvoegsels en komma's.
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="afdelingIdLijst"></param>
		/// <param name="afdelingsInfo"></param>
		/// <returns></returns>
		public static string PrintLijst(this HtmlHelper htmlHelper, IEnumerable<int> afdelingIdLijst, IEnumerable<AfdelingDetail> afdelingsInfo)
		{
			if (afdelingIdLijst == null || afdelingIdLijst.Count() < 1)
			{
				return string.Empty;
			}

			var sb = new StringBuilder();
			int geschreven = 0;

			foreach (var ai in afdelingsInfo.Where(ai => afdelingIdLijst.Contains(ai.AfdelingID)))
			{
				if (geschreven == afdelingIdLijst.Count() - 1)
				{
                    // de laatste afdeling
					sb.Append("de " + ai.AfdelingNaam + ".\n");
				}
				else if (geschreven == afdelingIdLijst.Count() - 2)
				{
                    // tweede en verdere
					sb.Append("de " + ai.AfdelingNaam + " en ");
				}
				else
				{
                    // de eerste afdeling
					sb.Append("de " + ai.AfdelingNaam + ", ");
				}
				geschreven++;
			}

			return sb.ToString();
		}
	}
}
