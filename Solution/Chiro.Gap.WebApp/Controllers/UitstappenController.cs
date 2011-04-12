using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	public class UitstappenController : BaseController
	{
		public UitstappenController(IServiceHelper serviceHelper, IVeelGebruikt veelGebruikt) : base(serviceHelper, veelGebruikt)
		{
		}

		/// <summary>
		/// Overzicht val alle uitstappen (ever) van een groep
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <returns>View met uistapoverzicht</returns>
		public override ActionResult Index(int groepID)
		{
			var model = new UitstapOverzichtModel();
			BaseModelInit(model, groepID);
			model.Titel = Properties.Resources.Uitstappen;
			model.Uitstappen =
				ServiceHelper.CallService<IUitstappenService, IEnumerable<UitstapInfo>>(svc => svc.OphalenVanGroep(groepID));
			return View(model);
		}

		/// <summary>
		/// Formulier om nieuwe uitstap te registreren
		/// </summary>
		/// <param name="groepID">ID van groep waarvoor uitstap moet worden geregistreerd</param>
		/// <returns>Het formulier</returns>
		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult Nieuw(int groepID)
		{
			var model = new UitstapModel();
			BaseModelInit(model, groepID);
			model.Titel = Properties.Resources.NieuweUitstap;
			model.Uitstap = new UitstapDetail();

			return View("Bewerken", model);
		}

		/// <summary>
		/// Verwerkt de gegevens uit het nieuwe-uitstapformulier
		/// </summary>
		/// <param name="model">Ingevlude gegevens</param>
		/// <param name="groepID">Groep waarvoor uitstap moet worden bewaard</param>
		/// <returns>Als alles gelukt is, de details van de toegevoegde uitstap.  Anders 
		/// opnieuw het formulier met feedback over de problemen.</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Nieuw(UitstapModel model, int groepID)
		{
			BaseModelInit(model, groepID);
			model.Titel = Properties.Resources.NieuweUitstap;

			if (ModelState.IsValid)
			{
				int uitstapID = ServiceHelper.CallService<IUitstappenService, int>(svc => svc.Bewaren(groepID, model.Uitstap));
				return RedirectToAction("Bekijken", new {groepID, id = uitstapID});
			}
			else
			{
				return View("Bewerken", model);
			}
		}

		/// <summary>
		/// Toont de details van een gegeven uitstap
		/// </summary>
		/// <param name="groepID">ID van de groep die momenteel geselecteerd is</param>
		/// <param name="id">ID van de uitstap waarin we geïnteresseerd zijn</param>
		/// <returns>De details van de gegeven uitstap</returns>
		public ActionResult Bekijken(int groepID, int id)
		{
			var model = new UitstapModel();
			BaseModelInit(model, groepID);
			model.Uitstap = ServiceHelper.CallService<IUitstappenService, UitstapDetail>(svc => svc.DetailsOphalen(id));
			model.Titel = model.Uitstap.Naam;


			return View(model);
		}

		/// <summary>
		/// Formulier om bestaande uitstap mee te bewerken
		/// </summary>
		/// <returns></returns>
		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult Bewerken()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Laat de user de bivakplaats voor een uitstap bewerken.
		/// </summary>
		/// <param name="groepID">ID van de groep waarmee we werken</param>
		/// <param name="id">ID van het bivak waarvoor de plaats moet worden ingegeven.</param>
		/// <returns>Het formulier voor het ingeven van een bivakplaats</returns>
		public ActionResult PlaatsBewerken(int groepID, int id)
		{
			var model = new UitstapModel();
			BaseModelInit(model, groepID);
			model.Uitstap = ServiceHelper.CallService<IUitstappenService, UitstapDetail>(svc => svc.DetailsOphalen(id));
			model.Titel = model.Uitstap.Naam;

			return View(model);
		}
	}
}