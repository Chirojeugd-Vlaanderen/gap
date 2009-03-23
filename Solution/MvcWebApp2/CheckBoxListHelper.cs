// dank aan http://blogs.msdn.com/miah/archive/2008/11/10/checkboxlist-helper-for-mvc.aspx

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Text;
using System.Web.Mvc;

namespace MvcWebApp2
{

    /// <summary>
    /// Klasse die de status van een checkbox omschrijft
    /// </summary>
    public class CheckBoxListInfo
    {
        public CheckBoxListInfo(string value, string displayText, bool isChecked)
        {
            this.Value = value;
            this.DisplayText = displayText;
            this.IsChecked = isChecked;
        }

        public string Value { get; private set; }
        public string DisplayText { get; private set; }
        public bool IsChecked { get; private set; }
    }

    /// <summary>
    /// HTML-helper voor een (dynamische) lijst van checkboxes
    /// </summary>
    public static class CheckBoxListHelper
    {
            public static string CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo)
            {
                return htmlHelper.CheckBoxList(name, listInfo,
                    ((IDictionary<string, object>)null));
            }

            public static string CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo,
                object htmlAttributes)
            {
                return htmlHelper.CheckBoxList(name, listInfo,
                    ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)));
            }

            public static string CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo,
                IDictionary<string, object> htmlAttributes)
            {
                if (String.IsNullOrEmpty(name))
                    throw new ArgumentException("The argument must have a value", "name");
                if (listInfo == null)
                    throw new ArgumentNullException("listInfo");
                if (listInfo.Count < 1)
                    throw new ArgumentException("The list must contain at least one value", "listInfo");

                StringBuilder sb = new StringBuilder();

                foreach (CheckBoxListInfo info in listInfo)
                {
                    TagBuilder builder = new TagBuilder("input");
                    if (info.IsChecked) builder.MergeAttribute("checked", "checked");
                    builder.MergeAttributes<string, object>(htmlAttributes);
                    builder.MergeAttribute("type", "checkbox");
                    builder.MergeAttribute("value", info.Value);
                    builder.MergeAttribute("name", name);
                    sb.Append("\n");
                    sb.Append(builder.ToString(TagRenderMode.SelfClosing));
                    sb.Append(info.DisplayText);
                    sb.Append("<br />");
                }

                return sb.ToString();
            }

    }
}
