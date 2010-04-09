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
using Chiro.Gap.Fouten;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using System.ServiceModel;

namespace Chiro.Gap.WebApp.Controllers
{
	public class AfdelingController : BaseController
	{
		public AfdelingController(IServiceHelper serviceHelper) : base(serviceHelper) { }

		// GET: /Afdeling/
		public ActionResult Index(int groepID)
		{
			// Recentste groepswerkjaar ophalen, en leden tonen.
			return List(ServiceHelper.CallService<IGroepenService, int>(svc => svc.RecentsteGroepsWerkJaarIDGet(groepID)), groepID);
		}

		//
		// GET: /Afdeling/List/{groepsWerkJaarId}
		public ActionResult List(int groepsWerkJaarID, int groepID)
		{
			var model = new Models.AfdelingInfoModel();
			BaseModelInit(model, groepID);

			// AfdelingDetails voor Afdelingen die in het opgegeven werkjaar voorkomen als AfdelingsJaar
			model.GebruikteAfdelingLijst =
				ServiceHelper.CallService<IGroepenService, IList<AfdelingDetail>>
				(groep => groep.AfdelingenOphalen(groepsWerkJaarID));

			// AfdelingDetails voor Afdelingen die in het opgegeven werkjaar voorkomen als AfdelingsJaar

			model.OngebruikteAfdelingLijst
				= ServiceHelper.CallService<IGroepenService, IList<AfdelingInfo>>(svc => svc.OngebruikteAfdelingenOphalen(groepsWerkJaarID));

			model.Titel = "Afdelingen";
			return View("Index", model);
		}

		//
		// GET: /Afdeling/Nieuw
		public ActionResult Nieuw(int groepID)
		{
			var model = new Models.AfdelingInfoModel();
			BaseModelInit(model, groepID);

			model.HuidigeAfdeling = new Afdeling();

			model.Titel = Properties.Resources.NieuweAfdelingTitel; 
			return View("Nieuw", model);
		}

		//
		// POST: /Afdeling/Nieuw
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Nieuw(Models.AfdelingInfoModel model, int groepID)
		{
			model.Titel = Properties.Resources.NieuweAfdelingTitel;
			BaseModelInit(model, groepID);

			if (ModelState.IsValid)
			{
				try
				{
					ServiceHelper.CallService<IGroepenService>(e => e.AfdelingAanmaken(groepID, model.HuidigeAfdeling.Naam, model.HuidigeAfdeling.Afkorting));

					TempData["feedback"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

					// (er wordt hier geredirect ipv de view te tonen,
					// zodat je bij een 'refresh' niet de vraag krijgt
					// of je de gegevens opnieuw wil posten.)
					return RedirectToAction("Index");
				}
				catch (FaultException<BlokkerendeObjectenFault<BestaatAlFoutCode, AfdelingInfo>> ex)
				{
					switch (ex.Detail.FoutCode)
					{
						case BestaatAlFoutCode.AfdelingsCodeBestaatAl:
							{
								ModelState.AddModelError(
									"HuidigeAfdeling.Afkorting",
									string.Format(
										Properties.Resources.AfdelingsCodeBestaatAl,
										ex.Detail.Objecten.First().Afkorting,
										ex.Detail.Objecten.First().Naam));
								break;
							}
						case BestaatAlFoutCode.AfdelingsNaamBestaatAl:
							{
								ModelState.AddModelError(
									"HuidigeAfdeling.Naam",
									string.Format(
										Properties.Resources.AfdelingsNaamBestaatAl,
										ex.Detail.Objecten.First().Afkorting,
										ex.Detail.Objecten.First().Naam));
								break;
							}
						default:
							{
								// Dit was niet verwacht
								throw;
							}
					}

					return View(model);
				}
			}
			else
			{
				// Modelstate bevat ongeldige waarden; toon de pagina opnieuw

				return View(model);
			}
		}

		/// <summary>
		/// Toont de view voor het activeren van een afdeling
		/// </summary>
		/// <param name="groepID">ID van de geselecteerde groep</param>
		/// <param name="id">AfdelingID van de te activeren afdeling</param>
		/// <returns>De view 'afdelingsjaar'</returns>
		public ActionResult Activeren(int groepID, int id)
		{
			var model = new Models.AfdelingInfoModel();
			BaseModelInit(model, groepID);

			model.HuidigAfdelingsJaar = new AfdelingsJaar();
			model.OfficieleAfdelingenLijst =
				ServiceHelper.CallService<IGroepenService, IList<OfficieleAfdeling>>
				(groep => groep.OfficieleAfdelingenOphalen());

			// Afdeling van afdelingsjaar invullen
			model.HuidigeAfdeling = ServiceHelper.CallService<IGroepenService, Afdeling>
				(groep => groep.AfdelingOphalen(id));

			model.Titel = "Afdeling activeren";
			return View("AfdelingsJaar", model);
		}

		//
		// POST: /Afdeling/Activeren/afdelingID
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Activeren(Models.AfdelingInfoModel model, int groepID, int id)
		{
			BaseModelInit(model, groepID);

			// Als de gebruiker een kleiner geboortejaar 'tot' als 'van' ingeeft, wisselen we die stiekem om.  (Ticket #289)

			if (model.HuidigAfdelingsJaar.GeboorteJaarTot < model.HuidigAfdelingsJaar.GeboorteJaarVan)
			{
				int tmp = model.HuidigAfdelingsJaar.GeboorteJaarVan;
				model.HuidigAfdelingsJaar.GeboorteJaarVan = model.HuidigAfdelingsJaar.GeboorteJaarTot;
				model.HuidigAfdelingsJaar.GeboorteJaarTot = tmp;
			}

			try
			{
				ServiceHelper.CallService<IGroepenService>(e => e.AfdelingsJaarAanmaken(groepID, id, model.OfficieleAfdelingID, model.HuidigAfdelingsJaar.GeboorteJaarVan, model.HuidigAfdelingsJaar.GeboorteJaarTot));

				TempData["feedback"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

				// (er wordt hier geredirect ipv de view te tonen,
				// zodat je bij een 'refresh' niet de vraag krijgt
				// of je de gegevens opnieuw wil posten.)
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				// OPM: meestal is die boodschap (min of meer) duidelijk, maar zijn er ook gevallen waarin dat niet zo is?
				TempData["feedback"] = ex.Message.ToString();
				return RedirectToAction("Activeren");
			}
		}

		//
		// GET: /Afdeling/Verwijderen/afdelingsJaarId
		public ActionResult Verwijderen(int groepID, int id)
		{
			// Afdeling van afdelingsjaar invullen
			ServiceHelper.CallService<IGroepenService>
				(groep => groep.AfdelingsJaarVerwijderen(id));
			return RedirectToAction("Index");
		}

		//
		// GET: /Afdeling/Bewerken/afdelingsJaarId
		public ActionResult Bewerken(int groepID, int id)
		{
			var model = new Models.AfdelingInfoModel();
			BaseModelInit(model, groepID);

			model.HuidigAfdelingsJaar =
				ServiceHelper.CallService<IGroepenService, AfdelingsJaar>
				(groep => groep.AfdelingsJaarOphalen(id));
			model.OfficieleAfdelingenLijst =
				ServiceHelper.CallService<IGroepenService, IList<OfficieleAfdeling>>
				(groep => groep.OfficieleAfdelingenOphalen());
			model.HuidigeAfdeling = model.HuidigAfdelingsJaar.Afdeling;
			model.OfficieleAfdelingID = model.HuidigAfdelingsJaar.OfficieleAfdeling.ID;

			model.Titel = "Afdeling bewerken";
			return View("AfdelingsJaar", model);
		}

		//
		// POST: /Afdeling/Bewerken/afdelingsJaarId
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Bewerken(Models.AfdelingInfoModel model, int groepID, int id)
		{
			BaseModelInit(model, groepID);

			// Als de gebruiker een kleiner geboortejaar 'tot' als 'van' ingeeft, wisselen we die stiekem om.  (Ticket #289)

			if (model.HuidigAfdelingsJaar.GeboorteJaarTot < model.HuidigAfdelingsJaar.GeboorteJaarVan)
			{
				int tmp = model.HuidigAfdelingsJaar.GeboorteJaarVan;
				model.HuidigAfdelingsJaar.GeboorteJaarVan = model.HuidigAfdelingsJaar.GeboorteJaarTot;
				model.HuidigAfdelingsJaar.GeboorteJaarTot = tmp;
			}

			try
			{
				// TODO: hier (of beter: in de service) moeten dezelfde controles gebeuren als bij AfdelingActiveren - zie ticket #326
				ServiceHelper.CallService<IGroepenService>(e => e.AfdelingsJaarBewarenMetWijzigingen(id, model.OfficieleAfdelingID, model.HuidigAfdelingsJaar.GeboorteJaarVan, model.HuidigAfdelingsJaar.GeboorteJaarTot, model.HuidigAfdelingsJaar.Geslacht));

				TempData["feedback"] = Properties.Resources.WijzigingenOpgeslagenFeedback;

				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				// TODO: duidelijke foutmelding - zie #325
				TempData["feedback"] = ex.Message.ToString();
				return RedirectToAction("Bewerken");
			}
		}
	}
}
