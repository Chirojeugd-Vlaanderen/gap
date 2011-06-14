// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Web;

namespace Chiro.Gap.WebApp
{
	/// <summary>
	/// Deze klasse dient om clientstate op te bouwen om bij te houden naar welke vorige pagina we kunnen 
	/// terugkeren, door tijdelijke cookies aan te maken. De cookies worden verwijderd als de browser sluit.
	/// <para />
	/// Er zijn twee cookies momenteel: een lijstcookie en een fichecookie. Dit maakt werken in twee lagen 
	/// mogelijk: eerst terugkeren naar de fiche als die nog bestaat en relevant is, anders naar de lijst 
	/// terugkeren. Belangrijk: wel zorgen dat naar de juiste fiche wordt teruggegaan, niet per ongeluk naar 
	/// een andere fiche die niet meer relevant is.
	/// </summary>
	public static class ClientState
	{
		private const string LIJSTCOOKIE = "VorigeLijst";
		private const string FICHECOOKIE = "VorigeFiche";

		/// <summary>
		/// Bevat de url van de vorig opgevraagde lijst.
		/// </summary>
		public static String VorigeLijst
		{
			get
			{
				var s = HttpContext.Current.Request.Cookies.Get(LIJSTCOOKIE);
				if (s == null || String.IsNullOrEmpty(s.Value))
				{
					return null;
				}
				return s.Value;
			}
			set
			{
				var s = HttpContext.Current.Response.Cookies.Get(LIJSTCOOKIE);
				if (s == null)
				{
					// TODO: Dit lijkt me het verkeerde exceptiontype

					throw new AccessViolationException(
						"De cookie kan niet gemaakt worden, zijn cookies uitgeschakeld?");
				}
				s.Value = value;
			}
		}

		/// <summary>
		/// Bevat de url van de vorig opgevraagde fiche.
		/// </summary>
		public static String VorigeFiche
		{
			get
			{
				var s = HttpContext.Current.Request.Cookies.Get(FICHECOOKIE);
				if (s == null || String.IsNullOrEmpty(s.Value))
				{
					return null;
				}
				return s.Value;
			}
			set
			{
				var s = HttpContext.Current.Response.Cookies.Get(FICHECOOKIE);
				if (s == null)
				{
					// TODO: Dit lijkt me het verkeerde exceptiontype

					throw new AccessViolationException(
						"De cookie kan niet gemaakt worden, zijn cookies uitgeschakeld?");
				}
				s.Value = value;
			}
		}
	}
}
