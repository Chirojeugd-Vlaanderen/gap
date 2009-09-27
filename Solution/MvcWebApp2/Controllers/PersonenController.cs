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
        [GroepActionFilter]
        public ActionResult Index()
        {
            return List(1);
        }

        //
        // GET: /Personen/List/{paginanummer}
        [GroepActionFilter]
        public ActionResult List(int page)
        {
            // Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten
            Sessie.LaatsteLijst = "Personen";
            Sessie.LaatstePagina = page;

            int totaal = 0;

            var model = new Models.PersoonInfoModel();
            model.PersoonInfoLijst = 
                ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonInfo>>
                (g => g.PaginaOphalenMetLidInfo(Sessie.GroepID, page, 20, out totaal));
            model.PageHuidig = page;
            model.PageTotaal = (int) Math.Ceiling(totaal / 20d);
            model.Title = "Personenoverzicht";

            return View("Index", model);
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
            model.Title = model.HuidigePersoon.Persoon.VolledigeNaam;
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
                // TODO: wat als er een fout optreedt bij PersoonBewaren?
                TempData["feedback"] = "Wijzigingen zijn opgeslagen";

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
        [GroepActionFilter]
        public ActionResult LidMaken(int id)
        {
            TempData["feedback"] = ServiceHelper.CallService<ILedenService, String>(l => l.LidMakenEnBewaren(id));
            return RedirectToAction("Index");
        }

        // GET: /Personen/Verhuizen/vanAdresID
        public ActionResult Verhuizen(int id, int aanvragerID)
        {
			VerhuisInfo model = new VerhuisInfo(id, aanvragerID);			
            model.Title = "Personen Verhuizen";
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

                ServiceHelper.CallService<IGelieerdePersonenService>(l => l.PersonenVerhuizen(model.PersoonIDs, model.NaarAdres, model.VanAdresMetBewoners.ID));

                // FIXME: Dit (onderstaand) is uiteraard niet de goede manier om 
                // de persoon te bepalen die getoond moet worden.  
                // Bovendien is het niet zeker of de gebruiker
                // wel een persoon heeft aangevinkt.  Maar voorlopig trek ik me
                // er nog niks van aan

                Debug.Assert(model.PersoonIDs.Count > 0);

                // Toon een persoon die woont op het nieuwe adres.
                // (wat hier moet gebeuren hangt voornamelijk af van de use case)

                return RedirectToAction("Edit", new { id = model.AanvragerID });
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

        // GET: /Personen/AdresVerwijderen/PersoonsAdresID
        public ActionResult AdresVerwijderen(int id)
        {
            return null;
            ///TODO zorgen dat de juiste methode gecalld wordt. Mss moet dit een post zijn?
        }

        // GET: /Personen/NieuwAdres/gelieerdePersoonID
        public ActionResult NieuwAdres(int id)
        {
            NieuwAdresModel model = new NieuwAdresModel(id);
            model.Title = "Nieuw adres toevoegen";
            return View("NieuwAdres", model);
        }

        // post: /Personen/NieuwAdres
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NieuwAdres(NieuwAdresModel model)
        {
            try
            {
                // De service zal model.NieuwAdres.ID negeren; dit wordt
                // steeds opnieuw opgezocht.  Adressen worden nooit
                // gewijzigd, enkel bijgemaakt (en eventueel verwijderd.)

                ServiceHelper.CallService<IGelieerdePersonenService>(l => l.AdresToevoegenAanPersonen(model.PersoonIDs, model.NieuwAdres));

                return RedirectToAction("Edit", new { id = model.AanvragerID });
            }
            catch (FaultException<AdresFault> ex)
            {
                new ModelStateWrapper(ModelState).BerichtenToevoegen(ex.Detail, "NieuwAdres.");

                // De mogelijke bewoners zijn op dit moment vergeten, en moeten dus
                // terug opgevraagd worden.

                model.HerstelMogelijkeBewoners();

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
