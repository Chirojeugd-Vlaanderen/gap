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
			BaseModelInit(model, groepID, "Handleiding");
			return View(model);
		}

		/// <summary>
		/// Default action als de gebruiker geen groep heeft of als de service niet beschikbaar is.
		/// Dan kunnen we geen gegevens ophalen, maar de gebruiker kan wel de handleiding bekijken.
		/// </summary>
		/// <param name="helpBestand">Naam van het bestand dat we moeten tonen, zonder pad of extensie</param>
		/// <param name="master">De naam van de masterpage (de default masterpage heeft gegevens over de groep nodig)</param>
		[HandleError]
		public ActionResult ByPassIndex(string helpBestand, string master)
		{
			var model = new HandleidingModel();
			model.Titel = "Handleiding";

			helpBestand = helpBestand + ".htm";
			helpBestand = System.IO.Path.Combine(Server.MapPath("~/Help"), helpBestand);
			model.HelpBestand = Server.HtmlDecode(System.IO.File.ReadAllText(helpBestand));
			return View("AlleenHandleidingTonen", master, model);
		}

		/// <summary>
		/// Initialiseert het model, met inbegrip van groepsgegevens, en toont dan het gevraagde helpbestand
		/// </summary>
		/// <param name="groepID">ID van de groep waar de gebruiker GAV van is</param>
		/// <param name="helpBestand">Naam van het bestand dat we moeten tonen, zonder pad of extensie</param>
		[HandleError]
		public ActionResult BestandTonen(int groepID, string helpBestand)
		{
			var model = new HandleidingModel();
			BaseModelInit(model, groepID);
			return BestandTonen(model, helpBestand);
		}

		/// <summary>
		/// Controleert of het helpbestand bestaat, en zorgt dat het getoond wordt
		/// </summary>
		/// <param name="model">Een geïnitialiseerd HandleidingModel, met ingevulde groepsinformatie</param>
		/// <param name="helpBestand">Naam van het bestand dat we moeten tonen, zonder pad of extensie</param>
		[HandleError]
		private ActionResult BestandTonen(HandleidingModel model, string helpBestand)
		{
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
				model.Titel = "Handleiding";
				TempData["fout"] = @"Ongeldige verwijzing naar de handleiding";
				return View(model);
			}
		}

		[HandleError]
		public ActionResult ViewTonen(int? groepID, string helpBestand)
		{
			var model = new HandleidingModel();
			model.Titel = "Handleiding";

			if (groepID != null && groepID > 0)
			{
				BaseModelInit(model, (int)groepID);
			}
			else
			{
				model.GroepID = 0;
			}
			return View(helpBestand, "Handleiding", model);
		}

		//[HandleError]
		//public ActionResult JaarOvergang(int groepID)
		//{
		//    var model = new HandleidingModel();
		//    BaseModelInit(model, groepID, "Handleiding: jaarovergang");
		//    return BestandTonen(model, "JaarOvergang");
		//}
	}
}