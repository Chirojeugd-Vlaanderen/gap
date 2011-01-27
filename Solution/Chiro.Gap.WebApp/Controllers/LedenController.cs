// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.ActionFilters;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	[HandleError]
	public class LedenController : BaseController
	{
		/// <summary>
		/// Standaardconstructor.  <paramref name="serviceHelper"/> en <paramref name="veelGebruikt"/> worden
		/// best toegewezen via inversion of control.
		/// </summary>
		/// <param name="serviceHelper">wordt gebruikt om de webservices van de backend aan te spreken</param>
		/// <param name="veelGebruikt">haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
		/// service</param>
		public LedenController(IServiceHelper serviceHelper, IVeelGebruikt veelGebruikt) : base(serviceHelper, veelGebruikt) { }

		/// <summary>
		/// Sorteert een rij records van type LidOverzicht
		/// </summary>
		/// <param name="rij">Te sorteren rij</param>
		/// <param name="sortering">Lideigenschap waarop gesorteerd moet worden</param>
		/// <returns>Gesorteerde rij</returns>
		private IEnumerable<LidOverzicht> Sorteren (IEnumerable<LidOverzicht> rij, LidEigenschap sortering)
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
						.ThenBy(src => src.Afdelingen.Count() > 0 ? src.Afdelingen.First().Afkorting : String.Empty)
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
		/// </summary>
		/// <param name="gwjID">ID van het groepswerkjaar waarvoor een ledenlijst wordt gevraagd.  
		///   Indien 0, het recentste groepswerkjaar van de groep met ID <paramref name="groepID"/>.</param>
		/// <param name="groepID">Groep waaruit de leden opgehaald moeten worden.</param>
		/// <param name="sortering">Bepaalt op welke eigenschap de lijst gesorteerd moet worden</param>
		/// <param name="afdelingID">Als verschillend van 0, worden enkel de leden uit de afdeling met
		///   dit AfdelingID getoond.</param>
		/// <param name="functieID">Als verschillend van 0, worden enkel de leden met de functie met
		///   dit FunctieID getoond.</param>
		/// <param name="ledenLijst">Hiermee kan je speciale lijsten opzoeken</param>
		/// <param name="metAdressen">Geeft aan of de adressen mee opgevraagd moeten worden
		/// (!duurt lang!)</param>
		/// <returns>een LidInfoModel, met in member LidInfoLijst de gevonden leden</returns>
		private LidInfoModel Zoeken(
			int gwjID,
			int groepID,
			LidEigenschap sortering,
			int afdelingID,
			int functieID,
			LidInfoModel.SpecialeLedenLijst ledenLijst,
			bool metAdressen)
		{
			// Het sorteren gebeurt nu in de webapp, en niet in de backend.  Sorteren is immers presentatie, dus de service
			// moet zich daar niet mee bezig houden.
			//
			// Voor de personenlijst ligt dat anders.  Als je daar een pagina opvraagt, dan is die
			// pagina afhankelijk van de gekozen sortering.  Daarom moet het sorteren voor personen
			// wel door de service gebeuren.  (Maar hier, bij de leden, is dat niet van toepassing.)

			// Als geen groepswerkjaar gegeven is: haal recentste op 
			int groepsWerkJaarID = gwjID == 0 ? ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID)) : gwjID;

			LidInfoModel model = LijstModelInitialiseren(groepsWerkJaarID, groepID, sortering);
			model.KanLedenBewerken = groepsWerkJaarID == (from wj in model.WerkJaarInfos
								      orderby wj.WerkJaar descending
								      select wj.ID).FirstOrDefault();

			// Bewaar gekozen filters in model, zodat de juiste items in de dropdownlijsten geselecteerd 
			// worden

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
				svc => svc.Zoeken(
					new LidFilter
						{
							GroepsWerkJaarID = groepsWerkJaarID,
							AfdelingID = (afdelingID == 0) ? null : (int?) afdelingID,
							FunctieID = (functieID == 0) ? null : (int?) functieID,
							ProbeerPeriodeNa =
								(ledenLijst == LidInfoModel.SpecialeLedenLijst.Probeerleden) ? (DateTime?) DateTime.Today : null,
							HeeftVoorkeurAdres = (ledenLijst == LidInfoModel.SpecialeLedenLijst.OntbrekendAdres) ? (bool?) false : null,
							HeeftTelefoonNummer =
								(ledenLijst == LidInfoModel.SpecialeLedenLijst.OntbrekendTelefoonNummer) ? (bool?) false : null,
							HeeftEmailAdres = (ledenLijst == LidInfoModel.SpecialeLedenLijst.LeidingZonderEmail) ? (bool?) false : null,
							LidType = (ledenLijst == LidInfoModel.SpecialeLedenLijst.LeidingZonderEmail) ? LidType.Leiding : LidType.Alles
					},
					metAdressen));

			if (functieID != 0)
			{
				// Naam van de functie opzoeken, zodat we ze kunnen invullen in de paginatitel
				string functieNaam = (from fi in model.FunctieInfoDictionary
						      where fi.Key == functieID
						      select fi).First().Value.Naam;

				model.Titel = String.Format(Properties.Resources.AfdelingsLijstTitel,
							    functieNaam,
							    model.JaartalGetoondGroepsWerkJaar,
							    model.JaartalGetoondGroepsWerkJaar + 1);

			}
			else if (afdelingID != 0)
			{

				AfdelingDetail af = (from a in model.AfdelingsInfoDictionary.AsQueryable()
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


		// GET: /Leden/
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
		/// <param name="sortering">Enumwaarde die aangeeft op welke parameter de lijst gesorteerd moet worden</param>
		/// <returns></returns>
		[HandleError]
		private LidInfoModel LijstModelInitialiseren(int groepsWerkJaarID, int groepID, LidEigenschap sortering)
		{
			// TODO: deze code moet opgekuist worden

			// Er wordt een nieuwe lijst opgevraagd, dus wordt deze vanaf hier bijgehouden als de lijst om terug naar te springen
			ClientState.VorigeLijst = Request.Url.ToString();

			var model = new LidInfoModel();
			BaseModelInit(model, groepID);

			// TODO check dat het gegeven groepswerkjaar id wel degelijk van de gegeven groep is

			// Laad de lijst van werkjaren in van de groep en zet de juiste info over het te tonen werkjaar
			model.WerkJaarInfos = ServiceHelper.CallService<IGroepenService, IEnumerable<WerkJaarInfo>>(e => e.WerkJarenOphalen(groepID));

			var gevraagdwerkjaar = (from g in model.WerkJaarInfos
									where g.ID == groepsWerkJaarID
									select g).First();

			model.IDGetoondGroepsWerkJaar = groepsWerkJaarID;
			model.JaartalGetoondGroepsWerkJaar = gevraagdwerkjaar.WerkJaar;

			// Haal alle afdelingsjaren op van de groep in het groepswerkjaar
			var list = ServiceHelper.CallService<IGroepenService, IList<AfdelingDetail>>(groep => groep.ActieveAfdelingenOphalen(groepsWerkJaarID));

			// Laad de afdelingen in het model in via een dictionary
			model.AfdelingsInfoDictionary = new Dictionary<int, AfdelingDetail>();
			foreach (AfdelingDetail ai in list)
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
			foreach (FunctieDetail fi in list2)
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
		///   Indien 0, het recentste groepswerkjaar van de groep met ID <paramref name="groepID"/>.</param>
		/// <param name="afdelingID">Als verschillend van 0, worden enkel de leden uit de afdeling met
		///   dit AfdelingID getoond.</param>
		/// <param name="functieID">Als verschillend van 0, worden enkel de leden met de functie met
		///   dit FunctieID getoond.</param>
		/// <param name="ledenLijst"></param>
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
			LidInfoModel model = Zoeken(id, groepID, sortering, afdelingID, functieID, ledenLijst, false);
			return View("Index", model);
		}


		/// <summary>
		/// Filtert een lijst op basis van de info in LidInfoModel
		/// </summary>
		/// <param name="id">ID van het groepswerkjaar.  Als dit 0 is, wordt het recentste groepswerkjaar gekozen</param>
		/// <param name="groepID">ID van de groep.  Enkel nodig als geen groepswerkjaar gegeven is, maar sowieso
		/// beschikbaar via URL.</param>
		/// <param name="model">LidInfoModel, waaruit de informatie over de gewenste filters opgehaald moet worden.</param>
		/// <returns>Deze method zal voornamelijk redirecten
		/// </returns>
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Lijst(int id, int groepID, LidInfoModel model)
		{
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
		/// Downloadt de lijst van leden uit groepswerkjaar met GroepsWerkJaarID <paramref name="id"/> als
		/// Exceldocument.
		/// </summary>
		/// <param name="id">ID van het gevraagde groepswerkjaar (komt uit url)</param>
		/// <param name="afdelingID">Als verschillend van 0, worden enkel de leden met afdelng bepaald door dit ID
		/// getoond.</param>
		/// <param name="functieID">Als verschillend van 0, worden enkel de leden met de functie bepaald door
		/// dit ID getoond.</param>
		/// <param name="groepID">ID van de groep (komt uit url)</param>
		/// <param name="sortering">Eigenschap waarop gesorteerd moet worden</param>
		/// <returns>Exceldocument met gevraagde ledenlijst</returns>
		[HandleError]
		public ActionResult Download(int id, int afdelingID, int functieID, int groepID, LidInfoModel.SpecialeLedenLijst ledenLijst, LidEigenschap sortering)
		{
			var model = Zoeken(id, groepID, sortering, afdelingID, functieID, ledenLijst, true);
			string bestandsNaam;

			// Als ExcelManip de kolomkoppen kan afleiden uit de (param)array, en dan liefst nog de DisplayName
			// gebruikt van de PersoonOverzicht-velden, dan is de regel hieronder niet nodig.
			string[] kolomkoppen = {
			                       	"Type", "AD-nr", "Voornaam", "Naam", "Afdelingen", "Functies", "Geboortedatum", "Geslacht",
			                       	"Straat", "Nr", "Bus", "Postcode", "Gemeente", "Tel", "Mail", "Betaald"
			                       };

			bestandsNaam = String.Format("{0}.xlsx", model.Titel.Replace(" ", "-"));


			var stream = (new ExcelManip()).ExcelTabel(
				model.LidInfoLijst,
				kolomkoppen,
				it => it.Type,
				it => it.AdNummer,
				it => it.VoorNaam,
				it => it.Naam,
				it => it.Afdelingen == null ? String.Empty : String.Concat(it.Afdelingen.Select(afd => afd.Afkorting + " ").ToArray()),
				it => it.Functies == null ? String.Empty : String.Concat(it.Functies.Select(fun => fun.Code + " ").ToArray()),
				it => it.GeboorteDatum,
				it => it.Geslacht,
				it => it.StraatNaam,
				it => it.HuisNummer,
				it => it.Bus,
				it => it.PostNummer,
				it => it.WoonPlaats,
				it => it.TelefoonNummer,
				it => it.Email,
				it => it.LidgeldBetaald ? "Ja" : "Nee");

			return new ExcelResult(stream, bestandsNaam);
		}

		/// <summary>
		/// Schrijft een gelieerde persoon uit, en toont de bijgewerkte ledenlijst
		/// </summary>
		/// <param name="id">GelieerdePersoonID van uit te schrijven gelieerde persoon</param>
		/// <param name="groepID">ID van de groep waarin de gebruker werkt</param>
		/// <returns></returns>
		/// <remarks>Uitschrijven wil zeggen: maak de leden gekoppeld aan de gelieerde persoon met
		/// GelieerdePersoonID <paramref name="id"/> inactief</remarks>
		[HandleError]
		public ActionResult DeActiveren(int id, int groepID)
		{
			string fouten = String.Empty; // TODO fouten opvangen
			ServiceHelper.CallService<ILedenService>(l => l.Uitschrijven(new List<int> { id }, out fouten));

			// TODO: beter manier om problemen op te vangen dan via een string

			if (fouten == String.Empty)
			{
				TempData["succes"] = Properties.Resources.LidNonActiefGemaakt;

				VeelGebruikt.FunctieProblemenResetten(groepID);
			}
			else
			{
				// TODO: vermijden dat output van de back-end rechtstreeks zichtbaar wordt voor
				// de user.

				TempData["fout"] = fouten;
			}

			return TerugNaarVorigeLijst();
		}

		// id = lidid
		// GET: /Leden/Activeren/id
		// Er worden alleen actieve leden getoond in de lijsten, dus is dit niet meer relevant (voor al de rest wordt "lid maken" gebruikt).
		/*[HandleError]
		public ActionResult Activeren(int id, int groepID)
		{
			string fouten; // TODO fouten opvangen
			ServiceHelper.CallService<ILedenService>(l => l.Inschrijven(new List<int> { id }, out fouten));
			TempData["succes"] = Properties.Resources.LidActiefGemaakt;

			return TerugNaarVorigeLijst();
		}*/

		/// <summary>
		/// Toont de view die toelaat om de afdeling(en) van een lid te wijzigen
		/// </summary>
		/// <param name="lidID">LidID van het lid met de te wijzigen afdeling(en)</param>
		/// <param name="groepID">Groep waarin de user momenteel werkt</param>
		/// <returns>De view 'AfdelingBewerken'</returns>
		public ActionResult AfdelingBewerken(int lidID, int groepID)
		{
			var model = new LidAfdelingenModel();
			BaseModelInit(model, groepID);

			model.BeschikbareAfdelingen = ServiceHelper.CallService<IGroepenService, IEnumerable<ActieveAfdelingInfo>>(
				svc => svc.HuidigeAfdelingsJarenOphalen(groepID));
			model.Info = ServiceHelper.CallService<ILedenService, LidAfdelingInfo>(
				svc => svc.AfdelingenOphalen(lidID));

			if (model.BeschikbareAfdelingen.FirstOrDefault() == null)
			{
				// Geen afdelingen.

				// Workaround via TempData["fout"].  Niet zeker of dat een geweldig goed
				// idee is.

				TempData["fout"] = String.Format(
					Properties.Resources.GeenActieveAfdelingen,
					Url.Action("Index", "Afdelingen", new { groepID }));

				return TerugNaarVorigeLijst();
			}
			else
			{
				model.Titel = String.Format(Properties.Resources.AfdelingenAanpassen, model.Info.VolledigeNaam);
				return View("AfdelingBewerken", model);
			}
		}

		// 
		// POST: /Leden/AfdelingBewerken?lidID=5
		// FIXME lidID wordt automatisch ingevuld als er eenzelfde argument in de GET methode van afdelingBewerken staat. Dit is eigenlijk helemaal niet mooi want wordt niet geverifieerd en zelfs 2de niveau afhankelijkheid van aspx.
		[AcceptVerbs(HttpVerbs.Post)]
		[HandleError]
		public ActionResult AfdelingBewerken(LidAfdelingenModel model, int lidID)
		{
			// FIXME: Het is geen prachtige code: een call van AfdelingenVervangen levert 'toevallig'
			// een GelieerdePersoonID op, die ik dan in dit specifieke geval
			// 'toevallig' kan gebruiken om naar de juiste personenfiche om te schakelen.

			// De returnwaarde van de volgende call hebben we nergens voor nodig.
			ServiceHelper.CallService<ILedenService, int>(svc => svc.AfdelingenVervangen(lidID, model.Info.AfdelingsJaarIDs));
			return TerugNaarVorigeFiche();
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

			// TODO: DetalsOphalen is eigenlijk overkill; we hebben enkel de volledige naam en 
			// het GelieerdePersoonID nodig.
			var info = ServiceHelper.CallService<ILedenService, PersoonLidInfo>(svc => svc.DetailsOphalen(id));

			model.LidID = id;
			model.GelieerdePersoonID = info.PersoonDetail.GelieerdePersoonID;
			model.VolledigeNaam = info.PersoonDetail.VolledigeNaam;
			model.Prijs = Properties.Settings.Default.PrijsVerzekeringLoonVerlies;
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
			int gelieerdePersoonID = ServiceHelper.CallService<ILedenService, int>(svc => svc.LoonVerliesVerzekeren(id));
			return RedirectToAction("EditRest", "Personen", new { id = gelieerdePersoonID });
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

			model.HuidigLid = ServiceHelper.CallService<ILedenService, PersoonLidInfo>(l => l.DetailsOphalen(id));

			if (model.HuidigLid != null)
			{
				// Ik had liever hierboven nog eens LidExtras.AlleAfdelingen meegegeven, maar
				// het datacontract (LidInfo) voorziet daar niets voor.

				FunctiesOphalen(model);

				model.Titel = String.Format(
					Properties.Resources.FunctiesVan,
					model.HuidigLid.PersoonDetail.VolledigeNaam);

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
		/// <param name="groepID">ID van de groep waarin de user momenteel aan het werken is</param>
		/// <param name="id">LidID te bewerken lid</param>
		/// <returns>De personenfiche, die de gewijzigde info toont.</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		[HandleError]
		public ActionResult FunctiesToekennen(LidFunctiesModel model, int id, int groepID)
		{
			// TODO: Dit moet een unitaire operatie zijn, om concurrencyproblemen te vermijden.
			try
			{
				ServiceHelper.CallService<ILedenService>(l => l.FunctiesVervangen(id, model.FunctieIDs));

				VeelGebruikt.FunctieProblemenResetten(groepID);

				TempData["succes"] = Properties.Resources.WijzigingenOpgeslagenFeedback;
				return RedirectToAction("EditRest", "Personen", new { groepID, id = model.HuidigLid.PersoonDetail.GelieerdePersoonID });
			}
			catch (Exception)
			{
				model.HuidigLid = ServiceHelper.CallService<ILedenService, PersoonLidInfo>(l => l.DetailsOphalen(id));
				TempData["fout"] = Properties.Resources.WijzigingenNietOpgeslagenFout;
				return View("FunctiesToekennen", model);

			}
		}

		/// <summary>
		/// Bekijkt model.HuidigLid.  Haalt alle functies van het groepswerkjaar van het lid op, relevant
		/// voor het type lid (kind/leiding), en bewaart ze in model.AlleFuncties.  
		/// In model.FunctieIDs komen de ID's van de toegekende functies voor het lid.
		/// </summary>
		/// <param name="model">Te bewerken model</param>
		[HandleError]
		public void FunctiesOphalen(LidFunctiesModel model)
		{
			model.AlleFuncties = ServiceHelper.CallService<IGroepenService, IEnumerable<FunctieDetail>>
				(svc => svc.FunctiesOphalen(
					model.HuidigLid.LidInfo.GroepsWerkJaarID,
					model.HuidigLid.LidInfo.Type));
			model.FunctieIDs = (from f in model.HuidigLid.LidInfo.Functies
								select f.ID).ToList();
		}

		/// <summary>
		/// 'Togglet' het vlaggetje 'lidgeld betaald' van een lid.
		/// </summary>
		/// <param name="id">LidID van lid met te toggelen vlagje</param>
		/// <param name="groepID">ID van de groep waarin wordt gewerkt</param>
		/// <returns>Daarna wordt terugverwezen naar de persoonsfiche</returns>
		[HandleError]
		public ActionResult LidGeldToggle(int id, int groepID)
		{
			int gelieerdePersoonID = ServiceHelper.CallService<ILedenService, int>(svc => svc.LidGeldToggle(id));

			return RedirectToAction("EditRest", "Personen", new {groepID, id = gelieerdePersoonID});
		}

		/// <summary>
		/// Verandert een kind in leiding of vice versa
		/// </summary>
		/// <returns>Opnieuw de persoonsfiche</returns>
		[HandleError]
		public ActionResult TypeToggle(int id, int groepID)
		{
			int gelieerdePersoonID = 0;

			try
			{
				gelieerdePersoonID = ServiceHelper.CallService<ILedenService, int>(svc => svc.TypeToggle(id));
			}
			catch (FaultException<FoutNummerFault> ex)
			{
				if (ex.Detail.FoutNummer == FoutNummer.AlgemeneKindFout)
				{
					TempData["fout"] = Properties.Resources.FoutToggleNaarKind;
				}
				else if (ex.Detail.FoutNummer == FoutNummer.AlgemeneLeidingFout)
				{
					TempData["fout"] = Properties.Resources.FoutToggleNaarLeiding;
				}
				else
				{
					throw;	
				}
				return TerugNaarVorigeFiche();
			}
			
			return RedirectToAction("EditRest", "Personen", new { groepID, id = gelieerdePersoonID });
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
		/// Toont de leden uit een bepaalde afdeling in het meest recente werkjaar
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
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waarvoor een ledenlijst wordt gevraagd.  
		///   Indien 0, het recentste groepswerkjaar van de groep met ID <paramref name="groepID"/>.</param>
		/// <param name="id">ID van de afdeling.</param>
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
		/// Toont de leden uit een bepaalde functie in het meest recente werkjaar
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
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waarvoor een functie wordt gevraagd.  
		///   Indien 0, het recentste groepswerkjaar van de groep met ID <paramref name="groepID"/>.</param>
		/// <param name="id">ID van de functie.</param>
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
