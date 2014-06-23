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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Diagnostics.ServiceContracts;
using Chiro.Gap.Diagnostics.WebApp.Models;

namespace Chiro.Gap.Diagnostics.WebApp.Controllers
{
    /// <summary>
    /// Controller voor adresbeheer
    /// </summary>
    public class AdressenController : Controller
    {
        /// <summary>
        /// De indexpagina voor het adresbeheer
        /// </summary>
        /// <returns>De indexpagina voor het adresbeheer</returns>
        public ActionResult Index()
        {
            var model = new AddressenIndexModel
                            {
                                AantalVerdwenenAdressen =
                                    ServiceHelper.CallService<IAdminService, int>(
                                        svc => svc.AantalVerdwenenAdressenOphalen()),
                                RapportUrl = Properties.Settings.Default.VerdwenenAdressenRapportUrl
                            };

            return View(model);
        }

        /// <summary>
        /// Stuur alle ontbrekende adressen opnieuw van GAP naar Kipadmin
        /// </summary>
        /// <returns>Opnieuw de view met ontbrekende adressen</returns>
        public ActionResult OpnieuwSyncen()
        {
            ServiceHelper.CallService<IAdminService>(svc => svc.OntbrekendeAdressenSyncen());
            return RedirectToAction("Index");
        }

    }
}
