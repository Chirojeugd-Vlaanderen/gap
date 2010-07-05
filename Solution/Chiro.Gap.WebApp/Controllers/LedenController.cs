// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	public class LedenController : BaseController
	{
		public LedenController(IServiceHelper serviceHelper) : base(serviceHelper) { }

		// GET: /Leden/
		public override ActionResult Index(int groepID)
		{
			// Recentste groepswerkjaar ophalen, en leden tonen.
			return AfdelingsLijst(ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID)), 0, groepID);
		}

		private LidInfoModel LijstModelInitialiseren(int groepsWerkJaarID, int groepID)
		{
			// Er wordt een nieuwe lijst opgevraagd, dus wordt deze vanaf hier bijgehouden als de lijst om terug naar te springen
			ClientState.VorigeLijst = Request.Url.ToString();

			var model = new LidInfoModel();
			BaseModelInit(model, groepID);

			// TODO check dat het gegeven groepswerkjaar id wel degelijk van de gegeven groep is

			// Laadt de lijst van werkjaren in van de groep en zet de juiste info over het te tonen werkjaar
			model.WerkJaarInfos = ServiceHelper.CallService<IGroepenService, IEnumerable<WerkJaarInfo>>(e => e.WerkJarenOphalen(groepID));

			var gevraagdwerkjaar = (from g in model.WerkJaarInfos
									where g.ID == groepsWerkJaarID
									select g).First();

			model.IDGetoondGroepsWerkJaar = groepsWerkJaarID;
			model.JaartalGetoondGroepsWerkJaar = gevraagdwerkjaar.WerkJaar;

			// Haal alle afdelingsjaren op van de groep in het groepswerkjaar
			var list = ServiceHelper.CallService<IGroepenService, IList<AfdelingDetail>>(groep => groep.AfdelingenOphalen(groepsWerkJaarID));

			// Laadt de afdelingen in het model in via een dictionary
			model.AfdelingsInfoDictionary = new Dictionary<int, AfdelingDetail>();
			foreach (AfdelingDetail ai in list)
			{
				model.AfdelingsInfoDictionary.Add(ai.AfdelingID, ai);
			}

			// Haal alle functies op van de groep in het groepswerkjaar
			var list2 = ServiceHelper.CallService<IGroepenService, IEnumerable<FunctieInfo>>(groep => groep.FunctiesOphalen(groepsWerkJaarID, LidType.Alles));

			model.FunctieInfoDictionary = new Dictionary<int, FunctieInfo>();
			foreach (FunctieInfo fi in list2)
			{
				model.FunctieInfoDictionary.Add(fi.ID, fi);
			}

			model.PageHuidig = model.IDGetoondGroepsWerkJaar;
			model.PageTotaal = model.WerkJaarInfos.Count();

			return model;
		}

		public ActionResult Lijst(int groepsWerkJaarID, int groepID)
		{
			if (groepsWerkJaarID == 0)
			{
				groepsWerkJaarID = ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID));
			}

			var model = LijstModelInitialiseren(groepsWerkJaarID, groepID);

			// TODO check dat de gegeven afdeling id wel degelijk van de gegeven groep is

			model.LidInfoLijst =
				ServiceHelper.CallService<ILedenService, IList<PersoonLidInfo>>
				(lid => lid.PaginaOphalen(groepsWerkJaarID));

			model.Titel = "Ledenoverzicht van het werkjaar " + model.JaartalGetoondGroepsWerkJaar + "-" + (model.JaartalGetoondGroepsWerkJaar + 1);

			return View("Index", model);
		}

		// GET: /Leden/AfdelingsLijst/{afdID}/{id}
		/// <summary>
		/// Toont de lijst van leden uit groepswerkjaar met GroepsWerkJaarID <paramref name="groepsWerkJaarID"/> volgens een gekozen afdeling. De paginering gebeurt per groepswerkjaar, niet per grootte van de pagina
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het gevraagde groepswerkjaar</param>
		/// <param name="afdID">Indien 0, worden alle leden getoond, anders enkel de leden uit de afdeling met het gegeven AfdelingsID</param>
		/// <param name="groepID">ID van de groep</param>
		/// <returns>De view 'Index' met een ledenlijst</returns>
		public ActionResult AfdelingsLijst(int groepsWerkJaarID, int afdID, int groepID)
		{
			if (afdID == 0)
			{
				return Lijst(groepsWerkJaarID, groepID);
			}

			var model = LijstModelInitialiseren(groepsWerkJaarID, groepID);

			// TODO check dat de gegeven afdeling id wel degelijk van de gegeven groep is

			model.LidInfoLijst = ServiceHelper.CallService<ILedenService, IList<PersoonLidInfo>>(lid => lid.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdID));

			AfdelingDetail af = (from a in model.AfdelingsInfoDictionary.AsQueryable()
								 where a.Value.AfdelingID == afdID
								 select a.Value).FirstOrDefault();

			model.Titel = "Ledenoverzicht van de " + af.AfdelingNaam + " van het werkjaar " + model.JaartalGetoondGroepsWerkJaar + "-" + (model.JaartalGetoondGroepsWerkJaar + 1);

			model.HuidigeAfdeling = afdID;
			return View("Index", model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AfdelingsLijst(LidInfoModel model, int groepID)
		{
			return RedirectToAction("AfdelingsLijst", new { groepsWerkJaarID = model.IDGetoondGroepsWerkJaar, afdID = model.GekozenAfdeling, groepID = groepID });
		}

		public ActionResult FunctieLijst(int groepsWerkJaarID, int funcID, int groepID)
		{
			if (funcID == 0)
			{
				return Lijst(groepsWerkJaarID, groepID);
			}

			var model = LijstModelInitialiseren(groepsWerkJaarID, groepID);

			// TODO check dat de gegeven functie id wel degelijk van de gegeven groep is

			model.LidInfoLijst = ServiceHelper.CallService<ILedenService, IList<PersoonLidInfo>>(lid => lid.PaginaOphalenVolgensFunctie(groepsWerkJaarID, funcID));

			// TODO functienaam
			model.Titel = "Ledenoverzicht van leden met functie TODO in het werkjaar " + model.JaartalGetoondGroepsWerkJaar + "-" + (model.JaartalGetoondGroepsWerkJaar + 1);

			// TODO naar volged werkjaar kunnen gaan met behoud van functie
			return View("Index", model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FunctieLijst(LidInfoModel model, int groepID)
		{
			return RedirectToAction("FunctieLijst", new { groepsWerkJaarID = model.IDGetoondGroepsWerkJaar, funcID = model.GekozenFunctie, groepID = groepID });
		}

		/// <summary>
		/// Downloadt de lijst van leden uit groepswerkjaar met GroepsWerkJaarID <paramref name="id"/> als
		/// Exceldocument.
		/// </summary>
		/// <param name="id">ID van het gevraagde groepswerkjaar</param>
		/// <param name="afdID">Indien 0, worden alle leden getoond, anders enkel de leden uit de afdeling
		/// met het gegeven AfdelingsID</param>
		/// <param name="groepID">ID van de groep</param>
		/// <returns>Exceldocument met gevraagde ledenlijst</returns>
		public ActionResult Download(int id, int afdID, int groepID)
		{
			IEnumerable<PersoonLidInfo> lijst;

			if (afdID == 0)
			{
				lijst =
					ServiceHelper.CallService<ILedenService, IList<PersoonLidInfo>>
					(lid => lid.PaginaOphalen(id));
			}
			else
			{
				lijst =
					ServiceHelper.CallService<ILedenService, IList<PersoonLidInfo>>
					(lid => lid.PaginaOphalenVolgensAfdeling(id, afdID));
			}

			var selectie = (from l in lijst
							select new
									{
										AdNummer = l.PersoonDetail.AdNummer,
										VolledigeNaam = l.PersoonDetail.VolledigeNaam,
										GeboorteDatum = l.PersoonDetail.GeboorteDatum,
										Geslacht = l.PersoonDetail.Geslacht == GeslachtsType.Man ? "jongen" : "meisje"
									}).AsQueryable();

			var stream = (new ExcelManip()).ExcelTabel(selectie, it => it.AdNummer, it => it.VolledigeNaam, it => it.GeboorteDatum, it => it.Geslacht);
			return new ExcelResult(stream, "leden.xlsx");
		}

		// id = lidid
		// GET: /Leden/DeActiveren/id
		public ActionResult DeActiveren(int id, int groepID)
		{
			ServiceHelper.CallService<ILedenService>(l => l.NonActiefMaken(id));
			TempData["feedback"] = "Lid is op non-actief gezet.";

			return TerugNaarVorigeLijst();
		}

		// id = lidid
		// GET: /Leden/Activeren/id
		public ActionResult Activeren(int id, int gwjID, int groepID)
		{
			ServiceHelper.CallService<ILedenService>(l => l.ActiefMaken(id));
			TempData["feedback"] = "Lid is weer geactiveerd.";

			return TerugNaarVorigeLijst();
		}

		/// <summary>
		/// Toont de view die toelaat om de afdeling(en) van een lid te wijzigen
		/// </summary>
		/// <param name="lidID">LidID van het lid met de te wijzigen afdeling(en)</param>
		/// <param name="groepsWerkJaarID">Groepswerkjaar waarin de wijziging moet gebeuren</param>
		/// <param name="groepID">Groep waarin de user momenteel werkt</param>
		/// <returns>De view 'AfdelingBewerken'</returns>
		public ActionResult AfdelingBewerken(int lidID, int groepsWerkJaarID, int groepID)
		{
			var model = new LidAfdelingenModel();
			BaseModelInit(model, groepID);

			model.BeschikbareAfdelingen = ServiceHelper.CallService<IGroepenService, IEnumerable<ActieveAfdelingInfo>>(
				svc => svc.BeschikbareAfdelingenOphalen(groepID));
			model.Info = ServiceHelper.CallService<ILedenService, LidAfdelingInfo>(
				svc => svc.AfdelingenOphalen(lidID));

			if (model.BeschikbareAfdelingen.FirstOrDefault() == null)
			{
				// Geen afdelingen.

				// Workaround via TempData["feedback"].  Niet zeker of dat een geweldig goed
				// idee is.

				TempData["feedback"] = String.Format(
					Properties.Resources.GeenActieveAfdelingen,
					Url.Action("Index", "Afdelingen", new
					{
						groepID = groepID
					}));

				return TerugNaarVorigeLijst();
			}
			else
			{
				model.Titel = String.Format(Properties.Resources.AfdelingenAanpassen, model.Info.VolledigeNaam);
				return View("AfdelingBewerken", model);
			}
		}

		// 
		// POST: /Leden/AfdelingBewerken/5
		// FIXME lidID wordt automatisch ingevuld als er eenzelfde argument in de GET methode van afdelingBewerken staat. Dit is eigenlijk helemaal niet mooi want wordt niet geverifieerd en zelfs 2de niveau afhankelijkheid van aspx.
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AfdelingBewerken(LidAfdelingenModel model, int groepID, int lidID)
		{
			// FIXME: Het is geen prachtige code: AfdelingenVervangen die 'toevallig'
			// een GelieerdePersoonID oplevert, die ik dan in dit specifieke geval
			// 'toevallig' kan gebruiken om naar de juiste personenfiche om te schakelen.

			int gelieerdePersoonID = ServiceHelper.CallService<ILedenService, int>(
				svc => svc.AfdelingenVervangen(lidID, model.Info.AfdelingsJaarIDs));
			return TerugNaarVorigeFiche();
		}

		/// <summary>
		/// Toont een view die toelaat de lidgegevens van het lid met LidID <paramref name="id"/> te bewerken.
		/// </summary>
		/// <param name="id">LidID te bewerken lid</param>
		/// <param name="groepID">ID van de huidig geselecteerde groep</param>
		/// <returns>De view 'EditLidGegevens'</returns>
		public ActionResult EditLidGegevens(int id, int groepID)
		{
			var model = new LedenModel();
			BaseModelInit(model, groepID);

			model.HuidigLid = ServiceHelper.CallService<ILedenService, PersoonLidInfo>(l => l.DetailsOphalen(id));

			if (model.HuidigLid != null)
			{
				// Ik had liever hierboven nog eens LidExtras.AlleAfdelingen meegegeven, maar
				// het datacontract (LidInfo) voorziet daar niets voor.

				AfdelingenOphalen(model);
				FunctiesOphalen(model);

				model.Titel = String.Format(
					"{0}: {1}",
					model.HuidigLid.PersoonDetail.VolledigeNaam,
					Properties.Resources.LidGegevens);

				return View("EditLidGegevens", model);
			}
			else
			{
				TempData["feedback"] = "Er is iets foutgelopen toen we de gegevens wilden opvragen.";
				return RedirectToAction("Index", groepID);
			}
		}

		/// <summary>
		/// Bewaart (niet) actief, dp-abonnement, probeerperiode en functies
		/// </summary>
		/// <param name="model">LedenModel met te bewaren gegevens (functie-ID's in <c>model.FunctieIDs</c>)</param>
		/// <param name="groepID">ID van de groep waarin de user momenteel aan het werken is</param>
		/// <returns>De personenfiche, die de gewijzigde info toont.</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult EditLidGegevens(LedenModel model, int groepID)
		{
			// TODO: Dit moet een unitaire operatie zijn, om concurrencyproblemen te vermijden.
			try
			{
				ServiceHelper.CallService<ILedenService>(l => l.Bewaren(model.HuidigLid));
				ServiceHelper.CallService<ILedenService>(l => l.FunctiesVervangen(model.HuidigLid.LidInfo.LidID, model.FunctieIDs));
				TempData["feedback"] = Properties.Resources.WijzigingenOpgeslagenFeedback;
			}
			catch (Exception)
			{
				TempData["feedback"] = Properties.Resources.WijzigingenNietOpgeslagenFout;
			}

			return RedirectToAction("EditLidGegevens");
		}

		/// <summary>
		/// Bekijkt model.HuidigLid.  Haalt alle afdelingen van het groepswerkjaar van het lid op, en
		/// bewaart ze in model.AlleAfdelingen.  In model.AfdelingIDs komen de ID's van de toegekende
		/// afdelingen voor het lid.
		/// </summary>
		/// <param name="model"></param>
		public void AfdelingenOphalen(LedenModel model)
		{
			model.AlleAfdelingen = ServiceHelper.CallService<IGroepenService, IList<AfdelingDetail>>
				(svc => svc.AfdelingenOphalen(model.HuidigLid.LidInfo.GroepsWerkJaarID));

			model.AfdelingIDs = model.HuidigLid.LidInfo.AfdelingIdLijst.ToList();
		}

		/// <summary>
		/// Bekijkt model.HuidigLid.  Haalt alle functies van het groepswerkjaar van het lid op, relevant
		/// voor het type lid (kind/leiding), en bewaart ze in model.AlleFuncties.  
		/// In model.FunctieIDs komen de ID's van de toegekende functies voor het lid.
		/// </summary>
		/// <param name="model">Te bewerken model</param>
		public void FunctiesOphalen(LedenModel model)
		{
			model.AlleFuncties = ServiceHelper.CallService<IGroepenService, IEnumerable<FunctieInfo>>
				(svc => svc.FunctiesOphalen(
					model.HuidigLid.LidInfo.GroepsWerkJaarID,
					model.HuidigLid.LidInfo.Type));
			model.FunctieIDs = (from f in model.HuidigLid.LidInfo.Functies
								select f.ID).ToList();
		}
	}
}
