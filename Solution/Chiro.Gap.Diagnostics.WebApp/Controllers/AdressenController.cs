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
    /// Controller voor adresbeheer
    /// </summary>
    public class AdressenController : Controller
    {
        /// <summary>
        /// De indexpagina voor het adresbeheer
        /// </summary>
        /// <returns>De indexpagina voor het adresbeheer</returns>
        public ActionResult Index()
        {
            var model = new AddressenIndexModel
                            {
                                AantalVerdwenenAdressen =
                                    ServiceHelper.CallService<IAdminService, int>(
                                        svc => svc.AantalVerdwenenAdressenOphalen()),
                                RapportUrl = Properties.Settings.Default.VerdwenenAdressenRapportUrl
                            };

            return View(model);
        }

    }
}
