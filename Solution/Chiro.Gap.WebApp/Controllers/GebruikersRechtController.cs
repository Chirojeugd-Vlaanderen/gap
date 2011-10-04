using System;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller voor 
    /// </summary>
    public class GebruikersRechtController : BaseController
    {
        public GebruikersRechtController(IVeelGebruikt veelGebruikt): base(veelGebruikt)
        {
        }

        public override ActionResult Index(int groepID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Kent een gebruikersrecht voor 14 maanden toe aan de gelieerde persoon met GelieerdePersoonID <paramref name="id"/>.
        /// Als het gebruikersrecht al bestaat, dan wordt het indien mogelijk verlengd tot 14 maanden vanaf vandaag.
        /// </summary>
        /// <param name="id">ID van de gelieerde persoon die gebruikersrecht moet krijgen</param>
        /// <returns>Redirect naar de personenfiche</returns>
        public ActionResult Toekennen(int id)
        {
            ServiceHelper.CallService<IGelieerdePersonenService>(svc => svc.GebruikersRechtToekennen(id));
            return RedirectToAction("EditRest", new {Controller = "Personen", id});
        }

        /// <summary>
        /// Neemt alle gebruikersrechten af van de gelieerde persoon met GelieerdePersoonID <paramref name="id"/>
        /// voor zijn eigen groep.  (Concreet wordt de vervaldatum op gisteren gezet.)
        /// </summary>
        /// <param name="id">ID van de gelieerde persoon</param>
        /// <returns>Redirect naar personenfiche</returns>
        public ActionResult Afnemen(int id)
        {
            ServiceHelper.CallService<IGelieerdePersonenService>(svc => svc.GebruikersRechtAfnemen(id));
            return RedirectToAction("EditRest", new { Controller = "Personen", id });
        }
    }
}