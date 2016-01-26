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

using System.Web.Mvc;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller die de verschillende pagina's van de handleiding toont
    /// </summary>
    [HandleError]
    public class HandleidingController : BaseController
    {
        /// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        public HandleidingController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper) : base(veelGebruikt, serviceHelper) { }

        [HandleError]
        public override ActionResult Index(int groepID)
        {
            var model = new HandleidingModel();
            BaseModelInit(model, groepID, "Handleiding");
            return View(model);
        }

        [HandleError]
        public ActionResult ViewTonen(int? groepID, string helpBestand)
        {
            var model = new HandleidingModel { Titel = "Handleiding" };

            if (groepID != null && groepID > 0)
            {
                BaseModelInit(model, (int)groepID);
            }
            else
            {
                model.GroepID = 0;
            }
            return View(helpBestand, "Handleiding", model);
        }
    }
}