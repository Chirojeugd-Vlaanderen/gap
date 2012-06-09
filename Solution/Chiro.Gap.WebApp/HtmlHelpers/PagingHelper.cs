// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
