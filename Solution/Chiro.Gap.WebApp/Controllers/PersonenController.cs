/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ExcelManip;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Validatie;
using Chiro.Gap.WebApp.ActionFilters;
using Chiro.Gap.WebApp.HtmlHelpers;
using Chiro.Gap.WebApp.Models;
using Chiro.Gap.WebApp.Properties;

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
        public PersonenController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper)
            : base(veelGebruikt, serviceHelper)
        {
        }
        // TODO er moeten ook nog een laatst gebruikte "actie" worden toegevoegd, niet alleen actie id

        #region personenlijst

        [HandleError]
        public override ActionResult Index(int groepID)
        {
            // redirect naar alle personen van de groep, pagina 1.
            return RedirectToAction("List", new
            {
                id = 0,
            });
        }


        /// <summary>
        /// Toont de persoonsinformatie (inclusief lidinfo) voor personen uit een bepaalde categorie, 
        /// en toont deze via de view 'Index'.
        /// </summary>
        /// <param name="groepID">Huidige groep waarin de gebruiker aan het werken is</param>
        /// <param name="id">ID van de gevraagde categorie.  Kan ook 0 zijn; dan worden alle personen
        /// geselecteerd.</param>
        /// <returns>De personenlijst in de view 'Index'</returns>
        /// <remarks>De attributen RouteValue en QueryStringValue laten toe dat we deze method overloaden.
        /// zie http://blog.abodit.com/2010/02/asp-net-mvc-ambiguous-match/ </remarks>
        [HandleError]
        [ParametersMatch]
        public ActionResult List([RouteValue]int groepID, [RouteValue]int id)
        {
            // Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten
            ClientState.VorigeLijst = Request.Url.ToString();

            int totaal = 0;

            var model = new PersoonInfoModel();
            BaseModelInit(model, groepID);
            model.GekozenCategorieID = id;

            model.GroepsCategorieen = ServiceHelper.CallService<IGroepenService, IList<CategorieInfo>>(svc => svc.CategorieenOphalen(groepID)).ToList();
            model.GroepsCategorieen.Add(new CategorieInfo
            {
                ID = 0,
                Naam = "Alle personen"
            });

            var categorieID = id;

            if (categorieID == 0)  // Alle personen bekijken
            {
                model.PersoonInfos = ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonDetail>>(g => g.DetailsOphalen(groepID));
                model.Titel = "Personenoverzicht";
                model.Totaal = totaal;
            }
            else	// Alleen personen uit de gekozen categorie bekijken
            {
                // TODO de catID is eigenlijk niet echt type-safe, maar wel het makkelijkste om te doen (lijkt te veel op PaginaOphalenLidInfo(groepid, ...))
                model.PersoonInfos =
                    ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonDetail>>
                    (g => g.OphalenUitCategorieMetLidInfo(categorieID, out totaal));

                // Ga in het lijstje met categorieën na welke er geselecteerd werd, zodat we de naam in de paginatitel kunnen zetten
                String naam = (from c in model.GroepsCategorieen
                               where c.ID == categorieID
                               select c).First().Naam;

                model.Titel = "Overzicht " + naam;
                model.Totaal = totaal;
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
                        //page = "A",
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
            IList<PersoonLidInfo> data;

            // Alle personen bekijken
            if (id == 0)
            {
                data =
                    ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonLidInfo>>
                    (g => g.AllenOphalenUitGroep(groepID)).AsQueryable().Sorteren(PersoonSorteringsEnum.Naam).ToList();
            }
            else
            {
                data =
                    ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonLidInfo>>
                    (g => g.AllenOphalenUitCategorie(id)).AsQueryable().Sorteren(PersoonSorteringsEnum.Naam).ToList();
            }

            var alleAfdelingen =
                ServiceHelper.CallService<IGroepenService, IList<AfdelingInfo>>(
                    svc => svc.AlleAfdelingenOphalen(groepID));

            const string bestandsNaam = "leden.xlsx";

            var pkg = GapExcelManip.LidExcelDocument(data, alleAfdelingen);

            return new ExcelResult(pkg, bestandsNaam);
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
                    r = Inschrijven(groepID, model.GekozenGelieerdePersoonIDs);
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

        #endregion

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
            var model = new NieuwePersoonModel();
            BaseModelInit(model, groepID);

            // zeken ophalen voor het model

            var groepsWerkJaar = VeelGebruikt.GroepsWerkJaarOphalen(groepID);
            model.GroepsWerkJaarID = groepsWerkJaar.WerkJaarID;
            model.AlleLanden = VeelGebruikt.LandenOphalen();
            model.TelefoonNummerType =
                ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieTypeInfo>(
                    svc => svc.CommunicatieTypeOphalen((int) CommunicatieTypeEnum.TelefoonNummer));
            model.EMailType =
                ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieTypeInfo>(
                    svc => svc.CommunicatieTypeOphalen((int)CommunicatieTypeEnum.Email));
            model.BeschikbareAfdelingen =
                ServiceHelper.CallService<IGroepenService, List<AfdelingDetail>>(svc => svc.ActieveAfdelingenOphalen(model.GroepsWerkJaarID));

            model.NieuwePersoon = new PersoonDetail();
            model.Land = Properties.Resources.Belgie;
            model.EMail = new CommunicatieInfo {CommunicatieTypeID = (int) CommunicatieTypeEnum.Email, Voorkeur = true};
            model.TelefoonNummer = new CommunicatieInfo
                                   {
                                       CommunicatieTypeID = (int) CommunicatieTypeEnum.TelefoonNummer,
                                       Voorkeur = true
                                   };
            model.BeschikbareWoonPlaatsen = new List<WoonPlaatsInfo>();
            model.Forceer = false;

            model.Titel = Properties.Resources.NieuwePersoonTitel;


            return View(model);
        }

        /// <summary>
        /// Gebruikt de ingevulde gegevens om een nieuwe persoon aan te maken, 
        /// wordt (indirect) opgeroepen vanuit jquery-persoons-fiche.js via bewaarGegevens!!
        /// </summary>
        /// <param name="model">Het ingevulde model</param>
        /// <param name="groepID">ID van de groep waaraan de nieuwe persoon gelieerd moet worden</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        [HandleError]
        public ActionResult Nieuw(NieuwePersoonModel model, int groepID)
        {
            bool gelukt;

            IDPersEnGP ids = null;

            BaseModelInit(model, groepID);
            model.Titel = Properties.Resources.NieuwePersoonTitel;

            if (!ModelState.IsValid)
            {
                gelukt = false;
            }
            else
            {
                var details = new NieuwePersoonDetails
                              {
                                  AfdelingsJaarIDs = model.AfdelingsJaarIDs,
                                  InschrijvenAls = model.InschrijvenAls,
                                  PersoonInfo = model.NieuwePersoon,
                                  AdresType = model.AdresType
                              };

                if (model.PostNr != 0 || !String.IsNullOrEmpty(model.WoonPlaatsBuitenLand))
                {
                    details.Adres = new AdresInfo
                                    {
                                        StraatNaamNaam = model.StraatNaamNaam,
                                        HuisNr = model.HuisNr,
                                        Bus = model.Bus,
                                        PostNr = model.PostNr,
                                        PostCode = model.PostCode,
                                        WoonPlaatsNaam =
                                            model.Land == Properties.Resources.Belgie
                                                ? model.WoonPlaatsNaam
                                                : model.WoonPlaatsBuitenLand,
                                        LandNaam = model.Land
                                    };
                }

                if (!String.IsNullOrEmpty(model.TelefoonNummer.Nummer))
                {
                    details.TelefoonNummer = model.TelefoonNummer;
                }

                if (!String.IsNullOrEmpty(model.EMail.Nummer))
                {
                    details.EMail = model.EMail;
                }

                try
                {
                    // (ivm forceer: 0: false, 1: true)
                    ids =
                        ServiceHelper.CallService<IGelieerdePersonenService, IDPersEnGP>(
                            l => l.Nieuw(details, groepID, model.Forceer));
                    gelukt = true;
                }
                catch (FaultException<BlokkerendeObjectenFault<PersoonDetail>> fault)
                {
                    model.GelijkaardigePersonen = fault.Detail.Objecten;
                    model.Forceer = true; // Probeer opnieuw; forceer.
                    gelukt = false;
                }
                catch (FaultException<OngeldigObjectFault> fault)
                {
                    var berichten = fault.Detail.Berichten;

                    new ModelStateWrapper(ModelState).BerichtenToevoegen(fault.Detail, String.Empty);
                    gelukt = false;
                }
            }

            if (!gelukt)
            {
                // Bouw model opnieuw op, en laat user opnieuw proberen.
                model.AlleLanden = VeelGebruikt.LandenOphalen();
                model.TelefoonNummerType =
                    ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieTypeInfo>(
                        svc => svc.CommunicatieTypeOphalen((int) CommunicatieTypeEnum.TelefoonNummer));
                model.EMailType =
                    ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieTypeInfo>(
                        svc => svc.CommunicatieTypeOphalen((int)CommunicatieTypeEnum.Email));
                model.BeschikbareAfdelingen =
                    ServiceHelper.CallService<IGroepenService, List<AfdelingDetail>>(
                        svc => svc.ActieveAfdelingenOphalen(model.GroepsWerkJaarID));
                model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.PostNr);
                return View(model);
            }

            // Voorlopig opnieuw redirecten naar Bewerken;
            // er zou wel gemeld moeten worden dat het wijzigen
            // gelukt is.
            // TODO Wat als er een fout optreedt bij PersoonBewaren?
            TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

            if (String.Compare(model.Button, "bewaren", StringComparison.OrdinalIgnoreCase) == 0)
            {
                Debug.Assert(ids != null);
                return RedirectToAction("Bewerken", new { id = ids.GelieerdePersoonID });
            }

            // bewaren en nog iemand toevoegen
            return RedirectToAction("Nieuw");
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
            var model = new NieuwePersoonModel();
            BaseModelInit(model, groepID);

            // Doe eerst iets gelijkaardigs als voor een nieuwe persoon

            model.NieuwePersoon = new PersoonDetail();
            model.GroepsWerkJaarID = VeelGebruikt.GroepsWerkJaarOphalen(groepID).WerkJaarID;
            model.Forceer = false;

            model.AlleLanden = VeelGebruikt.LandenOphalen();
            model.TelefoonNummerType =
                ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieTypeInfo>(
                    svc => svc.CommunicatieTypeOphalen((int)CommunicatieTypeEnum.TelefoonNummer));
            model.EMailType =
                ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieTypeInfo>(
                    svc => svc.CommunicatieTypeOphalen((int)CommunicatieTypeEnum.Email));
            model.BeschikbareAfdelingen =
                ServiceHelper.CallService<IGroepenService, List<AfdelingDetail>>(svc => svc.ActieveAfdelingenOphalen(model.GroepsWerkJaarID));
            
            // Neem een aantal gegevens over van origineel.

            var broerzus =
                ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<PersoonOverzicht>>(
                    l => l.OverzichtOphalen(new[] { gelieerdepersoonID })).FirstOrDefault();

            // Omdat we klonen, veronderstellen we dat het origineel bestaat en opgevraagd kan worden.
            Debug.Assert(broerzus != null);  

            model.NieuwePersoon.Naam = broerzus.Naam;

            model.StraatNaamNaam = broerzus.StraatNaam;
            model.HuisNr = broerzus.HuisNummer;
            model.PostNr = broerzus.PostNummer ?? 0;
            model.PostCode = broerzus.PostCode;
            model.Land = broerzus.Land;

            if (String.Compare(model.Land, Properties.Resources.Belgie, StringComparison.OrdinalIgnoreCase) == 0)
            {
                model.WoonPlaatsNaam = broerzus.WoonPlaats;
                model.BeschikbareWoonPlaatsen = VeelGebruikt.WoonPlaatsenOphalen(model.PostNr);
            }
            else
            {
                model.WoonPlaatsBuitenLand = broerzus.WoonPlaats;
                model.BeschikbareWoonPlaatsen = new List<WoonPlaatsInfo>();
            }

            model.TelefoonNummer = new CommunicatieInfo {Nummer = broerzus.TelefoonNummer};
            model.EMail = new CommunicatieInfo {Nummer = broerzus.Email};           

            model.Titel = Properties.Resources.NieuwePersoonTitel;
            return View("Nieuw", model);
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
        public ActionResult Kloon(NieuwePersoonModel model, int groepID)
        {
            return Nieuw(model, groepID);
        }
        
        /// <summary>
        /// Probeert de gewijzigde persoonsgegevens te persisteren via de webservice
        /// </summary>
        /// <param name="model"><c>PersoonsWijzigingModel</c> met gegevens gewijzigd door de gebruiker</param>
        /// <param name="groepID">GroepID van huidig geseecteerde groep</param>
        /// <returns>Redirect naar overzicht persoonsinfo indien alles ok, anders opnieuw de view
        /// 'Nieuw'.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        [HandleError]
        public ActionResult Bewerk(PersoonsWijzigingModel model, int groepID)
        {
            if (!ModelState.IsValid)
            {
                // Als er iets niet in orde is met de wijzigingen, vullen we de ontbrekende gegevens aan, en tonen we 
                // een view.
                // In praktijk zal die niet gebruikt worden, aangezien deze methode enkel wordt aangeroepen door JQuery.
                var persoonInfo = ServiceHelper.CallService<IGelieerdePersonenService, PersoonInfo>(
                    svc => svc.InfoOphalen(model.Wijziging.GelieerdePersoonID));

                if (model.Wijziging.ChiroLeefTijd == null)
                {
                    model.Wijziging.ChiroLeefTijd = persoonInfo.ChiroLeefTijd;
                }
                if (model.Wijziging.GeboorteDatum == null)
                {
                    model.Wijziging.GeboorteDatum = persoonInfo.GeboorteDatum;
                }
                if (model.Wijziging.Geslacht == null)
                {
                    model.Wijziging.Geslacht = persoonInfo.Geslacht;
                }
                if (model.Wijziging.Naam == null)
                {
                    model.Wijziging.Naam = persoonInfo.Naam;
                }
                if (model.Wijziging.SterfDatum == null)
                {
                    model.Wijziging.SterfDatum = persoonInfo.SterfDatum;
                }
                if (model.Wijziging.VoorNaam == null)
                {
                    model.Wijziging.VoorNaam = persoonInfo.VoorNaam;
                }

                return View("Nieuw", model);
            }

            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.Wijzigen(model.Wijziging));

            // Voorlopig opnieuw redirecten naar Bewerken;
            // er zou wel gemeld moeten worden dat het wijzigen
            // gelukt is.
            // TODO Wat als er een fout optreedt bij PersoonBewaren?
            TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

            // (er wordt hier geredirect ipv de view te tonen,
            // zodat je bij een 'refresh' niet de vraag krijgt
            // of je de gegevens opnieuw wil posten.)
            return RedirectToAction("Bewerken", new { id = model.Wijziging.GelieerdePersoonID, groepID });
        }

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
        public ActionResult Bewerken(int id, int groepID)
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
                                                 Settings.Default.LoonVerliesVanafLeeftijd;
                model.PrijsVerzekeringLoonVerlies = Settings.Default.PrijsVerzekeringLoonVerlies;
                model.Titel = model.PersoonLidInfo.PersoonDetail.VolledigeNaam;

                return View("Bewerken", model);
            }
            else
            {
                model.Titel = model.PersoonLidInfo.PersoonDetail.VolledigeNaam + " (†)";
                return View("OverledenPersoon", model);
            }
        }

        /// <summary>
        /// Bekijkt model.Persoon.  Haalt alle afdelingen van het groepswerkjaar van het lid op, en
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
        /// Genereert een voorstel om de gelieerde personen met GelieerdePersoonIDs uit TempData["gpids"]
        /// in te schrijven voor het huidige werkjaar.
        /// 
        /// Deze actie wordt op dit moment enkel gebruikt om naar te redirecten als er ingeschreven wordt
        /// vanuit een ledenoverzicht van een ouder werkjaar.
        /// </summary>
        /// <param name="groepID">actieve groep</param>
        /// <returns>Voorstel om de gelieerde personen met GelieerdePersoonIDs uit TempData["gpids"]
        /// in te schrijven voor het huidige werkjaar.</returns>
        public ActionResult InschrijvenTempData(int groepID)
        {
            return Inschrijven(groepID, TempData["gpids"] as List<int>);
        }


        /// <summary>
        /// Genereert een voorstel om de gelieerde personen met gegeven
        /// <paramref name="gelieerdePersoonIDs"/> in te schrijven voor de groep met
        /// gegeven <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">ID van groep waarvoor in te schrijven.</param>
        /// <param name="gelieerdePersoonIDs">ID's van in te schrijven gelieerde personen.</param>
        /// <returns>Een inschrijvingsvoorstel.</returns>
        /// <remarks>Het voorstel bevat een 'submit button' die expliciet verwijst naar
        /// Personen/Inschrijven. Deze method kan aangeroepen worden vanuit eender welke
        /// andere method; de postback komt altijd terecht.
        /// 
        /// Rechtstreeks aanroepen via de URL is lastiger, o.w.v. de collectie ID's.</remarks>
        public ActionResult Inschrijven(int groepID, List<int> gelieerdePersoonIDs)
        {
            var model = new InschrijvingsModel {GelieerdePersoonIDs = gelieerdePersoonIDs};
            return Inschrijven(groepID, model);
        }


        /// <summary>
        /// Inschrijven van een selectie gelieerde personen. Als model.Inschrijvingen leeg is, wordt
        /// een voorstel gedaan om de gelieerde personen met gelieerdePersoonID in
        /// model.GelieerdePersoonIDs in te schrijven. Als model.Inschrijvingen niet leeg is, wordt
        /// de inschrijving doorgevoerd op basis van de informatie in model.Inschrijvingen.
        /// Er wordt ingeschreven voor het huidige werkjaar van de groep
        /// met gegeven <paramref name="groepID"/>.
        /// </summary>
        /// <param name="groepID">Groep waarvoor personen ingeschreven moeten worden.</param>
        /// <param name="model">Bevat informatie om een inschrijvingsvoorstel te genereren of een 
        /// inschrijving effectief uit te voeren.</param>
        /// <returns>Inschrijvingsvoorstel of ledenlijst na effectieve inschrijving.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Inschrijven(int groepID, InschrijvingsModel model)
        {
            List<InschrijvingsVoorstel> personenOverzicht = null;

            BaseModelInit(model, groepID);           

            if (model.Inschrijvingen != null )
            {
                // Inschrijvingen gegeven: schrijf in.

                var inTeSchrijven = (from rij in model.Inschrijvingen
                    where rij.InTeSchrijven
                    select new InschrijvingsVerzoek
                    {
                        AfdelingsJaarIDs = rij.AfdelingsJaarIDs,
                        AfdelingsJaarIrrelevant = false,
                        GelieerdePersoonID = rij.GelieerdePersoonID,
                        LeidingMaken = rij.LeidingMaken
                    }).ToArray();

                // De user interface is zodanig gemaakt dat AfdelingsJaarIDs precies 1 ID bevat,
                // dat 0 kan zijn als het over 'geen' gaat. Dat is niet zoals het moet zijn, maar
                // wel de praktijk.
                // De service verwacht een lijst met AfdelingsJaarIDs. Ik ga dus alle lijsten die 
                // enkel 0 bevatten, vervangen door lege lijsten.

                foreach (
                    var ins in
                        inTeSchrijven.Where(
                            i =>
                                i.AfdelingsJaarIDs == null ||
                                (i.AfdelingsJaarIDs.Count() == 1 && i.AfdelingsJaarIDs.First() == 0)))
                {
                    ins.AfdelingsJaarIDs = new int[0]; // lege array van ints.
                }

                personenOverzicht = ServiceHelper.CallService<ILedenService, List<InschrijvingsVoorstel>>(
                    svc => svc.Inschrijven(inTeSchrijven));

                if (!personenOverzicht.Any())
                {
                    // Als er geen fouten optraden, redirecten we naar de ledenlijst.
                    if (inTeSchrijven.Count() == 1)
                    {
                        return RedirectToAction("Bewerken", "Personen", new { id = inTeSchrijven.First().GelieerdePersoonID });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Leden");
                    }
                }

                // In het andere geval tonen we nog de probleemgevallen.

                model.Titel = Properties.Resources.MultiInschrijvenMisluktFout;
            }
            else
            {
                model.Titel = Properties.Resources.PersonenInschrijven;

                // Er werd niet gevraagd om in te schrijven. Genereer een inschrijvingsvoorstel.

                personenOverzicht = ServiceHelper.CallService<ILedenService, IEnumerable<InschrijvingsVoorstel>>(
                    svc => svc.InschrijvingVoorstellen(model.GelieerdePersoonIDs)).ToList();
            }

            // Toon ofwel inschrijvingsvoorstel, ofwel lijst met mislukte inschrijvingen.

            model.Inschrijvingen = (from p in personenOverzicht
                select new InschrijfbaarLid
                {
                    AfdelingsJaarIDs = p.AfdelingsJaarIDs,
                    AfdelingsJaarIrrelevant = false,
                    GelieerdePersoonID = p.GelieerdePersoonID,
                    InTeSchrijven = p.FoutNummer == null,
                    LeidingMaken = p.LeidingMaken,
                    VolledigeNaam = p.VolledigeNaam,
                    FoutBoodschap = FeedbackLidMaken(p),
                    FoutNummer = p.FoutNummer,
                }).ToList();

            // Als er gegevens ontbraken, geven we extra informatie mee.
            // FIXME: dit is tamelijk hacky.

            var ontbrekendeGegevensFouten = new List<FoutNummer?>
            {
                FoutNummer.GeboorteDatumOntbreekt,
                FoutNummer.OnbekendGeslacht,
                FoutNummer.AdresOntbreekt,
                FoutNummer.TelefoonNummerOntbreekt,
                FoutNummer.EMailVerplicht
            };

            if ((from i in model.Inschrijvingen
                where ontbrekendeGegevensFouten.Contains(i.FoutNummer)
                select i.FoutNummer).Any())
            {
                model.ExtraUitleg = Properties.Resources.WaaromWeLidgegevensVragen;
            }

            model.BeschikbareAfdelingen =
                ServiceHelper.CallService<IGroepenService, List<ActieveAfdelingInfo>>(
                    svc => svc.HuidigeAfdelingsJarenOphalen(groepID));

            return View("Inschrijven", model);
        }

        private string FeedbackLidMaken(InschrijvingsVoorstel p)
        {
            switch (p.FoutNummer)
            {
                case null:
                    return String.Empty;
                case FoutNummer.AfdelingKindVerplicht:
                    return Properties.Resources.AfdelingKindVerplicht;
                case FoutNummer.AfdelingNietBeschikbaar:
                    return Properties.Resources.AfdelingNietBeschikbaar;
                case FoutNummer.GeboorteDatumOntbreekt:
                    return Properties.Resources.GeboorteDatumOntbreekt;
                case FoutNummer.LidTeJong:
                    return Properties.Resources.MinimumLeeftijd;
                case FoutNummer.PersoonOverleden:
                    return Properties.Resources.PersoonOverleden;
                case FoutNummer.OnbekendGeslacht:
                    return Properties.Resources.OnbekendGeslacht;
                case FoutNummer.LidWasAlIngeschreven:
                    return Properties.Resources.LidWasAlIngeschreven;
                case FoutNummer.GroepInactief:
                    return Properties.Resources.GroepInactief;
                case FoutNummer.AdresOntbreekt:
                    return Properties.Resources.LidAdresOntbreekt;
                case FoutNummer.TelefoonNummerOntbreekt:
                    return Properties.Resources.TelefoonNummerOntbreekt;
                case FoutNummer.EMailVerplicht:
                    return Properties.Resources.EmailVoorLeidingOntbreekt;
                default:
                    return Properties.Resources.OnverwachteFout;
            }
        }

        /// <summary>
        /// Doet een voorsel om alle leden uit groepswerkjaar met ID <paramref name="groepsWerkJaarID"/>
        /// opnieuw in te schrijven voor het recentste groepswerkjaar.
        /// </summary>
        /// <param name="groepID">ID van groep waarvoor we willen inschrijven</param>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar waarvan we de leden willen herinschrijven.</param>
        /// <returns>Voorstel voor de inschrijving</returns>
        public ActionResult Herinschrijven(int groepID, int groepsWerkJaarID)
        {
            // We hebben de gelieerdepersoonID's nodig van alle leden uit het gegeven (oude) werkjaar.
            // We doen dat hieronder door alle leden op te halen, en daaruit de gelieerdepersoonID's te 
            // halen.
            // TODO: Dit is een tamelijk dure operatie, omdat we gewoon de ID's nodig hebben. 
            // Bovendien halen we in een volgende stap opnieuw de persoonsinfo op.
            // Best eens herwerken dus.

            var filter = new LidFilter
            {
                GroepsWerkJaarID = groepsWerkJaarID,
                AfdelingID = null,
                FunctieID = null,
                ProbeerPeriodeNa = null,
                HeeftVoorkeurAdres = null,
                HeeftTelefoonNummer = null,
                HeeftEmailAdres = null,
                LidType = LidType.Alles
            };
            List<int> gelieerdepersoonIDs;

            try
            {
                gelieerdepersoonIDs =
                    ServiceHelper.CallService<ILedenService, IList<LidOverzicht>>(svc => svc.LijstZoekenLidOverzicht(filter, false)).Select(
                    e => e.GelieerdePersoonID).ToList();
            }
            catch (FaultException<FoutNummerFault>)
            {
                TempData["fout"] = Properties.Resources.VoorstelNieuweLedenMislukt;
                return RedirectToAction("Index", "Leden");
            }

            var inschrijvingsModel = new InschrijvingsModel()
            {
                GelieerdePersoonIDs = gelieerdepersoonIDs,
            };
            return Inschrijven(groepID, inschrijvingsModel);
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
            return RedirectToAction("Bewerken", new { ID = gelieerdepersoonID, groepID });
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
        /// <returns>De view 'Bewerken' indien OK, anders opnieuw de view 'AdresBewerken'.</returns>
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

                return RedirectToAction("Bewerken", "Personen", new { id = model.GelieerdePersoonIDs.First() });
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

            return RedirectToAction("Bewerken", new { id = model.AanvragerID });
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
            if (model.WoonPlaatsBuitenLand != null)
            {
                model.WoonPlaatsNaam = model.WoonPlaatsBuitenLand;
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

                return RedirectToAction("Bewerken", new { id = model.AanvragerID });
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

            return RedirectToAction("Bewerken", new { id = gelieerdePersoonID });
        }

        #endregion adressen

        #region communicatievormen

        /// <summary>
        /// Redirect naar het wijzigen van eigen e-mailadres.
        /// </summary>
        /// <returns>Een redirect naar het wijzigen van eigen e-mailadres</returns>
        public ActionResult MijnEmailInstellen(int groepID)
        {
            // Dit is tamelijk omslachtig, maar ik wil op dit moment niet veel meer
            // wijzigen aan de oude backend.

            var gavs =
                ServiceHelper.CallService<IGroepenService, IEnumerable<GebruikersDetail>>(
                    svc => svc.GebruikersOphalen(groepID));


            string mijnUser = System.Web.HttpContext.Current.User.Identity.Name;

            var mijnGav = (from gav in gavs
                           where String.Compare(gav.Login, mijnUser, StringComparison.OrdinalIgnoreCase) == 0
                           select gav).First();

            return RedirectToAction("NieuweCommVorm", new { groepID, gelieerdePersoonID = mijnGav.GelieerdePersoonID });
        }


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
            model.Titel = "Tel./mail/enz. toevoegen";
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
                model.Titel = "Tel./mail/enz. toevoegen";

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

                return RedirectToAction("Bewerken", new { id = gelieerdePersoonID });
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
            var gelieerdePersoonID = ServiceHelper.CallService<IGelieerdePersonenService, int>(l => l.CommunicatieVormVerwijderen(commvormID));

            VeelGebruikt.LedenProblemenResetten(groepID);

            return RedirectToAction("Bewerken", new { id = gelieerdePersoonID });
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
            model.Titel = "Tel./mail/enz. bewerken";
            return View("CommVormBewerken", model);
        } 

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
       // [ValidateInput(false)]
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
                model.Titel = "Tel./mail/enz. bewerken";

                return View("CommVormBewerken", model);
            }

            // Om bloat over de lijn te vermijden: downgraden naar minimale info

            var commInfo = new CommunicatieInfo();
            Mapper.CreateMap<CommunicatieDetail, CommunicatieInfo>();
            Mapper.Map(model.NieuweCommVorm, commInfo);

            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CommunicatieVormAanpassen(commInfo));
            return RedirectToAction("Bewerken", new { id = gelieerdePersoonID });
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
            ServiceHelper.CallService<IGelieerdePersonenService>(l => l.UitCategorieVerwijderen(list, categorieID));
            return RedirectToAction("Bewerken", new { id = gelieerdePersoonID });
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

            if (model.Categorieen.Any())
            {
                object value;
                TempData.TryGetValue("list", out value);
                model.GelieerdePersoonIDs = (List<int>)value;
                var persoonsnamen = ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<PersoonInfo>>(l => l.InfosOphalen(model.GelieerdePersoonIDs));
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
                return RedirectToAction("Bewerken", "Personen", new { id = model.GelieerdePersoonIDs.First(), groepID });
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

                // Dat gat hier via die TempData. Brr.

                TempData.TryGetValue("ids", out gevonden);
                var gelieerdePersoonIDs = gevonden as IList<int>;

                model.GelieerdePersonen =
                    ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<PersoonInfo>>(
                        svc => svc.InfosOphalen(gelieerdePersoonIDs));
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
                svc =>
                svc.Inschrijven(model.GelieerdePersoonIDs, model.GeselecteerdeUitstapID, model.LogistiekDeelnemer));

            string uitstapLink = HtmlHelper.GenerateLink(ControllerContext.RequestContext,
                                                         RouteTable.Routes, uitstap.Naam, null,
                                                         "Bekijken", "Uitstappen", new RouteValueDictionary(new {groepID, uitstap.ID}), null);

            TempData["succes"] = String.Format(Properties.Resources.DeelnemersToegevoegdFeedback, uitstapLink);

            return TerugNaarVorigeLijst();
        }
        #endregion


        #region Enkel nodig voor javascriptcalls van de client

        /// <summary>
        /// Haalt persoonsgegevens op in JSON-formaat
        /// </summary>
        /// <param name="groepID">ID van de groep wiens personen opgehaald moeten worden</param>
        /// <param name="pageSize">Paginagrootte</param>
        /// <param name="page">Op te halen pagina</param>
        /// <returns>Persoonsgegevens in JSON-formaat</returns>
        public JsonResult PersonenJson(int groepID, int pageSize, int page)
        {
            var resultaat = ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonDetail>>(g => g.PaginaOphalen(groepID, pageSize, page));
            return Json(resultaat, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Verandert het nummer van de communicatievorm met gegeven ID door de gegeven string.
        /// </summary>
        /// <param name="groepID">ID van de groep waarin we werken</param>
        /// <param name="wijziging">bevat ID van de te wijzigen communicatievorm, en nieuw nummer</param>
        /// <returns>Leeg JsonResult</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult NummerWijzigen(int groepID, IntStringModel wijziging)
        {
            ServiceHelper.CallService<IGelieerdePersonenService>(
                svc => svc.NummerCommunicatieVormWijzigen(wijziging.ID, wijziging.Waarde));
            return Json(null);
        }

        /// <summary>
        /// Schrijft een communicatievorm in of uit voor snelleberichtenlijst.
        /// </summary>
        /// <param name="groepID">Groep waarin we werken. Speelt eigenlijk geen rol.</param>
        /// <param name="wijziging">Bevat communciatieVormID, en een string. Als de string leeg is,
        /// wordt uitgeschreven, anders wordt ingeschreven.</param>
        /// <returns>Voorlopig leeg JsonResult. Zou beter een foutcode worden.</returns>
        public JsonResult SnelleBerichtenInschrijven(int groepID, IntStringModel wijziging)
        {
            ServiceHelper.CallService<IGelieerdePersonenService>(
                svc => svc.SnelleBerichtenInschrijven(wijziging.ID, !String.IsNullOrEmpty(wijziging.Waarde)));
            return Json(null);
        }

        #endregion

    }
}