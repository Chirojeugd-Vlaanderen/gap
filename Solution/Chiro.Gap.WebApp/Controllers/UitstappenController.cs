using System;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
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

		public ActionResult Nieuw(int groepID)
		{
			var model = new UitstapModel();
			BaseModelInit(model, groepID);
			model.Titel = Properties.Resources.NieuweUitstap;
			return View(model);
		}
	}
}