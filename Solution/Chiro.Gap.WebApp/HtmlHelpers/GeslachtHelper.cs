using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
