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
