/*
 * Copyright 2008-2013, 2017 the GAP developers. See the NOTICE file at the 
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

using System.Web.Mvc;
using Chiro.Cdf.Authentication;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller voor alles wat te maken heeft met de onafgewerkte taken van de groep
    /// </summary>
	[HandleError]
	public class GavTakenController : BaseController
	{
	    /// <summary>
	    /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
	    /// best toegewezen via inversion of control.
	    /// </summary>
	    /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
	    /// service</param>
	    /// <param name="serviceHelper"></param>
	    /// <param name="authenticator"></param>
	    public GavTakenController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper, IAuthenticator authenticator)
            : base(veelGebruikt, serviceHelper, authenticator)
        {
        }

        [HandleError]
		public override ActionResult Index(int groepID)
		{
			var model = new GavTakenModel();
			BaseModelInit(model, groepID);
			model.Titel = "Taken voor de groepsadministratieverantwoordelijke (GAV)";
			return View(model);
		}

        /// <summary>
        /// Cleart de ledenproblementelling en keert dan terug naar zichzelf
        /// </summary>
        /// <param name="groepID">ID van de actieve groep</param>
        [HandleError]
        public ActionResult LedenproblemenHertellen(int groepID)
        {
            VeelGebruikt.LedenProblemenResetten(groepID);
            return RedirectToAction("Index", groepID);
        }
    }
}