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
    public class BivakkenController : Controller
    {
        //
        // GET: /Bivakken/

        public ActionResult Index()
        {
            var model = new BivakkenIndexModel
            {
                AantalVerdwenenBivakken =
                    ServiceHelper.CallService<IAdminService, int>(
                        svc => svc.AantalVerdwenenBivakkenOphalen()),
            };

            return View(model);
        }

        /// <summary>
        /// Vraagt de backend om alle ontbrekende bivakken opnieuw te syncen.
        /// </summary>
        /// <returns>Redirect naar index</returns>
        public ActionResult OpnieuwSyncen()
        {
            ServiceHelper.CallService<IAdminService>(svc => svc.OntbrekendeBivakkenSyncen());
            return RedirectToAction("Index"); ;
        }
    }
}
