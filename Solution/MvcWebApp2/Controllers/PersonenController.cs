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
using MvcWebApp2.GelieerdePersonenServiceReference;

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
        public ActionResult Verhuizen(VerhuisInfo model)
        {
            try
            {
                using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new MvcWebApp2.GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
                {
                    // De service zal het meegeleverder model.Adres.ID negeren, en 
                    // opnieuw opzoeken.
                    //
                    // Adressen worden nooit gewijzigd, enkel bijgemaakt.  (en eventueel
                    // verwijderd.)

                    try
                    {
                        service.Verhuizen(model.GelieerdePersoonIDs, model.NaarAdres, model.VanAdresID);
                    }
                    catch (FaultException<CgFaultException> ex)
                    {
                        switch (ex.Detail.Code)
                        {
                            case FoutCode.OnbekendeStraat:
                                ViewData.ModelState.AddModelError("NaarAdres.Straat.Naam", ex.Detail.Boodschap);
                                break;
                            case FoutCode.OnbekendeGemeente:
                                ViewData.ModelState.AddModelError("NaarAdres.SubGemeente.Naam", ex.Detail.Boodschap);
                                break;
                        }

                        // Als ik de bewoners van het 'Van-adres' niet had getoond in
                        // de view, dan had ik de view meteen kunnen aanroepen met het
                        // model dat terug 'gebind' is.

                        // Maar ik toon de bewoners wel, dus moeten die hier opnieuw
                        // uit de database gehaald worden:

                        model.HerstelBewoners();

                        return View("AdresBewerken", model);
                    }
                }

                // FIXME: Dit is uiteraard niet de goede manier om de persoon te bepalen
                // die bekeken moet worden.  Bovendien is het niet zeker of de gebruiker
                // wel een persoon heeft aangevinkt.  Voorlopig ga ik er echter van uit
                // dat dat wel het geval is.

                Debug.Assert(model.GelieerdePersoonIDs.Count > 0);
                return RedirectToAction("Edit", new { id = model.GelieerdePersoonIDs[0] });
            }
            catch
            {
                return View("AdresBewerken", model);
            }
        }

        public ActionResult Hallo()
        {
            return View("Hallo");
        }
    }
}
