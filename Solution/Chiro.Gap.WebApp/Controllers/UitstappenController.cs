using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Mvc;

using AutoMapper;

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
    /// 
    /// </summary>
    public class UitstappenController : BaseController
    {
        public UitstappenController(IServiceHelper serviceHelper, IVeelGebruikt veelGebruikt)
            : base(serviceHelper, veelGebruikt)
        {
        }

        /// <summary>
        /// Overzicht val alle uitstappen (ever) van een groep
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <returns>View met uistapoverzicht</returns>
        public override ActionResult Index(int groepID)
        {
            var model = new UitstapOverzichtModel();
            BaseModelInit(model, groepID);
            model.Titel = Properties.Resources.Uitstappen;
            model.Uitstappen =
                ServiceHelper.CallService<IUitstappenService, IEnumerable<UitstapInfo>>(svc => svc.OphalenVanGroep(groepID, false));
            return View(model);
        }

        /// <summary>
        /// Formulier om nieuwe uitstap te registreren
        /// </summary>
        /// <param name="groepID">ID van groep waarvoor uitstap moet worden geregistreerd</param>
        /// <returns>Het formulier</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Nieuw(int groepID)
        {
            var model = new UitstapModel();
            BaseModelInit(model, groepID);
            model.Titel = Properties.Resources.NieuweUitstap;
            model.Uitstap = new UitstapDetail();

            return View("Bewerken", model);
        }

        /// <summary>
        /// Verwerkt de gegevens uit het nieuwe-uitstapformulier
        /// </summary>
        /// <param name="model">Ingevlude gegevens</param>
        /// <param name="groepID">Groep waarvoor uitstap moet worden bewaard</param>
        /// <returns>Als alles gelukt is, de details van de toegevoegde uitstap.  Anders 
        /// opnieuw het formulier met feedback over de problemen.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Nieuw(UitstapModel model, int groepID)
        {
            BaseModelInit(model, groepID);
            model.Titel = Properties.Resources.NieuweUitstap;

            if (ModelState.IsValid)
            {
                int uitstapID = ServiceHelper.CallService<IUitstappenService, int>(svc => svc.Bewaren(groepID, model.Uitstap));
                return RedirectToAction("Bekijken", new { groepID, id = uitstapID });
            }
            else
            {
                return View("Bewerken", model);
            }
        }

        /// <summary>
        /// Toont de details van een gegeven uitstap
        /// </summary>
        /// <param name="groepID">ID van de groep die momenteel geselecteerd is</param>
        /// <param name="id">ID van de uitstap waarin we ge�nteresseerd zijn</param>
        /// <returns>De details van de gegeven uitstap</returns>
        public ActionResult Bekijken(int groepID, int id)
        {
            var model = new UitstapDeelnemersModel();
            BaseModelInit(model, groepID);
            model.Uitstap = ServiceHelper.CallService<IUitstappenService, UitstapDetail>(svc => svc.DetailsOphalen(id));
            model.Deelnemers =
                ServiceHelper.CallService<IUitstappenService, IEnumerable<DeelnemerDetail>>(
                    svc => svc.DeelnemersOphalen(id));
            model.Titel = model.Uitstap.Naam;

            return View(model);
        }

        /// <summary>
        /// Formulier om bestaande uitstap mee te bewerken
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="id">ID van de te bewerken uitstap</param>
        /// <returns>Een formuliertje waarmee je datum en opmerkingen van een uitstap kunt aanpassen</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Bewerken(int groepID, int id)
        {
            var model = new UitstapModel();
            BaseModelInit(model, groepID);
            model.Uitstap = ServiceHelper.CallService<IUitstappenService, UitstapDetail>(svc => svc.DetailsOphalen(id));
            model.Titel = model.Uitstap.Naam;

            return View(model);
        }

        /// <summary>
        /// Stuurt de gegevens uit het 'uitstapbewerkenformulier' door naar de backend.
        /// </summary>
        /// <param name="model">Ingevulde gegevens</param>
        /// <param name="groepID">Groep waarvoor uitstap moet worden bewerkt</param>
        /// <param name="id">ID van de te bewerken uitstap</param>
        /// <returns>Als alles gelukt is, de details van de toegevoegde uitstap.  Anders 
        /// opnieuw het formulier met feedback over de problemen.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Bewerken(UitstapModel model, int groepID, int id)
        {
            BaseModelInit(model, groepID);
            // neem uitstapID over uit url, want ik denk dat daarvoor geen field is voorzien.
            model.Uitstap.ID = id;
            model.Titel = model.Uitstap.Naam;

            if (ModelState.IsValid)
            {
                int uitstapID = ServiceHelper.CallService<IUitstappenService, int>(svc => svc.Bewaren(groepID, model.Uitstap));
                return RedirectToAction("Bekijken", new { groepID, id = uitstapID });
            }
            else
            {
                return View("Bewerken", model);
            }
        }

        /// <summary>
        /// Laat de user de bivakplaats voor een uitstap bewerken.
        /// </summary>
        /// <param name="groepID">ID van de groep waarmee we werken</param>
        /// <param name="id">ID van het bivak waarvoor de plaats moet worden ingegeven.</param>
        /// <returns>Het formulier voor het ingeven van een bivakplaats</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult PlaatsBewerken(int groepID, int id)
        {
            var model = new UitstapPlaatsBewerekenModel();
            BaseModelInit(model, groepID);
            model.Uitstap = ServiceHelper.CallService<IUitstappenService, UitstapDetail>(svc => svc.DetailsOphalen(id));

            if (model.Uitstap.Adres == null)
            {
                model.Uitstap.Adres = new AdresInfo { LandNaam = Properties.Resources.Belgie };
            }

            model.Titel = model.Uitstap.Naam;
            model.AlleLanden = VeelGebruikt.LandenOphalen();
            model.BeschikbareWoonPlaatsen = new List<WoonPlaatsInfo>();

            return View(model);
        }

        /// <summary>
        /// Als <paramref name="model"/> geen fouten bevat, stuurt deze controller action de ge�pdatete plaats
        /// naar de backend om te persisteren.
        /// </summary>
        /// <param name="groepID">Huidige groep</param>
        /// <param name="model">Bevat de nieuwe plaats voor de uitstap</param>
        /// <param name="id">ID van de uitstap waarvoor de plaats moet aangepast worden</param>
        /// <returns>Als alles goed liep, wordt geredirect naar de details van de uitstap.  Anders
        /// opnieuw de view voor het aanpassen van de uitstap, met de nodige feedback</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlaatsBewerken(int groepID, int id, UitstapPlaatsBewerekenModel model)
        {
            // Als het adres buitenlands is, neem dan de woonplaats over uit het
            // vrij in te vullen veld.

            if (String.Compare(model.Land, Properties.Resources.Belgie, true) != 0)
            {
                model.WoonPlaats = model.WoonPlaatsBuitenLand;
            }

            try
            {
                // De service zal model.NieuwAdres.ID negeren; dit wordt
                // steeds opnieuw opgezocht.  Adressen worden nooit
                // gewijzigd, enkel bijgemaakt (en eventueel verwijderd.)

                ServiceHelper.CallService<IUitstappenService>(l => l.PlaatsBewaren(id, model.Uitstap.PlaatsNaam, model.Uitstap.Adres));
                // TODO (#950): problemen ontbrekende plaats resetten.
                // (maar die worden nog niet getoond)

                return RedirectToAction("Bekijken", new { groepID, id });
            }
            catch (FaultException<OngeldigObjectFault> ex)
            {
                BaseModelInit(model, groepID);

                new ModelStateWrapper(ModelState).BerichtenToevoegen(ex.Detail, String.Empty);

                model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.PostNr);
                model.AlleLanden = VeelGebruikt.LandenOphalen();

                return View(model);
            }
        }

        /// <summary>
        /// Stelt de deelnemer met ID <paramref name="id"/> in als contact voor de uitstap waaraan
        /// hij deelneemt.
        /// </summary>
        /// <param name="groepID">ID van groep die momenteel actief is</param>
        /// <param name="id"></param>
        /// <returns>Redirect naar overzicht uitstap</returns>
    	public ActionResult ContactInstellen(int groepID, int id)
        {
            int uitstapID = ServiceHelper.CallService<IUitstappenService, int>(svc => svc.ContactInstellen(id));
            return RedirectToAction("Bekijken", new {id = uitstapID});
        }

        /// <summary>
        /// Verwijdert de deelnemer met gegeven <paramref name="id"/> van zijn uitstap.
        /// </summary>
        /// <param name="groepID">Groep waarin we werken</param>
        /// <param name="id">DeelnemerID uit te schrijven deelnemer</param>
        /// <returns>Redirect naar overzicht uitstap</returns>
        public ActionResult Uitschrijven(int groepID, int id)
        {
            int uitstapID = ServiceHelper.CallService<IUitstappenService, int>(svc => svc.Uitschrijven(id));
            return RedirectToAction("Bekijken", new { id = uitstapID });
        }

        /// <summary>
        /// Toont een formulier waarmee de user de informatie over een deelnemer kan aanpassen
        /// </summary>
        /// <param name="groepID">huidige groep</param>
        /// <param name="id">DeelnemerID van de te bewerken deelnemer</param>
        /// <returns>De 'pas eens een deelnemer aan'-view.</returns>
        public ActionResult DeelnemerBewerken(int groepID, int id)
        {
            var model = new DeelnemerBewerkenModel();

            // We zouden hier waarschijnlijk beter wat meer details opvragen, maar omdat dat nog niet geimplementeerd is
            // in de backend, houden we het bij de beperkte gegevens.

            model.Deelnemer =
                ServiceHelper.CallService<IUitstappenService, DeelnemerDetail>(svc => svc.DeelnemerOphalen(id));
            BaseModelInit(model, groepID, String.Format(Properties.Resources.DeelnemerBewerken, model.Deelnemer.VoorNaam, model.Deelnemer.FamilieNaam));


            return View(model);
        }

        /// <summary>
        /// Stuurt de geupdatete deelnemersgegevens terug naar de service.
        /// </summary>
        /// <param name="groepID">ID van de actieve groep</param>
        /// <param name="id">ID van de deelnemer</param>
        /// <param name="model">Gegevens ingevuld in het form</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeelnemerBewerken(int groepID, int id, DeelnemerBewerkenModel model)
        {
            var info = new DeelnemerInfo
                           {
                               DeelnemerID = id,
                               HeeftBetaald = model.Deelnemer.HeeftBetaald,
                               IsLogistieker = model.Deelnemer.IsLogistieker,
                               MedischeFicheOk = model.Deelnemer.MedischeFicheOk,
                               Opmerkingen = model.Deelnemer.Opmerkingen
                           };

            ServiceHelper.CallService<IUitstappenService>(svc => svc.DeelnemerBewaren(info));

            return RedirectToAction("Bekijken", new {groepID, id = model.Deelnemer.UitstapID});
        }
    }
}