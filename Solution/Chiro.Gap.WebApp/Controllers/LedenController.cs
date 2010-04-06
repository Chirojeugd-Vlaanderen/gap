// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	public class LedenController : BaseController
	{
		public LedenController(IServiceHelper serviceHelper) : base(serviceHelper) { }

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

			int paginas = 0;

			var model = new Models.LidInfoModel();
			BaseModelInit(model, groepID);

			var list =
				ServiceHelper.CallService<IGroepenService, IList<AfdelingInfo>>
				(groep => groep.AfdelingenOphalen(id));

			model.AfdelingsInfoDictionary = new Dictionary<int, AfdelingInfo>();
			foreach (AfdelingInfo ai in list)
			{
				model.AfdelingsInfoDictionary.Add(ai.AfdelingID, ai);
			}

			model.GroepsWerkJaarLijst =
				ServiceHelper.CallService<IGroepenService, IList<GroepsWerkJaar>>
					(e => e.WerkJarenOphalen(groepID));

			GroepsWerkJaar huidig = (from g in model.GroepsWerkJaarLijst
									 where g.ID == id
									 select g).FirstOrDefault();

			model.GroepsWerkJaarIdZichtbaar = id;
			model.GroepsWerkJaartalZichtbaar = huidig.WerkJaar;

			if (afdID == 0)
			{
				model.LidInfoLijst =
					ServiceHelper.CallService<ILedenService, IList<LidInfo>>
					(lid => lid.PaginaOphalen(id, out paginas));

				model.Titel = "Ledenoverzicht van het werkjaar " + model.GroepsWerkJaartalZichtbaar + "-" + (int)(model.GroepsWerkJaartalZichtbaar+1);
			}
			else
			{
				model.LidInfoLijst =
					ServiceHelper.CallService<ILedenService, IList<LidInfo>>
					(lid => lid.PaginaOphalenVolgensAfdeling(id, afdID, out paginas));

				AfdelingInfo af = (from a in model.AfdelingsInfoDictionary.AsQueryable()
								   where a.Value.AfdelingID == afdID
								   select a.Value).FirstOrDefault();

				model.Titel = "Ledenoverzicht van de " + af.Naam + " van het werkjaar " + model.GroepsWerkJaartalZichtbaar + "-" + (int)(model.GroepsWerkJaartalZichtbaar + 1);
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
			IEnumerable<LidInfo> lijst;
			int paginas;

			if (afdID == 0)
			{
				lijst =
					ServiceHelper.CallService<ILedenService, IList<LidInfo>>
					(lid => lid.PaginaOphalen(id, out paginas));			}
			else
			{
				lijst =
					ServiceHelper.CallService<ILedenService, IList<LidInfo>>
					(lid => lid.PaginaOphalenVolgensAfdeling(id, afdID, out paginas));
			}

			var selectie = from l in lijst
				       select new
				       {
					       AdNummer = l.PersoonInfo.AdNummer,
					       VolledigeNaam = l.PersoonInfo.VolledigeNaam,
					       GeboorteDatum = String.Format("{0:dd/MM/yyyy}", l.PersoonInfo.GeboorteDatum),
					       Geslacht = l.PersoonInfo.Geslacht == GeslachtsType.Man ? "jongen" : "meisje"
				       };

			return new ExcelResult(
				"Leden.xls",
				selectie.AsQueryable(),
				new string[] { "AdNummer", "VolledigeNaam", "GeboorteDatum", "Geslacht"});

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

				return RedirectToAction("List", new { groepsWerkJaarId = Sessie.LaatstePagina, afdID = Sessie.LaatsteActieID });
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
			return RedirectToAction("List", new { groepsWerkJaarId = Sessie.LaatstePagina, afdID = Sessie.LaatsteActieID });
		}

		/// <summary>
		/// Toont de view die toelaat om de afdeling(en) van een lid te wijzigen
		/// </summary>
		/// <param name="id">LidID van het lid met de te wijzigen afdeling</param>
		/// <param name="groepID"></param>
		/// <returns></returns>
		public ActionResult AfdelingBewerken(int id, int groepID)
		{
			var model = new LedenModel();
			BaseModelInit(model, groepID);

			model.HuidigLid = ServiceHelper.CallService<ILedenService, LidInfo>
				(l => l.Ophalen(id, LidExtras.Groep|LidExtras.Afdelingen));

			AfdelingenOphalen(model);

			if (model.AlleAfdelingen.FirstOrDefault() == null)
			{
				// Geen afdelingen.

				// Workaround via TempData["feedback"].  Niet zeker of dat een geweldig goed
				// idee is.

				TempData["feedback"] = String.Format(
					Properties.Resources.GeenActieveAfdelingen,
					Url.Action("Index", "Afdeling", new { groepID = groepID }));
					

				return RedirectToAction("List", new { groepsWerkJaarId = Sessie.LaatstePagina, afdID = Sessie.LaatsteActieID });
			}
			else
			{
				model.Titel = "Afdelingen van " + model.HuidigLid.PersoonInfo.VolledigeNaam + "aanpassen";
				return View("AfdelingBewerken", model);
			}
		}

		//
		// POST: /Leden/AfdelingBewerken/5
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AfdelingBewerken(LedenModel model, int groepID)
		{
			IList<int> selectie = new List<int>();
			if (model.HuidigLid.Type == LidType.Kind)
			{
				selectie.Add(model.AfdelingID);
			}
			else
			{
				if (model.AfdelingIDs != null) // dit komt erop neer dat er iets geselecteerd
				{
					selectie = model.AfdelingIDs;
				}
			}

			try
			{
				int x = selectie.Count;
				LidInfo tebewarenlid = model.HuidigLid;
				tebewarenlid.AfdelingIdLijst = selectie;
				ServiceHelper.CallService<ILedenService>(e => e.BewarenMetAfdelingen(tebewarenlid));
				//TODO handle exception
				return RedirectToAction("EditRest", new { Controller = "Personen", id = model.HuidigLid.PersoonInfo.GelieerdePersoonID});
			}
			catch
			{
				return RedirectToAction("List", new { groepsWerkJaarId = Sessie.LaatstePagina, afdID = Sessie.LaatsteActieID });
			}
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

			model.HuidigLid = ServiceHelper.CallService<ILedenService, LidInfo>(l => l.Ophalen(
				id, 
				LidExtras.Groep|LidExtras.Afdelingen));
			
			// @broes: Hier wordt geloof ik nog niets mee gedaan, maar ik ben er wel voorstander
			// van om het wijzigen van afdelingen bij onder te brengen onder het bewerken van
			// lidgegevens.

			AfdelingenOphalen(model);

			model.Titel = String.Format(
				"{0}: {1}", 
				model.HuidigLid.PersoonInfo.VolledigeNaam,
				Properties.Resources.LidGegevens);

			return View("EditLidGegevens", model);
		}

		//
		// POST: /Leden/EditRest/{lidID}
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult EditLidGegevens(LedenModel model, int groepID)
		{
			//FIXME: bewaren gaat geen groep en afdelingen inladen, wat dus fout is (want de GET methode doet dit wel)
			ServiceHelper.CallService<ILedenService>(l => l.Bewaren(model.HuidigLid));
			//InladenAfdelingsNamen(model);
			//return View("EditRest", model);
			//return RedirectToAction("EditRest", new { lidID=model.HuidigLid.LidID, groepID=groepID});
			return RedirectToAction("EditRest", new { Controller = "Personen", id = model.HuidigLid.PersoonInfo.GelieerdePersoonID });
		}

		/// <summary>
		/// Bekijkt model.HuidigLid.  Haalt alle afdelingen van het groepswerkjaar van het lid op, en
		/// bewaart ze in model.AlleAfdelingen.  In model.AfdelingIDs komen de ID's van de toegekende
		/// afdelingen voor het lid.
		/// </summary>
		/// <param name="model"></param>
		public void AfdelingenOphalen(LedenModel model)
		{
			model.AlleAfdelingen = ServiceHelper.CallService<IGroepenService, IList<AfdelingInfo>>
				(groep => groep.AfdelingenOphalen(model.HuidigLid.GroepsWerkJaarID));

			model.AfdelingIDs = model.HuidigLid.AfdelingIdLijst.ToList();
		}
	}
}
