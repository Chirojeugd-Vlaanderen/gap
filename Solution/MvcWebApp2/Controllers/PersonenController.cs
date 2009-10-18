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
        public ActionResult Index(int groepID)
        {
            return List(1, groepID);
        }

        //
        // GET: /Personen/List/{paginanummer}
        public ActionResult List(int page, int groepID)
        {
            // Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten
            Sessie.LaatsteLijst = "Personen";
            Sessie.LaatstePagina = page;

            int totaal = 0;

            var model = new Models.PersoonInfoModel();
            BaseModelInit(model, groepID);

            model.PersoonInfoLijst = 
                ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonInfo>>
                (g => g.PaginaOphalenMetLidInfo(groepID, page, 20, out totaal));
            model.PageHuidig = page;
            model.PageTotaal = (int) Math.Ceiling(totaal / 20d);
            model.Title = "Personenoverzicht";
            model.Totaal = totaal;

            return View("Index", model);
        }

        //
        // GET: /Personen/Nieuw
        public ActionResult Nieuw(int groepID)
        {
            var model = new Models.GelieerdePersonenModel();
            BaseModelInit(model, groepID);
            model.NieuweHuidigePersoon();
            
            model.Title = Properties.Resources.NieuwePersoonTitel;
            return View("EditGegevens", model);
        }

        //
        // POST: /Personen/Nieuw
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Nieuw(GelieerdePersonenModel p, int groepID)
        {
            BaseModelInit(p, groepID);
            int i = ServiceHelper.CallService<IGelieerdePersonenService, int>(l => l.PersoonAanmaken(p.HuidigePersoon, groepID));

            // Voorlopig opnieuw redirecten naar EditRest;
            // er zou wel gemeld moeten worden dat het wijzigen
            // gelukt is.
            // TODO: wat als er een fout optreedt bij PersoonBewaren?
            TempData["feedback"] = "Wijzigingen zijn opgeslagen";

            // (er wordt hier geredirect ipv de view te tonen,
            // zodat je bij een 'refresh' niet de vraag krijgt
            // of je de gegevens opnieuw wil posten.)
            return RedirectToAction("EditRest", new { id = i });
        }

        //
        // GET: /Personen/EditRest/5
        public ActionResult EditRest(int id, int groepID)
        {
            var model = new Models.GelieerdePersonenModel();
            BaseModelInit(model, groepID);
            model.HuidigePersoon = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(id));
            model.Title = model.HuidigePersoon.Persoon.VolledigeNaam;
            return View("EditRest", model);
        }

        //
        // POST: /Personen/EditRest/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditRest(GelieerdePersonenModel p, int groepID)
        {
            var model = new Models.GelieerdePersonenModel();
            BaseModelInit(model, groepID);
            model.HuidigePersoon = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(p.HuidigePersoon.ID));
            model.Title = model.HuidigePersoon.Persoon.VolledigeNaam;
            return RedirectToAction("EditGegevens", new { id = p.HuidigePersoon.ID });
        }

        //
        // GET: /Personen/EditGegevens/5
        public ActionResult EditGegevens(int id, int groepID)
        {
            var model = new Models.GelieerdePersonenModel();
            BaseModelInit(model, groepID);
            model.HuidigePersoon = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(id));
            model.Title = model.HuidigePersoon.Persoon.VolledigeNaam;
            return View("EditGegevens", model);
        }

        //
        // POST: /Personen/EditGegevens/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditGegevens(GelieerdePersonenModel p, int groepID)
        {
            //try
            //{

                ServiceHelper.CallService<IGelieerdePersonenService>(l => l.PersoonBewaren(p.HuidigePersoon));

                // Voorlopig opnieuw redirecten naar EditRest;
                // er zou wel gemeld moeten worden dat het wijzigen
                // gelukt is.
                // TODO: wat als er een fout optreedt bij PersoonBewaren?
                TempData["feedback"] = "Wijzigingen zijn opgeslagen";

                // (er wordt hier geredirect ipv de view te tonen,
                // zodat je bij een 'refresh' niet de vraag krijgt
                // of je de gegevens opnieuw wil posten.)
                return RedirectToAction("EditRest", new { id = p.HuidigePersoon.ID });
            /*}
            catch
            {
                return View("EditGegevens", p);
            }*/
        }

        // GET: /Personen/LidMaken/id
        public ActionResult LidMaken(int id, int groepID)
        {
            TempData["feedback"] = ServiceHelper.CallService<ILedenService, String>(l => l.LidMakenEnBewaren(id));
            return RedirectToAction("Index");
        }

        // GET: /Personen/Verhuizen/vanAdresID?AanvragerID=#
        public ActionResult Verhuizen(int id, int aanvragerID, int groepID)
        {
			VerhuisModel model = new VerhuisModel(id, aanvragerID);
            BaseModelInit(model, groepID);

            model.Title = "Personen Verhuizen";
            return View("AdresBewerken", model);
        }

        // POST: /Personen/Verhuizen
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Verhuizen(VerhuisModel model, int groepID)
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

                return RedirectToAction("EditRest", new { id = model.AanvragerID });
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

        // GET: /Personen/AdresVerwijderen/AdresID
        public ActionResult AdresVerwijderen(int id, int gelieerdePersoonID, int groepID)
        {
            AdresVerwijderenModel model = new AdresVerwijderenModel(id, gelieerdePersoonID);
            BaseModelInit(model, groepID);

            model.Title = "Adres verwijderen";
            return View("AdresVerwijderen", model);
        }

        // POST: /Personen/AdresVerwijderen
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdresVerwijderen(AdresVerwijderenModel model, int groepID)
        {
            BaseModelInit(model, groepID);
            ServiceHelper.CallService<IGelieerdePersonenService>(foo => foo.AdresVerwijderenVanPersonen(model.PersoonIDs, model.AdresMetBewoners.ID));
            return RedirectToAction("EditRest", new { id = model.AanvragerGelieerdePersoonID });
        }

        // GET: /Personen/NieuwAdres/gelieerdePersoonID
        public ActionResult NieuwAdres(int id, int groepID)
        {
            NieuwAdresModel model = new NieuwAdresModel(id);
            BaseModelInit(model, groepID);
            model.Title = "Nieuw adres toevoegen";
            return View("NieuwAdres", model);
        }

        // post: /Personen/NieuwAdres
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NieuwAdres(NieuwAdresModel model, int groepID)
        {
            try
            {
                // De service zal model.NieuwAdres.ID negeren; dit wordt
                // steeds opnieuw opgezocht.  Adressen worden nooit
                // gewijzigd, enkel bijgemaakt (en eventueel verwijderd.)

                ServiceHelper.CallService<IGelieerdePersonenService>(l => l.AdresToevoegenAanPersonen(model.PersoonIDs, model.NieuwAdres));

                return RedirectToAction("EditRest", new { id = model.AanvragerID });
            }
            catch (FaultException<AdresFault> ex)
            {
                BaseModelInit(model, groepID);

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

        // GET: /Personen/NieuweCommVorm/gelieerdePersoonID
        public ActionResult NieuweCommVorm(int gelieerdePersoonID, int groepID)
        {
            GelieerdePersoon g = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(gelieerdePersoonID));
            IEnumerable<CommunicatieType> types = ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<CommunicatieType>>(l => l.ophalenCommunicatieTypes());
            NieuweCommVormModel model = new NieuweCommVormModel(g, types);
            BaseModelInit(model, groepID);
            model.Title = "Nieuwe communicatievorm toevoegen";
            return View("NieuweCommVorm", model);
        }

        // post: /Personen/NieuweCommVorm
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NieuweCommVorm(NieuweCommVormModel model, int groepID, int gelieerdePersoonID)
        {
            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CommVormToevoegenAanPersoon(gelieerdePersoonID, model.NieuweCommVorm, model.geselecteerdeCommVorm));
            return RedirectToAction("EditRest", new { id = gelieerdePersoonID });
        }

        // GET: /Personen/VerwijderenCommVorm/commvormid
        public ActionResult VerwijderenCommVorm(int commvormID, int gelieerdePersoonID, int groepID)
        {
            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CommVormVerwijderenVanPersoon(gelieerdePersoonID, commvormID));
            return RedirectToAction("EditRest", new { id = gelieerdePersoonID });
        }


        // GET: /Personen/CommVormBewerken/gelieerdePersoonID
        public ActionResult BewerkenCommVorm(int commvormID, int gelieerdePersoonID, int groepID)
        {
            GelieerdePersoon g = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(gelieerdePersoonID));
            CommunicatieVorm commv = ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieVorm>(l => l.ophalenCommVorm(commvormID));
            CommVormModel model = new CommVormModel(g, commv);
            BaseModelInit(model, groepID);
            model.Title = "Communicatievorm bewerken";
            return View("CommVormBewerken", model);
        }

        //TODO meerdere commvormen tegelijk

        // POST: /Personen/CommVormBewerken/gelieerdePersoonID
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BewerkenCommVorm(CommVormModel model, int gelieerdePersoonID, int groepID)
        {
            BaseModelInit(model, groepID);
            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.AanpassenCommVorm(model.NieuweCommVorm));
            return RedirectToAction("EditRest", new { id = gelieerdePersoonID });
            ///TODO catch exceptions overal
        }


        // GET: /Personen/VerwijderenCategorie/categorieID
        public ActionResult VerwijderenCategorie(int categorieID, int gelieerdePersoonID, int groepID)
        {
            IList<int> list = new List<int>();
            list.Add(gelieerdePersoonID);
            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CategorieVerwijderenVanPersoon(list, categorieID));
            return RedirectToAction("EditRest", new { id = gelieerdePersoonID });
        }

        // GET: /Personen/ToevoegenAanCategorie/categorieID
        public ActionResult ToevoegenAanCategorie(int gelieerdePersoonID, int groepID)
        {
            GelieerdePersoon g = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(gelieerdePersoonID));
            IEnumerable<Categorie> cats = ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<Categorie>>(l => l.ophalenCategorieen(groepID));
            CategorieModel model = new CategorieModel(cats, g);
            BaseModelInit(model, groepID);
            return View("CategorieToevoegen", model);
        }

        // POST: /Personen/ToevoegenAanCategorie/categorieID
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ToevoegenAanCategorie(CategorieModel model, int gelieerdePersoonID, int groepID)
        {   
            IList<int> list = new List<int>();
            list.Add(gelieerdePersoonID);
            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CategorieToevoegenAanPersoon(list, model.selectie));
            return RedirectToAction("EditRest", new { id = gelieerdePersoonID });
        }
        

        public ActionResult Hallo()
        {
            return View("Hallo");
        }
    }
}
