// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;

using AutoMapper;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Validatie;
using Chiro.Gap.WebApp.ActionFilters;
using Chiro.Gap.WebApp.HtmlHelpers;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    // Om te zorgen dat het terugkeren naar de vorige lijst en dergelijke werkt in samenwerking met het opvragen van subsets
    // (categorieën of zo), hebben we steeds een default (categorie, ...) die aangeeft dat alle personen moeten worden meegegeven

    /// <summary>
    /// Controller voor weergave en beheer van alle personen die gelieerd zijn aan de groep
    /// </summary>
    [HandleError]
    public class PersonenController : PersonenEnLedenController
    {
        /// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        public PersonenController(IVeelGebruikt veelGebruikt)
            : base(veelGebruikt)
        {
        }
        // TODO er moeten ook nog een laatst gebruikte "actie" worden toegevoegd, niet alleen actie id

        [HandleError]
        public override ActionResult Index(int groepID)
        {
            // redirect naar alle personen van de groep, pagina 1.
            return RedirectToAction("List", new
            {
                page = "A",
                id = 0,
                sortering = PersoonSorteringsEnum.Naam
            });
        }

        /// <summary>
        /// Toont de persoonsinformatie op (inclusief lidinfo) voor personen waarvan de familienaam begint met
        /// de letter <paramref name="page"/> uit een bepaalde categorie, en toont deze via de view 'Index'.
        /// </summary>
        /// <param name="page">Beginletter familienamen te tonen personen</param>
        /// <param name="groepID">Huidige groep waarin de gebruiker aan het werken is</param>
        /// <param name="id">ID van de gevraagde categorie.  Kan ook 0 zijn; dan worden alle personen
        /// geselecteerd.</param>
        /// <param name="sortering">Geeft de sortering van de pagina en lijst aan</param>
        /// <returns>De personenlijst in de view 'Index'</returns>
        /// <remarks>De attributen RouteValue en QueryStringValue laten toe dat we deze method overloaden.
        /// zie http://blog.abodit.com/2010/02/asp-net-mvc-ambiguous-match/ </remarks>
        [HandleError]
        [ParametersMatch]
        public ActionResult List([QueryStringValue]string page, [RouteValue]int groepID, [RouteValue]int id, [QueryStringValue]PersoonSorteringsEnum sortering)
        {
            // Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten
            ClientState.VorigeLijst = Request.Url.ToString();

            int totaal = 0;

            var model = new PersoonInfoModel();
            BaseModelInit(model, groepID);
            model.GekozenCategorieID = id;
            model.Sortering = sortering;

            model.GroepsCategorieen = ServiceHelper.CallService<IGroepenService, IList<CategorieInfo>>(svc => svc.CategorieenOphalen(groepID)).ToList();
            model.GroepsCategorieen.Add(new CategorieInfo
            {
                ID = 0,
                Naam = "Alle personen"
            });

            var categorieID = id;

            if (categorieID == 0)  // Alle personen bekijken
            {
                model.PersoonInfos = ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonDetail>>(g => g.OphalenMetLidInfoViaLetter(groepID, page, sortering, out totaal));
                model.HuidigePagina = page;
                model.Titel = "Personenoverzicht";
                model.Totaal = totaal;
                model.Paginas = ServiceHelper.CallService<IGelieerdePersonenService, IList<String>>(g => g.EersteLetterNamenOphalen(groepID));

                // Als er niemand met een naam is die met een A begint
                // is het nogal nutteloos dat we eerst op die pagina belanden.
                // Maar als de hele lijst leeg is, is het natuurlijk ook onnozel
                // dat we naar een andere pagina proberen te gaan.
                if (page == "A" && !model.PersoonInfos.Any() && model.Paginas.Count > 0)
                {
                    return RedirectToAction("List", new
                    {
                        page = model.Paginas.First().ToUpper(),
                        id = 0,
                        sortering = PersoonSorteringsEnum.Naam
                    });
                }
            }
            else	// Alleen personen uit de gekozen categorie bekijken
            {
                // TODO de catID is eigenlijk niet echt type-safe, maar wel het makkelijkste om te doen (lijkt te veel op PaginaOphalenLidInfo(groepid, ...))
                model.PersoonInfos =
                    ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonDetail>>
                    (g => g.PaginaOphalenUitCategorieMetLidInfo(categorieID, page, sortering, out totaal));
                model.HuidigePagina = page;

                // Ga in het lijstje met categorieën na welke er geselecteerd werd, zodat we de naam in de paginatitel kunnen zetten
                String naam = (from c in model.GroepsCategorieen
                               where c.ID == categorieID
                               select c).First().Naam;

                model.Titel = "Overzicht " + naam;
                model.Totaal = totaal;
                model.Paginas = ServiceHelper.CallService<IGelieerdePersonenService, IList<String>>(g => g.EersteLetterNamenOphalenCategorie(id));

                // Als er niemand met een naam is die met een A begint
                // is het nogal nutteloos dat we eerst op die pagina belanden.
                // Maar als de hele lijst leeg is, is het natuurlijk ook onnozel
                // dat we naar een andere pagina proberen te gaan.
                if (page == "A" && !model.PersoonInfos.Any() && model.Paginas.Count > 0)
                {
                    return RedirectToAction("List", new
                    {
                        page = model.Paginas.First().ToUpper(),
                        id = categorieID,
                        sortering = PersoonSorteringsEnum.Naam
                    });
                }
            }

            return View("Index", model);
        }

        /// <summary>
        /// Afhandelen postback naar list.  Als er iets relevant in 'GekozenActie' zit,
        /// wordt 'ToepassenOpSelectie' aangeroepen.  In het andere geval werd gewoon
        /// een categorie geselecterd.
        /// </summary>
        /// <param name="model">De property model.GekozenCategorieID bevat de ID van de categorie waarvan de
        /// personen getoond moeten worden.  Is deze 0, dan worden alle personen getoond</param>
        /// <param name="groepID">ID van de groep waarin de gebruiker momenteel werkt</param>
        /// <returns>Een redirect naar de juiste lijst</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult List(PersoonInfoModel model, int groepID)
        {
            if (model.GekozenActie > 0)
            {
                return ToepassenOpSelectie(model, groepID);
            }
            else
            {
                return RedirectToAction(
                    "List",
                    new
                    {
                        page = "A",
                        id = model.GekozenCategorieID,
                        sortering = model.Sortering
                    });
            }
        }

        /// <summary>
        /// Haalt een Excellijst op van alle personen in een groep, of als <paramref name="id"/> verschilt van 0, 
        /// de personen uit de categorie met ID <paramref name="id"/>.
        /// </summary>
        /// <param name="groepID">Huidige groep waarin de gebruiker aan het werken is</param>
        /// <param name="id">ID van de gevraagde categorie.  Kan ook 0 zijn; dan worden alle personen
        /// geselecteerd.</param>
        /// <returns>Een 'ExcelResult' met de gevraagde lijst</returns>
        [HandleError]
        public ActionResult Download(int groepID, int id)
        {
            IEnumerable<PersoonOverzicht> data;

            // Alle personen bekijken
            if (id == 0)
            {
                data =
                    ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<PersoonOverzicht>>
                    (g => g.AllenOphalenUitGroep(groepID, PersoonSorteringsEnum.Naam));
            }
            else
            {
                data =
                    ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<PersoonOverzicht>>
                    (g => g.AllenOphalenUitCategorie(id, PersoonSorteringsEnum.Naam));
            }

            // Als ExcelManip de kolomkoppen kan afleiden uit de (param)array, en dan liefst nog de DisplayName
            // gebruikt van de PersoonOverzicht-velden, dan is de regel hieronder niet nodig.
            string[] kolomkoppen = 
                                   {
			                       	"AD-nr", "Voornaam", "Naam", "Geboortedatum", "Geslacht", "Straat", "Nr", "Bus", "Postnr",
			                       	"Postcode", "Gemeente", "Land", "Tel", "Mail"
			                       };

            var stream = (new ExcelManip()).ExcelTabel(
                data,
                kolomkoppen,
                it => it.AdNummer,
                it => it.VoorNaam,
                it => it.Naam,
                it => it.GeboorteDatum,
                it => it.Geslacht,
                // Contactgegevens enkel opnemen bij levende mensen
                it => !it.SterfDatum.HasValue ? it.StraatNaam : string.Empty,
                it => !it.SterfDatum.HasValue ? it.HuisNummer : null,
                it => !it.SterfDatum.HasValue ? it.Bus : string.Empty,
                it => !it.SterfDatum.HasValue ? it.PostNummer : null,
                it => !it.SterfDatum.HasValue ? it.PostCode : string.Empty,
                it => !it.SterfDatum.HasValue ? it.WoonPlaats : string.Empty,
                it => !it.SterfDatum.HasValue ? it.Land : string.Empty,
                it => !it.SterfDatum.HasValue ? it.TelefoonNummer : string.Empty,
                it => !it.SterfDatum.HasValue ? it.Email : string.Empty);

            return new ExcelResult(stream, "personen.xlsx");
        }

        /// <summary>
        /// Voert de gekozen actie in de dropdownlist van de personenlijst uit op de geselecteerde
        /// personen.
        /// </summary>
        /// <param name="model">De property GekozenActie bepaalt wat er zal gebeuren met de gelieerde personen
        /// met ID's in de property SelectieGelieerdePersoonIDs.</param>
        /// <param name="groepID">ID van de groep waarin de gebruiker op dit moment aan het werken is.</param>
        /// <returns>Een redirect naar de juiste controller action</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult ToepassenOpSelectie(PersoonInfoModel model, int groepID)
        {
            if (model.GekozenGelieerdePersoonIDs == null || model.GekozenGelieerdePersoonIDs.Count == 0)
            {
                TempData["fout"] = Properties.Resources.NiemandGeselecteerdFout;
                return TerugNaarVorigeLijst();
            }

            ActionResult r;

            switch (model.GekozenActie)
            {
                case 1:
                case 2:  // 2 is stiekem verdwenen, zie #625.
                    r = GelieerdePersonenInschrijven(model.GekozenGelieerdePersoonIDs);
                    break;
                case 3:
                    TempData["list"] = model.GekozenGelieerdePersoonIDs;
                    r = RedirectToAction("CategorieToevoegenAanLijst");
                    break;
                case 4:
                    TempData["ids"] = model.GekozenGelieerdePersoonIDs;
                    r = RedirectToAction("InschrijvenVoorUitstap", new { groepID });
                    break;
                default:
                    TempData["fout"] = Properties.Resources.OnbestaandeActieFeedback;
                    r = TerugNaarVorigeLijst();
                    break;
            }

            return r;
        }

        #region personen

        /// <summary>
        /// Toont het formulier om een nieuwe persoon toe te voegen
        /// </summary>
        /// <param name="groepID">ID van de groep waaraan die persoon gelieerd moet worden</param>
        /// <returns></returns>
        /// <!-- GET: /Personen/Nieuw -->
        [HandleError]
        public ActionResult Nieuw(int groepID)
        {
            var model = new GelieerdePersonenModel();
            BaseModelInit(model, groepID);
            model.HuidigePersoon = new PersoonDetail();

            model.Titel = Properties.Resources.NieuwePersoonTitel;
            return View("EditGegevens", model);
        }

        /// <summary>
        /// Gebruikt de ingevulde gegevens om een nieuwe persoon aan te maken
        /// </summary>
        /// <param name="model">Het ingevulde model</param>
        /// <param name="groepID">ID van de groep waaraan de nieuwe persoon gelieerd moet worden</param>
        /// <returns></returns>
        /// <!-- POST: /Personen/Nieuw -->
        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        [HandleError]
        public ActionResult Nieuw(GelieerdePersonenModel model, int groepID)
        {
            IDPersEnGP ids;

            BaseModelInit(model, groepID);
            model.Titel = Properties.Resources.NieuwePersoonTitel;

            if (!ModelState.IsValid)
            {
                return View("EditGegevens", model);
            }

            try
            {
                // (ivm forceer: 0: false, 1: true)
                ids = ServiceHelper.CallService<IGelieerdePersonenService, IDPersEnGP>(l => l.AanmakenForceer(model.HuidigePersoon, groepID, model.Forceer));
            }
            catch (FaultException<BlokkerendeObjectenFault<PersoonDetail>> fault)
            {
                model.GelijkaardigePersonen = fault.Detail.Objecten;
                model.Forceer = true;
                return View("EditGegevens", model);
            }

            // Voorlopig opnieuw redirecten naar EditRest;
            // er zou wel gemeld moeten worden dat het wijzigen
            // gelukt is.
            // TODO Wat als er een fout optreedt bij PersoonBewaren?
            TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

            // (er wordt hier geredirect ipv de view te tonen,
            // zodat je bij een 'refresh' niet de vraag krijgt
            // of je de gegevens opnieuw wil posten.)
            return RedirectToAction("EditRest", new { id = ids.GelieerdePersoonID });
        }

        /// <summary>
        /// Maakt een broertje of zusje
        /// </summary>
        /// <param name="gelieerdepersoonID">ID van de gelieerde persoon van wie de
        /// contactgegevens en de familienaam gekopieerd worden</param>
        /// <param name="groepID">ID van de groep waarvoor de kloon een nieuwe
        /// gelieerde persoon wordt.</param>
        /// <returns></returns>
        /// <!-- GET: /Personen/Kloon -->
        [HandleError]
        public ActionResult Kloon(int gelieerdepersoonID, int groepID)
        {
            var model = new GelieerdePersonenModel();
            BaseModelInit(model, groepID);
            var broerzus = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailOphalen(gelieerdepersoonID));
            model.HuidigePersoon = new PersoonDetail();

            model.BroerzusID = broerzus.GelieerdePersoonID;
            model.HuidigePersoon.Naam = broerzus.Naam;

            model.Titel = Properties.Resources.NieuwePersoonTitel;
            return View("EditGegevens", model);
        }

        /// <summary>
        /// Vraagt aan de service om een nieuwe persoon aan te maken, op basis van de ingevulde gegevens.
        /// Die werden deels ingevuld op basis van een andere persoon, voor wie we hier een broer/zus
        /// aanmaken.
        /// </summary>
        /// <param name="model">Het ingevulde model</param>
        /// <param name="groepID">ID van de groep die een nieuwe gelieerde persoon aanmaakt, en waar die
        /// persoon dus aan gelinkt moet worden</param>
        /// <returns></returns>
        /// <!-- POST: /Personen/Nieuw -->
        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        [HandleError]
        public ActionResult Kloon(GelieerdePersonenModel model, int groepID)
        {
            // TODO (#1028): dit is mss iets om op de server te draaien?

            /////BEGIN DUPLICATE CODE

            if (model.BroerzusID == 0)
            {
                throw new InvalidOperationException("Zou niet 0 mogen zijn? Als wel zo is, maak volledig nieuwe persoon");
            }

            BaseModelInit(model, groepID);
            model.Titel = Properties.Resources.NieuwePersoonTitel;

            if (!ModelState.IsValid)
            {
                return View("EditGegevens", model);
            }

            IDPersEnGP ids;
            try
            {
                // (ivm forceer: 0: false, 1: true)
                ids = ServiceHelper.CallService<IGelieerdePersonenService, IDPersEnGP>(l => l.AanmakenForceer(model.HuidigePersoon, groepID, model.Forceer));
            }
            catch (FaultException<BlokkerendeObjectenFault<PersoonDetail>> fault)
            {
                model.GelijkaardigePersonen = fault.Detail.Objecten;
                model.Forceer = true;
                return View("EditGegevens", model);
            }
            /////END DUPLICATE CODE

            var broerzus = ServiceHelper.CallService<IGelieerdePersonenService, PersoonLidInfo>(l => l.AlleDetailsOphalen(model.BroerzusID));

            var gezinsComm = (from a in broerzus.CommunicatieInfo
                              where a.Voorkeur && a.IsGezinsGebonden
                              select a).ToList();

            if (gezinsComm.Count() != 0)
            {
                // vermijd bloat van te veel over de lijn te sturen
                foreach (var c in gezinsComm)
                {
                    var c1 = c;
                    ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CommunicatieVormToevoegen(ids.GelieerdePersoonID, c1));

                    // TODO errors opvangen? #1047
                }
            }

            if (broerzus.PersoonDetail.VoorkeursAdresID != null)
            {
                var voorkeursAdres = (from a in broerzus.PersoonsAdresInfo
                                      where a.PersoonsAdresID == broerzus.PersoonDetail.VoorkeursAdresID
                                      select a).FirstOrDefault();
                if (voorkeursAdres != null)
                {
                    var list = new List<int> { ids.GelieerdePersoonID };
                    ServiceHelper.CallService<IGelieerdePersonenService>(l => l.AdresToevoegenGelieerdePersonen(list, voorkeursAdres, true));
                }
            }

            return RedirectToAction("EditRest", new { id = ids.GelieerdePersoonID });
        }

        /// <summary>
        /// Laat toe persoonsgegevens te wijzigen
        /// </summary>
        /// <param name="id">GelieerdePersoonID van te wijzigen persoon</param>
        /// <param name="groepID">GroepID van de huidig geselecteerde groep</param>
        /// <returns>De view 'EditGegevens'</returns>
        [HandleError]
        public ActionResult EditGegevens(int id, int groepID)
        {
            var model = new GelieerdePersonenModel();
            BaseModelInit(model, groepID);
            // model.HuidigePersoon = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.AlleDetailsOphalen(id, groepID));
            model.HuidigePersoon = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailOphalen(id));
            model.Titel = model.HuidigePersoon.VolledigeNaam;
            return View("EditGegevens", model);
        }

        /// <summary>
        /// Probeert de gewijzigde persoonsgegevens te persisteren via de webservice
        /// </summary>
        /// <param name="model"><c>GelieerdePersonenModel</c> met gegevens gewijzigd door de gebruiker</param>
        /// <param name="groepID">GroepID van huidig geseecteerde groep</param>
        /// <returns>Redirect naar overzicht persoonsinfo indien alles ok, anders opnieuw de view
        /// 'EditGegevens'.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult EditGegevens(GelieerdePersonenModel model, int groepID)
        {
            if (!ModelState.IsValid)
            {
                return View("EditGegevens", model);
            }

            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.Bewaren(model.HuidigePersoon));

            // Voorlopig opnieuw redirecten naar EditRest;
            // er zou wel gemeld moeten worden dat het wijzigen
            // gelukt is.
            // TODO Wat als er een fout optreedt bij PersoonBewaren?
            TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

            // (er wordt hier geredirect ipv de view te tonen,
            // zodat je bij een 'refresh' niet de vraag krijgt
            // of je de gegevens opnieuw wil posten.)
            return RedirectToAction("EditRest", new { id = model.HuidigePersoon.GelieerdePersoonID, groepID });
        }

        // NEW CODE

        /// <summary>
        /// Deze actie met onduidelijke naam toont gewoon de personenfiche van de gelieerde
        /// persoon met id <paramref name="id"/>.
        /// </summary>
        /// <param name="id">ID van de te tonen gelieerde persoon</param>
        /// <param name="groepID">GroepID van de groep waarin de gebruiker aan het werken is</param>
        /// <returns>De view van de personenfiche</returns>
        // id = gelieerdepersonenid
        // GET
        [HandleError]
        public ActionResult EditRest(int id, int groepID)
        {
            var model = new PersoonEnLidModel();
            BaseModelInit(model, groepID);

            model.PersoonLidInfo = ServiceHelper.CallService<IGelieerdePersonenService, PersoonLidInfo>(l => l.AlleDetailsOphalen(id));

            if (!model.PersoonLidInfo.PersoonDetail.SterfDatum.HasValue)
            {
                AfdelingenOphalen(model);

                model.KanVerzekerenLoonVerlies = model.PersoonLidInfo.PersoonDetail.GeboorteDatum != null &&
                                                 DateTime.Today.Year -
                                                 ((DateTime)model.PersoonLidInfo.PersoonDetail.GeboorteDatum).Year >=
                                                 Properties.Settings.Default.LoonVerliesVanafLeeftijd;
                model.PrijsVerzekeringLoonVerlies = Properties.Settings.Default.PrijsVerzekeringLoonVerlies;
                model.PrijsDubbelPunt = Properties.Settings.Default.PrijsDubbelPunt;
                model.Titel = model.PersoonLidInfo.PersoonDetail.VolledigeNaam;

                return View("EditRest", model);
            }
            else
            {
                model.Titel = model.PersoonLidInfo.PersoonDetail.VolledigeNaam + " (†)";
                return View("OverledenPersoon", model);
            }
        }

        /// <summary>
        /// Bekijkt model.HuidigLid.  Haalt alle afdelingen van het groepswerkjaar van het lid op, en
        /// bewaart ze in model.AlleAfdelingen.  In model.AfdelingIDs komen de ID's van de toegekende
        /// afdelingen voor het lid.
        /// </summary>
        /// <param name="model">Het ingevulde model</param>
        [HandleError]
        private void AfdelingenOphalen(PersoonEnLidModel model)
        {
            if (model.PersoonLidInfo.LidInfo != null)
            {
                model.AlleAfdelingen = ServiceHelper.CallService<IGroepenService, IList<AfdelingDetail>>
                (groep => groep.ActieveAfdelingenOphalen(model.PersoonLidInfo.LidInfo.GroepsWerkJaarID));
            }
        }

        /// <summary>
        /// Stelt op basis van het begin van een voor- of achternaam
        /// een lijst suggesties samen met personen die de
        /// gebruiker mogelijk wil vinden
        /// </summary>
        /// <param name="naamOngeveer">Wat de gebruiker al intikte van naam om te zoeken</param>
        /// <param name="groepID">GroepID waarin we aan het werken zijn</param>
        /// <returns>Voorgestelde personen in JSON formaat</returns>
        [HandleError]
        public ActionResult PersoonZoeken(string naamOngeveer, int groepID)
        {
            IEnumerable<PersoonInfo> mogelijkePersonen = ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<PersoonInfo>>(x => x.ZoekenOpNaamVoornaamBegin(groepID, naamOngeveer));
            var personen = mogelijkePersonen.OrderBy(prs => prs.Naam).ThenBy(prs => prs.VoorNaam).Distinct();
            return Json(personen, JsonRequestBehavior.AllowGet);
        }

        #endregion personen

        #region leden

        /// <summary>
        /// Schrijft een gelieerde persoon in in de groep
        /// </summary>
        /// <param name="gelieerdepersoonID">ID van de gelieerde persoon die we willen inschrijven</param>
        /// <param name="groepID">ID van de groep die de bewerking uitvoert</param>
        /// <returns></returns>
        /// <!-- GET: /Personen/Inschrijven/gelieerdepersoonID -->
        [HandleError]
        public ActionResult Inschrijven(int gelieerdepersoonID, int groepID)
        {
            return GelieerdePersonenInschrijven(new List<int> { gelieerdepersoonID });
        }

        /// <summary>
        /// Schrijft een gelieerde persoon uit uit de groep
        /// </summary>
        /// <param name="gelieerdepersoonID">ID van de gelieerde persoon die we willen uitschrijven</param>
        /// <param name="groepID">ID van de groep die de bewerking uitvoert</param>
        /// <returns></returns>
        /// <!-- GET: /Personen/Uitschrijven/gelieerdepersoonID -->
        [HandleError]
        public ActionResult Uitschrijven(int gelieerdepersoonID, int groepID)
        {
            GelieerdePersonenUitschrijven(new List<int> { gelieerdepersoonID }, groepID, Properties.Resources.LedenUitgeschreven);
            return TerugNaarVorigeLijst();
        }

        #endregion leden

        #region adressen

        /// <summary>
        /// Laat de gebruiker een persoon en eventueel diens huisgenoten verhuizen
        /// </summary>
        /// <param name="id">AdresID van het 'van-adres'</param>
        /// <param name="aanvragerID">GelieerdePersoonID van de verhuizer</param>
        /// <param name="groepID">Momenteel geselecteerde groep</param>
        /// <returns>De view 'AdresBewerken'</returns>
        [HandleError]
        public ActionResult Verhuizen(int id, int aanvragerID, int groepID)
        {
            var model = new AdresModel();
            BaseModelInit(model, groepID);

            // AanvragerID: de gelieerde persoon die wil verhuizen.  Dit is enkel nodig om achteraf
            // terug te kunnen keren naar de detailfiche van deze persoon.
            model.AanvragerID = aanvragerID;

            // Haal info op over het gezin
            var a = ServiceHelper.CallService<IGelieerdePersonenService, GezinInfo>(l => l.GezinOphalen(id, groepID));

            // Onderstaande mapping mapt het gezin naar PersoonsAdresInfo, waarin enkel
            // informatie van het adres zit.  (De andere gezinsleden worden nu nog genegeerd.)
            Mapper.CreateMap<GezinInfo, PersoonsAdresInfo>()
                .ForMember(
                    dst => dst.AdresType,
                    opt => opt.Ignore());  // voorlopig negeren we adrestype; dat nemen we direct nog over.

            model.PersoonsAdresInfo = Mapper.Map<GezinInfo, PersoonsAdresInfo>(a);

            // Als het adres buitenlands is, dan moeten we de woonplaats nog eens overnemen in
            // WoonPlaatsBuitenland.  Dat is nodig voor de AdresBewerkenControl, die een beetje
            // raar ineen zit.

            if (String.Compare(model.PersoonsAdresInfo.LandNaam, Properties.Resources.Belgie, true) != 0)
            {
                model.WoonPlaatsBuitenLand = model.PersoonsAdresInfo.WoonPlaatsNaam;
            }

            // Het ID van het oude adres hebben we nodig om de verhuis door te geven aan de service
            model.OudAdresID = id;

            // Als adrestype kiezen we het adrestype van de verhuizende persoon op het oude adres.
            model.PersoonsAdresInfo.AdresType = (from bewoner in a.Bewoners
                                                 where bewoner.GelieerdePersoonID == aanvragerID
                                                 select bewoner.AdresType).FirstOrDefault();

            model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(a.PostNr);
            model.AlleLanden = VeelGebruikt.LandenOphalen();

            // GelieerdePersoonIDs bepaalt de personen die aangevinkt zijn als 'verhuizen mee'.
            // Standaard is dat iedereen die op het oude adres woont.
            model.GelieerdePersoonIDs = (from b in a.Bewoners
                                         select b.GelieerdePersoonID).ToList();

            // Tenslotte hebben we de  namen van de bewoners nodig in het model, zodat die
            // getoond kunnen worden.
            model.Bewoners = (from p in a.Bewoners
                              select new CheckBoxListInfo(
                                p.GelieerdePersoonID.ToString(),
                                p.PersoonVolledigeNaam,
                                model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID))).ToArray();

            model.Titel = "Personen verhuizen";
            return View("AdresBewerken", model);
        }

        /// <summary>
        /// Ook in de view Verhuizen krijg je - indien javascript niet werkt - een knop
        /// 'Woonplaatsen ophalen', waarmee het lijstje met woonplaatsen moet worden gevuld.
        /// </summary>
        /// <param name="model">Informatie over het nieuw adres</param>
        /// <param name="groepID">ID van de geselecteerde groep</param>
        /// <returns>Opnieuw de view AdresBewerken, maar met het lijstje woonplaatsen ingevuld</returns>
        [ActionName("Verhuizen")]
        [AcceptVerbs(HttpVerbs.Post)]
        [ParameterAccepteren(Naam = "action", Waarde = "Woonplaatsen ophalen")]
        [HandleError]
        public ActionResult Verhuizen_WoonplaatsenOphalen(AdresModel model, int groepID)
        {
            // TODO (#1037): Deze method is identiek aan NieuwAdres_WoonPlaatsenOphalen.
            // Dat moet dus niet dubbel geschreven zijn

            BaseModelInit(model, groepID);
            var bewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<BewonersInfo>>(svc => svc.HuisGenotenOphalenZelfdeGroep(model.AanvragerID));

            model.Bewoners = (from p in bewoners
                              select new CheckBoxListInfo(
                                  p.GelieerdePersoonID.ToString(),
                                  p.PersoonVolledigeNaam,
                                  model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID))).ToArray();

            model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);

            return View("AdresBewerken", model);
        }

        /// <summary>
        /// Verhuist de personen bepaald door <paramref name="model"/>.PersoonIDs van het adres
        /// bpaald door <paramref name="model"/>.OudAdresID naar <paramref name="model"/>.Adres.
        /// </summary>
        /// <param name="model">Bevat de nodige info voor de verhuis</param>
        /// <param name="groepID">Huidig geslecteerde groep van de gebruiker</param>
        /// <returns>De view 'EditRest' indien OK, anders opnieuw de view 'AdresBewerken'.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [ParameterAccepteren(Naam = "action", Waarde = "Bewaren")]
        [HandleError]
        public ActionResult Verhuizen(AdresModel model, int groepID)
        {
            // Als het adres buitenlands is, neem dan de woonplaats over uit het
            // vrij in te vullen veld.

            if (String.Compare(model.PersoonsAdresInfo.LandNaam, Properties.Resources.Belgie, true) != 0)
            {
                model.PersoonsAdresInfo.WoonPlaatsNaam = model.WoonPlaatsBuitenLand;
            }

            try
            {
                // De service zal het meegeleverder model.NaarAdres.ID negeren, en 
                // opnieuw opzoeken.
                // Adressen worden nooit gewijzigd, enkel bijgemaakt (en eventueel verwijderd.)

                ServiceHelper.CallService<IGelieerdePersonenService>(l => l.GelieerdePersonenVerhuizen(model.GelieerdePersoonIDs, model.PersoonsAdresInfo, model.OudAdresID));

                return RedirectToAction("EditRest", "Personen", new { id = model.GelieerdePersoonIDs.First() });
            }
            catch (FaultException<OngeldigObjectFault> ex)
            {
                BaseModelInit(model, groepID);

                new ModelStateWrapper(ModelState).BerichtenToevoegen(ex.Detail, String.Empty);

                // Als ik de bewoners van het 'Van-adres' niet had getoond in
                // de view, dan had ik de view meteen kunnen aanroepen met het
                // model dat terug 'gebind' is.

                // Maar ik toon de bewoners wel, dus moeten die hier opnieuw
                // uit de database gehaald worden:
                var bewoners = (ServiceHelper.CallService<IGelieerdePersonenService, GezinInfo>(l => l.GezinOphalen(model.OudAdresID, groepID))).Bewoners;

                model.Bewoners = (from p in bewoners
                                  select new CheckBoxListInfo(
                                      p.GelieerdePersoonID.ToString(),
                                      p.PersoonVolledigeNaam,
                                      model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID))).ToArray();

                model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);
                model.AlleLanden = VeelGebruikt.LandenOphalen();
                model.Titel = "Personen verhuizen";

                return View("AdresBewerken", model);
            }
            catch (FaultException<BlokkerendeObjectenFault<PersoonsAdresInfo2>> ex)
            {
                BaseModelInit(model, groepID);

                var bewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<BewonersInfo>>(svc => svc.HuisGenotenOphalenZelfdeGroep(model.AanvragerID));

                var probleemPersIDs = from pa in ex.Detail.Objecten
                                      select pa.PersoonID;

                model.Bewoners = (from p in bewoners
                                  select new CheckBoxListInfo(
                                      p.GelieerdePersoonID.ToString(),
                                      p.PersoonVolledigeNaam,
                                      model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID))).ToArray();

                model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);
                model.Titel = "Personen verhuizen";

                return View("AdresBewerken", model);
            }
        }

        /// <summary>
        /// Toont de view die de gebruiker toelaat een adres te verwijderen
        /// </summary>
        /// <param name="id">ID van te verwijderen persoonsadres</param>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon waarvan we komen.</param>
        /// <param name="groepID">ID van de groep waarin momenteel gewerkt wordt</param>
        /// <returns></returns>
        [HandleError]
        public ActionResult AdresVerwijderen(int id, int gelieerdePersoonID, int groepID)
        {
            var model = new AdresVerwijderenModel
                            {
                                AanvragerID = gelieerdePersoonID,
                                Adres = ServiceHelper.CallService<IGelieerdePersonenService, GezinInfo>(foo => foo.GezinOphalen(id, groepID)),
                                PersoonIDs = new List<int>
			            		             	{
			            		             		ServiceHelper.CallService<IGelieerdePersonenService, int>(
			            		             			srvc => srvc.PersoonIDGet(gelieerdePersoonID))
			            		             	}
                            };

            // Standaard vervalt enkel het adres van de aanvrager
            // Van de aanvrager heb ik het PersoonID nodig, en we hebben nu enkel het
            // ID van de GelieerdePersoon.  Het PersoonID

            BaseModelInit(model, groepID);

            model.Titel = "Adres verwijderen";
            return View("AdresVerwijderen", model);
        }

        /// <summary>
        /// Vraagt aan de services om de link tussen een adres en één of meerdere personen te verwijderen
        /// </summary>
        /// <param name="model">Het model met de ingevulde gegevens</param>
        /// <param name="groepID">ID van de groep die de bewerking uitvoert</param>
        /// <returns>De persoonlijkegegevensfiche van de persoon bij wie we oorspronkelijk de opdracht
        /// gaven om het adres te verwijderen.</returns>
        /// <!-- POST: /Personen/AdresVerwijderen -->
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult AdresVerwijderen(AdresVerwijderenModel model, int groepID)
        {
            BaseModelInit(model, groepID);
            ServiceHelper.CallService<IGelieerdePersonenService>(foo => foo.AdresVerwijderenVanPersonen(model.PersoonIDs, model.Adres.ID));
            VeelGebruikt.LedenProblemenResetten(groepID);

            return RedirectToAction("EditRest", new { id = model.AanvragerID });
        }

        /// <summary>
        /// Laat toe een nieuw adres te koppelen aan een gelieerde persoon.
        /// </summary>
        /// <param name="id">GelieerdePersoonID van de persoon die een nieuw adres moet krijgen</param>
        /// <param name="groepID">ID van de huidige geselecteerde groep</param>
        /// <returns>De view 'AdresBewerken'</returns>
        [HandleError]
        public ActionResult NieuwAdres(int id, int groepID)
        {
            var model = new AdresModel();
            BaseModelInit(model, groepID);

            model.AanvragerID = id;
            model.AlleLanden = VeelGebruikt.LandenOphalen();
            model.PersoonsAdresInfo.LandNaam = Properties.Resources.Belgie;

            var bewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<BewonersInfo>>(l => l.HuisGenotenOphalenZelfdeGroep(id));

            model.Bewoners = (from p in bewoners
                              select new CheckBoxListInfo(
                                  p.GelieerdePersoonID.ToString(),
                                  p.PersoonVolledigeNaam,
                                  p.GelieerdePersoonID == id)).ToArray();

            // Standaard krijgt alleen de aanvrager een nieuw adres.

            model.Titel = "Nieuw adres toevoegen";
            return View("AdresBewerken", model);
        }

        /// <summary>
        /// Bij het posten van een nieuw adres krijg je - indien javascript niet werkt - een knop
        /// 'Woonplaatsen ophalen', waarmee het lijstje met woonplaatsen wordt gevuld.
        /// </summary>
        /// <param name="model">Informatie over het nieuw adres</param>
        /// <param name="groepID">ID van de geselecteerde groep</param>
        /// <returns>Opnieuw de view AdresBewerken, maar met het lijstje woonplaatsen ingevuld</returns>
        [ActionName("NieuwAdres")]
        [AcceptVerbs(HttpVerbs.Post)]
        [ParameterAccepteren(Naam = "action", Waarde = "Woonplaatsen ophalen")]
        [HandleError]
        public ActionResult NieuwAdres_WoonplaatsenOphalen(AdresModel model, int groepID)
        {
            BaseModelInit(model, groepID);
            var bewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<BewonersInfo>>(svc => svc.HuisGenotenOphalenZelfdeGroep(model.AanvragerID));

            model.Bewoners = (from p in bewoners
                              select new CheckBoxListInfo(
                                  p.GelieerdePersoonID.ToString(),
                                  p.PersoonVolledigeNaam,
                                  model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID))).ToArray();

            model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);

            return View("AdresBewerken", model);
        }

        /// <summary>
        /// Actie voor post van nieuw adres
        /// </summary>
        /// <param name="model">Bevat de geposte informatie</param>
        /// <param name="groepID">ID van huidig geselecteerde groep</param>
        /// <returns>Zonder problemen wordt geredirect naar de actie 'persoon bewerken'.  Maar
        /// bij een ongeldig adres krijg je opnieuw de view 'AdresBewerken'.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [ParameterAccepteren(Naam = "action", Waarde = "Bewaren")]
        [HandleError]
        public ActionResult NieuwAdres(AdresModel model, int groepID)
        {
            // Als het adres buitenlands is, neem dan de woonplaats over uit het
            // vrij in te vullen veld.

            if (String.Compare(model.PersoonsAdresInfo.LandNaam, Properties.Resources.Belgie, true) != 0)
            {
                model.PersoonsAdresInfo.WoonPlaatsNaam = model.WoonPlaatsBuitenLand;
            }

            try
            {
                // De service zal model.NieuwAdres.ID negeren; dit wordt
                // steeds opnieuw opgezocht.  Adressen worden nooit
                // gewijzigd, enkel bijgemaakt (en eventueel verwijderd.)

                ServiceHelper.CallService<IGelieerdePersonenService>(l => l.AdresToevoegenGelieerdePersonen(model.GelieerdePersoonIDs, model.PersoonsAdresInfo, model.Voorkeur));
                VeelGebruikt.LedenProblemenResetten(groepID);

                // Door het AanvragerID te gebruiken ipv het tijdelijke cookie (uit TerugNaarVorigeFiche) werkt de
                // redirect ook naar behoren als ondertussen in een andere tab een andere persoon bekeken is.

                return RedirectToAction("EditRest", new { id = model.AanvragerID });
            }
            catch (FaultException<OngeldigObjectFault> ex)
            {
                BaseModelInit(model, groepID);

                new ModelStateWrapper(ModelState).BerichtenToevoegen(ex.Detail, String.Empty);

                // De mogelijke bewoners zijn op dit moment vergeten, en moeten dus
                // terug opgevraagd worden.
                var bewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<BewonersInfo>>(l => l.HuisGenotenOphalenZelfdeGroep(model.AanvragerID));

                model.Bewoners = (from p in bewoners
                                  select new CheckBoxListInfo(
                                      p.GelieerdePersoonID.ToString(),
                                      p.PersoonVolledigeNaam,
                                      model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID))).ToArray();

                model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);
                model.AlleLanden = VeelGebruikt.LandenOphalen();
                model.Titel = "Nieuw adres toevoegen";

                return View("AdresBewerken", model);
            }
            catch (FaultException<BlokkerendeObjectenFault<PersoonsAdresInfo2>> ex)
            {
                BaseModelInit(model, groepID);

                // TODO hier wordt niets over geprint!

                // De mogelijke bewoners zijn op dit moment vergeten, en moeten dus
                // terug opgevraagd worden.
                var bewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<BewonersInfo>>(l => l.HuisGenotenOphalenZelfdeGroep(model.AanvragerID));

                // Extraheer bewoners met problemen uit exceptie
                var probleemPersIDs = from pa in ex.Detail.Objecten
                                      select pa.PersoonID;

                model.Bewoners = (from p in bewoners
                                  select new CheckBoxListInfo(
                                      p.GelieerdePersoonID.ToString(),
                                      p.PersoonVolledigeNaam,
                                      model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID))).ToArray();

                model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);
                model.Titel = "Nieuw adres toevoegen";

                return View("AdresBewerken", model);
            }
        }

        /// <summary>
        /// Stelt een adres in als voorkeursadres van de persoon in kwestie
        /// </summary>
        /// <param name="persoonsAdresID">ID van de koppeling tussen adres en persoon</param>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon in kwestie</param>
        /// <param name="groepID">ID van de groep waar het over gaat</param>
        [HandleError]
        public ActionResult VoorkeurAdresMaken(int persoonsAdresID, int gelieerdePersoonID, int groepID)
        {
            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.VoorkeursAdresMaken(persoonsAdresID, gelieerdePersoonID));
            VeelGebruikt.LedenProblemenResetten(groepID);

            return RedirectToAction("EditRest", new { id = gelieerdePersoonID });
        }

        #endregion adressen

        #region commvormen

        /// <summary>
        /// Toont het formulier waarmee een nieuwe communicatievorm toegevoegd kan worden
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon voor wie we een nieuwe 
        /// communicatievorm toevoegen</param>
        /// <param name="groepID">ID van de groep die de communicatievorm toevoegt</param>
        /// <returns></returns>
        /// <!-- GET: /Personen/NieuweCommVorm/gelieerdePersoonID -->
        [HandleError]
        public ActionResult NieuweCommVorm(int gelieerdePersoonID, int groepID)
        {
            var persoonDetail = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailOphalen(gelieerdePersoonID));
            var types = ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<CommunicatieTypeInfo>>(l => l.CommunicatieTypesOphalen());
            var model = new NieuweCommVormModel(persoonDetail, types);
            BaseModelInit(model, groepID);
            model.Titel = "Nieuwe communicatievorm toevoegen";
            return View("NieuweCommVorm", model);
        }

        // post: /Personen/NieuweCommVorm
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult NieuweCommVorm(NieuweCommVormModel model, int groepID, int gelieerdePersoonID)
        {
            // Eerst een hoop gedoe om de CommunicatieInfo uit het model in een
            // CommunicatieDetail te krijgen, zodat de validatie kan gebeuren.

            var communicatieType = ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieTypeInfo>
                (svc => svc.CommunicatieTypeOphalen(model.NieuweCommVorm.CommunicatieTypeID));

            // Ik begrijp onderstaande code niet.  Wordt automapper hier gebruikt om te klonen?
            // En zo ja: wat is de meerwaarde?

            Mapper.CreateMap<CommunicatieDetail, CommunicatieDetail>()
                .ForMember(dst => dst.CommunicatieTypeValidatie, opt => opt.Ignore());

            var communicatieDetail = Mapper.Map<CommunicatieDetail, CommunicatieDetail>(
                model.NieuweCommVorm);

            communicatieDetail.CommunicatieTypeOmschrijving = communicatieType.Omschrijving;
            communicatieDetail.CommunicatieTypeValidatie = communicatieType.Validatie;
            communicatieDetail.CommunicatieTypeVoorbeeld = communicatieType.Voorbeeld;

            var validator = new CommunicatieVormValidator();

            // De validatie van de vorm van telefoonnrs, e-mailadressen,... kan niet automatisch;
            // dat doen we eerst.
            if (!validator.Valideer(communicatieDetail))
            {
                // voeg gevonden fout toe aan modelstate.
                ModelState.AddModelError(
                    "Model.NieuweCommVorm.Nummer",
                    string.Format(
                        Properties.Resources.FormatValidatieFout,
                        communicatieType.Omschrijving,
                        communicatieType.Voorbeeld));
            }

            if (!ModelState.IsValid)
            {
                // Zowel bij automatisch gedetecteerde fout, als bij fout in vorm van
                // communicatievorm: model herstellen, en gebruiker opnieuw laten proberen.

                BaseModelInit(model, groepID);

                // info voor model herstellen
                model.Aanvrager = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailOphalen(gelieerdePersoonID));
                model.Types = ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<CommunicatieTypeInfo>>(l => l.CommunicatieTypesOphalen());
                model.Titel = "Nieuwe communicatievorm toevoegen";

                return View("NieuweCommVorm", model);
            }
            else
            {
                // vermijd bloat van te veel over de lijn te sturen

                var commInfo = new CommunicatieInfo();
                Mapper.CreateMap<CommunicatieDetail, CommunicatieInfo>();
                Mapper.Map(model.NieuweCommVorm, commInfo);

                ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CommunicatieVormToevoegen(gelieerdePersoonID, commInfo));
                VeelGebruikt.LedenProblemenResetten(groepID);

                return RedirectToAction("EditRest", new { id = gelieerdePersoonID });
            }
        }

        /// <summary>
        /// Verwijdert de link tussen een persoon en een communicatievorm
        /// </summary>
        /// <param name="commvormID">ID van de communicatievorm</param>
        /// <param name="groepID">ID van de groep die de communicatievorm verwijdert</param>
        /// <returns></returns>
        /// <!-- GET: /Personen/VerwijderenCommVorm/commvormid -->
        [HandleError]
        public ActionResult VerwijderenCommVorm(int commvormID, int groepID)
        {
            var gelieerdePersoonID = ServiceHelper.CallService<IGelieerdePersonenService, int>(l => l.CommunicatieVormVerwijderenVanPersoon(commvormID));

            VeelGebruikt.LedenProblemenResetten(groepID);

            return RedirectToAction("EditRest", new { id = gelieerdePersoonID });
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="commvormID"></param>
        /// <param name="gelieerdePersoonID"></param>
        /// <param name="groepID">ID van de groep die de bewerking uitvoert</param>
        /// <returns></returns>
        /// <!-- GET: /Personen/CommVormBewerken/gelieerdePersoonID -->
        [HandleError]
        public ActionResult CommVormBewerken(int commvormID, int gelieerdePersoonID, int groepID)
        {
            // TODO (#1026): dit is niet juist broes, want hij haalt 2 keer de persoon op?
            var persoonDetail = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailOphalen(gelieerdePersoonID));
            var commv = ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieDetail>(l => l.CommunicatieVormOphalen(commvormID));
            var model = new CommVormModel(persoonDetail, commv);
            BaseModelInit(model, groepID);
            model.Titel = "Communicatievorm bewerken";
            return View("CommVormBewerken", model);
        }

        // TODO (#1027): meerdere commvormen tegelijk

        /// <summary>
        /// Stuurt de ingevulde gegevens naar de service, die de geselecteerde communicatievorm aanpast
        /// </summary>
        /// <param name="model">Het ingevulde model</param>
        /// <param name="gelieerdePersoonID">De ID van de gelieerde persoon voor wie we de bewerking uitvoeren</param>
        /// <param name="groepID">ID van de groep die de bewerking uitvoert</param>
        /// <returns>Bij succes gaan we naar de persoonlijkegegevensfiche, anders blijven we op het formulier 
        /// om de gegevens mee aan te passen</returns>
        /// <!-- POST: /Personen/CommVormBewerken/gelieerdePersoonID -->
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult CommVormBewerken(CommVormModel model, int gelieerdePersoonID, int groepID)
        {
            var validator = new CommunicatieVormValidator();
            var commVorm = ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieDetail>(l => l.CommunicatieVormOphalen(model.NieuweCommVorm.ID));

            // communicatietype van de oorspronkelijke communicatievorm overnemen.
            // (gedoe)

            model.NieuweCommVorm.CommunicatieTypeID = commVorm.CommunicatieTypeID;
            model.NieuweCommVorm.CommunicatieTypeOmschrijving = commVorm.CommunicatieTypeOmschrijving;
            model.NieuweCommVorm.CommunicatieTypeValidatie = commVorm.CommunicatieTypeValidatie;
            model.NieuweCommVorm.CommunicatieTypeVoorbeeld = commVorm.CommunicatieTypeVoorbeeld;

            // De validatie van de vorm van telefoonnrs, e-mailadressen,... kan niet automatisch;
            // dat doen we eerst.
            if (!validator.Valideer(model.NieuweCommVorm))
            {
                // voeg gevonden fout toe aan modelstate.
                ModelState.AddModelError("Model.NieuweCommVorm.Nummer", string.Format(
                    Properties.Resources.FormatValidatieFout,
                    model.NieuweCommVorm.CommunicatieTypeOmschrijving,
                    model.NieuweCommVorm.CommunicatieTypeVoorbeeld));
            }

            if (!ModelState.IsValid)
            {
                // Zowel bij automatisch gedetecteerde fout (op basis van attributen) als bij
                // fout in vorm communicatievorm: model herstellen en gebruiker opnieuw laten
                // proberen.

                BaseModelInit(model, groepID);

                model.Aanvrager = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailOphalen(gelieerdePersoonID));
                model.NieuweCommVorm = commVorm;
                model.Titel = "Communicatievorm bewerken";

                return View("CommVormBewerken", model);
            }

            // Om bloat over de lijn te vermijden: downgraden naar minimale info

            var commInfo = new CommunicatieInfo();
            Mapper.CreateMap<CommunicatieDetail, CommunicatieInfo>();
            Mapper.Map(model.NieuweCommVorm, commInfo);

            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CommunicatieVormAanpassen(commInfo));
            return RedirectToAction("EditRest", new { id = gelieerdePersoonID });
        }

        #endregion commvormen

        #region categorieën

        /// <summary>
        /// Haalt een gelieerde persoon uit een categorie.
        /// </summary>
        /// <param name="categorieID">ID van de categorie waartoe de persoon niet meer mag behoren</param>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon die we uit een categorie willen halen</param>
        /// <param name="groepID">ID van de groep die de bewerking uitvoert</param>
        /// <returns>Keert terug naar de persoonlijkegegevensfiche</returns>
        /// <!-- GET: /Personen/VerwijderenCategorie/categorieID -->
        [HandleError]
        public ActionResult VerwijderenCategorie(int categorieID, int gelieerdePersoonID, int groepID)
        {
            IList<int> list = new List<int> { gelieerdePersoonID };
            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CategorieVerwijderen(list, categorieID));
            return RedirectToAction("EditRest", new { id = gelieerdePersoonID });
        }

        /// <summary>
        /// Voegt een gelieerde persoon toe aan een categorie
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon die we in een categorie willen stoppen</param>
        /// <param name="groepID">ID van de groep die de bewerking uitvoert</param>
        /// <returns></returns>
        /// <!-- GET: /Personen/ToevoegenAanCategorie/categorieID -->
        [HandleError]
        public ActionResult ToevoegenAanCategorie(int gelieerdePersoonID, int groepID)
        {
            IList<int> l = new List<int> { gelieerdePersoonID };
            TempData["list"] = l;
            return RedirectToAction("CategorieToevoegenAanLijst");
        }

        /// <summary>
        /// Toont de view 'CategorieToevoegen', die toelaat om personen in een categorie onder te
        /// brengen.  
        /// De ID's van onder te brengen personen worden opgevist uit TempData["list"].
        /// TODO (#1137): Kan dat niet properder? => er moet informatie worden doorgegeven aan een method die nu een GET is, omdat er daarna er een POST op moet kunnen gebeuren
        /// vanuit CategorieToevoegenAanLijst.aspx
        /// </summary>
        /// <param name="groepID">ID van de groep waarin de gebruiker momenteel aan het werken is</param>
        /// <returns>De view 'CategorieToevoegen'</returns>
        [HandleError]
        public ActionResult CategorieToevoegenAanLijst(int groepID)
        {
            var model = new CategorieModel();
            BaseModelInit(model, groepID);
            model.Titel = "Toevoegen aan categorie";
            model.Categorieen = ServiceHelper.CallService<IGroepenService, IEnumerable<CategorieInfo>>(l => l.CategorieenOphalen(groepID));

            if (model.Categorieen.Count() > 0)
            {
                object value;
                TempData.TryGetValue("list", out value);
                model.GelieerdePersoonIDs = (List<int>)value;
                var persoonsnamen = ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<PersoonInfo>>(l => l.PersoonInfoOphalen(model.GelieerdePersoonIDs));
                model.GelieerdePersoonNamen = persoonsnamen.Select(e => e.VoorNaam + " " + e.Naam).ToList();
                return View("CategorieToevoegenAanLijst", model);
            }
            else
            {
                TempData["fout"] = Properties.Resources.GeenCategorieenFout;
                return TerugNaarVorigeLijst();
            }
        }

        /// <summary>
        /// Koppelt de gelieerde personen bepaald door <paramref name="model"/>.GelieerdePersonenIDs aan de 
        /// categorieën met ID's <paramref name="model"/>.GeselecteerdeCategorieIDs
        /// </summary>
        /// <param name="model"><c>CategorieModel</c> met ID's van gelieerde personen en categorieën</param>
        /// <param name="groepID">Bepaalt de groep waarin de gebruiker nu werkt</param>
        /// <returns>Als 1 persoon aan een categorie toegekend moet worden, wordt geredirect naar de
        /// details van die persoon.  Anders krijg je de laatst opgroepen lijst.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult CategorieToevoegenAanLijst(CategorieModel model, int groepID)
        {
            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CategorieKoppelen(model.GelieerdePersoonIDs, model.GeselecteerdeCategorieIDs));

            if (model.GelieerdePersoonIDs.Count == 1)
            {
                return RedirectToAction("EditRest", "Personen", new { id = model.GelieerdePersoonIDs.First(), groepID });
            }
            else
            {
                return TerugNaarVorigeLijst();
            }
        }

        #endregion categorieën

        #region uitstappen
        /// <summary>
        /// Toont een view die de gebruiker toelaat om een bivak te kiezen waarvoor de gelieerde personen
        /// met GelieerdePersoonIDs in TempData["ids"] ingeschreven moeten worden.  Bovendien moet
        /// aangevinkt worden of het al dan niet over 'logistieke deelnemers' gaat.
        /// </summary>
        /// <param name="groepID">ID van de groep die de bewerking uitvoert</param>
        /// <returns>Het inschrijfscherm</returns>
        public ActionResult InschrijvenVoorUitstap(int groepID)
        {
            var model = new UitstapInschrijfModel();
            BaseModelInit(model, groepID);
            model.Titel = String.Format(Properties.Resources.UitstapInschrijving);

            model.Uitstappen =
                ServiceHelper.CallService<IUitstappenService, IEnumerable<UitstapInfo>>(svc => svc.OphalenVanGroep(groepID, true));

            if (model.Uitstappen.FirstOrDefault() == null)
            {
                TempData["fout"] = Properties.Resources.GeenUitstappenFout;
                return TerugNaarVorigeLijst();
            }
            else
            {
                object gevonden;
                TempData.TryGetValue("ids", out gevonden);
                var gelieerdePersoonIDs = gevonden as IList<int>;

                model.GelieerdePersonen =
                    ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<PersoonInfo>>(
                        svc => svc.PersoonInfoOphalen(gelieerdePersoonIDs));
                return View(model);
            }
        }

        /// <summary>
        /// Action voor postback van het uitstapinschrijvingsformulier.  Voert de gevraagde inschrijving uit.
        /// </summary>
        /// <param name="groepID">ID van momenteel actieve groep</param>
        /// <param name="model">De members <c>GelieerdePersoonIDs</c>, <c>GeselecteerdUitstap</c> en 
        /// <c>LogistiekDeelnemer</c> bevatten de informatie voor de inschrijving</param>
        /// <returns>Redirect naar de recentste lijst</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult InschrijvenVoorUitstap(int groepID, UitstapInschrijfModel model)
        {
            var uitstap = ServiceHelper.CallService<IUitstappenService, UitstapInfo>(
                svc => svc.Inschrijven(model.GelieerdePersoonIDs, model.GeselecteerdeUitstapID, model.LogistiekDeelnemer));

            TempData["succes"] = String.Format(Properties.Resources.DeelnemersToegevoegdFeedback, uitstap.Naam);

            return TerugNaarVorigeLijst();
        }
        #endregion
    }
}