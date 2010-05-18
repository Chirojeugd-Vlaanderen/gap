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
		public LedenController(IServiceHelper serviceHelper)
			: base(serviceHelper)
		{
		}

		//
		// GET: /Leden/
		public ActionResult Index(int groepID)
		{
			// Recentste groepswerkjaar ophalen, en leden tonen.
			return List(ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID)), 0, groepID);
		}

		// De paginering zal gebeuren per groepswerkjaar, niet per grootte van de pagina
		// Er wordt ook meegegeven welke afdeling er gevraagd is (0 is alles)
		// GET: /Leden/List/{afdID}/{groepsWerkJaarId}

		/// <summary>
		/// Toont de lijst van leden uit groepswerkjaar met GroepsWerkJaarID <paramref name="id"/>.
		/// </summary>
		/// <param name="id">ID van het gevraagde groepswerkjaar</param>
		/// <param name="afdID">Indien 0, worden alle leden getoond, anders enkel de leden uit de afdeling
		/// met het gegeven AfdelingsID</param>
		/// <param name="groepID">ID van de groep</param>
		/// <returns>De view 'Index' met de ledenlijst</returns>
		public ActionResult List(int id, int afdID, int groepID)
		{
			// Bijhouden welke lijst we laatst bekeken en op welke pagina we zaten. Paginering gebeurt hier per werkjaar.
			Sessie.LaatsteLijst = "Leden";
			Sessie.LaatsteActieID = afdID;
			Sessie.LaatstePagina = id;

			int paginas;

			var model = new LidInfoModel();
			BaseModelInit(model, groepID);

			var list =
				ServiceHelper.CallService<IGroepenService, IList<AfdelingDetail>>
				(groep => groep.AfdelingenOphalen(id));

			model.AfdelingsInfoDictionary = new Dictionary<int, AfdelingDetail>();
			foreach (AfdelingDetail ai in list)
			{
				model.AfdelingsInfoDictionary.Add(ai.AfdelingID, ai);
			}

			model.WerkJaarInfos =
				ServiceHelper.CallService<IGroepenService, IEnumerable<WerkJaarInfo>>
					(e => e.WerkJarenOphalen(groepID));

			var huidig = (from g in model.WerkJaarInfos
					 where g.ID == id
					 select g).First();

			model.GroepsWerkJaarIdZichtbaar = id;
			model.GroepsWerkJaartalZichtbaar = huidig.WerkJaar;

			if (afdID == 0)
			{
				model.LidInfoLijst =
					ServiceHelper.CallService<ILedenService, IList<PersoonLidInfo>>
					(lid => lid.PaginaOphalen(id, out paginas));

				model.Titel = "Ledenoverzicht van het werkjaar " + model.GroepsWerkJaartalZichtbaar + "-" + (model.GroepsWerkJaartalZichtbaar + 1);
			}
			else
			{
				model.LidInfoLijst =
					ServiceHelper.CallService<ILedenService, IList<PersoonLidInfo>>
					(lid => lid.PaginaOphalenVolgensAfdeling(id, afdID, out paginas));

				AfdelingDetail af = (from a in model.AfdelingsInfoDictionary.AsQueryable()
									 where a.Value.AfdelingID == afdID
									 select a.Value).FirstOrDefault();

				model.Titel = "Ledenoverzicht van de " + af.AfdelingNaam + " van het werkjaar " + model.GroepsWerkJaartalZichtbaar + "-" + (model.GroepsWerkJaartalZichtbaar + 1);
			}

			model.PageHuidig = model.GroepsWerkJaarIdZichtbaar;
			model.PageTotaal = model.LidInfoLijst.Count;
			model.HuidigeAfdeling = afdID;
			return View("Index", model);
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
			int paginas;

			if (afdID == 0)
			{
				lijst =
					ServiceHelper.CallService<ILedenService, IList<PersoonLidInfo>>
					(lid => lid.PaginaOphalen(id, out paginas));
			}
			else
			{
				lijst =
					ServiceHelper.CallService<ILedenService, IList<PersoonLidInfo>>
					(lid => lid.PaginaOphalenVolgensAfdeling(id, afdID, out paginas));
			}

			var selectie = from l in lijst
						   select new
						   {
							   AdNummer = l.PersoonDetail.AdNummer,
							   VolledigeNaam = l.PersoonDetail.VolledigeNaam,
							   GeboorteDatum = String.Format("{0:dd/MM/yyyy}", l.PersoonDetail.GeboorteDatum),
							   Geslacht = l.PersoonDetail.Geslacht == GeslachtsType.Man ? "jongen" : "meisje"
						   };

			return new ExcelResult(
				"Leden.xls",
				selectie.AsQueryable(),
				new string[] { "AdNummer", "VolledigeNaam", "GeboorteDatum", "Geslacht" });

		}

		//
		// GET: /Leden/Create
		public ActionResult Create(int groepID)
		{
			return View();
		}

		//
		// POST: /Leden/Create
        [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Create(FormCollection collection, int groepID)
		{
			try
			{
				// TODO: Add insert logic here

				return RedirectToAction("List", new
				{
					groepsWerkJaarId = Sessie.LaatstePagina,
					afdID = Sessie.LaatsteActieID
				});
			}
			catch
			{
				return View();
			}
		}

		//
		// GET: /Leden/Verwijderen/5
		public ActionResult Verwijderen(int id, int groepID)
		{
			ServiceHelper.CallService<ILedenService>(l => l.Verwijderen(id));
			TempData["feedback"] = "Lid is verwijderd";
			return RedirectToAction("List", new
			{
				groepsWerkJaarId = Sessie.LaatstePagina,
				afdID = Sessie.LaatsteActieID
			});
		}

		/// <summary>
		/// Toont de view die toelaat om de afdeling(en) van een lid te wijzigen
		/// </summary>
		/// <param name="id">LidID van het lid met de te wijzigen afdeling(en)</param>
		/// <param name="groepID">Groep waarin de user momenteel werkt</param>
		/// <returns>De view 'AfdelingBewerken'</returns>
		public ActionResult AfdelingBewerken(int id, int groepID)
		{
			var model = new LidAfdelingenModel();
			BaseModelInit(model, groepID);

			model.BeschikbareAfdelingen = ServiceHelper.CallService<IGroepenService, IEnumerable<ActieveAfdelingInfo>> (
				svc => svc.BeschikbareAfdelingenOphalen(groepID));
			model.Info = ServiceHelper.CallService<ILedenService, LidAfdelingInfo>(
				svc => svc.AfdelingenOphalen(id));


			if (model.BeschikbareAfdelingen.FirstOrDefault() == null)
			{
				// Geen afdelingen.

				// Workaround via TempData["feedback"].  Niet zeker of dat een geweldig goed
				// idee is.

				TempData["feedback"] = String.Format(
					Properties.Resources.GeenActieveAfdelingen,
					Url.Action("Index", "Afdeling", new
					{
						groepID = groepID
					}));

				return RedirectToAction("List", new
				{
					groepsWerkJaarId = Sessie.LaatstePagina,
					afdID = Sessie.LaatsteActieID
				});
			} 
			else
			{
				model.Titel = String.Format(Properties.Resources.AfdelingenAanpassen, model.Info.VolledigeNaam);
				return View("AfdelingBewerken", model);
			}
		}

		//
		// POST: /Leden/AfdelingBewerken/5
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AfdelingBewerken(LidAfdelingenModel model, int groepID, int id)
		{
			// FIXME: Het is geen prachtige code: AfdelingenVervangen die 'toevallig'
			// een GelieerdePersoonID oplevert, die ik dan in dit specifieke geval
			// 'toevallig' kan gebruiken om naar de juiste personenfiche om te schakelen.

			int gelieerdePersoonID = ServiceHelper.CallService<ILedenService, int>(
				svc => svc.AfdelingenVervangen(id, model.Info.AfdelingsJaarIDs));
			return RedirectToAction(
				"EditRest", 
				new { Controller = "Personen", id = gelieerdePersoonID});
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

			ServiceHelper.CallService<ILedenService>(l => l.Bewaren(model.HuidigLid));
			ServiceHelper.CallService<ILedenService>(l => l.FunctiesVervangen(model.HuidigLid.LidInfo.LidID, model.FunctieIDs));

			return RedirectToAction("EditRest", new
			{
				Controller = "Personen",
				id = model.HuidigLid.PersoonDetail.GelieerdePersoonID
			});
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
			model.AlleFuncties = ServiceHelper.CallService<IGroepenService, IList<FunctieInfo>>
				(svc => svc.FunctiesOphalen(
					model.HuidigLid.LidInfo.GroepsWerkJaarID,
					model.HuidigLid.LidInfo.Type));
			model.FunctieIDs = (from f in model.HuidigLid.LidInfo.Functies
								select f.ID).ToList();
		}

	}
}
