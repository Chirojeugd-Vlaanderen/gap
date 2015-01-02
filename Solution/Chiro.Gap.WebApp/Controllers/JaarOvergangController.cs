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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller die de jaarovergangspagina's toont en input verwerkt
    /// </summary>
    public class JaarOvergangController : BaseController
    {
        /// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        public JaarOvergangController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper) : base(veelGebruikt, serviceHelper) { }

        public override ActionResult Index(int groepID)
        {
            return Stap1AfdelingenSelecteren(groepID);
        }

        // Geef een lijst weer van alle beschikbare afdelingen, waaruit de groep kan kiezen welke afdelingen ze volgend jaar zullen hebben
        // (ze kunnen er ook nieuwe bijmaken)
        // Als de groep een kadergroep is, dan slaan we de afdelingsstappen over
        private ActionResult Stap1AfdelingenSelecteren(int groepID)
        {
            var model = new JaarOvergangAfdelingsModel();
            BaseModelInit(model, groepID);

            // Als we met een gewest/verbond te doen hebben, dan zijn de afdelingen niet relevant
            if (model.GroepsNiveau.HeeftNiveau(Niveau.KaderGroep))
            {
                // Een 'gewone' groep krijgt eerst de mogelijkheid om zijn afdelingsjaren te 
                // definieren, en krijgt dan de mogelijkheid om meteen voor alle leden te bevestigen
                // of ze opnieuw ingeschreven moeten worden of niet.

                // Een gewest of verbond heeft geen afdelingen, dus we kunnen deze stap gerust overslaan.
                // We schakelen wel direct door naar het scherm dat toelaat de in te schrijven leden
                // aan te vinken; voor een gewest/verbond is dat typisch niet veel werk, en 
                // het aansluitingsgeld is toch onafhankelijk van het aantal ploegleden.

                var postmodel = new JaarOvergangAfdelingsJaarModel();
                postmodel.LedenMeteenInschrijven = true;
                return Stap2AfdelingsJarenVerdelen(postmodel, groepID);
            }

            model.Titel = "Jaarovergang stap 1: welke afdelingen heeft je groep volgend jaar?";
            model.Afdelingen = ServiceHelper.CallService<IGroepenService, IEnumerable<AfdelingInfo>>(g => g.AlleAfdelingenOphalen(groepID));
            return View("Stap1AfdelingenSelecteren", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Stap1AfdelingenSelecteren(JaarOvergangAfdelingsModel model1, int groepID)
        {
            return !ModelState.IsValid
                       ? Stap1AfdelingenSelecteren(groepID)
                       : Stap2AfdelingsJarenVerdelen(model1.GekozenAfdelingsIDs.ToArray(), groepID);
        }

        /// <summary>
        /// Deze actie vertrekt van de afdelingID's van de afdelingen die volgend jaar actief moeten zijn,
        /// en toont het scherm met de voorgestelde afdelingsjaren van volgend jaar (geboortedata, geslacht)
        /// </summary>
        /// <param name="gekozenAfdelingsIDs">ID's van de afdelingen waarvoor afdelingsjaren gedefinieerd moeten worden</param>
        /// <param name="groepID">ID van de groep waarin we werken</param>
        /// <returns>De view 'Stap2AfdelingsJarenVerdelen'</returns>
        private ActionResult Stap2AfdelingsJarenVerdelen(int[] gekozenAfdelingsIDs, int groepID)
        {
            var model = new JaarOvergangAfdelingsJaarModel();
            BaseModelInit(model, groepID);

            model.Titel = "Jaarovergang stap 2:  instellingen van je afdelingen";
            model.NieuwWerkjaar = ServiceHelper.CallService<IGroepenService, int>(g => g.NieuwWerkJaarOphalen(groepID));
            model.LedenMeteenInschrijven = true;
            model.OfficieleAfdelingen =
                ServiceHelper.CallService<IGroepenService, IEnumerable<OfficieleAfdelingDetail>>(
                    e => e.OfficieleAfdelingenOphalen()).ToArray();
            model.Afdelingen =
                ServiceHelper.CallService<IGroepenService, IList<AfdelingDetail>>(
                    svc => svc.NieuweAfdelingsJarenVoorstellen(gekozenAfdelingsIDs, groepID));


            // TODO extra info pagina voor jaarovergang
            // TODO kan validatie in de listhelper worden bijgecodeerd?
            return View("Stap2AfdelingsJarenVerdelen", model);
        }


        /// <summary>
        /// Nadat de user de geboortejaren van en tot heeft gedefinieerd voor iedere nieuw afdelingsjaar,
        /// wordt het nieuwe werkjaar gemaakt. Zo nodig wordt er geredirect naar een pagina die toelaat
        /// iedereen uit het vorige werkjaar opnieuw in te schrijven.
        /// </summary>
        /// <param name="model">bevat de nieuwe afdelingsjaren</param>
        /// <param name="groepID">groep waarvoor we de jaarovergang uitvoeren.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Stap2AfdelingsJarenVerdelen(JaarOvergangAfdelingsJaarModel model, int groepID)
        {
            // Als alles goed loopt, hebben we deze straks nodig.
            int vorigGwjID = ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID));

            // We gebruiken de modelstate om te laten zien dat het model al dan niet geldig is. Die verificatie
            // gebeurt op dit moment gedeeltelijk door de backend, dus moeten we eerst de backend aanroepen.

            try
            {
                ServiceHelper.CallService<IGroepenService>(s => s.JaarOvergangUitvoeren(model.Afdelingen, groepID));
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                switch (ex.Detail.FoutNummer)
                {
                    case FoutNummer.OngeldigeGeboorteJarenVoorAfdeling:
                        ModelState.AddModelError("NieuwWerkjaar", Properties.Resources.OngeldigeGeboorteJarenVoorAfdeling );
                        break;
                    default:
                        ModelState.AddModelError("NieuwWerkJaar", ex.Detail.Bericht);
                        break;
                }
            }

            if (!ModelState.IsValid)
            {
                // Model herstellen op basis van ingevulde gegevens.
                
                BaseModelInit(model, groepID);

                model.NieuwWerkjaar =
                    ServiceHelper.CallService<IGroepenService, int>(g => g.NieuwWerkJaarOphalen(groepID));
                model.OfficieleAfdelingen =
                    ServiceHelper.CallService<IGroepenService, IEnumerable<OfficieleAfdelingDetail>>(
                        e => e.OfficieleAfdelingenOphalen()).ToArray();

                var alleAfdelingen =
                    ServiceHelper.CallService<IGroepenService, IList<AfdelingInfo>>(
                        svc => svc.AlleAfdelingenOphalen(groepID));

                // De meeste gegevens in model.Afdelingen komen mooi terug uit het form. We moeten
                // enkel de namen van de afdelingen terug invullen.

                foreach (var aj in model.Afdelingen)
                {
                    aj.AfdelingNaam =
                        (from afd in alleAfdelingen where afd.ID == aj.AfdelingID select afd.Naam).FirstOrDefault();
                }
                
                return View("Stap2AfdelingsJarenVerdelen", model);
            }

            VeelGebruikt.JaarOvergangReset(groepID);

            if (!model.LedenMeteenInschrijven)
            {
                return RedirectToAction("Index", "Leden");
            }

            return RedirectToAction("Herinschrijven", "Personen", new {groepID, groepsWerkJaarID = vorigGwjID});
        }

        /// <summary>
        /// Toont de view die toelaat een nieuwe afdeling te maken.
        /// </summary>
        /// <param name="groepID">Groep waarvoor de afdeling gemaakt moet worden</param>
        /// <returns>De view die toelaat een nieuwe afdeling te maken.</returns>
        public ActionResult NieuweAfdelingMaken(int groepID)
        {
            var model = new AfdelingInfoModel
                            {
                                Titel = Properties.Resources.NieuweAfdelingTitel,
                                Info = new AfdelingInfo()
                            };

            BaseModelInit(model, groepID);

            return View("NieuweAfdelingMaken", model);
        }

        /// <summary>
        /// Maakt een nieuwe afdeling, op basis van <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Bevat naam en code voor de nieuwe afdeling</param>
        /// <param name="groepID">ID van de groep waarvoor de afdeling gemaakt moet worden</param>
        /// <returns>Het overzicht van de afdelingen, indien de nieuwe afdeling goed gemaakt is.
        /// In het andere geval opnieuw de view om een afdeling bij te maken.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NieuweAfdelingMaken(AfdelingInfoModel model, int groepID)
        {
            model.Titel = Properties.Resources.NieuweAfdelingTitel;
            BaseModelInit(model, groepID);

            if (!ModelState.IsValid)
            {
                // Modelstate bevat ongeldige waarden; toon de pagina opnieuw
                return View(model);
            }

            try
            {
                ServiceHelper.CallService<IGroepenService>(e => e.AfdelingAanmaken(groepID, model.Info.Naam, model.Info.Afkorting));

                TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

                return RedirectToAction("Index");
            }
            catch (FaultException<BestaatAlFault<AfdelingInfo>> ex)
            {
                if (string.Compare(ex.Detail.Bestaande.Afkorting, model.Info.Afkorting, true) == 0)
                {
                    ModelState.AddModelError(
                        "Info.Afkorting",
                        string.Format(Properties.Resources.AfdelingsCodeBestaatAl, ex.Detail.Bestaande.Afkorting, ex.Detail.Bestaande.Naam));
                }
                else if (string.Compare(ex.Detail.Bestaande.Naam, model.Info.Naam, true) == 0)
                {
                    ModelState.AddModelError(
                        "Info.Naam",
                        string.Format(
                            Properties.Resources.AfdelingsNaamBestaatAl, ex.Detail.Bestaande.Afkorting, ex.Detail.Bestaande.Naam));
                }
                else
                {
                    // Mag niet gebeuren
                    ModelState.AddModelError("Er heeft zich een fout voorgedaan, gelieve contact op te nemen met het secretariaat.", ex);
                    Debug.Assert(false);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Toont de view om een afdeling aan te passen, gegeven een afdelingsID
        /// </summary>
        /// <param name="groepID">ID van de groep die de bewerking uitvoert</param>
        /// <param name="afdelingID"></param>
        /// <returns></returns>
        public ActionResult Bewerken(int groepID, int afdelingID)
        {
            var model = new AfdelingInfoModel();
            BaseModelInit(model, groepID);

            model.Info = ServiceHelper.CallService<IGroepenService, AfdelingInfo>(svc => svc.AfdelingOphalen(afdelingID));

            model.Titel = "Afdeling bewerken";
            return View("Afdeling", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Bewerken(AfdelingInfoModel model, int groepID)
        {
            BaseModelInit(model, groepID);

            try
            {
                ServiceHelper.CallService<IGroepenService>(e => e.AfdelingBewaren(model.Info));

                TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // TODO dit kan beter
                TempData["fout"] = ex.Message;

                model.Info = ServiceHelper.CallService<IGroepenService, AfdelingInfo>(svc => svc.AfdelingOphalen(model.Info.ID));

                model.Titel = "Afdeling bewerken";
                return View("Afdeling", model);
            }
        }
    }
}
