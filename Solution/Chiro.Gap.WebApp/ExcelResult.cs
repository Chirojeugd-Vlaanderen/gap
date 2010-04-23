// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.Mvc;

namespace Chiro.Gap.WebApp
{
	/// <summary>
	/// Actionresult voor een Exceldocument.  Schaamteloos overgenomen van
	/// http://stephenwalther.com/blog/archive/2008/06/16/asp-net-mvc-tip-2-create-a-custom-action-result-that-returns-microsoft-excel-documents.aspx
	/// </summary>
	public class ExcelResult : ActionResult
	{
		private readonly string _fileName;
		private readonly IQueryable _rows;
		private readonly string[] _headers = null;

		private readonly TableStyle _tableStyle;
		private readonly TableItemStyle _headerStyle;
		private readonly TableItemStyle _itemStyle;

		public string FileName
		{
			get
			{
				return _fileName;
			}
		}

		public IQueryable Rows
		{
			get
			{
				return _rows;
			}
		}


		public ExcelResult(IQueryable rows, string fileName)
			: this(rows, fileName, null, null, null, null)
		{
		}

		public ExcelResult(string fileName, IQueryable rows, string[] headers)
			: this(rows, fileName, headers, null, null, null)
		{
		}

		public ExcelResult(IQueryable rows, string fileName, string[] headers, TableStyle tableStyle, TableItemStyle headerStyle, TableItemStyle itemStyle)
		{
			_rows = rows;
			_fileName = fileName;
			_headers = headers;
			_tableStyle = tableStyle;
			_headerStyle = headerStyle;
			_itemStyle = itemStyle;

			// provide defaults
			if (_tableStyle == null)
			{
				_tableStyle = new TableStyle();
				_tableStyle.BorderStyle = BorderStyle.Solid;
				_tableStyle.BorderColor = Color.Black;
				_tableStyle.BorderWidth = Unit.Parse("2px");
			}
			if (_headerStyle == null)
			{
				_headerStyle = new TableItemStyle();
				_headerStyle.BackColor = Color.LightGray;
			}
		}

		public override void ExecuteResult(ControllerContext context)
		{
			// Create HtmlTextWriter
			var sw = new StringWriter();
			var tw = new HtmlTextWriter(sw);

			// Build HTML Table from Items
			if (_tableStyle != null)
			{
				_tableStyle.AddAttributesToRender(tw);
			}
			tw.RenderBeginTag(HtmlTextWriterTag.Table);

			// Ik heb hier geen datacontext, dus ik kan daar de headers niet uit halen, zoals dat gebeurde
			// in het originele artikel. Misschien is er nog wel iets anders op te vinden.
			if (_headers == null)
			{
				// _headers = _dataContext.Mapping.GetMetaType(_rows.ElementType).PersistentDataMembers.Select(m => m.Name).ToArray();
			}


			// Create Header Row

			tw.RenderBeginTag(HtmlTextWriterTag.Thead);
			foreach (String header in _headers)
			{
				if (_headerStyle != null)
				{
					_headerStyle.AddAttributesToRender(tw);
				}
				tw.RenderBeginTag(HtmlTextWriterTag.Th);
				tw.Write(header);
				tw.RenderEndTag();
			}
			tw.RenderEndTag();



			// Create Data Rows
			tw.RenderBeginTag(HtmlTextWriterTag.Tbody);
			foreach (Object row in _rows)
			{
				tw.RenderBeginTag(HtmlTextWriterTag.Tr);
				foreach (string header in _headers)
				{
					var value = row.GetType().GetProperty(header).GetValue(row, null);
					string strValue = (value == null ? string.Empty : value.ToString());
					strValue = ReplaceSpecialCharacters(strValue);
					if (_itemStyle != null)
					{
						_itemStyle.AddAttributesToRender(tw);
					}
					tw.RenderBeginTag(HtmlTextWriterTag.Td);
					tw.Write(HttpUtility.HtmlEncode(strValue));
					tw.RenderEndTag();
				}
				tw.RenderEndTag();
			}
			tw.RenderEndTag(); // tbody

			tw.RenderEndTag(); // table
			WriteFile(_fileName, "application/ms-excel", sw.ToString());
		}


		private static string ReplaceSpecialCharacters(string value)
		{
			value = value.Replace("’", "'");
			value = value.Replace("“", "\"");
			value = value.Replace("”", "\"");
			value = value.Replace("–", "-");
			value = value.Replace("…", "...");
			return value;
		}

		private static void WriteFile(string fileName, string contentType, string content)
		{
			HttpContext context = HttpContext.Current;
			context.Response.Clear();
			context.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
			context.Response.Charset = string.Empty;
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.ContentType = contentType;
			context.Response.Write(content);
			context.Response.End();
		}
	}

}
