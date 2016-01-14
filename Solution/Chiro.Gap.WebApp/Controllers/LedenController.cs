/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ExcelManip;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.ActionFilters;
using Chiro.Gap.WebApp.Models;
using Chiro.Gap.WebApp.Properties;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller voor weergave en beheer van ledenlijst
    /// </summary>
    [HandleError]
    public class LedenController : PersonenEnLedenController
    {
        /// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        public LedenController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper) : base(veelGebruikt, serviceHelper) { }

        /// <summary>
        /// Sorteert een rij records van type LidOverzicht
        /// </summary>
        /// <param name="rij">Te sorteren rij</param>
        /// <param name="sortering">Lideigenschap waarop gesorteerd moet worden</param>
        /// <returns>Gesorteerde rij</returns>
        private IEnumerable<LidOverzicht> Sorteren(IEnumerable<LidOverzicht> rij, LidEigenschap sortering)
        {
            // In de vorige revisie was het zo dat steeds eerst de leiding getoond werd, en dan de leden.
            // Ik ben er niet zeker van of dat een bug of een feature is.  Ik implementeer het alleszins
            // opnieuw op deze manier.

            IEnumerable<LidOverzicht> gesorteerd;

            switch (sortering)
            {
                case LidEigenschap.Naam:
                    gesorteerd = rij
                        .OrderByDescending(src => src.Type)
                        .ThenBy(src => src.Naam)
                        .ThenBy(src => src.VoorNaam);
                    break;
                case LidEigenschap.Leeftijd:
                    gesorteerd = rij
                        .OrderByDescending(src => src.Type)
                        .ThenByDescending(src => src.GeboorteDatum)
                        .ThenBy(src => src.Naam)
                        .ThenBy(src => src.VoorNaam);
                    break;
                case LidEigenschap.Afdeling:
                    gesorteerd = rij
                        .OrderByDescending(src => src.Type)
                        .ThenByDescending(src => src.Afdelingen.Any() ? src.Afdelingen.First().GeboorteJaarVan : DateTime.Now.Year)
                        .ThenBy(src => src.Naam)
                        .ThenBy(src => src.VoorNaam);
                    break;
                case LidEigenschap.InstapPeriode:
                    gesorteerd = rij
                        .OrderBy(src => src.EindeInstapPeriode == null) // eerst die met instapperiode
                        .ThenBy(src => src.EindeInstapPeriode)
                        .ThenBy(src => src.Naam)
                        .ThenBy(src => src.VoorNaam);
                    break;
                case LidEigenschap.Adres:
                    gesorteerd = rij
                        .OrderBy(src => src.PostNummer)
                        .ThenBy(src => src.StraatNaam)
                        .ThenBy(src => src.HuisNummer)
                        .ThenBy(src => src.Bus)
                        .ThenBy(src => src.Naam)
                        .ThenBy(src => src.VoorNaam);
                    break;
                case LidEigenschap.Verjaardag:
                    gesorteerd = rij
                        .OrderBy(src => src.GeboorteDatum.HasValue ? src.GeboorteDatum.Value.Month : 0)
                        .ThenBy(src => src.GeboorteDatum.HasValue ? src.GeboorteDatum.Value.Day : 0)
                        .ThenBy(src => src.Naam)
                        .ThenBy(src => src.VoorNaam);
                    break;

                default:
                    gesorteerd = rij;
                    break;
            }
            return gesorteerd;
        }

        /// <summary>
        /// Voert een zoekopdracht uit op de leden, en plaatst het resultaat in een LidInfoModel.
        /// Levert normaal geen adressen op.
        /// </summary>
        /// <param name="gwjID">ID van het groepswerkjaar waarvoor een ledenlijst wordt gevraagd.  
        ///  Indien 0, het recentste groepswerkjaar van de groep met ID <paramref name="groepID"/>.</param>
        /// <param name="groepID">Groep waaruit de leden opgehaald moeten worden.</param>
        /// <param name="sortering">Bepaalt op welke eigenschap de lijst gesorteerd moet worden</param>
        /// <param name="afdelingID">Als verschillend van 0, worden enkel de leden uit de afdeling met
        ///  dit AfdelingID getoond.</param>
        /// <param name="functieID">Als verschillend van 0, worden enkel de leden met de functie met
        ///  dit FunctieID getoond.</param>
        /// <param name="ledenLijst">Hiermee kan je speciale lijsten opzoeken</param>
        /// <returns>Een LidInfoModel, met in member LidInfoLijst de gevonden leden. Null als de groep en groepswerkjaar niet geassocieerd zijn</returns>
        private LidInfoModel Zoeken(
            int gwjID,
            int groepID,
            LidEigenschap sortering,
            int afdelingID,
            int functieID,
            LidInfoModel.SpecialeLedenLijst ledenLijst)
        {
            // Het sorteren gebeurt nu in de webapp, en niet in de backend.  Sorteren is immers presentatie, dus de service
            // moet zich daar niet mee bezig houden.
            //
            // Voor de personenlijst ligt dat anders.  Als je daar een pagina opvraagt, dan is die
            // pagina afhankelijk van de gekozen sortering.  Daarom moet het sorteren voor personen
            // wel door de service gebeuren.  (Maar hier, bij de leden, is dat niet van toepassing.)

            // Als geen groepswerkjaar gegeven is: haal recentste op 
            var groepsWerkJaarID = gwjID == 0 ? ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID)) : gwjID;

            var model = LijstModelInitialiseren(groepsWerkJaarID, groepID, sortering);

            if (model == null) // Groep en gwj matchen niet
            {
                TempData["fout"] = Properties.Resources.GroepNietBijGroepsWerKjaar;
                return null;
            }

            model.KanLedenBewerken = groepsWerkJaarID == (from wj in model.WerkJaarInfos
                                                          orderby wj.WerkJaar descending
                                                          select wj.ID).FirstOrDefault();

            // Bewaar gekozen filters in model, zodat de juiste items in de dropdownlijsten geselecteerd worden

            model.AfdelingID = afdelingID;
            model.FunctieID = functieID;
            model.SpecialeLijst = ledenLijst;

            // Bouw het lijstje met speciale lijsten op
            if ((model.GroepsNiveau & (Niveau.Gewest | Niveau.Verbond)) == 0)
            {
                model.SpecialeLijsten.Add(
                    LidInfoModel.SpecialeLedenLijst.Probeerleden,
                    Properties.Resources.LijstProbeerLeden);
            }
            model.SpecialeLijsten.Add(
                LidInfoModel.SpecialeLedenLijst.VerjaardagsLijst,
                Properties.Resources.LijstVerjaardagen);
            model.SpecialeLijsten.Add(
                LidInfoModel.SpecialeLedenLijst.OntbrekendAdres,
                Properties.Resources.LijstOntbrekendAdres);
            model.SpecialeLijsten.Add(
                LidInfoModel.SpecialeLedenLijst.OntbrekendTelefoonNummer,
                Properties.Resources.LijstOntbrekendTelefoonNummer);
            model.SpecialeLijsten.Add(
                LidInfoModel.SpecialeLedenLijst.LeidingZonderEmail,
                Properties.Resources.LijstLeidingZonderEmail);
            model.SpecialeLijsten.Add(
                LidInfoModel.SpecialeLedenLijst.Alles,
                Properties.Resources.LijstAlles);

            // Haal de op te lijsten leden op; de filter wordt bepaald uit de method parameters.
            model.LidInfoLijst = ServiceHelper.CallService<ILedenService, IList<LidOverzicht>>(
                svc => svc.LijstZoekenLidOverzicht(
                    FilterMaken(afdelingID, functieID, ledenLijst, groepsWerkJaarID),
                    false)); // false om geen adressen op te halen.

            if (functieID != 0)
            {
                // Naam van de functie opzoeken, zodat we ze kunnen invullen in de paginatitel
                var functieNaam = (from fi in model.FunctieInfoDictionary
                                   where fi.Key == functieID
                                   select fi).First().Value.Naam;

                model.Titel = String.Format(Properties.Resources.AfdelingsLijstTitel,
                                functieNaam,
                                model.JaartalGetoondGroepsWerkJaar,
                                model.JaartalGetoondGroepsWerkJaar + 1);
            }
            else if (afdelingID != 0)
            {
                var af = (from a in model.AfdelingsInfoDictionary.AsQueryable()
                          where a.Value.AfdelingID == afdelingID
                          select a.Value).FirstOrDefault();

                if (af == null)
                {
                    model.Titel = String.Format(
                        Properties.Resources.AfdelingBestondNiet,
                        model.JaartalGetoondGroepsWerkJaar,
                        model.JaartalGetoondGroepsWerkJaar + 1);
                }
                else
                {
                    model.Titel = String.Format(Properties.Resources.AfdelingsLijstTitel,
                                    af.AfdelingNaam,
                                    model.JaartalGetoondGroepsWerkJaar,
                                    model.JaartalGetoondGroepsWerkJaar + 1);
                }
            }
            else
            {
                model.Titel = String.Format(Properties.Resources.LedenOverzicht,
                                model.JaartalGetoondGroepsWerkJaar,
                                model.JaartalGetoondGroepsWerkJaar + 1);
            }
            model.LidInfoLijst = Sorteren(model.LidInfoLijst, sortering).ToList();
            return model;
        }

        private static LidFilter FilterMaken(int afdelingID, int functieID, LidInfoModel.SpecialeLedenLijst ledenLijst, int groepsWerkJaarID)
        {
            return new LidFilter
                   {
                       GroepsWerkJaarID = groepsWerkJaarID,
                       AfdelingID = (afdelingID == 0) ? null : (int?)afdelingID,
                       FunctieID = (functieID == 0) ? null : (int?)functieID,
                       ProbeerPeriodeNa =
                           (ledenLijst == LidInfoModel.SpecialeLedenLijst.Probeerleden) ? (DateTime?)DateTime.Today : null,
                       HeeftVoorkeurAdres = (ledenLijst == LidInfoModel.SpecialeLedenLijst.OntbrekendAdres) ? (bool?)false : null,
                       HeeftTelefoonNummer =
                           (ledenLijst == LidInfoModel.SpecialeLedenLijst.OntbrekendTelefoonNummer) ? (bool?)false : null,
                       HeeftEmailAdres = (ledenLijst == LidInfoModel.SpecialeLedenLijst.LeidingZonderEmail) ? (bool?)false : null,
                       LidType = (ledenLijst == LidInfoModel.SpecialeLedenLijst.LeidingZonderEmail) ? LidType.Leiding : LidType.Alles
                   };
        }

        /// <summary>
        /// Toont de ledenlijst van de gegeven groep in het huidige werkJaar
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan we de ledenlijst willen tonen</param>
        /// <returns></returns>
        /// <!-- GET: /Leden/ -->
        [HandleError]
        public override ActionResult Index(int groepID)
        {
            // Recentste groepswerkjaar ophalen, en leden tonen.
            return Lijst(ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID)), 0, 0, LidInfoModel.SpecialeLedenLijst.Geen, LidEigenschap.Naam, groepID);
        }

        /// <summary>
        /// Maakt een 'leeg' model voor een lijst met leden uit een gegeven groepswerkjaar.  Dat wil zeggen: ophalen van beschikbare
        /// groepswerkjaren, afdelingen, en functies, en vastleggen van geselecteerd groepswerkjaar.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het gevraagde groepswerkjaar</param>
        /// <param name="groepID">ID van de groep waar het over gaat</param>
        /// <param name="sortering">Enumwaarde die aangeeft op welke parameter de lijst gesorteerd moet worden (enkel nog relevant voor de verjaardagslijst</param>
        /// <returns>NULL als het groep en groepswerkjaar ID niet bij dezelfde groep horen</returns>
        [HandleError]
        private LidInfoModel LijstModelInitialiseren(int groepsWerkJaarID, int groepID, LidEigenschap sortering)
        {
            // TODO (#1033): deze code moet opgekuist worden

            var model = new LidInfoModel();
            BaseModelInit(model, groepID);

            // Laad de lijst van werkjaren in van de groep en zet de juiste info over het te tonen werkJaar
            model.WerkJaarInfos = ServiceHelper.CallService<IGroepenService, IEnumerable<WerkJaarInfo>>(e => e.WerkJarenOphalen(groepID));

            var gevraagdwerkjaar = (from g in model.WerkJaarInfos
                                    where g.ID == groepsWerkJaarID
                                    select g).FirstOrDefault();

            if (gevraagdwerkjaar == null) // Het is geen groepswerkjaar van de gegeven groep
            {
                return null;
            }

            model.IDGetoondGroepsWerkJaar = groepsWerkJaarID;
            model.JaartalGetoondGroepsWerkJaar = gevraagdwerkjaar.WerkJaar;

            // TODO (#1033): Hetgeen hieronder opgevraagd wordt, zit volgens mij al in de reeds opgehaalde model.WerkJaarInfos.  Na te kijken.
            model.JaartalHuidigGroepsWerkJaar = ServiceHelper.CallService<IGroepenService, GroepsWerkJaarDetail>(e => e.RecentsteGroepsWerkJaarOphalen(groepID)).WerkJaar;

            // Haal alle afdelingsjaren op van de groep in het groepswerkjaar
            var list = ServiceHelper.CallService<IGroepenService, IList<AfdelingDetail>>(groep => groep.ActieveAfdelingenOphalen(groepsWerkJaarID));

            // Laad de afdelingen in het model in via een dictionary
            model.AfdelingsInfoDictionary = new Dictionary<int, AfdelingDetail>();
            foreach (var ai in list)
            {
                model.AfdelingsInfoDictionary.Add(ai.AfdelingID, ai);
            }

            // Haal alle functies op van de groep in het groepswerkjaar

            // *****************************************************
            // ** OPGELET! Als je debugger hieronder crasht, dan  **
            // ** zit er waarschijnlijk een functie met ongeldig  **
            // ** lidtype in de functietabel!                     **
            // *****************************************************
            var list2 = ServiceHelper.CallService<IGroepenService, IEnumerable<FunctieDetail>>(groep => groep.FunctiesOphalen(groepsWerkJaarID, LidType.Alles));

            model.FunctieInfoDictionary = new Dictionary<int, FunctieDetail>();
            foreach (var fi in list2)
            {
                model.FunctieInfoDictionary.Add(fi.ID, fi);
            }

            model.PageHuidig = model.IDGetoondGroepsWerkJaar;
            model.PageTotaal = model.WerkJaarInfos.Count();

            model.GekozenSortering = sortering;

            return model;
        }

        /// <summary>
        /// Toont een gesorteerde ledenlijst
        /// </summary>
        /// <param name="id">ID van het groepswerkjaar waarvoor een ledenlijst wordt gevraagd.  
        ///  Indien 0, het recentste groepswerkjaar van de groep met ID <paramref name="groepID"/>.</param>
        /// <param name="afdelingID">Als verschillend van 0, worden enkel de leden uit de afdeling met
        ///  dit AfdelingID getoond.</param>
        /// <param name="functieID">Als verschillend van 0, worden enkel de leden met de functie met
        ///  dit FunctieID getoond.</param>
        /// <param name="ledenLijst">Het soort lijst dat we moeten tonen</param>
        /// <param name="sortering">Bepaalt op welke eigenschap de lijst gesorteerd moet worden</param>
        /// <param name="groepID">Groep waaruit de leden opgehaald moeten worden.</param>
        /// <returns>Een view met de gevraagde ledenlijst</returns>
        /// <remarks>De attributen RouteValue en QueryStringValue laten toe dat we deze method overloaden.
        /// zie http://blog.abodit.com/2010/02/asp-net-mvc-ambiguous-match/ </remarks>
        [ParametersMatch]
        public ActionResult Lijst(
            [RouteValue]int id,
            [QueryStringValue]int afdelingID,
            [QueryStringValue]int functieID,
            [QueryStringValue]LidInfoModel.SpecialeLedenLijst ledenLijst,
            [QueryStringValue]LidEigenschap sortering,
            [RouteValue]int groepID)
        {
            Debug.Assert(Request.Url != null);
            // Er wordt een nieuwe lijst opgevraagd, dus wordt deze vanaf hier bijgehouden als de lijst om terug naar te springen
            ClientState.VorigeLijst = Request.Url.ToString();
            var model = Zoeken(id, groepID, sortering, afdelingID, functieID, ledenLijst);
            return model == null ? TerugNaarVorigeLijst() : View("Index", model);
        }

        /// <summary>
        /// Afhandelen van postback ledenlijst.  Ofwel is een actie gekozen op de selectie, ofwel
        /// moet de lijst gefilterd worden.
        /// </summary>
        /// <param name="id">ID van het groepswerkjaar.  Als dit 0 is, wordt het recentste groepswerkjaar gekozen</param>
        /// <param name="groepID">ID van de groep.  Enkel nodig als geen groepswerkjaar gegeven is, maar sowieso
        /// beschikbaar via URL.</param>
        /// <param name="model">LidInfoModel, waaruit de informatie over de gewenste actie/filter opgehaald moet 
        /// worden.</param>
        /// <returns>Deze method zal voornamelijk redirecten
        /// </returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lijst(int id, int groepID, LidInfoModel model)
        {
            if (model.GekozenActie > 0)
            {
                return ToepassenOpSelectie(model, groepID);
            }
            switch (model.SpecialeLijst)
            {
                case LidInfoModel.SpecialeLedenLijst.Alles:
                    return RedirectToAction("Lijst", new { id, groepID });
                case LidInfoModel.SpecialeLedenLijst.VerjaardagsLijst:
                    return RedirectToAction("Lijst",
                                new
                                {
                                    id,
                                    afdelingID = model.AfdelingID,
                                    functieID = model.FunctieID,
                                    ledenLijst = model.SpecialeLijst,
                                    sortering = LidEigenschap.Verjaardag,
                                    groepID
                                });
                default:
                    return RedirectToAction("Lijst",
                                new
                                {
                                    id,
                                    afdelingID = model.AfdelingID,
                                    functieID = model.FunctieID,
                                    ledenLijst = model.SpecialeLijst,
                                    sortering = model.GekozenSortering,
                                    groepID
                                });
            }
        }

        /// <summary>
        /// Voert de gekozen actie in de dropdownlist van de ledenlijst uit op de geselecteerde
        /// personen.
        /// </summary>
        /// <param name="model">De property GekozenActie bepaalt wat er zal gebeuren met de gelieerde personen
        /// met ID's in de property SelectieGelieerdePersoonIDs.</param>
        /// <param name="groepID">ID van de groep waarin de gebruiker op dit moment aan het werken is.</param>
        /// <returns>Een redirect naar de juiste controller action</returns>
        private ActionResult ToepassenOpSelectie(LidInfoModel model, int groepID)
        {
            // In eerste instantie is dit voornamelijk copy/paste uit de personencontroller.

            if (model.SelectieGelieerdePersoonIDs == null || !model.SelectieGelieerdePersoonIDs.Any())
            {
                TempData["fout"] = Properties.Resources.NiemandGeselecteerdFout;
                return TerugNaarVorigeLijst();
            }

            switch (model.GekozenActie)
            {
                case 1:
                    // hack met tempdata, want inschrijven zit in de personencontroller.
                    // Als je dit zou wijzigen, let er dan op dat je #2714 niet terug introduceert.

                    TempData["gpids"] = model.SelectieGelieerdePersoonIDs;
                    return RedirectToAction("InschrijvenTempData", "Personen",
                        new { groepID });
                case 2:
                    GelieerdePersonenUitschrijven(model.SelectieGelieerdePersoonIDs, groepID, Properties.Resources.LedenUitgeschreven);
                    return RedirectToAction("Index", new {groepID});
                case 3:
                    return AfdelingenBewerken(model.SelectieGelieerdePersoonIDs, groepID);
                case 4:
                    TempData["ids"] = model.SelectieGelieerdePersoonIDs;
                    return RedirectToAction("InschrijvenVoorUitstap", "Personen", new { groepID });
                default:
                    TempData["fout"] = Properties.Resources.OnbestaandeActieFeedback;
                    return TerugNaarVorigeLijst();
            }
        }

        /// <summary>
        /// Downloadt de lijst van leden uit groepswerkjaar met GroepsWerkJaarID <paramref name="id"/> als
        /// Exceldocument.
        /// </summary>
        /// <param name="id">ID van het gevraagde groepswerkjaar (komt uit url). Gebruik 0 voor het huidige werkjaar.</param>
        /// <param name="afdelingID">Als verschillend van 0, worden enkel de leden met afdelng bepaald door dit ID
        /// getoond.</param>
        /// <param name="functieID">Als verschillend van 0, worden enkel de leden met de functie bepaald door
        /// dit ID getoond.</param>
        /// <param name="groepID">ID van de groep (komt uit url)</param>
        /// <param name="ledenLijst">Welk soort selectie we moeten maken (alle leden, alleen mensen in probeerperiode, 
        /// enz.)</param>
        /// <param name="sortering">Eigenschap waarop gesorteerd moet worden</param>
        /// <returns>Exceldocument met gevraagde ledenlijst</returns>
        [HandleError]
        [ParametersMatch]
        public ActionResult Download([RouteValue]int id, [QueryStringValue]int afdelingID, [QueryStringValue]int functieID, [RouteValue]int groepID, [QueryStringValue]LidInfoModel.SpecialeLedenLijst ledenLijst, [QueryStringValue]LidEigenschap sortering)
        {
            WerkJaarInfo werkJaarInfo;
            int groepsWerkJaarID;

            // Als geen groepswerkjaar gegeven is: haal recentste op 

            if (id == 0)
            {
                var gwj = VeelGebruikt.GroepsWerkJaarOphalen(groepID);
                werkJaarInfo = new WerkJaarInfo {WerkJaar = gwj.WerkJaar, ID = gwj.WerkJaarID};
                groepsWerkJaarID = werkJaarInfo.ID;
            }
            else
            {
                var gwjs = ServiceHelper.CallService<IGroepenService, IEnumerable<WerkJaarInfo>>(svc => svc.WerkJarenOphalen(groepID));
                werkJaarInfo = (from wj in gwjs
                    where wj.ID == id
                    select wj).FirstOrDefault();
                groepsWerkJaarID = id;
            }

            // Haal de op te lijsten leden op; de filter wordt bepaald uit de method parameters.
            var leden = ServiceHelper.CallService<ILedenService, IList<PersoonLidInfo>>(
                svc => svc.LijstZoekenPersoonLidInfo(
                    FilterMaken(afdelingID, functieID, ledenLijst, groepsWerkJaarID)));

            // Alle afdelingen is eigenlijk overkill. De actieve zou genoeg zijn. Maar meestal
            // zal daar niet zo' n verschil opzitten.
            var alleAfdelingen =
                ServiceHelper.CallService<IGroepenService, IList<AfdelingInfo>>(
                    svc => svc.AlleAfdelingenOphalen(groepID));

            const string bestandsNaam = "leden.xlsx";

            var pkg = GapExcelManip.LidExcelDocument(leden, alleAfdelingen);
            
            return new ExcelResult(pkg, bestandsNaam);
        }

        /// <summary>
        /// Downloadt een Exceldocument met de leden uit het groepswerkjaar met gegeven <paramref name="id"/>.
        /// </summary>
        /// <param name="id">ID van een groepswerkjaar. 0 voor recentste.</param>
        /// <param name="groepID">ID van de groep waarvan de leden te downloaden zijn (komt uit url)</param>
        /// <returns>Exceldocument met de leden uit het gevraagde werkjaar</returns>
        [HandleError]
        [ParametersMatch]
        public ActionResult Download([RouteValue]int id, [RouteValue] int groepID)
        {
            return Download(id, 0, 0, groepID, LidInfoModel.SpecialeLedenLijst.Geen, LidEigenschap.Afdeling);
        }

        // TODO (#967): Er zijn methods 'AfdelingBewerken' (1 persoon) en 'AfdelingenBewerken' (meerdere personen)
        // Waarschijnlijk kan er een en ander vereenvoudigd worden

        /// <summary>
        /// Toont de view die toelaat om de afdeling(en) van een lid te wijzigen
        /// </summary>
        /// <param name="lidID">LidID van het lid met de te wijzigen afdeling(en)</param>
        /// <param name="groepID">Groep waarin de user momenteel werkt</param>
        /// <returns>De view 'AfdelingBewerken'</returns>
        /// ActionResult
        public JsonResult AfdelingBewerken(int lidID, int groepID)
        {
            var model = new LidAfdelingenModel();
            BaseModelInit(model, groepID);

            model.BeschikbareAfdelingen = ServiceHelper.CallService<IGroepenService, IEnumerable<ActieveAfdelingInfo>>(
                svc => svc.HuidigeAfdelingsJarenOphalen(groepID));
            model.Info = ServiceHelper.CallService<ILedenService, LidAfdelingInfo>(
                svc => svc.AfdelingenOphalen(lidID));
            model.LidID = lidID;

            if (model.BeschikbareAfdelingen.FirstOrDefault() == null)
            {
                // Geen afdelingen.

                // Tempdata is niet relevant als het resultaat Json is.

                // Volgens mij loopt dit sowieso verkeerd af:
                return Json("{data:geen afdelingen}"); //TerugNaarVorigeLijst();
            }
            else
            {
                model.Titel = String.Format(Properties.Resources.AfdelingenAanpassen, model.Info.VolledigeNaam);
                return Json(model,JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult AfdelingBewerken(LidAfdelingenModel model)
        {
            int gpID = ServiceHelper.CallService<ILedenService, int>(svc => svc.AfdelingenVervangen(model.LidID, model.Info.AfdelingsJaarIDs));

            return RedirectToAction("Bewerken", "Personen", new { id = gpID });
        }

        /// <summary>
        /// Genereert een view die de gebruiker de geselecteerde personen nog eens toont, en toelaat
        /// een nieuwe afdeling te kiezen.
        /// </summary>
        /// <param name="selectieGelieerdePersoonIDs">ID's van de *gelieerde* personen</param>
        /// <param name="groepID">Groep waarvoor de gelieerde personen dit werkJaar lid moeten zijn</param>
        /// <returns>Een view die toelaat een andere afdeling te kiezen</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        private ActionResult AfdelingenBewerken(IEnumerable<int> selectieGelieerdePersoonIDs, int groepID)
        {
            // We verwachten een niet-lege lijst gelieerdePersoonIDs

            Debug.Assert(selectieGelieerdePersoonIDs != null);
            Debug.Assert(selectieGelieerdePersoonIDs.FirstOrDefault() != 0);

            var model = new AfdelingenBewerkenModel();
            BaseModelInit(model, groepID);

            model.BeschikbareAfdelingen = ServiceHelper.CallService<IGroepenService, IEnumerable<ActieveAfdelingInfo>>(
                svc => svc.HuidigeAfdelingsJarenOphalen(groepID));

            model.Personen = ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonDetail>>(
                svc => svc.OphalenMetLidInfo(selectieGelieerdePersoonIDs));

            if (model.BeschikbareAfdelingen.FirstOrDefault() == null)
            {
                // Geen afdelingen.

                // Geef een foutmelding dat er geen afdelingen zijn, met een link
                // naar de afdelingsinstellingen.

                TempData["fout"] = String.Format(
                    Properties.Resources.GeenActieveAfdelingen,
                    Url.Action("Afdelingen", "Groep", new { groepID }));

                return TerugNaarVorigeLijst();
            }
            else
            {
                model.Titel = String.Format(Properties.Resources.AfdelingenAanpassen);
                return View("AfdelingenBewerken", model);
            }
        }

        /// <summary>
        /// Bewaart de nieuw toegekende afdeling(en) uit <paramref name="model"/>
        /// </summary>
        /// <param name="model">AfdelingenBewerkenModel met info over welke leden welke
        /// afdelingen moeten krijgen</param>
        /// <param name="groepID">Groep waarin wordt gewerkt</param>
        /// <returns>Er wordt geredirect naar de ledenlijst van de groep</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AfdelingenBewerken(AfdelingenBewerkenModel model, int groepID)
        {
            ServiceHelper.CallService<ILedenService>(svc => svc.AfdelingenVervangenBulk(model.LidIDs, model.AfdelingsJaarIDs));

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Toont een view waarin gevraagd wordt te bevestigen dat het lid met LidID <paramref name="id"/> verzekerd moet worden
        /// tegen loonverlies.
        /// </summary>
        /// <param name="groepID">ID van de groep waarin we momenteel werken</param>
        /// <param name="id">LidID van het te verzekeren lid</param>
        /// <returns>
        /// Een view waarin gevraagd wordt te bevestigen dat het lid met LidID <paramref name="id"/> verzekerd moet worden
        /// </returns>
        [HandleError]
        public ActionResult LoonVerliesVerzekeren(int groepID, int id)
        {
            var model = new BevestigingsModel();
            BaseModelInit(model, groepID);

            // TODO (#1031): DetailsOphalen is eigenlijk overkill; we hebben enkel de volledige naam en 
            // het GelieerdePersoonID nodig.
            var info = ServiceHelper.CallService<ILedenService, PersoonInfo>(svc => svc.PersoonOphalen(id));

            model.LidID = id;
            model.GelieerdePersoonID = info.GelieerdePersoonID;
            model.VolledigeNaam = string.Format("{0} {1}", info.VoorNaam, info.Naam);
            model.Prijs = Settings.Default.PrijsVerzekeringLoonVerlies;
            model.Titel = String.Format("{0} verzekeren tegen loonverlies", model.VolledigeNaam);

            return View(model);
        }

        /// <summary>
        /// Verzekert het lid met LidID <paramref name="id"/> voor loonverlies, en redirect naar de detailfiche van de persoon.
        /// </summary>
        /// <param name="model">Een BevestigingsModel, puur pro forma, want alle relevante info zit in de url</param>
        /// <param name="groepID">ID van de groep waarin wordt gewerkt</param>
        /// <param name="id">LidID van te verzekeren lid</param>
        /// <returns>Redirect naar detailfiche van het betreffende lid</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult LoonVerliesVerzekeren(BevestigingsModel model, int groepID, int id)
        {
            try 
            {
                ServiceHelper.CallService<ILedenService, int>(svc => svc.LoonVerliesVerzekeren(id));
                TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;                
            }
            catch (FaultException<FoutNummerFault> ex) 
            {
                var lidInfo = ServiceHelper.CallService<ILedenService, PersoonLidInfo>(svc => svc.DetailsOphalen(id));
                switch (ex.Detail.FoutNummer) 
                {
                    case FoutNummer.GroepInactief:
                        TempData["fout"] = String.Format(Properties.Resources.GroepInactief);
                        break;
                    case FoutNummer.ChronologieFout:
                        var url = Url.Action("Index","JaarOvergang");
                        var werkJaar = VeelGebruikt.GroepsWerkJaarOphalen(groepID).WerkJaar;
                        TempData["fout"] = String.Format(Properties.Resources.WerkJaarInOvergang, werkJaar + 1, werkJaar + 2, url);
                        break;
                    default:
                        throw;
                }
            }
            catch (FaultException<BestaatAlFault<String>>) 
            {
                TempData["fout"] = String.Format(Properties.Resources.VerzekeringBestaatAl);
            }
            // TODO (#1031): DetailsOphalen is eigenlijk overkill; we hebben enkel de volledige naam en het GelieerdePersoonID nodig.
            var info = ServiceHelper.CallService<ILedenService, PersoonInfo>(svc => svc.PersoonOphalen(id));
            return RedirectToAction("Bewerken", "Personen", new { id = info.GelieerdePersoonID });
        }

        /// <summary>
        /// Toont een view die toelaat de functies van het lid met LidID <paramref name="id"/> te bewerken.
        /// </summary>
        /// <param name="id">LidID te bewerken lid</param>
        /// <param name="groepID">ID van de huidig geselecteerde groep</param>
        /// <returns>De view 'FunctiesToekennen'</returns>
        [HandleError]
        public ActionResult FunctiesToekennen(int id, int groepID)
        {
            var model = new LidFunctiesModel();
            BaseModelInit(model, groepID);

            model.Persoon = ServiceHelper.CallService<ILedenService, PersoonInfo>(l => l.PersoonOphalen(id));
            model.LidInfo = ServiceHelper.CallService<ILedenService, LidInfo>(svc => svc.LidInfoOphalen(id));

            if (model.Persoon != null)
            {
                // Ik had liever hierboven nog eens LidExtras.AlleAfdelingen meegegeven, maar
                // het datacontract (LidInfo) voorziet daar niets voor.

                FunctiesOphalen(model);

                model.Titel = String.Format(
                    Properties.Resources.FunctiesVan,
                    String.Format("{0} {1}", model.Persoon.VoorNaam, model.Persoon.Naam));

                return View("FunctiesToekennen", model);
            }
            else
            {
                TempData["fout"] = Properties.Resources.GegevensOpvragenMisluktFout;
                return RedirectToAction("Index", groepID);
            }
        }

        /// <summary>
        /// Bewaart functies
        /// </summary>
        /// <param name="model">LidFunctiesModel met te bewaren gegevens (functie-ID's in <c>model.FunctieIDs</c>)</param>
        /// <param name="id">LidID te bewerken lid</param>
        /// <param name="groepID">ID van de groep waarin de user momenteel aan het werken is</param>
        /// <returns>De personenfiche, die de gewijzigde info toont.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult FunctiesToekennen(LidFunctiesModel model, int id, int groepID)
        {
            // TODO (#1036): Dit moet een atomaire operatie zijn, om concurrencyproblemen te vermijden.
            try
            {
                ServiceHelper.CallService<ILedenService>(l => l.FunctiesVervangen(id, model.FunctieIDs));

                VeelGebruikt.FunctieProblemenResetten(groepID);

                TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                var lidInfo = ServiceHelper.CallService<ILedenService, PersoonLidInfo>(svc => svc.DetailsOphalen(id));
                string naam = lidInfo.PersoonDetail.VolledigeNaam;
                string persoonlijk = lidInfo.PersoonDetail.Geslacht == GeslachtsType.Vrouw ? "haar" : "hem";
                string bezittelijk = lidInfo.PersoonDetail.Geslacht == GeslachtsType.Vrouw ? "haar" : "zijn";
                switch (ex.Detail.FoutNummer)
                {
                    case FoutNummer.EMailVerplicht:
                        TempData["fout"] = String.Format(Properties.Resources.EmailVoorContactOntbreekt, naam,
                            persoonlijk);
                        break;
                    case FoutNummer.ContactMoetNieuwsBriefKrijgen:
                        TempData["fout"] = String.Format(Properties.Resources.ContactMoetNieuwsBriefKrijgen, naam, bezittelijk);
                        break;
                    default:
                        throw;
                }
            }
            return RedirectToAction("Bewerken", "Personen", new { groepID, id = model.Persoon.GelieerdePersoonID });
        }

        /// <summary> 
        /// Bekijkt model.Persoon.  Haalt alle functies van het groepswerkjaar van het lid op, relevant
        /// voor het type lid (kind/leiding), en bewaart ze in model.AlleFuncties.  
        /// In model.FunctieIDs komen de ID's van de toegekende functies voor het lid.
        /// </summary>
        /// <param name="model">Te bewerken model</param>
        [HandleError]
        public void FunctiesOphalen(LidFunctiesModel model)
        {
            model.AlleFuncties = ServiceHelper.CallService<IGroepenService, IEnumerable<FunctieDetail>>
                (svc => svc.FunctiesOphalen(
                    model.LidInfo.GroepsWerkJaarID,
                    model.LidInfo.Type));
            model.FunctieIDs = (from f in model.LidInfo.Functies
                                select f.ID).ToList();
        }

        /// <summary>
        /// Togglet het vlaggetje 'lidgeld betaald' van een lid.
        /// </summary>
        /// <param name="id">LidID van lid met te toggelen vlagje</param>
        /// <param name="groepID">ID van de groep waarin wordt gewerkt</param>
        /// <returns>Daarna wordt terugverwezen naar de persoonsfiche</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult LidGeldToggle(int id, int groepID)
        {
            int gelieerdePersoonID = ServiceHelper.CallService<ILedenService, int>(svc => svc.LidGeldToggle(id));

            return RedirectToAction("Bewerken", "Personen", new { groepID, id = gelieerdePersoonID });
        }

        /// <summary>
        /// Verandert een kind in leiding of vice versa
        /// </summary>
        /// <param name="id">LidID</param>
        /// <param name="groepID">ID van de groep die de bewerking uitvoert</param>
        /// <returns>JSON resultaat, met een fout- of succesboodschap. RTFS.</returns>
        [HandleError]
        public JsonResult TypeToggle(int id, int groepID)
        {
            var nieuwLidId = 0;

            // TODO (#1812): Feedback van callbackfunctie standaardiseren.

            try
            {
                nieuwLidId = ServiceHelper.CallService<ILedenService, int>(svc => svc.TypeToggle(id));
                VeelGebruikt.LedenProblemenResetten(groepID);
                VeelGebruikt.FunctieProblemenResetten(groepID);
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                return Json("{\"fout\" : \"" + ex.Message + "\"}"); // moet met strip tags??
            }
            return Json("{ \"succes\" : \"" + nieuwLidId + "\"}");
        }

        #region Verkorte url's, die eigenlijk gewoon Lijst aanroepen met de jusite parameters

        /// <summary>
        /// Toont een gesorteerde ledenlijst
        /// </summary>
        /// <param name="id">ID van het groepswerkjaar waarvoor een ledenlijst wordt gevraagd.  
        /// Indien 0, het recentste groepswerkjaar van de groep met ID <paramref name="groepID"/>.</param>
        /// <param name="groepID">Groep waaruit de leden opgehaald moeten worden.</param>
        /// <returns>Een view met de gevraagde ledenlijst</returns>
        /// <remarks>De attributen RouteValue en QueryStringValue laten toe dat we deze method overloaden.
        /// zie http://blog.abodit.com/2010/02/asp-net-mvc-ambiguous-match/ </remarks>
        [ParametersMatch]
        public ActionResult Lijst(
            [RouteValue]int id,
            [RouteValue]int groepID)
        {
            return Lijst(id, 0, 0, LidInfoModel.SpecialeLedenLijst.Geen, LidEigenschap.Naam, groepID);
        }

        /// <summary>
        /// Toont de lijst van de probeerleden
        /// </summary>
        /// <param name="groepID">Groep waarvoor de probeerleden getoond moeten worden</param>
        /// <returns>View voor de probeerleden</returns>
        /// <remarks>Deze actie wordt (nog) nergens gebruikt in de app, maar er wordt wel naar verwezen
        /// in het mailtje ivm de probeerleden (gemakkelijke url).</remarks>
        public ActionResult ProbeerLeden(int groepID)
        {
            return Lijst(0, 0, 0, LidInfoModel.SpecialeLedenLijst.Probeerleden, LidEigenschap.Naam, groepID);
        }

        /// <summary>
        /// Toont de leden waarvan het adres onvolledig is
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <returns>De ledenlijst van leden zonder volledig adres</returns>
        public ActionResult ZonderAdres(int groepID)
        {
            return Lijst(0, 0, 0, LidInfoModel.SpecialeLedenLijst.OntbrekendAdres, LidEigenschap.Naam, groepID);
        }

        /// <summary>
        /// Toont de leid(st)ers zonder e-mailadres
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <returns>Een view met de leid(st)ers zonder e-mailadres</returns>
        public ActionResult LeidingZonderMail(int groepID)
        {
            return Lijst(0, 0, 0, LidInfoModel.SpecialeLedenLijst.LeidingZonderEmail, LidEigenschap.Naam, groepID);
        }

        /// <summary>
        /// Toont de leden uit groep met ID <paramref name="groepID"/> zonder telefoonnummer
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <returns>Leden uit groep met ID <paramref name="groepID"/> zonder telefoonnummer</returns>
        public ActionResult ZonderTelefoon(int groepID)
        {
            return Lijst(0, 0, 0, LidInfoModel.SpecialeLedenLijst.OntbrekendTelefoonNummer, LidEigenschap.Naam, groepID);
        }

        /// <summary>
        /// Toont de leden uit een bepaalde afdeling in het meest recente werkJaar
        /// </summary>
        /// <param name="id">ID van de afdeling.</param>
        /// <param name="groepID">Groep waaruit de leden opgehaald moeten worden.</param>
        /// <returns>Een view met de gevraagde ledenlijst</returns>
        /// <remarks>De attributen RouteValue en QueryStringValue laten toe dat we deze method overloaden.
        /// zie http://blog.abodit.com/2010/02/asp-net-mvc-ambiguous-match/ </remarks>
        [ParametersMatch]
        public ActionResult Afdeling(
            [RouteValue]int id,
            [RouteValue]int groepID)
        {
            return Lijst(0, id, 0, LidInfoModel.SpecialeLedenLijst.Geen, LidEigenschap.Naam, groepID);
        }

        /// <summary>
        /// Toont de leden uit een bepaalde afdeling
        /// </summary>
        /// <param name="id">ID van de afdeling.</param>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar waarvoor een ledenlijst wordt gevraagd.  
        ///  Indien 0, het recentste groepswerkjaar van de groep met ID <paramref name="groepID"/>.</param>
        /// <param name="groepID">Groep waaruit de leden opgehaald moeten worden.</param>
        /// <returns>Een view met de gevraagde ledenlijst</returns>
        /// <remarks>De attributen RouteValue en QueryStringValue laten toe dat we deze method overloaden.
        /// zie http://blog.abodit.com/2010/02/asp-net-mvc-ambiguous-match/ </remarks>
        [ParametersMatch]
        public ActionResult Afdeling(
            [RouteValue]int id,
            [QueryStringValue]int groepsWerkJaarID,
            [RouteValue]int groepID)
        {
            return Lijst(groepsWerkJaarID, id, 0, LidInfoModel.SpecialeLedenLijst.Geen, LidEigenschap.Naam, groepID);
        }

        /// <summary>
        /// Toont de leden uit een bepaalde functie in het meest recente werkJaar
        /// </summary>
        /// <param name="id">ID van de functie.</param>
        /// <param name="groepID">Groep waaruit de leden opgehaald moeten worden.</param>
        /// <returns>Een view met de gevraagde ledenlijst</returns>
        /// <remarks>De attributen RouteValue en QueryStringValue laten toe dat we deze method overloaden.
        /// zie http://blog.abodit.com/2010/02/asp-net-mvc-ambiguous-match/ </remarks>
        [ParametersMatch]
        public ActionResult Functie(
            [RouteValue]int id,
            [RouteValue]int groepID)
        {
            return Lijst(0, 0, id, LidInfoModel.SpecialeLedenLijst.Geen, LidEigenschap.Naam, groepID);
        }

        /// <summary>
        /// Toont de leden uit een bepaalde functie
        /// </summary>
        /// <param name="id">ID van de functie.</param>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar waarvoor een functie wordt gevraagd.  
        ///  Indien 0, het recentste groepswerkjaar van de groep met ID <paramref name="groepID"/>.</param>
        /// <param name="groepID">Groep waaruit de leden opgehaald moeten worden.</param>
        /// <returns>Een view met de gevraagde ledenlijst</returns>
        /// <remarks>De attributen RouteValue en QueryStringValue laten toe dat we deze method overloaden.
        /// zie http://blog.abodit.com/2010/02/asp-net-mvc-ambiguous-match/ </remarks>
        [ParametersMatch]
        public ActionResult Functie(
            [RouteValue]int id,
            [QueryStringValue]int groepsWerkJaarID,
            [RouteValue]int groepID)
        {
            return Lijst(groepsWerkJaarID, 0, id, LidInfoModel.SpecialeLedenLijst.Geen, LidEigenschap.Naam, groepID);
        }
        #endregion
    }
}
