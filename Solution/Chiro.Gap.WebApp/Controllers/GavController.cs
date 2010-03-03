// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Controllers
{
	public class GavController : BaseController
	{
		public GavController(IServiceHelper serviceHelper): base(serviceHelper){}

		//
		// GET: /Gav/
		public ActionResult Index()
		{
			// Als de gebruiker GAV is van 1 groep, dan wordt er doorgeschakeld naar de
			// personenlijst van deze groep.  Zo niet krijgt de gebruiker de keuze

			var groepInfos = ServiceHelper.CallService<IGroepenService, IEnumerable<GroepInfo>>
			    (g => g.MijnGroepenOphalen());

			if (groepInfos.Count() == 1)
			{
				// Redirect naar personenlijst van gevraagde groep;

				return RedirectToAction("List", new { Controller = "Personen", groepID = groepInfos.First().ID, page = 1 });
			}
			else
			{
				var model = new Models.GavModel();
				BaseModelInit(model, 0);    // 0:nog geen groep gekozen

				model.Titel = "Kies je Chirogroep";
				model.GroepenLijst = ServiceHelper.CallService<IGroepenService, IEnumerable<GroepInfo>>
					(g => g.MijnGroepenOphalen());

				return View("Index", model);
			}
		}
	}
}
