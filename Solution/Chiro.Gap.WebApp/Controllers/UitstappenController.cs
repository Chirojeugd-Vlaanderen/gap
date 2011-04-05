using System;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;
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
			return View(model);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult Nieuw(int groepID)
		{
			var model = new UitstapModel();
			BaseModelInit(model, groepID);
			model.Titel = Properties.Resources.NieuweUitstap;
			return View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Nieuw(UitstapModel model, int groepID)
		{
			if (ModelState.IsValid)
			{
				int uitstapID = ServiceHelper.CallService<IUitstappenService, int>(svc => svc.Nieuw(groepID, model.Uitstap));
				return RedirectToAction("Bewerken", new {groepID, id = uitstapID});
			}
			else
			{
				BaseModelInit(model, groepID);
				return View(model);
			}
		}

		public ActionResult Bewerken()
		{
			throw new NotImplementedException();
		}
	}
}