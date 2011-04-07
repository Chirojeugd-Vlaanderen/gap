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

		public override ActionResult Index(int groepID)
		{
			var model = new UitstapOverzichtModel();
			BaseModelInit(model, groepID);
			model.Titel = Properties.Resources.Uitstappen;
			model.Uitstappen =
				ServiceHelper.CallService<IUitstappenService, IEnumerable<UitstapInfo>>(svc => svc.OphalenVanGroep(groepID));
			return View(model);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult Nieuw(int groepID)
		{
			var model = new UitstapModel();
			BaseModelInit(model, groepID);
			model.Titel = Properties.Resources.NieuweUitstap;
			model.Uitstap = new UitstapDetail();

			return View("Bewerken", model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Nieuw(UitstapModel model, int groepID)
		{
			BaseModelInit(model, groepID);
			model.Titel = Properties.Resources.NieuweUitstap;

			if (ModelState.IsValid)
			{
				int uitstapID = ServiceHelper.CallService<IUitstappenService, int>(svc => svc.Bewaren(groepID, model.Uitstap));
				return RedirectToAction("Bewerken", new {groepID, id = uitstapID});
			}
			else
			{
				return View("Bewerken", model);
			}
		}

		public ActionResult Bewerken()
		{
			throw new NotImplementedException();
		}
	}
}