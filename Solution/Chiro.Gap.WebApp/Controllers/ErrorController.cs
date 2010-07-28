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
			Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View("Error");
        }

		//
		// GET: /Error/NotFound
		public ActionResult NotFound()
		{
			Response.StatusCode = (int) HttpStatusCode.NotFound;
			return View("Error");
		}
    }
}
