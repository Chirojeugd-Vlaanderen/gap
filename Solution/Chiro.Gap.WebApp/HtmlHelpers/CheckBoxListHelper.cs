// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
	/// <remarks>
	/// Dank aan http://blogs.msdn.com/miah/archive/2008/11/10/checkboxlist-helper-for-mvc.aspx
	/// </remarks>
	public class CheckBoxListInfo
	{
		/// <summary>
		/// Constructor voor CheckBoxListInfo
		/// </summary>
		/// <param name="displayText">Tekst die de gebruiker te zien krijgt na de checkbox</param>
		/// <param name="value">Waarde die doorgegeven moet worden als het item aangekruist is</param>
		/// <param name="isChecked">Bepaalt de huidige status van de checkbox
		/// getoond.</param>
		public CheckBoxListInfo(string value, string displayText, bool isChecked)
			: this(value, displayText, isChecked, String.Empty) { }


		/// <summary>
		/// Constructor voor CheckBoxListInfo
		/// </summary>
		/// <param name="displayText">Tekst die de gebruiker te zien krijgt na de checkbox</param>
		/// <param name="value">Waarde die doorgegeven moet worden als het item aangekruist is</param>
		/// <param name="isChecked">Bepaalt de huidige status van de checkbox</param>
		/// <param name="foutBericht">Indien niet leeg, wordt dit foutbericht na de DisplayText 
		/// getoond.</param>
		public CheckBoxListInfo(string value, string displayText, bool isChecked, string foutBericht)
		{
			Value = value;
			DisplayText = displayText;
			IsChecked = isChecked;
			FoutBericht = foutBericht;
		}

		public string Value { get; private set; }
		public string DisplayText { get; private set; }
		public bool IsChecked { get; private set; }
		public string FoutBericht { get; set; }
	}

	/// <summary>
	/// HTML-helper voor een (dynamische) lijst van checkboxes
	/// Belangrijk: als er niets wordt aangeklikt, wordt NULL teruggegeven ipv een lege lijst
	/// </summary>
	public static class CheckBoxListHelper
	{
		public static string CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<CheckBoxListInfo> listInfo)
		{
			return htmlHelper.CheckBoxList(name, listInfo, null);
		}

		public static string CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<CheckBoxListInfo> listInfo,
			object htmlAttributes)
		{
			return htmlHelper.CheckBoxList(name, listInfo, new RouteValueDictionary(htmlAttributes));
		}

		public static string CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<CheckBoxListInfo> listInfo,
			IDictionary<string, object> htmlAttributes)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException("The argument must have a value", "name");
			}
			if (listInfo == null)
			{
				throw new ArgumentNullException("listInfo");
			}
			if (listInfo.Count() < 1)
			{
				throw new ArgumentException("The list must contain at least one value", "listInfo");
			}

			var sb = new StringBuilder();

			foreach (CheckBoxListInfo info in listInfo)
			{
				var builder = new TagBuilder("input");
				if (info.IsChecked)
				{
					builder.MergeAttribute("checked", "checked");
				}
				builder.MergeAttributes(htmlAttributes);
				builder.MergeAttribute("type", "checkbox");
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
		public static string CheckBoxList(this HtmlHelper htmlHelper, string name, CheckBoxListInfo info)
		{
			return htmlHelper.CheckBoxList(name, info, null);
		}

		public static string CheckBoxList(this HtmlHelper htmlHelper, string name, CheckBoxListInfo info,
		IDictionary<string, object> htmlAttributes)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException("The argument must have a value", "name");
			}
			if (info == null)
			{
				throw new ArgumentNullException("listInfo");
			}

			var sb = new StringBuilder();

			var builder = new TagBuilder("input");
			if (info.IsChecked)
			{
				builder.MergeAttribute("checked", "checked");
			}
			builder.MergeAttributes(htmlAttributes);
			builder.MergeAttribute("type", "checkbox");
			builder.MergeAttribute("value", info.Value);
			builder.MergeAttribute("name", name);
			// sb.Append("\n");
			sb.Append(builder.ToString(TagRenderMode.SelfClosing));
			sb.Append(info.DisplayText);
			// sb.Append("<br />");

			return sb.ToString();
		}
	}
}
