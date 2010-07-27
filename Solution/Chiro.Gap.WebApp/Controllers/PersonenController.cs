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
using Chiro.Gap.Validatie;
using Chiro.Gap.WebApp.Models;
using Chiro.Gap.WebApp.HtmlHelpers;
using AutoMapper;

namespace Chiro.Gap.WebApp.Controllers
{
	// Om te zorgen dat het terugkeren naar de vorige lijst en dergelijke werkt in samenwerking met het opvragen van subsets
	// (categorieën of zo), hebben we steeds een default (categorie, ...) die aangeeft dat alle personen moeten worden meegegeven

	public class PersonenController : BaseController
	{
		private readonly AdressenHelper _adressenHelper;

		public PersonenController(IServiceHelper serviceHelper)
			: base(serviceHelper)
		{
			_adressenHelper = new AdressenHelper(serviceHelper);
		}
		// TODO er moeten ook nog een laatst gebruikte "actie" worden toegevoegd, niet alleen actie id

		// GET: /Personen/
		public override ActionResult Index(int groepID)
		{
			// redirect naar alle personen van de groep, pagina 1.
			return RedirectToAction("List", new
			{
				page = 1,
				id = 0,
				sortering = PersoonSorteringsEnum.Naam
			});
		}

		/// <summary>
		/// Haal een 'pagina' met persoonsinformatie op (inclusief lidinfo) voor personen uit een
		/// bepaalde categorie, en toont deze pagina via de view 'Index'.
		/// </summary>
		/// <param name="page">Nummer van de 'pagina'</param>
		/// <param name="groepID">Huidige groep waarin de gebruiker aan het werken is</param>
		/// <param name="id">ID van de gevraagde categorie.  Kan ook 0 zijn; dan worden alle personen
		/// geselecteerd.</param>
		/// <param name="sortering">Geeft de sortering van de pagina en lijst aan</param>
		/// <returns>De personenlijst in de view 'Index'</returns>
		//		public ActionResult List(int page, int groepID, int id)
		public ActionResult List(int page, int groepID, int id, PersoonSorteringsEnum sortering)
		{
			// Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten
			ClientState.VorigeLijst = Request.Url.ToString();
			ClientState.VorigeFiche = string.Empty; // Op null zetten zodat die eigenlijk "vergeten" wordt nu

			int totaal = 0;

			var model = new PersoonInfoModel();
			BaseModelInit(model, groepID);
			model.GekozenCategorieID = id;
			model.Sortering = sortering;

			// Alle personen bekijken
			if (id == 0)
			{
				model.PersoonInfos =
					ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonDetail>>
					(g => g.PaginaOphalenMetLidInfo(groepID, page, 20, sortering, out totaal));
				model.HuidigePagina = page;
				model.AantalPaginas = (int)Math.Ceiling(totaal / 20d);
				model.Titel = "Personenoverzicht";
				model.Totaal = totaal;
			}
			else
			{
				// TODO de catID is eigenlijk niet echt type-safe, maar wel het makkelijkste om te doen (lijkt teveel op PaginaOphalenLidInfo(groepid, ...))
				model.PersoonInfos =
					ServiceHelper.CallService<IGelieerdePersonenService, IList<PersoonDetail>>
					(g => g.PaginaOphalenUitCategorieMetLidInfo(id, page, 20, sortering, out totaal));
				model.HuidigePagina = page;
				model.AantalPaginas = (int)Math.Ceiling(totaal / 20d);

				String naam = (from c in model.PersoonInfos.First().CategorieLijst
							   where c.ID == id
							   select c).First().Naam;

				model.Titel = "Overzicht " + naam;
				model.Totaal = totaal;
			}

			model.GroepsCategorieen = ServiceHelper.CallService<IGroepenService, IList<CategorieInfo>>(
				svc => svc.CategorieenOphalen(groepID)).ToList();
			model.GroepsCategorieen.Add(new CategorieInfo
			{
				ID = 0,
				Naam = "Alle personen"
			});

			return View("Index", model);
		}

		/// <summary>
		/// Haalt een Excellijst op van alle personen in een groep, of als <paramref name="id"/> verschilt van 0, 
		/// de personen uit de categorie met ID <paramref name="id"/>.
		/// </summary>
		/// <param name="groepID">Huidige groep waarin de gebruiker aan het werken is</param>
		/// <param name="id">ID van de gevraagde categorie.  Kan ook 0 zijn; dan worden alle personen
		/// geselecteerd.</param>
		/// <returns>Een 'ExcelResult' met de gevraagde lijst</returns>
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


			var stream = (new ExcelManip()).ExcelTabel(
				data, 
				it => it.AdNummer, 
				it => it.VoorNaam,
				it => it.Naam,
				it => it.GeboorteDatum, 
				it => it.Geslacht,
				it => it.StraatNaam,
				it => it.HuisNummer,
				it => it.Bus,
				it => it.PostNummer,
				it => it.WoonPlaats,
				it => it.TelefoonNummer,
				it => it.Email);
			return new ExcelResult(stream, "personen.xlsx");
		}

		/// <summary>
		/// Bij postback naar List, wordt er gekeken wat er in model.GekozenCategorieID zit, en
		/// wordt de lijst getoond van personen in die categorie
		/// </summary>
		/// <param name="model">model.GekozenCategorieID bevat de ID van de categorie waarvan de
		/// personen getoond moeten worden.  Is deze 0, dan worden alle personen getoond</param>
		/// <param name="groepID">ID van de groep waarin de gebruiker momenteel werkt</param>
		/// <returns>Een redirect naar de juiste lijst</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult List(PersoonInfoModel model, int groepID)
		{
			return RedirectToAction(
				"List",
				new
				{
					page = 1,
					id = model.GekozenCategorieID,
					sortering = model.Sortering
				});
		}

		/// <summary>
		/// Voert de gekozen actie in de dropdownlist van de personenlijstcontrol uit op de geselecteerde
		/// personen.
		/// </summary>
		/// <param name="model">De property GekozenActie bepaalt wat er zal gebeuren met de gelieerde personen
		/// met ID's in de property GeselecteerdePersonen.</param>
		/// <param name="groepID">ID van de groep waarin de gebruiker op dit moment aan het werken is.</param>
		/// <returns>Een redirect naar de juiste controller action</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ToepassenOpSelectie(PersoonInfoModel model, int groepID)
		{
			if (model.GekozenGelieerdePersoonIDs == null || model.GekozenGelieerdePersoonIDs.Count == 0)
			{
				TempData["feedback"] = Properties.Resources.NiemandGeselecteerdFout;
				return TerugNaarVorigeLijst();
			}

			ActionResult r;

			switch (model.GekozenActie)
			{
				case 1:
				case 2:
					string foutBerichten = String.Empty;

					ServiceHelper.CallService<ILedenService, IEnumerable<int>>(g => g.LedenMaken(
						model.GekozenGelieerdePersoonIDs,
						model.GekozenActie == 1 ? LidType.Kind : LidType.Leiding,
						out foutBerichten));
					TempData["feedback"] = String.IsNullOrEmpty(foutBerichten) ? Properties.Resources.LedenGemaaktFeedback : foutBerichten;
					r = TerugNaarVorigeLijst();
					break;
				case 3:
					TempData.Add("list", model.GekozenGelieerdePersoonIDs);
					r = RedirectToAction("CategorieToevoegenAanLijst");
					break;
				default:
					TempData["feedback"] = Properties.Resources.OnbestaandeActieFeedback;
					r = TerugNaarVorigeLijst();
					break;
			}

			return r;
		}

		#region personen

		//
		// GET: /Personen/Nieuw
		public ActionResult Nieuw(int groepID)
		{
			var model = new GelieerdePersonenModel();
			BaseModelInit(model, groepID);
			model.HuidigePersoon = new PersoonDetail();

			model.Titel = Properties.Resources.NieuwePersoonTitel;
			return View("EditGegevens", model);
		}

		//
		// POST: /Personen/Nieuw
		[AcceptVerbs(HttpVerbs.Post)]
		[HttpPost]
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
				ids = ServiceHelper.CallService<IGelieerdePersonenService, IDPersEnGP>(l => l.GeforceerdAanmaken(model.HuidigePersoon, groepID, model.Forceer));
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
			// TODO: wat als er een fout optreedt bij PersoonBewaren?
			TempData["feedback"] = "Wijzigingen zijn opgeslagen";

			// (er wordt hier geredirect ipv de view te tonen,
			// zodat je bij een 'refresh' niet de vraag krijgt
			// of je de gegevens opnieuw wil posten.)
			return RedirectToAction("EditRest", new { id = ids.GelieerdePersoonID });
		}

		// aka maak een broertje of zusje
		// GET: /Personen/Kloon
		public ActionResult Kloon(int gelieerdepersoonID, int groepID)
		{
			var model = new GelieerdePersonenModel();
			BaseModelInit(model, groepID);
			var broerzus = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailsOphalen(gelieerdepersoonID));
			model.HuidigePersoon = new PersoonDetail();

			model.BroerzusID = broerzus.GelieerdePersoonID;
			model.HuidigePersoon.Naam = broerzus.Naam;

			model.Titel = Properties.Resources.NieuwePersoonTitel;
			return View("EditGegevens", model);
		}

		// POST: /Personen/Nieuw
		[AcceptVerbs(HttpVerbs.Post)]
		[HttpPost]
		// TODO dit is mss iets om op de server te draaien?
		public ActionResult Kloon(GelieerdePersonenModel model, int groepID)
		{
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
				ids = ServiceHelper.CallService<IGelieerdePersonenService, IDPersEnGP>(l => l.GeforceerdAanmaken(model.HuidigePersoon, groepID, model.Forceer));
			}
			catch (FaultException<BlokkerendeObjectenFault<PersoonDetail>> fault)
			{
				model.GelijkaardigePersonen = fault.Detail.Objecten;
				model.Forceer = true;
				return View("EditGegevens", model);
			}
			/////END DUPLICATE CODE

			var broerzus = ServiceHelper.CallService<IGelieerdePersonenService, PersoonLidInfo>(l => l.AlleDetailsOphalen(model.BroerzusID));

			var voorkeursComm = (from a in broerzus.CommunicatieInfo
								 where a.Voorkeur && a.IsGezinsGebonden
								 select a).FirstOrDefault();

			if (voorkeursComm != null)
			{
				ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CommunicatieVormToevoegen(ids.GelieerdePersoonID, voorkeursComm));
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
		public ActionResult EditGegevens(int id, int groepID)
		{
			var model = new GelieerdePersonenModel();
			BaseModelInit(model, groepID);
			// model.HuidigePersoon = ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.AlleDetailsOphalen(id, groepID));
			model.HuidigePersoon = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailsOphalen(id));
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
			// TODO: wat als er een fout optreedt bij PersoonBewaren?
			TempData["feedback"] = "Wijzigingen zijn opgeslagen";

			// (er wordt hier geredirect ipv de view te tonen,
			// zodat je bij een 'refresh' niet de vraag krijgt
			// of je de gegevens opnieuw wil posten.)
			return TerugNaarVorigeFiche();
		}

		// NEW CODE

		// GET
		/// <summary>
		/// Deze actie (TODO:) met onduidelijke naam toont gewoon de personenfiche van de gelieerde
		/// persoon met id <paramref name="id"/>.
		/// </summary>
		/// <param name="id">ID van de te tonen gelieerde persoon</param>
		/// <param name="groepID">GroepID van de groep waarin de gebruiker aan het werken is</param>
		/// <returns>De view van de personenfiche</returns>
		// id = gelieerdepersonenid
		public ActionResult EditRest(int id, int groepID)
		{
			// Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten
			ClientState.VorigeFiche = Request.Url.ToString();

			var model = new PersonenLedenModel();
			BaseModelInit(model, groepID);

			model.PersoonLidInfo = ServiceHelper.CallService<IGelieerdePersonenService, PersoonLidInfo>(l => l.AlleDetailsOphalen(id));

			AfdelingenOphalen(model);

			model.KanVerzekerenLoonVerlies = model.PersoonLidInfo.PersoonDetail.GeboorteDatum != null &&
			                                 DateTime.Today.Year -
			                                 ((DateTime) model.PersoonLidInfo.PersoonDetail.GeboorteDatum).Year >=
			                                 Properties.Settings.Default.LoonVerliesVanafLeeftijd;
			model.PrijsVerzekeringLoonVerlies = Properties.Settings.Default.PrijsVerzekeringLoonVerlies;

			model.Titel = model.PersoonLidInfo.PersoonDetail.VolledigeNaam;
			return View("EditRest", model);
		}

		/// <summary>
		/// Bekijkt model.HuidigLid.  Haalt alle afdelingen van het groepswerkjaar van het lid op, en
		/// bewaart ze in model.AlleAfdelingen.  In model.AfdelingIDs komen de ID's van de toegekende
		/// afdelingen voor het lid.
		/// </summary>
		/// <param name="model"></param>
		private void AfdelingenOphalen(PersonenLedenModel model)
		{
			if (model.PersoonLidInfo.LidInfo != null)
			{
				model.AlleAfdelingen = ServiceHelper.CallService<IGroepenService, IList<AfdelingDetail>>
				(groep => groep.ActieveAfdelingenOphalen(model.PersoonLidInfo.LidInfo.GroepsWerkJaarID));
			}
		}

		#endregion personen

		#region leden

		// GET: /Personen/LidMaken/gelieerdepersoonID
		public ActionResult LidMaken(int gelieerdepersoonID, int groepID)
		{
			IList<int> ids = new List<int> { gelieerdepersoonID };
			string foutBerichten = String.Empty;

			ServiceHelper.CallService<ILedenService, IEnumerable<int>>(l => l.LedenMaken(ids, LidType.Kind, out foutBerichten));
			TempData["feedback"] = String.IsNullOrEmpty(foutBerichten) ? Properties.Resources.LidGemaaktFeedback : string.Concat(Properties.Resources.LidMakenMisluktFout, Environment.NewLine, foutBerichten);

			return TerugNaarVorigeFiche();
		}

		// GET: /Personen/LeidingMaken/gelieerdepersoonID
		public ActionResult LeidingMaken(int gelieerdepersoonID, int groepID)
		{
			IList<int> ids = new List<int> { gelieerdepersoonID };
			string foutBerichten = String.Empty;

			ServiceHelper.CallService<ILedenService, IEnumerable<int>>(l => l.LedenMaken(ids, LidType.Leiding, out foutBerichten));
			TempData["feedback"] = String.IsNullOrEmpty(foutBerichten) ? Properties.Resources.LidGemaaktFeedback : string.Concat(Properties.Resources.LidMakenMisluktFout, Environment.NewLine, foutBerichten);

			return TerugNaarVorigeFiche();
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
		public ActionResult Verhuizen(int id, int aanvragerID, int groepID)
		{
			var model = new AdresModel();
			BaseModelInit(model, groepID);

			model.AanvragerID = aanvragerID;
			var a = ServiceHelper.CallService<IGelieerdePersonenService, GezinInfo>(l => l.GezinOphalen(id, groepID));

			Mapper.CreateMap<GezinInfo, PersoonsAdresInfo>()
				.ForMember(
					dst => dst.AdresType,
					opt => opt.Ignore());  // voorlopig negeren we adrestype; dat nemen we direct nog over.

			model.PersoonsAdresInfo = Mapper.Map<GezinInfo, PersoonsAdresInfo>(a);

			model.OudAdresID = id;
			model.PersoonsAdresInfo.AdresType = (from bewoner in a.Bewoners
												 where bewoner.GelieerdePersoonID == aanvragerID
												 select bewoner.AdresType).FirstOrDefault();

			model.WoonPlaatsen = _adressenHelper.WoonPlaatsenOphalen(a.PostNr);

			// Standaard verhuist iedereen mee.
			model.GelieerdePersoonIDs = (from b in a.Bewoners
										 select b.GelieerdePersoonID).ToList();

			model.Bewoners = (from p in a.Bewoners
							  select new CheckBoxListInfo(
								p.GelieerdePersoonID.ToString(),
								p.PersoonVolledigeNaam,
								model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID))).ToArray();

			model.Titel = "Personen Verhuizen";
			return View("AdresBewerken", model);
		}

		/// <summary>
		/// Ook in de view Verhuizen krijg je - indien javascript niet werkt - een knop
		/// 'Woonplaatsen ophalen', waarmee het lijstje met woonplaatsen moet worden gevuld.
		/// </summary>
		/// <param name="model">informatie over het nieuw adres</param>
		/// <param name="groepID">ID van de geselecteerde groep</param>
		/// <returns>Opnieuw de view AdresBewerken, maar met het lijstje woonplaatsen ingevuld</returns>
		[ActionName("Verhuizen")]
		[AcceptVerbs(HttpVerbs.Post)]
		[ParameterAccepteren(Naam = "action", Waarde = "Woonplaatsen ophalen")]
		public ActionResult Verhuizen_WoonplaatsenOphalen(AdresModel model, int groepID)
		{
			// TODO: Deze method is identiek aan NieuwAdres_WoonPlaatsenOphalen.
			// Dat moet dus niet dubbel geschreven zijn

			BaseModelInit(model, groepID);
			var bewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<BewonersInfo>>(svc => svc.HuisGenotenOphalenZelfdeGroep(model.AanvragerID));

			model.Bewoners = (from p in bewoners
							  select new CheckBoxListInfo(
								  p.GelieerdePersoonID.ToString(),
								  p.PersoonVolledigeNaam,
								  model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID))).ToArray();

			model.WoonPlaatsen = _adressenHelper.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);

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
		public ActionResult Verhuizen(AdresModel model, int groepID)
		{
			try
			{
				// De service zal het meegeleverder model.NaarAdres.ID negeren, en 
				// opnieuw opzoeken.
				//
				// Adressen worden nooit gewijzigd, enkel bijgemaakt.  (en eventueel verwijderd.)

				ServiceHelper.CallService<IGelieerdePersonenService>(l => l.GelieerdePersonenVerhuizen(model.GelieerdePersoonIDs, model.PersoonsAdresInfo, model.OudAdresID));

				// Toon een persoon die woont op het nieuwe adres.
				// (wat hier moet gebeuren hangt voornamelijk af van de use case)

				return TerugNaarVorigeFiche();
			}
			catch (FaultException<OngeldigObjectFault> ex)
			{
				BaseModelInit(model, groepID);

				new ModelStateWrapper(ModelState).BerichtenToevoegen(ex.Detail, "PersoonsAdresInfo.");

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

				model.WoonPlaatsen = _adressenHelper.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);
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
									  model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID),
									  probleemPersIDs.Contains(p.PersoonID) ? Properties.Resources.WoontDaarAl : string.Empty)).ToArray();

				model.WoonPlaatsen = _adressenHelper.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);
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

		// POST: /Personen/AdresVerwijderen
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AdresVerwijderen(AdresVerwijderenModel model, int groepID)
		{
			BaseModelInit(model, groepID);
			ServiceHelper.CallService<IGelieerdePersonenService>(foo => foo.AdresVerwijderenVanPersonen(model.PersoonIDs, model.Adres.ID));
			return RedirectToAction("EditRest", new { id = model.AanvragerID });
		}

		/// <summary>
		/// Laat toe een nieuw adres te koppelen aan een gelieerde persoon.
		/// </summary>
		/// <param name="id">GelieerdePersoonID van de persoon die een nieuw adres moet krijgen</param>
		/// <param name="groepID">ID van de huidige geselecteerde groep</param>
		/// <returns>De view 'AdresBewerken'</returns>
		public ActionResult NieuwAdres(int id, int groepID)
		{
			var model = new AdresModel();
			BaseModelInit(model, groepID);

			model.AanvragerID = id;
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
		/// <param name="model">informatie over het nieuw adres</param>
		/// <param name="groepID">ID van de geselecteerde groep</param>
		/// <returns>Opnieuw de view AdresBewerken, maar met het lijstje woonplaatsen ingevuld</returns>
		[ActionName("NieuwAdres")]
		[AcceptVerbs(HttpVerbs.Post)]
		[ParameterAccepteren(Naam = "action", Waarde = "Woonplaatsen ophalen")]
		public ActionResult NieuwAdres_WoonplaatsenOphalen(AdresModel model, int groepID)
		{
			BaseModelInit(model, groepID);
			var bewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<BewonersInfo>>(svc => svc.HuisGenotenOphalenZelfdeGroep(model.AanvragerID));

			model.Bewoners = (from p in bewoners
							  select new CheckBoxListInfo(
								  p.GelieerdePersoonID.ToString(),
								  p.PersoonVolledigeNaam,
								  model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID))).ToArray();

			model.WoonPlaatsen = _adressenHelper.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);

			return View("AdresBewerken", model);
		}

		/// <summary>
		/// Actie voor post van nieuw adres
		/// </summary>
		/// <param name="model">bevat de geposte informatie</param>
		/// <param name="groepID">ID van huidig geselecteerde groep</param>
		/// <returns>Zonder problemen wordt geredirect naar de actie 'persoon bewerken'.  Maar
		/// bij een ongeldig adres krijg je opnieuw de view 'AdresBewerken'.</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		[ParameterAccepteren(Naam = "action", Waarde = "Bewaren")]
		public ActionResult NieuwAdres(AdresModel model, int groepID)
		{
			try
			{
				// De service zal model.NieuwAdres.ID negeren; dit wordt
				// steeds opnieuw opgezocht.  Adressen worden nooit
				// gewijzigd, enkel bijgemaakt (en eventueel verwijderd.)

				ServiceHelper.CallService<IGelieerdePersonenService>(l => l.AdresToevoegenGelieerdePersonen(model.GelieerdePersoonIDs, model.PersoonsAdresInfo, model.Voorkeur));

				// return TerugNaarVorigeFiche();

				// Door het AanvragerID te gebruiken ipv het tijdelijke cookie (uit TerugNaarVorigeFiche) werkt de
				// redirect ook naar behoren als ondertussen in een andere tab een andere persoon bekeken is.

				return RedirectToAction("EditRest", new { id = model.AanvragerID });
			}
			catch (FaultException<OngeldigObjectFault> ex)
			{
				BaseModelInit(model, groepID);

				new ModelStateWrapper(ModelState).BerichtenToevoegen(ex.Detail, "PersoonsAdresInfo.");

				// De mogelijke bewoners zijn op dit moment vergeten, en moeten dus
				// terug opgevraagd worden.
				var bewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<BewonersInfo>>(l => l.HuisGenotenOphalenZelfdeGroep(model.AanvragerID));

				model.Bewoners = (from p in bewoners
								  select new CheckBoxListInfo(
									  p.GelieerdePersoonID.ToString(),
									  p.PersoonVolledigeNaam,
									  model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID))).ToArray();

				model.WoonPlaatsen = _adressenHelper.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);
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
									  model.GelieerdePersoonIDs.Contains(p.GelieerdePersoonID),
									  probleemPersIDs.Contains(p.PersoonID) ? Properties.Resources.WoontDaarAl : string.Empty)).ToArray();

				model.WoonPlaatsen = _adressenHelper.WoonPlaatsenOphalen(model.PersoonsAdresInfo.PostNr);

				return View("AdresBewerken", model);
			}
		}

		public ActionResult VoorkeurAdresMaken(int persoonsAdresID, int gelieerdePersoonID, int groepID)
		{
			ServiceHelper.CallService<IGelieerdePersonenService>(l => l.VoorkeursAdresMaken(persoonsAdresID, gelieerdePersoonID));

			return TerugNaarVorigeFiche();
		}

		#endregion adressen

		#region commvormen

		// GET: /Personen/NieuweCommVorm/gelieerdePersoonID
		public ActionResult NieuweCommVorm(int gelieerdePersoonID, int groepID)
		{
			var persoonDetail = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailsOphalen(gelieerdePersoonID));
			var types = ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<CommunicatieTypeInfo>>(l => l.CommunicatieTypesOphalen());
			var model = new NieuweCommVormModel(persoonDetail, types);
			BaseModelInit(model, groepID);
			model.Titel = "Nieuwe communicatievorm toevoegen";
			return View("NieuweCommVorm", model);
		}

		// post: /Personen/NieuweCommVorm
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult NieuweCommVorm(NieuweCommVormModel model, int groepID, int gelieerdePersoonID)
		{
			// Eerst een hoop gedoe om de CommunicatieInfo uit het model in een
			// CommunicatieDetail te krijgen, zodat de validatie kan gebeuren.

			var communicatieType = ServiceHelper.CallService<
				IGelieerdePersonenService,
				CommunicatieTypeInfo>(svc => svc.CommunicatieTypeOphalen(
					model.NieuweCommVorm.CommunicatieTypeID));

			Mapper.CreateMap<CommunicatieInfo, CommunicatieInfo>()
				.ForMember(dst => dst.CommunicatieTypeValidatie, opt => opt.Ignore());

			var communicatieDetail = Mapper.Map<CommunicatieInfo, CommunicatieInfo>(
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
				model.Aanvrager = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailsOphalen(gelieerdePersoonID));
				model.Types = ServiceHelper.CallService<IGelieerdePersonenService, IEnumerable<CommunicatieTypeInfo>>(l => l.CommunicatieTypesOphalen());
				model.Titel = "Nieuwe communicatievorm toevoegen";

				return View("NieuweCommVorm", model);
			}
			else
			{
				ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CommunicatieVormToevoegen(gelieerdePersoonID, model.NieuweCommVorm));
				return TerugNaarVorigeFiche();
			}
		}

		// GET: /Personen/VerwijderenCommVorm/commvormid
		public ActionResult VerwijderenCommVorm(int commvormID, int groepID)
		{
			ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CommunicatieVormVerwijderenVanPersoon(commvormID));
			return TerugNaarVorigeFiche();
		}

		// GET: /Personen/CommVormBewerken/gelieerdePersoonID
		public ActionResult CommVormBewerken(int commvormID, int gelieerdePersoonID, int groepID)
		{
			// TODO dit is niet juist broes, want hij haalt 2 keer de persoon op?
			var persoonDetail = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailsOphalen(gelieerdePersoonID));
			var commv = ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieInfo>(l => l.CommunicatieVormOphalen(commvormID));
			var model = new CommVormModel(persoonDetail, commv);
			BaseModelInit(model, groepID);
			model.Titel = "Communicatievorm bewerken";
			return View("CommVormBewerken", model);
		}

		// TODO meerdere commvormen tegelijk

		// POST: /Personen/CommVormBewerken/gelieerdePersoonID
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult CommVormBewerken(CommVormModel model, int gelieerdePersoonID, int groepID)
		{
			var validator = new CommunicatieVormValidator();
			var commVorm = ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieInfo>(l => l.CommunicatieVormOphalen(model.NieuweCommVorm.ID));

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

				model.Aanvrager = ServiceHelper.CallService<IGelieerdePersonenService, PersoonDetail>(l => l.DetailsOphalen(gelieerdePersoonID));
				model.NieuweCommVorm = commVorm;
				model.Titel = "Communicatievorm bewerken";

				return View("CommVormBewerken", model);
			}
			else
			{
				ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CommunicatieVormAanpassen(model.NieuweCommVorm));
				return TerugNaarVorigeFiche();
			}
			// TODO catch exceptions overal
		}

		#endregion commvormen

		#region categorieën

		// GET: /Personen/VerwijderenCategorie/categorieID
		public ActionResult VerwijderenCategorie(int categorieID, int gelieerdePersoonID, int groepID)
		{
			IList<int> list = new List<int> { gelieerdePersoonID };
			ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CategorieVerwijderen(list, categorieID));
			return TerugNaarVorigeFiche();
		}

		// GET: /Personen/ToevoegenAanCategorie/categorieID
		public ActionResult ToevoegenAanCategorie(int gelieerdePersoonID, int groepID)
		{
			IList<int> l = new List<int> { gelieerdePersoonID };
			TempData.Add("list", l);
			return RedirectToAction("CategorieToevoegenAanLijst");
		}

		/// <summary>
		/// Toont de view 'CategorieToevoegen', die toelaat om personen in een categorie onder te
		/// brengen.  
		/// De ID's van onder te brengen personen worden opgevist uit TempData["list"].
		/// TODO: Kan dat niet properder?
		/// </summary>
		/// <param name="groepID">ID van de groep waarin de gebruiker momenteel aan het werken is</param>
		/// <returns>De view 'CategorieToevoegen'</returns>
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

				return View("CategorieToevoegenAanLijst", model);
			}
			else
			{
				TempData["feedback"] = Properties.Resources.CategoriserenZonderCategorieënFout;
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
		public ActionResult CategorieToevoegenAanLijst(CategorieModel model, int groepID)
		{
			ServiceHelper.CallService<IGelieerdePersonenService>(l => l.CategorieKoppelen(
				model.GelieerdePersoonIDs,
				model.GeselecteerdeCategorieIDs));

			if (model.GelieerdePersoonIDs.Count == 1)
			{
				return TerugNaarVorigeFiche();
			}
			else
			{
				return TerugNaarVorigeLijst();
			}
		}

		#endregion categorieën
	}
}