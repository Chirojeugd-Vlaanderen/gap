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

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
	/// <summary>
	/// HtmlHelper die de links maakt om door een 'gepagineerde' lijst te navigeren.
	/// </summary>
	public static class PagingHelper
	{
		public static string PagerLinksLetters(this HtmlHelper html, string huidigePagina, IList<String> paginas, Func<string, string> url, bool ookIedereen)
        {
            var resultaat = new StringBuilder();
            foreach (String letter in paginas)
            {
                var tag = new TagBuilder("a");   // Maakt een <a>-tag
                tag.MergeAttribute("href", url(letter));
                tag.InnerHtml = letter.ToUpper();
                if (letter == huidigePagina)
                {
                    tag.AddCssClass("geselecteerd");
                }
                resultaat.AppendLine(tag.ToString());
            }

            // Na alle letters ook nog een link 'iedereen tonen'
            if (ookIedereen)
            {
                var tag = new TagBuilder("a");   // Maakt een <a>-tag
                tag.MergeAttribute("href", url("A-Z"));
                tag.InnerHtml = "Iedereen";
                if ("A-Z" == huidigePagina)
                {
                    tag.AddCssClass("geselecteerd");
                }
                resultaat.AppendLine(tag.ToString());
            }

		    return resultaat.ToString();
        }

		/// <summary>
		/// Genereert een opeenvolging van links op basis van een rij werkjaren
		/// </summary>
		/// <param name="html">HtmlHelper waarvoor dit een extension method is</param>
		/// <param name="huidigWerkJaarID">GroepsWerkJaarID van te 'highlighten' link</param>
		/// <param name="werkjaren">Rij met werkjaarinfo</param>
		/// <param name="url">Functie die een url maakt op basis van WerkJaarInro</param>
		/// <returns></returns>
		public static string WerkJaarLinks(
			this HtmlHelper html,
			int huidigWerkJaarID,
			IEnumerable<WerkJaarInfo> werkjaren,
			Func<WerkJaarInfo, string> url)
		{
			var resultaat = new StringBuilder();
			foreach (var wj in werkjaren)
			{
				var tag = new TagBuilder("a");   // Maakt een <a>-tag
				tag.MergeAttribute("href", url(wj));
				tag.InnerHtml = string.Format("{0}-{1}", wj.WerkJaar, wj.WerkJaar + 1);
				if (wj.ID == huidigWerkJaarID)
				{
					tag.AddCssClass("geselecteerd");
				}
				resultaat.AppendLine(tag.ToString());
			}
			return resultaat.ToString();
		}
	}
}
