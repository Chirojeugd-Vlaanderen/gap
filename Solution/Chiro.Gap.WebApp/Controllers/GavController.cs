/*
 * Copyright 2008-2013, 2017 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Web.Mvc;
using Chiro.Cdf.Authentication;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.Models;
using DotNetCasClient;

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
	    /// <param name="serviceHelper"></param>
	    /// <param name="authenticator"></param>
	    public GavController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper, IAuthenticator authenticator)
            : base(veelGebruikt, serviceHelper, authenticator)
        {
        }

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
					var model = new GavModel();
					BaseModelInit(model, null);    // null: nog geen groep gekozen

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

			return r;
		}

        public ActionResult TestGroepToevoegen()
        {
            ServiceHelper.CallService<IDbHacksService>(svc => svc.WillekeurigeGroepToekennen());
            return RedirectToAction("Index");
        }

	    public ActionResult Logout()
	    {
	        CasAuthentication.SingleSignOut();
	        return RedirectToAction("Index");
	    }
	}
}
