using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Cg2.Orm;
using System.Configuration;
using Cg2.Adf.ServiceModel;
using Cg2.ServiceContracts;

namespace MvcWebApp2.Controllers
{
    public class LedenController : BaseController

    {
        //
        // GET: /Leden/
        [GroepActionFilter]
        public ActionResult Index()
        {
            // Recentste groepswerkjaar ophalen, en leden tonen.
            return List(ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(Sessie.GroepID)));
        }

        // TODO: verder uitwerken paginering
        //
        // GET: /Leden/List/{groepsWerkJaarId}
        public ActionResult List(int groepsWerkJaarId)
        {
            // Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten. Paginering gebeurt hier per werkjaar.
            Sessie.LaatsteLijst = "Leden";
            Sessie.LaatstePagina = groepsWerkJaarId;

            var model = new Models.LidInfoModel();
            model.LidInfoLijst =
                ServiceHelper.CallService<ILedenService, IList<LidInfo>>
                (lid => lid.PaginaOphalen(groepsWerkJaarId));
            model.GroepsWerkJaarIdZichtbaar = groepsWerkJaarId;
            // TODO: lijst opbouwen met alle GroepsWerkJaren van de huidige groep
            // model.GroepsWerkJaarLijst = ...;
            model.Title = "Ledenoverzicht";
            return View("Index", model);
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
        // GET: /Leden/Verwijderen/5
        public ActionResult Verwijderen(int id)
        {
            Boolean success = ServiceHelper.CallService<ILedenService, Boolean>(l => l.Verwijderen(id));
            if (success)
            {
                TempData["feedback"] = "Lid is verwijderd";
            }
            return RedirectToAction("Index");
        }

        //
        // POST: /Leden/Verwijderen/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Verwijderen(int id, FormCollection collection)
        {
            return RedirectToAction("Index");
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
