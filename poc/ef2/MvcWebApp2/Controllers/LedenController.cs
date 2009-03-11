using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Cg2.Orm;
using System.Configuration;

namespace MvcWebApp2.Controllers
{
    public class LedenController : Controller
    {
        //
        // GET: /Leden/

        public ActionResult Index()
        {
            IList<Lid> leden;
            using (LedenServiceReference.LedenServiceClient service = new MvcWebApp2.LedenServiceReference.LedenServiceClient())
            {
                int aantal;
                leden = service.PaginaOphalen(out aantal, int.Parse(ConfigurationSettings.AppSettings["TestGroepsWerkJaarID"]), 1, 12);
            }
            return View("Index", leden);
        }

        //
        // GET: /Leden/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Leden/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Leden/Create

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
        // GET: /Leden/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Leden/Edit/5

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
