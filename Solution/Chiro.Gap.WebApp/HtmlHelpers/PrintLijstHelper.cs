/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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
				return "(geen)";
			}

			var sb = new StringBuilder();
			int geschreven = 0;

			foreach (var ai in afdelingsInfo.Where(ai => afdelingIdLijst.Contains(ai.AfdelingID)))
			{
				if (geschreven == afdelingIdLijst.Count() - 1)
				{
                    // de laatste afdeling
					sb.Append(ai.AfdelingNaam);
				}
				else if (geschreven == afdelingIdLijst.Count() - 2)
				{
                    // tweede en verdere
					sb.Append(ai.AfdelingNaam + " en ");
				}
				else
				{
                    // de eerste afdeling
					sb.Append(ai.AfdelingNaam + ", ");
				}
				geschreven++;
			}

			return sb.ToString();
		}
	}
}
