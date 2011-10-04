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
    }
}