/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
                // Workaround voor #1273
                if (HttpContext.Current == null)
                {
                    return null;
                }

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
                    // TODO (#1056): Dit lijkt me het verkeerde exceptiontype

					throw new AccessViolationException(
						"De cookie kan niet gemaakt worden, zijn cookies uitgeschakeld?");
				}
				s.Value = value;
			}
		}
	}
}
