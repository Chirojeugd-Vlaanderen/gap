// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using System.Web.Routing;

using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	public class GroepController : BaseController
	{
		public GroepController(IServiceHelper serviceHelper) : base(serviceHelper) { }

		/// <summary>
		/// Methode probeert terug te keren naar de vorige (in cookie) opgeslagen pagina. Als dit niet lukt gaat hij naar de indexpagina van de controller terug.
		/// </summary>
		/// <returns></returns>
		public ActionResult TerugNaarVorige()
		{
			string url = ClientState.VorigePagina;
			if (url == null)
			{
				return RedirectToAction("Index");
			}
			return Redirect(url);
		}

		/// <summary>
		/// Genereert een view met algemene gegevens over de groep
		/// </summary>
		/// <param name="groepID">ID van de gewenste groep</param>
		/// <returns>View met algemene gegevens over de groep</returns>
		public ActionResult Index(int groepID)
		{
			var model = new GroepsInstellingenModel
							{
								Titel = Properties.Resources.GroepsInstellingenTitel,
								Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(
									svc => svc.DetailOphalen(groepID))
							};

			return View(model);
		}
	}
}
