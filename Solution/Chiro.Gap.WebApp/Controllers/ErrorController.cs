// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Net;
using System.Web.Mvc;

namespace Chiro.Gap.WebApp.Controllers
{
	public class ErrorController : Controller
	{
		//
		// GET: /Error/
		public ActionResult Index()
		{
			// Normaal gezien passeren we hier nooit, want de defaultRedirect in web.config gaat naar ~/Shared/Error.aspx.
			Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			return View("Error");
		}

		//
		// GET: /Error/NietGevonden
		public ActionResult NietGevonden()
		{
			Response.StatusCode = (int)HttpStatusCode.NotFound;
			return View("Error");
		}

		public ActionResult GeenVerbinding()
		{
			Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
			return View("Error");
		}

		public ActionResult GeenToegang()
		{
			Response.StatusCode = (int) HttpStatusCode.Forbidden;
			return View("Error");
		}
	}
}
