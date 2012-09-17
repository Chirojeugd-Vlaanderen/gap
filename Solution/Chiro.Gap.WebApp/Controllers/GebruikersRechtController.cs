// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
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
        public GebruikersRechtController(IVeelGebruikt veelGebruikt) : base(veelGebruikt)
        {
        }

        /// <summary>
        /// Kent een gebruikersrecht voor 14 maanden toe aan de gelieerde persoon met GelieerdePersoonID <paramref name="id"/>.
        /// Als het gebruikersrecht al bestaat, dan wordt het indien mogelijk verlengd tot 14 maanden vanaf vandaag.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor gebruikersrecht toegekend moet worden</param>
        /// <param name="id">ID van de gelieerde persoon die gebruikersrecht moet krijgen</param>
        /// <returns>Redirect naar de personenfiche</returns>
        public ActionResult AanGpToekennen(int groepID, int id)
        {
            ServiceHelper.CallService<IGebruikersService>(
                svc => svc.RechtenToekennen(id, new[] {new GebruikersRecht {GroepID = groepID, Rol = Rol.Gav}}));
            return RedirectToAction("EditRest", new { Controller = "Personen", id });
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
            return RedirectToAction("EditRest", new { Controller = "Personen", id });
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