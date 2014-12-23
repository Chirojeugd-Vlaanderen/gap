/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
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

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Mvc;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Validatie;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller voor toegang tot groepsinstellingen
    /// </summary>
	[HandleError]
	public class GroepController : BaseController
	{
		/// <summary>
		/// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
		/// best toegewezen via inversion of control.
		/// </summary>
		/// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
		/// service</param>
        public GroepController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper) : base(veelGebruikt, serviceHelper) { }

		/// <summary>
		/// Genereert een view met algemene gegevens over de groep
		/// </summary>
		/// <param name="groepID">ID van de gewenste groep</param>
		/// <returns>View met algemene gegevens over de groep</returns>
		[HandleError]
		public override ActionResult Index(int groepID)
		{
            var werkjaarID = ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID));
            var model = new GroepsInstellingenModel
            {
                Titel = Properties.Resources.GroepsInstellingenTitel,
                Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(svc => svc.DetailOphalen(groepID)),
                NonActieveAfdelingen = ServiceHelper.CallService<IGroepenService, List<AfdelingInfo>>(svc => svc.OngebruikteAfdelingenOphalen(werkjaarID))
            };

            // Ook hier nakijken of we live zijn.
            model.IsLive = VeelGebruikt.IsLive();

            return View(model);
		}

        // NOTE: onderstaande methodes komen voort uit opsplitsing groepenscherm, maar halen dus eigenlijk wat teveel info op.

        /// <summary>
        /// Genereert een view met actieve afdelings gegevens over de groep
        /// </summary>
        /// <param name="groepID">ID van de gewenste groep</param>
        /// <returns>View met actieve afdelings  gegevens over de groep</returns>
        [HandleError]
        public ActionResult Afdelingen(int groepID)
        {
            return Index(groepID);
        }

        /// <summary>
        /// Genereert een view met categorie gegevens over de groep
        /// </summary>
        /// <param name="groepID">ID van de gewenste groep</param>
        /// <returns>View met categorie gegevens over de groep</returns>
        [HandleError]
        public ActionResult Categorieen(int groepID)
        {
            return Index(groepID);
        }

        /// <summary>
        /// Genereert een view met functie gegevens over de groep
        /// </summary>
        /// <param name="groepID">ID van de gewenste groep</param>
        /// <returns>View met functie gegevens over de groep</returns>
        [HandleError]
        public ActionResult Functies(int groepID)
        {
            return Index(groepID);
        }

        /// <summary>
        /// Laat de gebruiker de naam van de groep <paramref name="groepID"/> bewerken.
        /// </summary>
        /// <param name="groepID">ID van de geselecteerde groep</param>
        /// <returns>De view 'afdelingsinstellingen'</returns>
        [HandleError]
        public JsonResult NaamWijzigen(int groepID)
        {
            var model = new GroepsInstellingenModel
            {
                Titel = "Groepsnaam wijzigen",
                Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(svc => svc.DetailOphalen(groepID))
            };
           
            // Ook hier nakijken of we live zijn.
            model.IsLive = VeelGebruikt.IsLive();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Postback voor bewerken van de groepsnaam
        /// </summary>
        /// <param name="model">De property <c>model.AfdelingsJaar</c> bevat de relevante details over de groep</param>
        /// <param name="groepID">Groep waarin de gebruiker momenteel aan het werken is</param>
        /// <returns>De view 'afdelingsinstellingen'</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult NaamWijzigen(GroepInfoModel model, int groepID)
        {
            BaseModelInit(model, groepID);

            try
            {    
                ServiceHelper.CallService<IGroepenService>(e => e.Bewaren(model.Info));
                TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;
                // Aangemaakt om de gecachte naam te kunnen leegmaken. Zie bug #1270
                VeelGebruikt.GroepsWerkJaarResetten(groepID);
                
                return RedirectToAction("Index");
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                ModelState.AddModelError("fout", ex.Detail.Bericht);

                
                model.Titel = "Groepsnaam wijzigen";
                return View("NaamWijzigen", model);
            }
        }

        /// <summary>
        /// Laat de user het adres van de lokalen bewerken.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan we het adres willen bewerken.</param>
        /// <returns>Het formulier voor het aanpassen van het lokalenadres.</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult AdresBewerken(int groepID)
        {
            var model = new GroepsAdresModel();
            BaseModelInit(model, groepID);

            model.Adres =
                ServiceHelper.CallService<IGroepenService, GroepDetail>(svc => svc.DetailOphalen(groepID)).Adres ??
                new AdresInfo { LandNaam = Properties.Resources.Belgie };

            // Als het adres buitenlands is, dan moeten we de woonplaats nog eens overnemen in
            // WoonPlaatsBuitenland.  Dat is nodig voor de AdresBewerkenControl, die een beetje
            // raar ineen zit.
            if (String.Compare(model.Adres.LandNaam, Properties.Resources.Belgie, StringComparison.OrdinalIgnoreCase) != 0)
            {
                model.WoonPlaatsBuitenLand = model.Adres.WoonPlaatsNaam;
            }

            model.Titel = Properties.Resources.AdresLokalen;
            model.AlleLanden = VeelGebruikt.LandenOphalen();
            model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.Adres.PostNr);

            return View(model);
        }

        /// <summary>
        /// Als <paramref name="model"/> geen fouten bevat, stuurt deze controller action het 
        /// bijgewerkte adres naar de backend om te persisteren.
        /// </summary>
        /// <param name="groepID">Huidige groep</param>
        /// <param name="model">Bevat de nieuw adres voor de lokalen van een groep</param>
        /// <returns>Als alles goed liep, wordt geredirect naar de groepsinstellingen.  Anders
        /// opnieuw de view voor het aanpassen van de uitstap, met de nodige feedback</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdresBewerken(int groepID, GroepsAdresModel model)
        {
            // Als het adres buitenlands is, neem dan de woonplaats over uit het
            // vrij in te vullen veld.

            if (String.Compare(model.Land, Properties.Resources.Belgie, StringComparison.OrdinalIgnoreCase) != 0)
            {
                model.WoonPlaatsNaam = model.WoonPlaatsBuitenLand;
            }

            try
            {
                // De service zal model.Adres.ID negeren; dit wordt
                // steeds opnieuw opgezocht.  Adressen worden nooit
                // gewijzigd, enkel bijgemaakt (en eventueel verwijderd.)

                ServiceHelper.CallService<IGroepenService>(l => l.AdresInstellen(groepID, model.Adres));

                return RedirectToAction("Index");
            }
            catch (FaultException<OngeldigObjectFault> ex)
            {
                BaseModelInit(model, groepID);

                new ModelStateWrapper(ModelState).BerichtenToevoegen(ex.Detail, String.Empty);

                model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.PostNr);
                model.AlleLanden = VeelGebruikt.LandenOphalen();
                model.Titel = Properties.Resources.AdresLokalen; 

                return View(model);
            }
        }
	}
}
