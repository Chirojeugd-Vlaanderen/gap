// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
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
        public GebruikersRechtController(IVeelGebruikt veelGebruikt) : base(veelGebruikt)
        {
        }

        /// <summary>
        /// Kent een gebruikersrecht voor 14 maanden toe aan de gelieerde persoon met GelieerdePersoonID <paramref name="id"/>.
        /// Als het gebruikersrecht al bestaat, dan wordt het indien mogelijk verlengd tot 14 maanden vanaf vandaag.
        /// </summary>
        /// <param name="id">ID van de gelieerde persoon die gebruikersrecht moet krijgen</param>
        /// <returns>Redirect naar de personenfiche</returns>
        public ActionResult AanGpToekennen(int id)
        {
            ServiceHelper.CallService<IGelieerdePersonenService>(svc => svc.GelieerdePersoonRechtenGeven(id));
            return RedirectToAction("EditRest", new { Controller = "Personen", id });
        }

        /// <summary>
        /// Verlengt het gebruikersrecht met gegeven <paramref name="id"/> tot 14 maanden na vandaag.
        /// </summary>
        /// <param name="id">GebruikersRechtID te verlengen gebruikersrecht</param>
        /// <returns>Redirect naar het GAV-overzicht</returns>
        public ActionResult Verlengen(int id)
        {
            // Aangezien niet aan iedere GAV een persoon gekoppeld is, is het niet mogelijk om eerst gewoon 
            // de GelieerdePersoonID van de login te bepalen, en daarna ToekennenOfVerlengen(gpid) aan
            // te roepen.  Er moet dus een servicemethod gebruikt worden die rechtstreeks op het gebruikersrecht

            ServiceHelper.CallService<IGelieerdePersonenService>(svc => svc.GebruikersRechtVerlengen(id));
            return RedirectToAction("Index", new { id });
        }

        /// <summary>
        /// Neemt alle gebruikersrechten af van de gelieerde persoon met GelieerdePersoonID <paramref name="id"/>
        /// voor zijn eigen groep.  (Concreet wordt de vervaldatum op gisteren gezet.)
        /// </summary>
        /// <param name="id">ID van de gelieerde persoon</param>
        /// <returns>Redirect naar personenfiche</returns>
        public ActionResult VanGpAfnemen(int id)
        {
            ServiceHelper.CallService<IGelieerdePersonenService>(svc => svc.GelieerdePersoonRechtenAfnemen(id));
            return RedirectToAction("EditRest", new { Controller = "Personen", id });
        }

        /// <summary>
        /// Trekt het gebruikersrecht met gegeven <paramref name="id"/> in (i.e. zet vervaldatum op gisteren)
        /// </summary>
        /// <param name="id">De ID van het GebruikersRecht</param>
        /// <returns>Redirect naar GAV-overzicht</returns>
        public ActionResult Intrekken(int id)
        {
            ServiceHelper.CallService<IGelieerdePersonenService>(svc => svc.GebruikersRechtIntrekken(id));
            return RedirectToAction("Index", new { id });
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
    }
}