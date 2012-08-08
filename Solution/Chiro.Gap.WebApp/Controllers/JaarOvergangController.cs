// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
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
        public JaarOvergangController(IVeelGebruikt veelGebruikt) : base(veelGebruikt) { }

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
                    e => e.OfficieleAfdelingenOphalen(groepID)).ToArray();
            model.Afdelingen =
                ServiceHelper.CallService<IGroepenService, IList<AfdelingDetail>>(
                    svc => svc.NieuweAfdelingsJarenVoorstellen(gekozenAfdelingsIDs, groepID));


            // TODO extra info pagina voor jaarovergang
            // TODO kan validatie in de listhelper worden bijgecodeerd?
            return View("Stap2AfdelingsJarenVerdelen", model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Stap2AfdelingsJarenVerdelen(JaarOvergangAfdelingsJaarModel model, int groepID)
        {
            // Geldigheid van het model moet gecontroleerd worden met ModelState.

            if (!ModelState.IsValid)
            {
                // Model herstellen op basis van ingevulde gegevens.
                
                BaseModelInit(model, groepID);

                model.NieuwWerkjaar =
                    ServiceHelper.CallService<IGroepenService, int>(g => g.NieuwWerkJaarOphalen(groepID));
                model.OfficieleAfdelingen =
                    ServiceHelper.CallService<IGroepenService, IEnumerable<OfficieleAfdelingDetail>>(
                        e => e.OfficieleAfdelingenOphalen(groepID)).ToArray();
                
                return View("Stap2AfdelingsJarenVerdelen", model);
            }


            // Leden zoeken in het vorige actieve werkjaar, dus opvragen voor we de jaarovergang zelf doen
            var vorigGwjID = ServiceHelper.CallService<IGroepenService, int>(g => g.RecentsteGroepsWerkJaarIDGet(groepID));

            try
            {
                ServiceHelper.CallService<IGroepenService>(s => s.JaarovergangUitvoeren(model.Afdelingen, groepID));
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                TempData["fout"] = ex.Message;
                return View("Stap2AfdelingsJarenVerdelen", model);
            }

            VeelGebruikt.JaarOvergangReset(groepID);

            if (!model.LedenMeteenInschrijven)
            {
                return RedirectToAction("Index", "Leden");
            }

            // We hebben de gelieerdepersoonID's nodig van alle leden uit het huidige (oude) werkjaar.
            // We doen dat hieronder door alle leden op te halen, en daaruit de gelieerdepersoonID's te 
            // halen.
            // TODO: Dit is een tamelijk dure operatie, omdat we gewoon de ID's nodig hebben. 
            // Bovendien halen we in een volgende stap opnieuw de persoonsinfo op.
            // Best eens herwerken dus.

            var filter = new LidFilter
                            {
                                GroepsWerkJaarID = vorigGwjID,
                                AfdelingID = null,
                                FunctieID = null,
                                ProbeerPeriodeNa = null,
                                HeeftVoorkeurAdres = null,
                                HeeftTelefoonNummer = null,
                                HeeftEmailAdres = null,
                                LidType = LidType.Alles
                            };

            var gelieerdepersoonIDs =
                ServiceHelper.CallService<ILedenService, IList<LidOverzicht>>(svc => svc.Zoeken(filter, false)).Select(
                    e => e.GelieerdePersoonID).ToList();

            TempData["list"] = gelieerdepersoonIDs;
            return RedirectToAction("LedenMaken", "Leden");
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
