using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Chiro.Gap.Orm;
using System.Configuration;
using Chiro.Adf.ServiceModel;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Controllers
{
    public class LedenController : BaseController

    {
        //
        // GET: /Leden/
        public ActionResult Index(int groepID)
        {
            // Recentste groepswerkjaar ophalen, en leden tonen.
            return List(ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID)), groepID);
        }

        // TODO: verder uitwerken paginering
        //
        // GET: /Leden/List/{groepsWerkJaarId}
        public ActionResult List(int groepsWerkJaarId, int groepID)
        {
            // Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten. Paginering gebeurt hier per werkjaar.
            Sessie.LaatsteLijst = "Leden";
            Sessie.LaatstePagina = groepsWerkJaarId;

            var model = new Models.LidInfoModel();
            BaseModelInit(model, groepID);
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

        public ActionResult Create(int groepID)
        {
            return View();
        } 

        //
        // POST: /Leden/Create

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(FormCollection collection, int groepID)
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
        public ActionResult Verwijderen(int id, int groepID)
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
        public ActionResult Verwijderen(int id, FormCollection collection, int groepID)
        {
            return RedirectToAction("Index");
        }

        //
        // GET: /Leden/Edit/5
 
        public ActionResult Edit(int id, int groepID)
        {
            return View();
        }

        //
        // POST: /Leden/Edit/5

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, FormCollection collection, int groepID)
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
