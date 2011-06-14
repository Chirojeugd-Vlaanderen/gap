// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

// Dank aan http://blogs.msdn.com/miah/archive/2008/11/10/checkboxlist-helper-for-mvc.aspx

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
	/// <summary>
	/// Klasse die de status van een checkbox omschrijft
	/// </summary>
	public class TextFieldListInfo
	{
		/// <summary>
		/// Constructor voor TextFieldListInfo
		/// </summary>
		/// <param name="displayText">Tekst die de gebruiker te zien krijgt na de checkbox</param>
		/// <param name="value">Waarde die doorgegeven moet worden als het item aangekruist is</param>
		public TextFieldListInfo(string value, string displayText)
			: this(value, displayText, String.Empty) { }

		/// <summary>
		/// Constructor voor TextFieldListInfo
		/// </summary>
		/// <param name="displayText">Tekst die de gebruiker te zien krijgt na de checkbox</param>
		/// <param name="value">Waarde die doorgegeven moet worden als het item aangekruist is</param>
		/// <param name="foutBericht">Indien niet leeg, wordt dit foutbericht na de DisplayText 
		/// getoond.</param>
		public TextFieldListInfo(string value, string displayText, string foutBericht)
		{
			Value = value;
			DisplayText = displayText;
			FoutBericht = foutBericht;
		}

		public string Value { get; private set; }
		public string DisplayText { get; private set; }
		public string FoutBericht { get; set; }
	}

	/// <summary>
	/// HTML-helper voor een (dynamische) lijst van checkboxes
	/// Belangrijk: als er niets wordt aangeklikt, wordt NULL teruggegeven ipv een lege lijst
	/// </summary>
	public static class TextFieldListHelper
	{
		public static string TextFieldList(this HtmlHelper htmlHelper, string name, IEnumerable<TextFieldListInfo> listInfo)
		{
			return htmlHelper.TextFieldList(name, listInfo, null);
		}

		public static string TextFieldList(this HtmlHelper htmlHelper, string name, IEnumerable<TextFieldListInfo> listInfo,
			object htmlAttributes)
		{
			return htmlHelper.TextFieldList(name, listInfo, new RouteValueDictionary(htmlAttributes));
		}

		public static string TextFieldList(this HtmlHelper htmlHelper, string name, IEnumerable<TextFieldListInfo> listInfo,
			IDictionary<string, object> htmlAttributes)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException(@"The argument must have a value", "name");
			}
			if (listInfo == null)
			{
				throw new ArgumentNullException("listInfo");
			}
			if (listInfo.Count() < 1)
			{
				throw new ArgumentException(@"The list must contain at least one value", "listInfo");
			}

			var sb = new StringBuilder();

			foreach (TextFieldListInfo info in listInfo)
			{
				var builder = new TagBuilder("input");

				builder.MergeAttributes(htmlAttributes);
				builder.MergeAttribute("type", @"text");
				builder.MergeAttribute("value", info.Value);
				builder.MergeAttribute("name", name);
				sb.Append("\n");
				sb.Append(builder.ToString(TagRenderMode.SelfClosing));
				sb.Append(info.DisplayText);
				if (info.FoutBericht != String.Empty)
				{
					sb.Append(String.Format("  <span class='field-validation-error'>{0}</span>", info.FoutBericht));
				}
				sb.Append("<br />");
			}

			return sb.ToString();
		}

		/// <summary>
		/// The same method, but can be used one at a time: it takes only one checkboxlistinfo
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="name"></param>
		/// <param name="info"></param>
		/// <returns></returns>
		public static string TextFieldList(this HtmlHelper htmlHelper, string name, TextFieldListInfo info)
		{
			return htmlHelper.TextFieldList(name, info, null);
		}

		public static string TextFieldList(this HtmlHelper htmlHelper, string name, TextFieldListInfo info,
		IDictionary<string, object> htmlAttributes)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException(@"The argument must have a value", "name");
			}
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			var sb = new StringBuilder();

			var builder = new TagBuilder("input");
			builder.MergeAttributes(htmlAttributes);
			builder.MergeAttribute("type", @"text");
			builder.MergeAttribute("value", info.Value);
			builder.MergeAttribute("name", name);
			// sb.Append("\n");
			sb.Append(builder.ToString(TagRenderMode.SelfClosing));
			sb.Append(info.DisplayText);
			// sb.Append("<br />");

			return sb.ToString();
		}

		public static string LabelFieldList(this HtmlHelper htmlHelper, string name, TextFieldListInfo info)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException(@"The argument must have a value", "name");
			}
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			var sb = new StringBuilder();

			var builder = new TagBuilder("input");
			builder.MergeAttribute("type", @"hidden");
			builder.MergeAttribute("value", info.Value);
			builder.MergeAttribute("id", name);
			builder.MergeAttribute("name", name);
			sb.Append(builder.ToString(TagRenderMode.SelfClosing));
			sb.Append(info.DisplayText);

			return sb.ToString();
		}
	}
}
