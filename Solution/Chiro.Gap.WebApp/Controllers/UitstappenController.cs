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
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
using Chiro.Cdf.ExcelManip;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Validatie;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller voor weergave en beheer van bivakken en uitstappen
    /// </summary>
    public class UitstappenController : BaseController
    {
        public UitstappenController(IVeelGebruikt veelGebruikt) : base(veelGebruikt)
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
            model.Uitstap = new UitstapOverzicht();

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
            var validator = new PeriodeValidator();

            if (!validator.Valideer(model.Uitstap))
            {
                ModelState.AddModelError("Uitstap.DatumVan", string.Format(Properties.Resources.VanTotUitstap));
                ModelState.AddModelError("Uitstap.DatumTot", string.Format(Properties.Resources.VanTotUitstap));
            }

            BaseModelInit(model, groepID);
            model.Titel = Properties.Resources.NieuweUitstap;

            if (ModelState.IsValid)
            {
                int uitstapID = ServiceHelper.CallService<IUitstappenService, int>(svc => svc.Bewaren(groepID, model.Uitstap));
                VeelGebruikt.BivakStatusResetten(groepID);
                return RedirectToAction("Bekijken", new { groepID, id = uitstapID });
            }

            return View("Bewerken", model);
        }

        /// <summary>
        /// Toont de details van een gegeven uitstap
        /// </summary>
        /// <param name="groepID">ID van de groep die momenteel geselecteerd is</param>
        /// <param name="id">ID van de uitstap waarin we geïnteresseerd zijn</param>
        /// <returns>De details van de gegeven uitstap</returns>
        public ActionResult Bekijken(int groepID, int id)
        {
            var model = new UitstapDeelnemersModel();
            BaseModelInit(model, groepID);
            model.Uitstap = ServiceHelper.CallService<IUitstappenService, UitstapOverzicht>(svc => svc.DetailsOphalen(id));
            model.Deelnemers =
                ServiceHelper.CallService<IUitstappenService, IEnumerable<DeelnemerDetail>>(
                    svc => svc.DeelnemersOphalen(id));
            model.Titel = model.Uitstap.Naam;

            if (model.Uitstap.IsBivak)
            {
                // TODO Dit is volgens mij niet helemaal juist.  De feedbackregio wordt normaal gezien enkel gebruikt voor feedback op de vorige actie
                // Voor een bivak moet er een adres ingevuld zijn, anders is de bivakaangifte nog niet in orde.
                if (model.Uitstap.Adres == null)
                {
                    TempData["fout"] = Properties.Resources.GeenAdresVoorBivak;
                }

                // Voor een bivak moet er ook een contactpersoon aangeduid zijn
                bool heeftContact = (from dln in model.Deelnemers
                                     where dln.IsContact
                                     select dln).FirstOrDefault() != null;

                if (!heeftContact)
                {
                    TempData["fout"] += Properties.Resources.GeenContactVoorBivak;
                }
            }

            return View(model);
        }

        /// <summary>
        /// Verwijderd de uitstap
        /// </summary>
        /// <param name="groepID">ID van de groep die momenteel geselecteerd is</param>
        /// <param name="uitstapID">ID van de uitstap die we willen verwijderen</param>
        /// <returns>Verwijderd de uitstap en gaat terug naar het overzichtscherm</returns>
        public ActionResult Verwijderen(int groepID, int uitstapID)
        {
            // Afdeling van afdelingsjaar invullen
            try
            {
                ServiceHelper.CallService<IUitstappenService>(svc => svc.UitstapVerwijderen(uitstapID));

                TempData["succes"] = Properties.Resources.UitstapVerwijderd;
            }
            catch (FaultException)
            {
                TempData["fout"] = Properties.Resources.UitstapVerwijderenFout;
                // TODO (#1139): specifieke exceptions catchen en weergeven via de modelstate, en niet via tempdata.
            }

            return RedirectToAction("Index");
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
            model.Uitstap = ServiceHelper.CallService<IUitstappenService, UitstapOverzicht>(svc => svc.DetailsOphalen(id));
            model.Titel = model.Uitstap.Naam;
            return View(model);
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
            var model = new UitstapPlaatsBewerkenModel();
            BaseModelInit(model, groepID);
            model.Uitstap = ServiceHelper.CallService<IUitstappenService, UitstapOverzicht>(svc => svc.DetailsOphalen(id));

            if (model.Uitstap.Adres == null)
            {
                model.Uitstap.Adres = new AdresInfo { LandNaam = Properties.Resources.Belgie };
            }

            // Als het adres buitenlands is, dan moeten we de woonplaats nog eens overnemen in
            // WoonPlaatsBuitenland.  Dat is nodig voor de AdresBewerkenControl, die een beetje
            // raar ineen zit.
            if (String.Compare(model.Uitstap.Adres.LandNaam, Properties.Resources.Belgie, true) != 0)
            {
                model.WoonPlaatsBuitenLand = model.Uitstap.Adres.WoonPlaatsNaam;
            }

            model.Titel = model.Uitstap.Naam;
            model.AlleLanden = VeelGebruikt.LandenOphalen();
            model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.Uitstap.Adres.PostNr);

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
            var validator = new PeriodeValidator();

            if (!validator.Valideer(model.Uitstap))
            {
                ModelState.AddModelError("Uitstap.DatumVan", string.Format(Properties.Resources.VanTotUitstap));
                ModelState.AddModelError("Uitstap.DatumTot", string.Format(Properties.Resources.VanTotUitstap));
            }

            BaseModelInit(model, groepID);
            // neem uitstapID over uit url, want ik denk dat daarvoor geen field is voorzien.
            model.Uitstap.ID = id;
            model.Titel = model.Uitstap.Naam;

            if (ModelState.IsValid)
            {
                var uitstapID = ServiceHelper.CallService<IUitstappenService, int>(svc => svc.Bewaren(groepID, model.Uitstap));
                VeelGebruikt.BivakStatusResetten(groepID);
                return RedirectToAction("Bekijken", new { groepID, id = uitstapID });
            }

            return View("Bewerken", model);
        }

        /// <summary>
        /// Als <paramref name="model"/> geen fouten bevat, stuurt deze controller action de geüpdatete plaats
        /// naar de backend om te persisteren.
        /// </summary>
        /// <param name="groepID">Huidige groep</param>
        /// <param name="id">ID van de uitstap waarvoor de plaats moet aangepast worden</param>
        /// <param name="model">Bevat de nieuwe plaats voor de uitstap</param>
        /// <returns>Als alles goed liep, wordt geredirect naar de details van de uitstap.  Anders
        /// opnieuw de view voor het aanpassen van de uitstap, met de nodige feedback</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlaatsBewerken(int groepID, int id, UitstapPlaatsBewerkenModel model)
        {
            // Als het adres buitenlands is, neem dan de woonplaats over uit het
            // vrij in te vullen veld.

            if (String.Compare(model.Land, Properties.Resources.Belgie, true) != 0)
            {
                model.WoonPlaatsNaam = model.WoonPlaatsBuitenLand;
            }

            try
            {
                // De service zal model.NieuwAdres.ID negeren; dit wordt
                // steeds opnieuw opgezocht.  Adressen worden nooit
                // gewijzigd, enkel bijgemaakt (en eventueel verwijderd.)

                ServiceHelper.CallService<IUitstappenService>(l => l.PlaatsBewaren(id, model.Uitstap.PlaatsNaam, model.Uitstap.Adres));
                VeelGebruikt.BivakStatusResetten(groepID);

                return RedirectToAction("Bekijken", new { groepID, id });
            }
            catch (FaultException<OngeldigObjectFault> ex)
            {
                BaseModelInit(model, groepID);

                new ModelStateWrapper(ModelState).BerichtenToevoegen(ex.Detail, String.Empty);

                model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.PostNr);
                model.AlleLanden = VeelGebruikt.LandenOphalen();
                model.Titel = model.Uitstap.Naam;

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
            VeelGebruikt.BivakStatusResetten(groepID);
            return RedirectToAction("Bekijken", new { id = uitstapID });
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
            TempData["succes"] = Properties.Resources.DeelnemerUitgeschreven;
            return RedirectToAction("Bekijken", new { id = uitstapID });
        }

        /// <summary>
        /// Toont een formulier waarmee de user de informatie over een deelnemer kan aanpassen
        /// </summary>
        /// <param name="groepID">Huidige groep</param>
        /// <param name="id">DeelnemerID van de te bewerken deelnemer</param>
        /// <returns>De 'pas eens een deelnemer aan'-view.</returns>
        public ActionResult DeelnemerBewerken(int groepID, int id)
        {
            var model = new DeelnemerBewerkenModel
                            {
                                Deelnemer =
                                    ServiceHelper.CallService<IUitstappenService, DeelnemerDetail>(svc => svc.DeelnemerOphalen(id))
                            };

            // We zouden hier waarschijnlijk beter wat meer details opvragen, maar omdat dat nog niet geïmplementeerd is
            // in de backend, houden we het bij de beperkte gegevens.

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

            return RedirectToAction("Bekijken", new { groepID, id = model.Deelnemer.UitstapID });
        }

        /// <summary>
        /// Downloadt de lijst van deelnemers aan de uitstap <paramref name="id"/> als
        /// Exceldocument.
        /// </summary>
        /// <param name="id">ID van de gevraagde uitstap (komt uit url)</param>
        /// <returns>Exceldocument met gevraagde ledenlijst</returns>
        [HandleError]
        public ActionResult Download(int id)
        {
            var uitstap = ServiceHelper.CallService<IUitstappenService, UitstapOverzicht>(s => s.DetailsOphalen(id));
            var lijst = ServiceHelper.CallService<IUitstappenService, IEnumerable<DeelnemerDetail>>(s => s.DeelnemersOphalen(id)).ToList();

            var personenlijst = ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<PersoonOverzicht>>(g => g.OverzichtOphalen(lijst.Select(e => e.GelieerdePersoonID).ToList()));
            foreach (var persoonLidInfo in personenlijst)
            {
                PersoonOverzicht info = persoonLidInfo;
                lijst.Where(e => e.GelieerdePersoonID == info.GelieerdePersoonID).First().PersoonOverzicht =
                    persoonLidInfo;
            }

            string[] kolomkoppen = 
                    {
			        "ADnummer", "Voornaam", "Familienaam", "Geboortedatum", "Geslacht", "Telefoon", "Straat", "Huisnummer", "Postcode", "Woonplaats", "Afdelingen", "Functie", "Contactpersoon", "Betaald", "Medische fiche", "Opmerkingen"
			        };

            var bestandsNaam = String.Format("{0}.xlsx", uitstap.Naam.Replace(" ", "-"));
            var stream = (new ExcelManip.GapExcelManip()).ExcelTabel(
                lijst,
                kolomkoppen,
                it => it.PersoonOverzicht.AdNummer,
                it => it.VoorNaam,
                it => it.FamilieNaam,
                it => it.PersoonOverzicht.GeboorteDatum,
                it => it.PersoonOverzicht.Geslacht,
                it => it.PersoonOverzicht.TelefoonNummer,
                it => it.PersoonOverzicht.StraatNaam,
                it => it.PersoonOverzicht.HuisNummer,
                it => it.PersoonOverzicht.PostCode,
                it => it.PersoonOverzicht.WoonPlaats,
                it => it.Afdelingen == null ? String.Empty : String.Concat(it.Afdelingen.Select(afd => afd.Afkorting + " ").ToArray()),
                it => it.Type,
                it => it.IsContact ? "Ja" : "Nee",
                it => it.HeeftBetaald ? "Ja" : "Nee",
                it => it.MedischeFicheOk ? "Ja" : "Nee",
                it => it.Opmerkingen);

            return new ExcelResult(stream, bestandsNaam);
        }
    }
}