/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Controllers
{
	public abstract class PersonenEnLedenController : BaseController
	{
		/// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
        /// best toegewezen via inversion of control.
        /// </summary>
        /// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
        /// service</param>
        protected PersonenEnLedenController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper) : base(veelGebruikt, serviceHelper) { }

		[HandleError]
		public abstract override ActionResult Index(int groepID);

        /// <summary>
        /// Schrijft via de backend de gelieerde personen met gegeven <paramref name="gelieerdePersoonIDs"/> uit
        /// uit de groep met gegeven <paramref name="groepID"/>.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's uit te schrijven gelieerde personen</param>
        /// <param name="groepID">ID van groep waarvoor uit te schrijven</param>
        /// <param name="succesboodschap">Feedback die gegeven zal worden bij succes</param>
		protected void GelieerdePersonenUitschrijven(IList<int> gelieerdePersoonIDs, int groepID, string succesboodschap)
		{
            // Ik vind het een beetje vreemd dat het succesbericht hier een parameter is.
            
			var fouten = String.Empty; // TODO (#1035): fouten opvangen

			ServiceHelper.CallService<ILedenService>(l => l.Uitschrijven(gelieerdePersoonIDs, out fouten));

			// TODO (#1035): beter manier om problemen op te vangen dan via een string

			if (fouten == String.Empty)
			{
				TempData["succes"] = succesboodschap;

				VeelGebruikt.FunctieProblemenResetten(groepID);
                VeelGebruikt.LedenProblemenResetten(groepID);
			}
			else
			{
				// TODO (#1035): vermijden dat output van de back-end rechtstreeks zichtbaar wordt voor de user.
				TempData["fout"] = fouten;
			}
		}
	}
}