﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using System.Web.Routing;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller voor alles wat te maken heeft met beheer van functies
    /// </summary>
    [HandleError]
    public class FunctiesController : BaseController
    {
        /// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        public FunctiesController(IVeelGebruikt veelGebruikt) : base(veelGebruikt) { }

        /// <summary>
        /// Genereert een view met een lijstje van alle beschikbare functies en geeft de mogelijkheid om er één nieuwe toe te voegen.
        /// </summary>
        /// <param name="groepID">ID van de gewenste groep</param>
        /// <returns>View met algemene gegevens over de groep</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        [HandleError]
        public override ActionResult Index(int groepID)
        {
            var model = new GroepsInstellingenModel
            {
                Titel = Properties.Resources.FunctiesBewerkenTitel,
                Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(
                    svc => svc.DetailOphalen(groepID))
            };

            return View(model);
        }

        /// <summary>
        /// Van een groepsgebonden functie aangeven dat de groep ze niet meer wil gebruiken
        /// </summary>
        /// <param name="groepID">De ID van de groep</param>
        /// <param name="id">De ID van de functie</param>
        /// <returns></returns>
        [HandleError]
        public ActionResult Verwijderen(int groepID, int id)
        {
            try
            {
                ServiceHelper.CallService<IGroepenService>(svc => svc.FunctieVerwijderen(id, false));
                TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;
                return RedirectToAction("Index", new { groepID });
            }
            catch (FaultException<BlokkerendeObjectenFault<PersoonLidInfo>> ex)
            {
                // Functie was niet leeg

                var model = new LedenLinksModel();
                BaseModelInit(model, groepID);

                model.Leden = ex.Detail.Objecten;
                model.FunctieID = id;
                model.VolledigeLijstUrl = Url.Action("Lijst", "Leden", new RouteValueDictionary(new { groepID, functieID = id }));
                model.TotaalAantal = ex.Detail.Aantal;

                // Vis functienaam op uit de gekoppelde functies van de leden.
                // Niet elegant, maar werkt wel.

                var functieNaam = (from f in model.Leden.First().LidInfo.Functies
                                   where f.ID == id
                                   select f).First().Naam;
                model.Titel = String.Format("Functie '{0}' verwijderen", functieNaam);

                return View("Verwijderen", model);
            }
        }

        /// <summary>
        /// Forceert het verwijderen van een functie
        /// </summary>
        /// <param name="model">LedenLinkModel, waarvan enkel GroepID en FunctieID van belang zijn</param>
        /// <returns>Een redirect naar de actie 'Index'</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult Verwijderen(LedenLinksModel model)
        {
            ServiceHelper.CallService<IGroepenService>(svc => svc.FunctieVerwijderen(
                model.FunctieID,
                true));	// forceer; ook als er nog mensen zijn met die functie

            return RedirectToAction("Index", new { groepID = model.GroepID });
        }

        /// <summary>
        /// Actie voor als de gebruiker gegevens heeft ingevuld voor een nieuwe functie
        /// </summary>
        /// <param name="model">Model voor Groepsinstellingen</param>
        /// <param name="groepID">ID van de gewenste groep</param>
        /// <returns>Opnieuw de view Functies/Index</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult Index(GroepsInstellingenModel model, int groepID)
        {
            // Ik heb chance, want momenteel staan er enkel constraints op de velden die de
            // gebruiker invulde, en die meegeleverd worden via model binding.
            // Dus het is nog niet nodig om hier het model aan te vullen...
            // Zodra er wel constraints staan op de groepsinfo/afdelingsinfo/... moet
            // het model toch op voorhand aangevuld worden.

            if (ModelState.IsValid)
            {
                // Als de ModelState geldig is: functie toevoegen
                try
                {
                    ServiceHelper.CallService<IGroepenService>(svc => svc.FunctieToevoegen(
                        groepID,
                        model.NieuweFunctie.Naam,
                        model.NieuweFunctie.Code,
                        model.NieuweFunctie.MaxAantal,
                        model.NieuweFunctie.MinAantal,
                        model.NieuweFunctie.Type,
                        model.NieuweFunctie.WerkJaarVan));

                    return RedirectToAction("Index", new { groepID });
                }
                catch (FaultException<BestaatAlFault<FunctieDetail>> ex)
                {
                    if (String.Compare(
                        model.NieuweFunctie.Code,
                        ex.Detail.Bestaande.Code,
                        true) == 0)
                    {
                        // Geef feedback aan de gebruiker: de naam of de code worden al gebruikt
                        ModelState.AddModelError(
                            "NieuweFunctie.Code",
                            String.Format(
                                Properties.Resources.FunctieCodeBestaatAl,
                                ex.Detail.Bestaande.Code,
                                ex.Detail.Bestaande.Naam));
                    }
                    else if (String.Compare(
                        model.NieuweFunctie.Naam,
                        ex.Detail.Bestaande.Naam,
                        true) == 0)
                    {
                        // Geef feedback aan de gebruiker: de naam of de code worden al gebruikt
                        ModelState.AddModelError(
                            "NieuweFunctie.Naam",
                            String.Format(
                                Properties.Resources.FunctieNaamBestaatAl,
                                ex.Detail.Bestaande.Code,
                                ex.Detail.Bestaande.Naam));
                    }
                    else
                    {
                        Debug.Assert(false);
                    }

                    model.Titel = Properties.Resources.GroepsInstellingenTitel;
                    model.Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(
                        svc => svc.DetailOphalen(groepID));

                    return View(model);
                }
            }
            else
            {
                // ModelState bevat ongeldige waarden, dus toon de pagina opnieuw
                model.Titel = Properties.Resources.GroepsInstellingenTitel;
                model.Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(
                    svc => svc.DetailOphalen(groepID));

                return View(model);
            }
        }
    }
}