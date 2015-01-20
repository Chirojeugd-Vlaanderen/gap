/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * Verfijnen gebruikersrechten Copyright 2015 Chirojeugd-Vlaanderen vzw
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

using System.Collections.Generic;
using System.Web.Mvc;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller voor 
    /// </summary>
    public class GebruikersRechtController : BaseController
    {
        public GebruikersRechtController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper)
            : base(veelGebruikt, serviceHelper)
        {
        }

        /// <summary>
        /// Kent een GAV-gebruikersrecht voor 14 maanden toe aan de gelieerde persoon met GelieerdePersoonID <paramref name="id"/>.
        /// Als het gebruikersrecht al bestaat, dan wordt het indien mogelijk verlengd tot 14 maanden vanaf vandaag.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor gebruikersrecht toegekend moet worden</param>
        /// <param name="id">ID van de gelieerde persoon die gebruikersrecht moet krijgen</param>
        /// <returns>Redirect naar de personenfiche</returns>
        public ActionResult AanGpToekennen(int groepID, int id)
        {
            ServiceHelper.CallService<IGebruikersService>(
                svc => svc.RechtenToekennen(id, new GebruikersRecht {GroepsPermissies = Permissies.Bewerken, IedereenPermissies = Permissies.Bewerken}));
            return RedirectToAction("Bewerken", new { Controller = "Personen", id });
        }

        /// <summary>
        /// Neemt alle gebruikersrechten af van de gelieerde persoon met GelieerdePersoonID <paramref name="id"/>
        /// voor de groep met gegeven <paramref name="groepID"/>.  (Concreet wordt de vervaldatum op gisteren gezet.)
        /// </summary>
        /// <param name="id">ID van de gelieerde persoon</param>
        /// <param name="groepID"/>
        /// <returns>Redirect naar personenfiche</returns>
        public ActionResult VanGpAfnemen(int groepID, int id)
        {
            ServiceHelper.CallService<IGebruikersService>(svc => svc.RechtenAfnemen(id, new[] {groepID}));
            return RedirectToAction("Bewerken", new { Controller = "Personen", id });
        }

        /// <summary>
        /// Toont view voor het GAV-beheer voor groep met ID <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor we de GAV's willen zien/beheren</param>
        /// <returns></returns>
        public override ActionResult Index(int groepID)
        {
            var model = new GavOverzichtModel();
            BaseModelInit(model, groepID);

            model.GebruikersDetails = ServiceHelper.CallService<IGroepenService, IEnumerable<GebruikersDetail>>(svc => svc.GebruikersOphalen(groepID));

            model.Titel = Properties.Resources.GebruikersOverzicht;
            return View(model);
        }

        /// <summary>
        /// Creert of verlengt het gebruikersrecht op de groep met gegeven <paramref name="groepID"/> van de
        /// gebruiker met gegeven GelieerdePersoonID <paramref name="id"/>
        /// </summary>
        /// <param name="id">GelieerdePersoonID van gelieerde persoon die gebruikersrecht moet krijgen.</param>
        /// <param name="groepID">ID van groep waarvoor gebruikersrecht aan te maken of te verlengen</param>
        /// <returns>Een redirect naar het gebruikersrechtenoverzicht</returns>
        public ActionResult AanmakenOfVerlengen(int groepID, int id)
        {
            ServiceHelper.CallService<IGebruikersService>(
                gs =>
                    gs.RechtenToekennen(id,
                        new GebruikersRecht
                        {
                            GroepsPermissies = Permissies.Bewerken,
                            IedereenPermissies = Permissies.Bewerken
                        }));

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Ontneemt de user met GelieerdePersoonID <paramref name="id"/> alle rechten op de groep met
        /// gegeven <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">groepID van groep waarop de user geen gebruikersrechten meer mag hebben</param>
        /// <param name="id">GelieerdePersoonID van user die geen gebruikersrechten meer mag hebben op
        /// gegeven groep</param>
        /// <returns>Een redirect naar het gebruikersrechtenoverzicht</returns>
        public ActionResult Intrekken(int groepID, int id)
        {
            ServiceHelper.CallService<IGebruikersService>(
                gs => gs.RechtenAfnemenGelieerdePersoon(id, new[] {groepID}));
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Maakt een login voor de gelieerde persoon met gelieerdepersoonID <paramref name="id"/>.
        /// Kent geen rechten toe.
        /// </summary>
        /// <param name="groepID">ID van groep waarin we werken (eigenlijk irrelevant)</param>
        /// <param name="id">GelieerdePersoonID van persoon die een gebruiker moet krijgen</param>
        /// <returns>Een redirect naar de details van de gegeven gelieerdepersoon</returns>
        public ActionResult LoginMaken(int groepID, int id)
        {
            ServiceHelper.CallService<IGebruikersService>(svc => svc.RechtenToekennen(id, null));
            return RedirectToAction("Bewerken", "Personen", new { groepID, id });
        }
    }
}
