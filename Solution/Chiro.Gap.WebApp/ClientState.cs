// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Web;
using System.Web.Mvc;

namespace Chiro.Gap.WebApp
{
	/// <summary>
	/// 
	/// </summary>
	public static class ClientState
	{
		private const string URLCOOKIE = "VorigeURL";

		/// <summary>
		/// Bevat de url van de vorige opgeslagen pagina
		/// </summary>
		public static String VorigePagina
		{
			get
			{
				var s = HttpContext.Current.Request.Cookies.Get(URLCOOKIE);
				if (s == null || String.IsNullOrEmpty(s.Value))
				{
					return null;
				}
				else
				{
					return s.Value;
				}
			}
			set
			{
				var s = HttpContext.Current.Response.Cookies.Get(URLCOOKIE);
				if(s==null)
				{
					throw new AccessViolationException("De cookie kan niet gemaakt worden.");
				}
				s.Value = value;
			}
		}
	}
}
