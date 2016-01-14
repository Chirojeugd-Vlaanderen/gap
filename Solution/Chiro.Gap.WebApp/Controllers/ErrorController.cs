/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
