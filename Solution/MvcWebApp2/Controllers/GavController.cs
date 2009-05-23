using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Cg2.Adf.ServiceModel;
using Cg2.ServiceContracts;

namespace MvcWebApp2.Controllers
{
    public class GavController : Controller
    {
        //
        // GET: /Gav/

        public ActionResult Index()
        {
            var model = new Models.GavModel();
            model.GroepenLijst = ServiceHelper.CallService<IGroepenService, IEnumerable<GroepInfo>>
                    (g => g.OphalenMijnGroepen());

            return View("Index", model);
        }

    }
}
