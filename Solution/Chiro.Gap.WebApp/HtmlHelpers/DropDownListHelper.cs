// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
	/// <summary>
	/// HTML-helper voor een (dynamische) lijst van checkboxes
	/// Belangrijk: als er niets wordt aangeklikt, wordt NULL teruggegeven ipv een lege lijst
	/// </summary>
	public static class DropDownListHelper
	{
		public static string OffAfdelingsDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<OfficieleAfdelingDetail> afdelingen, string officieleAfdelingNaam)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException(@"The argument must have a value", "name");
			}

			var sb = new StringBuilder();

			var select = new TagBuilder("select");
			select.MergeAttribute("id", name);
			select.MergeAttribute("name", name);
			sb.Append(select.ToString(TagRenderMode.StartTag));

			foreach (var afdeling in afdelingen)
			{
				var option1 = new TagBuilder("option");
				option1.MergeAttribute("value", afdeling.ID.ToString());
				if (officieleAfdelingNaam != null && officieleAfdelingNaam.Equals(afdeling.Naam))
				{
					option1.MergeAttribute("selected", @"yes");
				}
				sb.Append(option1.ToString(TagRenderMode.StartTag));
				sb.Append(afdeling.Naam);
				sb.Append(option1.ToString(TagRenderMode.EndTag));
			}

			sb.Append(select.ToString(TagRenderMode.EndTag));

			return sb.ToString();
		}

		public static string GeslachtsDropDownList(this HtmlHelper htmlHelper, string name, GeslachtsType g)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException(@"The argument must have a value", "name");
			}

			var sb = new StringBuilder();

			var select = new TagBuilder("select");
			select.MergeAttribute("id", name);
			select.MergeAttribute("name", name);
			sb.Append(select.ToString(TagRenderMode.StartTag));

			foreach (var geslacht in Enum.GetValues(typeof(GeslachtsType)))
			{
				var option1 = new TagBuilder("option");
				option1.MergeAttribute("value", ((int)geslacht).ToString());
				if ((GeslachtsType)geslacht == g)
				{
					option1.MergeAttribute("selected", @"yes");
				}
				sb.Append(option1.ToString(TagRenderMode.StartTag));
				sb.Append(geslacht.ToString());
				sb.Append(option1.ToString(TagRenderMode.EndTag));
			}

			sb.Append(select.ToString(TagRenderMode.EndTag));

			return sb.ToString();
		}
	}
}
