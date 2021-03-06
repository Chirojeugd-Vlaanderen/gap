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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
	/// <summary>
	/// Klasse die de status van een checkbox omschrijft
	/// </summary>
	/// <remarks>
	/// Dank aan http://blogs.msdn.com/miah/archive/2008/11/10/checkboxlist-helper-for-mvc.aspx
	/// </remarks>
	public class RadioButtonListInfo
	{
		public string Value { get; set; }
		public string DisplayText { get; set; }
		public bool IsChecked { get; set; }
	}

	/// <summary>
	/// HTML-helper voor een (dynamische) lijst van checkboxes
	/// Belangrijk: als er niets wordt aangeklikt, wordt NULL teruggegeven ipv een lege lijst
	/// </summary>
	public static class RadioButtonListHelper
	{
		// TODO dit zou moeten werken vergelijkbaar met checkboxlist, maar bij radiobuttons lukt het blijkbaar niet om die onafhankelijk te laten werken en het geheel van resultaten in een lijst te steken (in asp.net mvc2).

		private static string RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<RadioButtonListInfo> listInfo, int counter)
		{
			if (listInfo == null)
			{
				throw new ArgumentNullException("listInfo");
			}
			if (!listInfo.Any())
			{
				throw new ArgumentException(@"The list must contain at least one value", "listInfo");
			}

			var sb = new StringBuilder();
			foreach (var info in listInfo)
			{
				sb.Append(htmlHelper.RadioButtonList(name, info, counter));
			}
			return sb.ToString();
		}

		/// <summary>
		/// Voeg 1 checkbox toe aan de lijst per keer
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="opslagLijst"></param>
		/// <param name="info"></param>
		/// <param name="counter"></param>
		/// <returns></returns>
		private static string RadioButtonList(this HtmlHelper htmlHelper, string opslagLijst, RadioButtonListInfo info, int counter)
		{
			if (String.IsNullOrEmpty(opslagLijst))
			{
				throw new ArgumentException(@"The argument must have a value", "opslagLijst");
			}
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			var sb = new StringBuilder();

			var builder = new TagBuilder("input");
			if (info.IsChecked)
			{
				builder.MergeAttribute("checked", @"checked");
			}
			// builder.MergeAttributes(htmlAttributes);
			builder.MergeAttribute("type", @"radio");
			builder.MergeAttribute("value", info.Value);
			builder.MergeAttribute("id", counter.ToString());
			builder.MergeAttribute("name", opslagLijst);
			sb.Append(builder.ToString(TagRenderMode.SelfClosing));
			sb.Append(info.DisplayText);

			return sb.ToString();
		}
	}
}
