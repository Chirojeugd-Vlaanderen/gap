// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
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
    /// <summary>
    /// 
    /// </summary>
	[HandleError]
	public class GavController : BaseController
	{
		/// <summary>
		/// Standaardconstructor.  <paramref name="serviceHelper"/> en <paramref name="veelGebruikt"/> worden
		/// best toegewezen via inversion of control.
		/// </summary>
		/// <param name="serviceHelper">wordt gebruikt om de webservices van de backend aan te spreken</param>
		/// <param name="veelGebruikt">haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
		/// service</param>
		public GavController(IServiceHelper serviceHelper, IVeelGebruikt veelGebruikt) : base(serviceHelper, veelGebruikt) { }

		//
		// GET: /Gav/
		[HandleError]
		public override ActionResult Index([DefaultParameterValue(0)]int dummyint)
		{
			ActionResult r;

			// Als de gebruiker GAV is van 1 groep, dan wordt er doorgeschakeld naar de
			// 'startpagina' van deze groep.  Zo niet krijgt de gebruiker de keuze.

			try
			{
				if (VeelGebruikt.UniekeGroepGav(User.Identity.Name) != 0)
				{
					// Redirect naar personenlijst van gevraagde groep;
					r = RedirectToAction("Index", new { Controller = "Handleiding", groepID = VeelGebruikt.UniekeGroepGav(User.Identity.Name) });
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
