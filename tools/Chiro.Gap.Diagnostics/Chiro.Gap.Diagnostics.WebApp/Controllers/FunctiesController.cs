using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Diagnostics.ServiceContracts;
using Chiro.Gap.Diagnostics.WebApp.Models;

namespace Chiro.Gap.Diagnostics.WebApp.Controllers
{
    /// <summary>
    /// Controller voor verkeerd of niet overgezette functies
    /// </summary>
    public class FunctiesController : Controller
    {
        /// <summary>
        /// Overzicht ivm niet of verkeerd overgezette functies
        /// </summary>
        /// <returns>Een view die gewoon het aantal niet of verkeerd overgezette functies toont</returns>
        public ActionResult Index()
        {
            var model = new FunctiesIndexModel
                            {
                                AantalProblemen =
                                    ServiceHelper.CallService<IAdminService, int>(
                                        svc => svc.AantalFunctieFoutenOphalen()),
                                ProblemenRapportUrl = Properties.Settings.Default.FunctieFoutenRapportUrl
                            };

            return View(model);
        }

        /// <summary>
        /// Synct de functies van alle leden met functieproblemen opnieuw
        /// </summary>
        /// <returns>Redirect naar de index</returns>
        public ActionResult OpnieuwSyncen()
        {
            ServiceHelper.CallService<IAdminService>(svc => svc.FunctieProbleemLedenOpnieuwSyncen());
            return RedirectToAction("Index");
        }


    }
}
