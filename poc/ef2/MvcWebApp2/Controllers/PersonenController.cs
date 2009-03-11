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
                IList<GelieerdePersoon> personen = service.PaginaOphalenMetLidInfo(out aantal, int.Parse(ConfigurationSettings.AppSettings["TestGroepID"]), 1, 12);
                return View("Index", personen);
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

        public ActionResult LidMaken(int id)
        {
            using (LedenServiceReference.LedenServiceClient service = new MvcWebApp2.LedenServiceReference.LedenServiceClient())
            {
                // Beter zou zijn:
                //  via de service de definitie van een lid ophalen
                //  een view tonen met die lidgegevens, zodat ze aangepast kunnen worden
                //  pas als de gebruiker dan bevestigt: bewaren

                service.LidMakenEnBewaren(id);
            }

            return RedirectToAction("Index");

        }

        public ActionResult Hallo()
        {
            return View("Hallo");
        }
    }
}
