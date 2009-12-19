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
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    public class LedenController : BaseController

    {
        //
        // GET: /Leden/
        public ActionResult Index(int groepID)
        {
            // Recentste groepswerkjaar ophalen, en leden tonen.
            return List(ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID)), 1, groepID);
        }

        // TODO: verder uitwerken paginering
        //
        // GET: /Leden/List/{groepsWerkJaarId}/{page}
        public ActionResult List(int groepsWerkJaarId, int page, int groepID)
        {
            // Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten. Paginering gebeurt hier per werkjaar.
            Sessie.LaatsteLijst = "Leden";
            Sessie.LaatstePagina = page;
            //TODO groepswerkjaarid moet hierbij

            int totaal = 0;

            var model = new Models.LidInfoModel();
            BaseModelInit(model, groepID);

            var list =
                ServiceHelper.CallService<IGroepenService, IList<AfdelingInfo>>
                (groep => groep.AfdelingenOphalen(groepsWerkJaarId));
            model.AfdelingsInfoDictionary = new Dictionary<int, AfdelingInfo>();
            foreach (AfdelingInfo ai in list)
            {
                model.AfdelingsInfoDictionary.Add(ai.ID, ai);
            }

            model.LidInfoLijst =
                ServiceHelper.CallService<ILedenService, IList<LidInfo>>
                (lid => lid.PaginaOphalen(groepsWerkJaarId, page, 20, out totaal));
            model.GroepsWerkJaarIdZichtbaar = groepsWerkJaarId;
            // TODO: lijst opbouwen met alle GroepsWerkJaren van de huidige groep
            // model.GroepsWerkJaarLijst = ...;
            model.Title = "Ledenoverzicht";
            model.PageHuidig = page;
            model.PageTotaal = (int)Math.Ceiling(totaal / 20d);
            model.Totaal = totaal;
            return View("Index", model);
        }

        //TODO verder uitwerken paginering
        // GET: /Leden/Afdeling/{afdelingsID}/{page}
        public ActionResult Afdeling(int page, int id, int groepID)
        {
            // Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten. Paginering gebeurt hier per werkjaar.
            //Sessie.LaatsteLijst = "Leden";
            //Sessie.LaatstePagina = groepsWerkJaarId;

            int groepsWerkJaarId = ServiceHelper.CallService<IGroepenService, int>(l => l.RecentsteGroepsWerkJaarIDGet(groepID));
            int totaal = 0;

            var model = new Models.LidInfoModel();
            BaseModelInit(model, groepID);
            model.LidInfoLijst =
                ServiceHelper.CallService<ILedenService, IList<LidInfo>>
                (lid => lid.PaginaOphalenVolgensCategorie(id, groepsWerkJaarId, page, 20, out totaal));
            //model.GroepsWerkJaarIdZichtbaar = groepsWerkJaarId;
            // TODO: lijst opbouwen met alle GroepsWerkJaren van de huidige groep
            // model.GroepsWerkJaarLijst = ...;
            model.Title = "Overzicht leden " + "categorie";
            model.PageHuidig = page;
            model.PageTotaal = (int)Math.Ceiling(totaal / 20d);
            model.Totaal = totaal;
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
        // GET: /Leden/AfdelingBewerken/{groepsWerkJaarID}/{lidID}
        public ActionResult AfdelingBewerken(int groepsWerkJaarID, int lidID, int groepID)
        {
            var model = new LedenModel();
            BaseModelInit(model, groepID);

            var list =
                ServiceHelper.CallService<IGroepenService, IList<AfdelingInfo>>
                (groep => groep.AfdelingenOphalen(groepsWerkJaarID));
            model.AfdelingsInfoDictionary = new Dictionary<int, AfdelingInfo>();
            foreach (AfdelingInfo ai in list)
            {
                model.AfdelingsInfoDictionary.Add(ai.ID, ai);
            }

            model.HuidigLid = ServiceHelper.CallService<ILedenService, LidInfo>
                (l => l.LidOphalenMetAfdelingen(lidID));

            model.AfdelingIDs = model.HuidigLid.AfdelingIdLijst.ToList();

            model.Title = "Ledenoverzicht";
            
            return View("AfdelingBewerken", model);
        }

        //
        // POST: /Leden/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AfdelingBewerken(LedenModel model, int groepID)
        {
            IList<int> selectie = new List<int>();
            if (model.HuidigLid.Type == LidType.Kind)
            {
                selectie.Add(model.AfdelingID);
            }
            else
            {
                if (model.AfdelingIDs != null) // dit komt erop neer dat er iets geselecteerd
                {
                    selectie = model.AfdelingIDs;
                }
            }

            try
            {
                int x = selectie.Count;
                ServiceHelper.CallService<ILedenService>(e => e.BewarenMetAfdelingen(model.HuidigLid.LidID, selectie));
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
