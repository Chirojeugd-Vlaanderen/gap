// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Net;
using System.Web.Mvc;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller die foutmeldingen laat weergeven
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// De standaardconstructor
        /// </summary>
        /// <returns></returns>
        /// <!-- GET: /Error/ -->
        public ActionResult Index()
        {
            // Normaal gezien passeren we hier nooit, want de defaultRedirect in web.config gaat naar ~/Shared/Error.aspx.
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View("Error");
        }

        /// <summary>
        /// Toont de foutpagina met een aangepaste foutcode
        /// </summary>
        /// <!-- GET: /Error/NietGevonden -->
        public ActionResult NietGevonden()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View("Error");
        }

        /// <summary>
        /// Toont de foutpagina met een aangepaste foutcode
        /// </summary>
        /// <!-- GET: /Error/GeenVerbinding -->
        public ActionResult GeenVerbinding()
        {
            Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            return View("Error");
        }

        /// <summary>
        /// Toont de foutpagina met een aangepaste foutcode
        /// </summary>
        /// <!-- GET: /Error/GeenToegang -->
        public ActionResult GeenToegang()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return View("Error");
        }
    }
}
