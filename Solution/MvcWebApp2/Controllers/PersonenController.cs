using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Configuration;
using Cg2.Orm;
using MvcWebApp2.Models;
using System.Diagnostics;
using System.ServiceModel;
using Cg2.Fouten.FaultContracts;
using Cg2.Adf.ServiceModel;
using Cg2.ServiceContracts;
using Cg2.Validatie;

namespace MvcWebApp2.Controllers
{
    public class PersonenController : BaseController
    {
        //
        // GET: /Personen/
        public ActionResult Index()
        {
            return List(1);
        }

        //
        // GET: /Personen/List/4
        public ActionResult List(int page)
        {
            int totaal = 0;

            var model = new Models.PersoonInfoModel();
            model.PersoonInfoLijst = ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonInfo>>(g => g.PaginaOphalenMetLidInfo(int.Parse(ConfigurationSettings.AppSettings["TestGroepID"]), page, 20, out totaal));
            model.PageVorige = page - 1 >= 1 ? page - 1 : -1;
            model.PageHuidige = page;
            model.PageVolgende = page + 1 <= totaal/20 ? page + 1 : -1;
            model.Title = "Personen-overzicht";

            return View("Index", model);
        }

        //
        // GET: /Personen/Details/5
        public ActionResult Details(int id)
        {

            var model = new Models.GelieerdePersonenModel();
            model.HuidigePersoon = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(id));
            //GelieerdePersoon p = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(id));
            model.Title = "Personen Detail";
            return View("Details", model);
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

            var model = new Models.GelieerdePersonenModel();
            model.HuidigePersoon = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(id));
            model.Title = "Persoon Bewerken " + model.HuidigePersoon.Persoon.VolledigeNaam;
            return View("Edit", model);
        }

        //
        // POST: /Personen/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(GelieerdePersonenModel p)
        {
            try
            {
                ServiceHelper.CallService<IGelieerdePersonenService>(l => l.PersoonBewaren(p.HuidigePersoon));

                // Voorlopig opnieuw redirecten naar Edit;
                // er zou wel gemeld moeten worden dat het wijzigen
                // gelukt is.

                // (er wordt hier geredirect ipv de view te tonen,
                // zodat je bij een 'refresh' niet de vraag krijgt
                // of je de gegevens opnieuw wil posten.)

                return RedirectToAction("Edit", new { id = p.HuidigePersoon.ID });
            }
            catch
            {
                return View("Edit", p);
            }
        }

        // GET: /Personen/LidMaken/id
        public ActionResult LidMaken(int id)
        {
            ServiceHelper.CallService<ILedenService>(l => l.LidMakenEnBewaren(id));

            return RedirectToAction("Index");
        }

        // GET: /Personen/Verhuizen/adresID
        public ActionResult Verhuizen(int id)
        {
            
            var model = new VerhuisInfo(id);
            model.Title = "Personen Verhuis";
            return View("AdresBewerken", model);
        }

        // POST: /Personen/Verhuizen
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Verhuizen(VerhuisInfo model)
        {
            try
            {
                // De service zal het meegeleverder model.NaarAdres.ID negeren, en 
                // opnieuw opzoeken.
                //
                // Adressen worden nooit gewijzigd, enkel bijgemaakt.  (en eventueel
                // verwijderd.)

                ServiceHelper.CallService<IGelieerdePersonenService>(l => l.Verhuizen(model.GelieerdePersoonIDs, model.NaarAdres, model.VanAdresMetBewoners.ID));

                // FIXME: Dit (onderstaand) is uiteraard niet de goede manier om 
                // de persoon te bepalen die getoond moet worden.  
                // Bovendien is het niet zeker of de gebruiker
                // wel een persoon heeft aangevinkt.  Maar voorlopig trek ik me
                // er nog niks van aan

                Debug.Assert(model.GelieerdePersoonIDs.Count > 0);

                // Toon een persoon die woont op het nieuwe adres.
                // (wat hier moet gebeuren hangt voornamelijk af van de use case)

                return RedirectToAction("Edit", new { id = model.GelieerdePersoonIDs[0] });
            }
            catch (FaultException<AdresFault> ex)
            {
                new ModelStateWrapper(ModelState).BerichtenToevoegen(ex.Detail, "NaarAdres.");

                // Als ik de bewoners van het 'Van-adres' niet had getoond in
                // de view, dan had ik de view meteen kunnen aanroepen met het
                // model dat terug 'gebind' is.

                // Maar ik toon de bewoners wel, dus moeten die hier opnieuw
                // uit de database gehaald worden:

                model.HerstelVanAdres();

                return View("AdresBewerken", model);
            }
            catch
            {
                throw;  // onverwachte exceptie gewoon verder throwen
            }
        }

        // GET: /Personen/NieuwAdres/gelieerdePersoonID
        public ActionResult NieuwAdres(int id)
        {
            NieuwAdresInfo model = new NieuwAdresInfo(id);
            return View("NieuwAdres", model);
        }

        // post: /Personen/NieuwAdres
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NieuwAdres(NieuwAdresInfo model)
        {
            try
            {
                // De service zal model.NieuwAdres.ID negeren; dit wordt
                // steeds opnieuw opgezocht.  Adressen worden nooit
                // gewijzigd, enkel bijgemaakt (en eventueel verwijderd.)

                ServiceHelper.CallService<IGelieerdePersonenService>(l => l.AdresToevoegen(model.GelieerdePersoonIDs, model.NieuwAdres));

                return RedirectToAction("Edit", new { id = model.AanvragerID });
            }
            catch (FaultException<AdresFault> ex)
            {
                new ModelStateWrapper(ModelState).BerichtenToevoegen(ex.Detail, "NieuwAdres");
                return View("NieuwAdres", model);
            }
            catch
            {
                throw;
            }
        }

        public ActionResult Hallo()
        {
            return View("Hallo");
        }
    }
}
