/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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

using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Chiro.Cdf.ExcelManip
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

			// Zetten van de cacheability verwijderd, omdat dat problemen opleverde
			// met Internet Explorer (#775)
			// context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

			context.Response.ContentType = @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			context.Response.BinaryWrite(_stream.ToArray());
			context.Response.End();
		}
	}
}
