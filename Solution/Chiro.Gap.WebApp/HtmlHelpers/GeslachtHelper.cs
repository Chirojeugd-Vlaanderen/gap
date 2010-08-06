// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Web.Mvc;

using Chiro.Gap.Domain;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
	/// <summary>
	/// Html-helper die een 'GeslachtsType' omzet naar een symbooltje
	/// </summary>
	public static class GeslachtHelper
	{
		public static string Geslacht(this HtmlHelper htmlHelper, GeslachtsType g)
		{
			return g == GeslachtsType.Man ? "&#9794;" : g == GeslachtsType.Vrouw ? "&#9792;" : "&#9794;&#9792;?";
		}
	}
}
