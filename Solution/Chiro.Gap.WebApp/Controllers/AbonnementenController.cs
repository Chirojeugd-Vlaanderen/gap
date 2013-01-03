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
    /// Controller die gebruikt wordt om een gelieerde persoon te abonneren op een tijdschrift, voor
    /// het huidige werkJaar.  Momenteel zijn er enkel Dubbelpuntabonnementen, op termijn mogelijk
    /// ook andere publicaties.
    /// </summary>
    public class AbonnementenController : BaseController
    {
        /// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        public AbonnementenController(IVeelGebruikt veelGebruikt) : base(veelGebruikt) { }

        public override ActionResult Index(int groepID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Vraag de gebruiker bevestiging voor het bestellen van een dubbelpuntabonnement voor de persoon
        /// gekoppeld aan de gelieerde persoon met GelieerdePersoonID <paramref name="id"/>
        /// </summary>
        /// <param name="groepID">Groep waarn de gebruiker aan het werken is</param>
        /// <param name="id">ID van een gelieerde persoon</param>
        /// <returns>Een view die de gebruiker toelaat te bevestigen</returns>
        public ActionResult DubbelPuntAanvragen(int groepID, int id)
        {
            var model = new BevestigingsModel();
            BaseModelInit(model, groepID);


            var info =
                ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonInfo>>(
                    svc => svc.PersoonInfoOphalen(new List<int> {id})).FirstOrDefault();


            Debug.Assert(info != null);

            model.LidID = 0;	// LidID is hier niet relevant

            model.GelieerdePersoonID = info.GelieerdePersoonID;
            model.VolledigeNaam = string.Format("{0} {1}", info.VoorNaam, info.Naam);

            model.Prijs = Properties.Settings.Default.PrijsDubbelPunt;
            model.Titel = String.Format("Dubbelpunt bestellen voor {0}", model.VolledigeNaam);

            switch (ServiceHelper.CallService<IAbonnementenService, Boolean?>(svc => svc.HeeftGratisDubbelpunt(id)))
            {
                case true:
                    // Persoon is contactpersoon. Zal sowieso dubbelpunt krijgen.
                    model.ExtraWaarschuwing = String.Format(Properties.Resources.KrijgtAlDubbelpunt, model.VolledigeNaam);
                    break;
                case null:
                    // Persoon is contactpersoon, maar er zijn meerdere contactpersonen (wat een fout van de gebruiker is)
                    // 1 contactpersoon krijgt gratis Dubbelpunt, dat zullen we al eens melden.
                    model.ExtraWaarschuwing = String.Format(Properties.Resources.MisschienGratisDubbelpunt, model.VolledigeNaam);
                    break;
                default:
                    model.ExtraWaarschuwing = null;
                    break;
            }

            return View(model);
        }

        /// <summary>
        /// Bestelt een Dubbelpuntabonnement voor de persoon met GelieerdePersoonID <paramref name="id"/>.
        /// </summary>
        /// <param name="model">Een BevestigingsModel, puur pro forma, want alle relevante info zit in de url</param>
        /// <param name="groepID">ID van de groep waarin wordt gewerkt</param>
        /// <param name="id">GelieerdePersoonID van persoon die Dubbelpunt wil</param>
        /// <returns>Redirect naar detailfiche van de betreffende persoon</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult DubbelPuntAanvragen(BevestigingsModel model, int groepID, int id)
        {
            try
            {
                ServiceHelper.CallService<IAbonnementenService>(svc => svc.DubbelPuntBestellen(id));
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                if (ex.Detail.FoutNummer == FoutNummer.AdresOntbreekt)
                {
                    TempData["fout"] = Properties.Resources.DubbelPuntZonderAdres;
                    return RedirectToAction("EditRest", "Personen", new { groepID, id });
                }
                else if (ex.Detail.FoutNummer == FoutNummer.BestelPeriodeDubbelpuntVoorbij)
                {
                    var huidigWerkjaar = VeelGebruikt.GroepsWerkJaarOphalen(groepID);
                    TempData["fout"] = String.Format(
                        Properties.Resources.BestelPeriodeDubbelpuntVoorbij,
                        huidigWerkjaar.WerkJaar,
                        huidigWerkjaar.WerkJaar + 1);
                    return RedirectToAction("EditRest", "Personen", new { groepID, id });
                }

                else
                {
                    throw;
                }
            }

            return RedirectToAction("EditRest", "Personen", new { id });
        }
    }
}