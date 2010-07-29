// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.WebApp.Controllers
{
	[HandleError]
	public class GavController : BaseController
	{
		public GavController(IServiceHelper serviceHelper) : base(serviceHelper) { }

		//
		// GET: /Gav/
		[HandleError]
		public override ActionResult Index([DefaultParameterValue(0)]int dummyint)
		{
		    ActionResult r;

			// Als de gebruiker GAV is van 1 groep, dan wordt er doorgeschakeld naar de
			// personenlijst van deze groep.  Zo niet krijgt de gebruiker de keuze.

			try
			{
				var groepInfos = ServiceHelper.CallService<IGroepenService, IEnumerable<GroepInfo>>
				(g => g.MijnGroepenOphalen());
				if (groepInfos.Count() == 1)
				{
					// Redirect naar personenlijst van gevraagde groep;
					r = RedirectToAction("Index", new { Controller = "Handleiding", groepID = groepInfos.First().ID });
				}
				else
				{
					var model = new Models.GavModel();
					BaseModelInit(model, 0);    // 0:nog geen groep gekozen

					model.Titel = "Kies je Chirogroep";
					model.GroepenLijst = ServiceHelper.CallService<IGroepenService, IEnumerable<GroepInfo>>
						(g => g.MijnGroepenOphalen());

					r = View("Index", model);
				}
			}
			catch (FaultException<FoutNummerFault> ex)
			{
				r = RedirectToAction(ex.Detail.FoutNummer == FoutNummer.GeenDatabaseVerbinding ? "GeenVerbinding" : "Index", "Error");
			}
			catch (Exception)
			{
				r = RedirectToAction("Index", "Error");
			}

			return r;
		}
	}
}
