using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Chiro.Gap.Orm;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
    /// <summary>
    /// HtmlHelper die de links maakt om door een 'gepagineerde' lijst te navigeren.
    /// </summary>
    public static class PagingHelper
    {
        public static string PagerLinks(this HtmlHelper html, int huidigePagina, int aantalPaginas, Func<int, string> url) 
        {
            StringBuilder resultaat = new StringBuilder();
            for (int i = 1; i <= aantalPaginas; i++)
            {
                TagBuilder tag = new TagBuilder("a");   // Maakt een <a>-tag
                tag.MergeAttribute("href", url(i));
                tag.InnerHtml = i.ToString();
                if (i == huidigePagina)
                {
                    tag.AddCssClass("geselecteerd");
                }
                resultaat.AppendLine(tag.ToString());
            }
            return resultaat.ToString();
        }

        public static string WerkJaarLinks(this HtmlHelper html, int huidigWerkJaar, IList<GroepsWerkJaar> werkjaren, Func<int, string> url)
        {
            StringBuilder resultaat = new StringBuilder();
            for (int i = 0; i < werkjaren.Count; i++)
            {
                TagBuilder tag = new TagBuilder("a");   // Maakt een <a>-tag
                tag.MergeAttribute("href", url(i));
                tag.InnerHtml = werkjaren[i].WerkJaar.ToString();
                if (werkjaren[i].ID == huidigWerkJaar)
                {
                    tag.AddCssClass("geselecteerd");
                }
                resultaat.AppendLine(tag.ToString());
            }
            return resultaat.ToString();
        }
    }
}
