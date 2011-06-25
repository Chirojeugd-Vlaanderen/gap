// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
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
		public class DropDownListItem<T>
		{
			public T Waarde { get; set; }
			public string DisplayNaam { get; set; }
		}

		public static string DropDownList<T>(this HtmlHelper htmlHelper, string opslagLijstNaam, IEnumerable<DropDownListItem<T>> mogelijkewaarden, T geselecteerd)
		{
			var sb = new StringBuilder();

			var select = new TagBuilder("select");
			select.MergeAttribute("id", opslagLijstNaam);
			select.MergeAttribute("name", opslagLijstNaam);
			sb.Append(select.ToString(TagRenderMode.StartTag));

			foreach (var mogelijkewaarde in mogelijkewaarden)
			{
				var option1 = new TagBuilder("option");
				option1.MergeAttribute("value", mogelijkewaarde.Waarde.ToString());
				if (geselecteerd.Equals(mogelijkewaarde.Waarde))
				{
					option1.MergeAttribute("selected", @"yes");
				}
				sb.Append(option1.ToString(TagRenderMode.StartTag));
				sb.Append(mogelijkewaarde.DisplayNaam);
				sb.Append(option1.ToString(TagRenderMode.EndTag));
			}

			sb.Append(select.ToString(TagRenderMode.EndTag));

			return sb.ToString();
		}
	}
}
