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
using Chiro.Gap.Diagnostics.ServiceContracts.DataContracts;
using Chiro.Gap.Diagnostics.WebApp.Models;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.Diagnostics.WebApp.Controllers
{
    /// <summary>
    /// Controller voor het toekennen van tijdelijke gebruikersrechten
    /// </summary>
    public class LoginsController : Controller
    {
        /// <summary>
        /// Toont de groepen waar de gebruiker al toegang toe heeft, met een link
        /// naar het GAP van die groepen.  Via een formulier wordt de mogelijkheid
        /// geboden tijdelijke rechten te krijgen voor een bijkomende groep.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new LoginsModel
                            {
                                GapUrl = Properties.Settings.Default.GapUrl,
                                Groepen = ServiceHelper.CallService<IGroepenService, IEnumerable<GroepInfo>>
                                    (g => g.MijnGroepenOphalen())
                            };

            return View(model);
        }

        /// <summary>
        /// Deze method wordt aangeroepen als de user een stamnummer invulde in de Index-view, om tijdelijke
        /// gebruikersrechten te krijgen voor een groep.
        /// </summary>
        /// <param name="model">Het stamnummer in het model is het stamnummer van de groep waarvoor je
        /// gebruikersrechten wilt</param>
        /// <returns>Een redirect naar 'RechtenToekennen'</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(LoginsModel model)
        {
            return RedirectToAction("RechtenToekennen", new {stamNummer = model.StamNummer});
        }

        /// <summary>
        /// Vraagt bevestiging om tijdelijke gebruikersrechten voor de groep te krijgen.  De gebruiker moet
        /// iemand van de 'contactpersonen' (contactpersoon van de groep, of een van de GAV's als we weten
        /// wie het is) selecteren, die een notificatie zal krijgen.
        /// </summary>
        /// <param name="stamNummer">stamNummer van groep waarvoor je gebruikersrechten wilt</param>
        /// <returns>De view die om bevestiging vraagt</returns>
        public ActionResult RechtenToekennen(string stamNummer)
        {
            var model = new NotificatieModel
                            {
                                GroepContactInfo =
                                    ServiceHelper.CallService<IAdminService, GroepContactInfo>(
                                        svc => svc.ContactInfoOphalen(stamNummer))
                            };

            return View(model);
        }

        /// <summary>
        /// Roept de service aan om rechten toe te kennen aan de groep met stamnummer
        /// <code>model.StamNummer</code>; de service zal mailen naar de persoon met 
        /// gelieerdepersoonID <code>model.MailOntvangerGelieerdePersoonID</code>.
        /// </summary>
        /// <param name="model">Bevat stamnummer en gelieerdepersoonID</param>
        /// <returns>Achteraf wordt geredirect naar het overzicht van de logins</returns>
        /// <remarks>Controle of de gelieerde persoon wel gekoppeld is aan de groep,
        /// gebeurt door de service</remarks>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RechtenToekennen(NotificatieModel model)
        {
            ServiceHelper.CallService<IAdminService>(
                svc => svc.TijdelijkeRechtenGeven(model.MailOntvangerGelieerdePersoonID, model.Reden));
            return RedirectToAction("Index");
        }
    }
}
