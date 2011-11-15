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

        public ActionResult Index(LoginsModel model)
        {
            throw new NotImplementedException();
        }
    }
}
