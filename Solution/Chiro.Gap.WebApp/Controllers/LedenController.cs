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
            return List(ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID)), 0, groepID);
        }

        // De paginering zal gebeuren per groepswerkjaar, niet per grootte van de pagina
        // Er wordt ook meegegeven welke afdeling er gevraagd is (0 is alles)
        // GET: /Leden/List/{afdID}/{groepsWerkJaarId}
        public ActionResult List(int groepsWerkJaarId, int afdID, int groepID)
        {
            // Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten. Paginering gebeurt hier per werkjaar.
            Sessie.LaatsteLijst = "Leden";
            Sessie.LaatsteActieID = afdID;
            Sessie.LaatstePagina = groepsWerkJaarId;

            int paginas = 0;

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

            model.GroepsWerkJaarLijst =
                ServiceHelper.CallService<IGroepenService, IList<GroepsWerkJaar>>
                        (e => e.WerkJarenOphalen(groepID));

            GroepsWerkJaar huidig = (from g in model.GroepsWerkJaarLijst
                                     where g.ID == groepsWerkJaarId
                                     select g).FirstOrDefault();

            model.GroepsWerkJaarIdZichtbaar = groepsWerkJaarId;
            model.GroepsWerkJaartalZichtbaar = huidig.WerkJaar;

            if (afdID == 0)
            {
                model.LidInfoLijst =
                    ServiceHelper.CallService<ILedenService, IList<LidInfo>>
                        (lid => lid.PaginaOphalen(groepsWerkJaarId, out paginas));

                model.Title = "Ledenoverzicht van het jaar " + model.GroepsWerkJaartalZichtbaar;

            }
            else
            {
                model.LidInfoLijst =
                    ServiceHelper.CallService<ILedenService, IList<LidInfo>>
                    (lid => lid.PaginaOphalenVolgensAfdeling(groepsWerkJaarId, afdID, out paginas));

                AfdelingInfo af = (from a in model.AfdelingsInfoDictionary.AsQueryable()
                                    where a.Value.ID == afdID
                                    select a.Value).FirstOrDefault();

                model.Title = "Ledenoverzicht van de " + af.Naam + " van het jaar " + model.GroepsWerkJaartalZichtbaar;
            }
            
            model.PageHuidig = model.GroepsWerkJaarIdZichtbaar;
            model.PageTotaal = model.LidInfoLijst.Count;
            model.HuidigeAfdeling = afdID;
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

                return RedirectToAction("List", new { groepsWerkJaarId = Sessie.LaatstePagina, afdID = Sessie.LaatsteActieID });
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
            return RedirectToAction("List", new { groepsWerkJaarId = Sessie.LaatstePagina, afdID = Sessie.LaatsteActieID });
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
        // POST: /Leden/AfdelingBewerken/5
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
                return RedirectToAction("List", new { groepsWerkJaarId = Sessie.LaatstePagina, afdID = Sessie.LaatsteActieID });
            }
            catch
            {
                return View();
            }
        }
    }
}
