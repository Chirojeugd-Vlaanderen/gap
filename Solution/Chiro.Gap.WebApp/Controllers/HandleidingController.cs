// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Text.RegularExpressions;
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
			if (Regex.IsMatch(helpBestand, "^[A-Za-z0-9]{3,}$"))
			// minstens 3 characters, alleen alfanumeriek (dan kan de gebruiker geen bestandspad invullen - cfr. ticket #600)
			{
				helpBestand = helpBestand + ".htm";
				helpBestand = System.IO.Path.Combine(Server.MapPath("~/Help"), helpBestand);
				if (System.IO.File.Exists(helpBestand))
				{
					model.HelpBestand = Server.HtmlDecode(System.IO.File.ReadAllText(helpBestand));
				}
				else
				{
					TempData["fout"] = @"Bestand niet gevonden";
				}
				return View(model);
			}
			else
			{
				TempData["fout"] = @"Ongeldige verwijzing naar de handleiding";
				return View(model);
			}
		}
	}
}