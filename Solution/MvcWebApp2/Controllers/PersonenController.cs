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
using Cg2.ServiceContracts.FaultContracts;
using Capgemini.Adf.ServiceModel;
using Cg2.ServiceContracts;

namespace MvcWebApp2.Controllers
{
    public class PersonenController : Controller
    {
        //
        // GET: /Personen/
        public ActionResult Index()
        {
            int aantal;
            IList<GelieerdePersoon> personen = ServiceHelper.CallService<IGelieerdePersonenService, IList<GelieerdePersoon> >(l => l.PaginaOphalenMetLidInfo(int.Parse(ConfigurationSettings.AppSettings["TestGroepID"]), 1, 12, out aantal));

            return View("Index", personen);
        }

        //
        // GET: /Personen/Details/5
        public ActionResult Details(int id)
        {
            GelieerdePersoon p = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(id));
            return View("Details", p);
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
            GelieerdePersoon p = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(id));
            return View("Edit", p);
        }

        //
        // POST: /Personen/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(GelieerdePersoon p)
        {
            try
            {
                ServiceHelper.CallService<IGelieerdePersonenService>(l => l.PersoonBewaren(p));
 
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
            ServiceHelper.CallService<ILedenService>(l => l.LidMakenEnBewaren(id));

            return RedirectToAction("Index");
        }

        // GET: /Personen/Verhuizen/adresID
        public ActionResult Verhuizen(int id)
        {
            VerhuisInfo model = new VerhuisInfo(id);

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
            catch (FaultException<VerhuisFault> ex)
            {
                switch (ex.Detail.Code)
                {
                    case FoutCode.OnbekendeStraat:
                        ViewData.ModelState.AddModelError("NaarAdres.Straat.Naam", ex.Detail.Boodschap);
                        break;
                    case FoutCode.OnbekendeGemeente:
                        this.ModelState.AddModelError("NaarAdres.Straat.Naam", "oeps");
                        ViewData.ModelState.AddModelError("NaarAdres.SubGemeente.Naam", ex.Detail.Boodschap);
                        break;
                    default:
                        throw;  // onverwachte exceptie gewoon verder throwen
                }

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

        public ActionResult Hallo()
        {
            return View("Hallo");
        }
    }
}
