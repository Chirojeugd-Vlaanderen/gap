// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller die nagaat van welke groepen de bezoeker GAV is en op basis daarvan direct doorverwijst
    /// of eerst laat vragen om een keuze te maken
    /// </summary>
	[HandleError]
	public class GavController : BaseController
	{
		/// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
		/// best toegewezen via inversion of control.
		/// </summary>
		/// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
		/// service</param>
		public GavController(IVeelGebruikt veelGebruikt) : base(veelGebruikt) { }

        /// <summary>
        /// Brengt de gebruiker naar de relevante startpagina
        /// </summary>
        /// <param name="dummyint">Als er geen groepID meegegeven wordt, geven we 0 mee,
        /// om aan te geven dat er nog geen groep gekozen is.</param>
        /// <returns>Als de gebruiker GAV is van 1 groep, dan wordt er doorgeschakeld naar de
        /// 'startpagina' van deze groep.  Zo niet krijgt de gebruiker de keuze met welke groep hij of zij
        /// wil werken.
        /// </returns>
        /// <!-- GET: /Gav/ -->
		[HandleError]
		public override ActionResult Index([DefaultParameterValue(0)]int dummyint)
		{
			ActionResult r;

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
					BaseModelInit(model, 0);    // 0: nog geen groep gekozen

					model.Titel = "Kies je Chirogroep";

                    // Als we hier crashen, kill dan de backend eens, zodat die wordt herstart.

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