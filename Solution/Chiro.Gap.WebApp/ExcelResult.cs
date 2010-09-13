// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.IO;
using System.Web;
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
		private readonly MemoryStream _stream;

		public ExcelResult(MemoryStream stream, string fileName)
		{
			_stream = stream;
			_fileName = fileName;
		}

		public override void ExecuteResult(ControllerContext cContext)
		{
			HttpContext context = HttpContext.Current;
			context.Response.Clear();
			context.Response.AddHeader(@"content-disposition", @"attachment; filename=" + _fileName);
			context.Response.Charset = string.Empty;
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.ContentType = @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			context.Response.BinaryWrite(_stream.ToArray());
			context.Response.End();
		}
	}
}
