using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Diagnostics.WebApp.Models;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.Diagnostics.WebApp.Controllers
{
    /// <summary>
    /// Controller voor het toekennen van tijdelijke gebruikersrechten
    /// </summary>
    public class LoginsController : Controller
    {
        /// <summary>
        /// Toont de groepen waar de gebruiker al toegang toe heeft, met een link
        /// naar het GAP van die groepen.  Via een formulier wordt de mogelijkheid
        /// geboden tijdelijke rechten te krijgen voor een bijkomende groep.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new LoginsModel
                            {
                                GapUrl = Properties.Settings.Default.GapUrl,
                                Groepen = ServiceHelper.CallService<IGroepenService, IEnumerable<GroepInfo>>
                                    (g => g.MijnGroepenOphalen())
                            };

            return View(model);
        }

        /// <summary>
        /// Deze method wordt aangeroepen als de user een stamnummer invulde in de Index-view, om tijdelijke
        /// gebruikersrechten te krijgen voor een groep.
        /// </summary>
        /// <param name="model">Het stamnummer in het model is het stamnummer van de groep waarvoor je
        /// gebruikersrechten wilt</param>
        /// <returns>Een redirect naar 'RechtenToekennen'</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(LoginsModel model)
        {
            return RedirectToAction("RechtenToekennen", new {stamNummer = model.StamNummer});
        }

        /// <summary>
        /// Vraagt bevestiging om tijdelijke gebruikersrechten voor de groep te krijgen.  De gebruiker moet
        /// iemand van de 'contactpersonen' (contactpersoon van de groep, of een van de GAV's als we weten
        /// wie het is) selecteren, die een notificatie zal krijgen.
        /// </summary>
        /// <param name="stamNummer">stamNummer van groep waarvoor je gebruikersrechten wilt</param>
        /// <returns>De view die om bevestiging vraagt</returns>
        public ActionResult RechtenToekennen(string stamNummer)
        {
            var model = new NotificatieModel();

            // TODO: model fatsoenlijk opvullen

            model.GroepInfo = new GroepInfo();

            return View(model);
        }
    }
}
