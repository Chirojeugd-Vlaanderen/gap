using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Configuration;
using Cg2.Orm;

namespace MvcWebApp2.Controllers
{
    public class PersonenController : Controller
    {
        //
        // GET: /Personen/

        public ActionResult Index()
        {
            using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
            {
                int aantal;
                IList<GelieerdePersoon> personen = service.PaginaOphalen(out aantal, int.Parse(ConfigurationSettings.AppSettings["TestGroepID"]), 1, 12);
                return View(personen);
            }
        }

        //
        // GET: /Personen/Details/5

        public ActionResult Details(int id)
        {
            using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
            {
                GelieerdePersoon p = service.DetailsOphalen(id);
                return View("Details", p);
            }
        }

        //
        // GET: /Personen/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Personen/Create

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Personen/Edit/5
 
        public ActionResult Edit(int id)
        {
            using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
            {
                GelieerdePersoon p = service.DetailsOphalen(id);
                return View("Edit", p);
            }
        }

        //
        // POST: /Personen/Edit/5

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(GelieerdePersoon p)
        {
            try
            {
                using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
                {
                    service.Bewaren(p);
                }
 
                return RedirectToAction("Details", new { id = p.ID });
            }
            catch
            {
                return View("Edit" ,p);
            }
        }

        public ActionResult Hallo()
        {
            return View("Hallo");
        }
    }
}
