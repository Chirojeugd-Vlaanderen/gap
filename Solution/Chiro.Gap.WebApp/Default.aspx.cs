// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Chiro.Gap.WebApp
{
	public class _Default : Page
	{
		public void Page_Load(object sender, System.EventArgs e)
		{
			HttpContext.Current.RewritePath(Request.ApplicationPath, false);
			IHttpHandler httpHandler = new MvcHttpHandler();
			httpHandler.ProcessRequest(HttpContext.Current);
		}
	}
}
