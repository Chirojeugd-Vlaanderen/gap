using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Configuration;
using Cg2.Orm;
using MvcWebApp2.Models;

namespace MvcWebApp2.Controllers
{
    public class PersonenController : Controller
    {
        //
        // GET: /Personen/
        public ActionResult Index()
        {
            IList<GelieerdePersoon> personen;

            using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
            {
                int aantal;
                personen = service.PaginaOphalenMetLidInfo(out aantal, int.Parse(ConfigurationSettings.AppSettings["TestGroepID"]), 1, 12);
            }
            return View("Index", personen);
        }

        //
        // GET: /Personen/Details/5
        public ActionResult Details(int id)
        {
            using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
            {
                GelieerdePersoon p = service.PersoonOphalenMetDetails(id);
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
                GelieerdePersoon p = service.PersoonOphalenMetDetails(id);
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
                    service.PersoonBewaren(p);
                }
 
                // Voorlopig opnieuw redirecten naar Edit;
                // er zou wel gemeld moeten worden dat het wijzigen
                // gelukt is.

                // (er wordt hier geredirect ipv de view te tonen,
                // zodat je bij een 'refresh' niet de vraag krijgt
                // of je de gegevens opnieuw wil posten.)

                return RedirectToAction("Edit", new { id = p.ID });
            }
            catch
            {
                return View("Edit" ,p);
            }
        }

        // GET: /Personen/LidMaken/id
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

        // GET: /Personen/Verhuizen/adresID
        public ActionResult Verhuizen(int id)
        {
            VerhuisInfo model = new VerhuisInfo(id);

            return View("AdresBewerken", model);
        }

        // POST: /Personen/Verhuizen/adresID
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Verhuizen()
        {
            throw new NotImplementedException();
        }

        public ActionResult Hallo()
        {
            return View("Hallo");
        }
    }
}
