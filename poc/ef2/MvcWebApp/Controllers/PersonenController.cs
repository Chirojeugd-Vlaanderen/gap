using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Configuration;
using Cg2.Orm;

namespace MvcWebApp.Controllers
{
    public class PersonenController : Controller
    {
        public ActionResult PersonenLijst() 
        {
            using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new MvcWebApp.GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
            {
                int aantal;
                return View("PersonenLijst", service.PaginaOphalen(out aantal, int.Parse(ConfigurationSettings.AppSettings["TestGroepID"]), 1, 12));
            }
        }

        public ActionResult Details(int id)
        {
            using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new MvcWebApp.GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
            {
                GelieerdePersoon p = service.DetailsOphalen(id);

                return View("PersoonsDetails", p);
            }
        }

        public ActionResult Index()
        {
            // Add action logic here
            throw new NotImplementedException();
        }
    }
}
