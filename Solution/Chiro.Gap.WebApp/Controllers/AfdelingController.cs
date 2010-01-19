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
    public class AfdelingController : BaseController
    {
        //
        // GET: /Afdeling/
        public ActionResult Index(int groepID)
        {
            // Recentste groepswerkjaar ophalen, en leden tonen.
            return List(ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID)), groepID);
        }

        //
        // GET: /Afdeling/List/{groepsWerkJaarId}
        public ActionResult List(int groepsWerkJaarId, int groepID)
        {
            var model = new Models.AfdelingInfoModel();
            BaseModelInit(model, groepID);

            // lijst met de AfdelingInfo voor Afdelingen die in het opgegeven werkjaar voorkomen als AfdelingsJaar
            model.GebruikteAfdelingLijst =
                ServiceHelper.CallService<IGroepenService, IList<AfdelingInfo>>
                (groep => groep.AfdelingenOphalen(groepsWerkJaarId));

            // lijst met de AfdelingInfo voor Afdelingen die in het opgegeven werkjaar voorkomen als AfdelingsJaar
            Groep g = ServiceHelper.CallService<IGroepenService, Groep>
                (groep => groep.OphalenMetAfdelingen(groepID));
            model.OngebruikteAfdelingLijst = new List<AfdelingInfo>();
            foreach (Afdeling a in g.Afdeling)
            {
                Boolean found = false;
                foreach (AfdelingInfo ai in model.GebruikteAfdelingLijst)
                {
                    if (ai.ID == a.ID)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    model.OngebruikteAfdelingLijst.Add(new AfdelingInfo { ID = a.ID, Naam = a.Naam, Afkorting = a.Afkorting });
                }
            }

            model.Titel = "Afdelingen";
            return View("Index", model);
        }

        //
        // GET: /Afdeling/Nieuw
        public ActionResult Nieuw(int groepID)
        {
            var model = new Models.AfdelingInfoModel();
            BaseModelInit(model, groepID);

            model.HuidigeAfdeling = new Afdeling();

            model.Titel = "Nieuwe afdeling";
            return View("Nieuw", model);
        }

        //
        // POST: /Afdeling/Nieuw
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Nieuw(Models.AfdelingInfoModel model, int groepID)
        {
            BaseModelInit(model, groepID);
            ServiceHelper.CallService<IGroepenService>(e => e.AfdelingAanmaken(groepID, model.HuidigeAfdeling.Naam, model.HuidigeAfdeling.Afkorting));

            // TODO: wat als er een fout optreedt bij AfdelingAanmaken?
            TempData["feedback"] = "Wijzigingen zijn opgeslagen";

            // (er wordt hier geredirect ipv de view te tonen,
            // zodat je bij een 'refresh' niet de vraag krijgt
            // of je de gegevens opnieuw wil posten.)
            return RedirectToAction("Index");
        }

        //
        // GET: /Afdeling/Activeren/afdelingID
        public ActionResult Activeren(int groepID, int id)
        {
            var model = new Models.AfdelingInfoModel();
            BaseModelInit(model, groepID);

            model.HuidigAfdelingsJaar = new AfdelingsJaar();
            model.OfficieleAfdelingenLijst =
                ServiceHelper.CallService<IGroepenService, IList<OfficieleAfdeling>>
                (groep => groep.OfficieleAfdelingenOphalen());

            // Afdeling van afdelingsjaar invullen
            Groep g = ServiceHelper.CallService<IGroepenService, Groep>
                (groep => groep.OphalenMetAfdelingen(groepID));
            foreach (Afdeling a in g.Afdeling)
            {
                if (a.ID == id)
                {
                    model.HuidigeAfdeling = a;
                }
            }

            model.Titel = "Afdeling activeren";
            return View("AfdelingsJaar", model);
        }

        //
        // POST: /Afdeling/Activeren/afdelingID
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Activeren(Models.AfdelingInfoModel model, int groepID, int id)
        {
            BaseModelInit(model, groepID);
            ServiceHelper.CallService<IGroepenService>(e => e.AfdelingsJaarAanmaken(groepID, id, model.OfficieleAfdelingID, model.HuidigAfdelingsJaar.GeboorteJaarVan, model.HuidigAfdelingsJaar.GeboorteJaarTot));

            // TODO: wat als er een fout optreedt bij AfdelingAanmaken?
            TempData["feedback"] = "Wijzigingen zijn opgeslagen";

            // (er wordt hier geredirect ipv de view te tonen,
            // zodat je bij een 'refresh' niet de vraag krijgt
            // of je de gegevens opnieuw wil posten.)
            return RedirectToAction("Index");
        }

        //
        // GET: /Afdeling/Verwijderen/afdelingsJaarId
        public ActionResult Verwijderen(int groepID, int id)
        {
            // Afdeling van afdelingsjaar invullen
            ServiceHelper.CallService<IGroepenService>
                (groep => groep.AfdelingsJaarVerwijderen(id));
            return RedirectToAction("Index");
        }

        //
        // GET: /Afdeling/Bewerken/afdelingsJaarId
        public ActionResult Bewerken(int groepID, int id)
        {
            var model = new Models.AfdelingInfoModel();
            BaseModelInit(model, groepID);

            model.HuidigAfdelingsJaar =
                ServiceHelper.CallService<IGroepenService, AfdelingsJaar>
                (groep => groep.AfdelingsJaarOphalen(id));
            model.OfficieleAfdelingenLijst =
                ServiceHelper.CallService<IGroepenService, IList<OfficieleAfdeling>>
                (groep => groep.OfficieleAfdelingenOphalen());
            model.HuidigeAfdeling = model.HuidigAfdelingsJaar.Afdeling;
            model.OfficieleAfdelingID = model.HuidigAfdelingsJaar.OfficieleAfdeling.ID;

            model.Titel = "Afdeling bewerken";
            return View("AfdelingsJaar", model);
        }

        //
        // POST: /Afdeling/Bewerken/afdelingsJaarId
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Bewerken(Models.AfdelingInfoModel model, int groepID, int id)
        {
            BaseModelInit(model, groepID);
            ServiceHelper.CallService<IGroepenService>(e => e.AfdelingsJaarBewarenMetWijzigingen(id, model.OfficieleAfdelingID, model.HuidigAfdelingsJaar.GeboorteJaarVan, model.HuidigAfdelingsJaar.GeboorteJaarTot));

            // TODO: wat als er een fout optreedt bij AfdelingAanmaken?
            TempData["feedback"] = "Wijzigingen zijn opgeslagen";

            return RedirectToAction("Index");
        }

    }
}
