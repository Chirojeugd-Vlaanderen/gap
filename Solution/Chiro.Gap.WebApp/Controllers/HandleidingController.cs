// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	[HandleError]
	public class HandleidingController : BaseController
	{
		public HandleidingController(IServiceHelper serviceHelper) : base(serviceHelper) { }

		[HandleError]
		public override ActionResult Index(int groepID)
		{
			var model = new HandleidingModel();
			BaseModelInit(model, groepID);
			model.Titel = "Handleiding";
			return View(model);
		}

		[HandleError]
		public ActionResult BestandTonen(int groepID, string helpBestand)
		{
			var model = new HandleidingModel();
			BaseModelInit(model, groepID);
			model.Titel = "Handleiding";
			helpBestand = helpBestand + ".htm";
			helpBestand = System.IO.Path.Combine(Server.MapPath("/Help"), helpBestand);
			if (System.IO.File.Exists(helpBestand))
			{
				model.HelpBestand = Server.HtmlDecode(System.IO.File.ReadAllText(helpBestand));
			}
			else
			{
				TempData.Add("Feedback", @"Bestand niet gevonden");
			}
			return View(model);
		}
	}
}