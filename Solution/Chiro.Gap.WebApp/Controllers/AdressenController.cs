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
using System.Linq;
using System.Web.Mvc;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Controllers
{
	/// <summary>
	/// Deze controller dient enkel om gegevens aan te leveren voor de adressen-autocomplete.
	/// </summary>
	/// <remarks>
	/// Koppelen van adressen aan personen, gebeurt in de <see cref="PersonenController"/>.
	/// </remarks>
	[HandleError]
	public class AdressenController : BaseController
	{
		/// <summary>
        /// Standaardconstructor.  <paramref name="veelGebruikt"/> wordt
		/// best toegewezen via inversion of control.
		/// </summary>
		/// <param name="veelGebruikt">Haalt veel gebruikte zaken op uit cache, of indien niet beschikbaar, via 
		/// service</param>
		public AdressenController(IVeelGebruikt veelGebruikt) : base(veelGebruikt)
		{
		}

		/// <summary>
		/// Genereert een lijst tags (Json) met namen van deelgemeentes voor het autosuggestiescript
		/// </summary>
		/// <param name="q">Eerste letters van de te zoeken deelgemeente</param>
		/// <param name="limit">Maximale lengte van de lijst.  Indien 0, wordt de standaardlengte gekozen.</param>
		/// <returns></returns>
		[HandleError]
		public ActionResult GemeentesVoorstellen(String q, int limit)
		{
			if (limit == 0)
			{
				limit = Properties.Settings.Default.AutoSuggestieStandaardLimiet;
			}

			var gemeenteLijst = VeelGebruikt.WoonPlaatsenOphalen();

			var tags = (from g in gemeenteLijst
						orderby g.Naam
						select new { Tag = g.Naam })
				   .Where(x => x.Tag.StartsWith(q, StringComparison.CurrentCultureIgnoreCase))
				   .Distinct()
				   .Take(limit)
				   .ToList();

			// Return the result set as JSON
			return Json(tags);
		}

		/// <summary>
		/// Genereert een JSON-lijst met WoonPlaatsInfo corresponderend met het gegeven
		/// <paramref name="postNummer"/>.
		/// </summary>
		/// <param name="postNummer">Postnummer waarvan woonplaatsen gevraagd</param>
		/// <returns>
		/// JSON-lijst met WoonPlaatsInfo corresponderend met het gegeven
		/// <paramref name="postNummer"/>.
		/// </returns>
		[HandleError]
        //Actionresult
		public JsonResult WoonPlaatsenOphalen(int postNummer)
		{
			var resultaat = (from g in VeelGebruikt.WoonPlaatsenOphalen()
							 where g.PostNummer == postNummer
							 orderby g.Naam
							 select g);

			return Json(resultaat, JsonRequestBehavior.AllowGet);
		}

	    ///
	    /// 
	    public JsonResult LandenVoorstellen()
	    {
	        List<LandInfo> landen =  VeelGebruikt.LandenOphalen();
	        return Json(landen, JsonRequestBehavior.AllowGet);
	    }


		/// <summary>
		/// Stelt op basis van een gedeeltelijke straatnaam en een 
		/// gemeentenaam een lijst suggesties samen met straatnamen die de
		/// gebruiker mogelijk zinnes is in te vullen.
		/// </summary>
		/// <param name="q">Wat de gebruiker al intikte</param>
		/// <param name="postNummer">Postnummer waarin gezocht moet worden</param>
		/// <returns>Voorgestelde straatnamen in plain text, nieuwe regel na elke straat</returns>
		[HandleError]
		public JsonResult StratenVoorstellen(String q, int postNummer)
		{
			IEnumerable<StraatInfo> mogelijkeStraten =
				ServiceHelper.CallService<IGroepenService, IEnumerable<StraatInfo>>(
					x => x.StratenOphalen(q, postNummer));

			var namen = (from straat in mogelijkeStraten
						 orderby straat.Naam
						 select straat.Naam).Distinct();

            return Json(namen, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Zoekt een postnummer dat correspondeert met de gegeven deelgemeentenaam.
		/// </summary>
		/// <param name="gemeente">Naam van de deelgemeente</param>
		/// <returns>Een willekeurig postnummer dat hoort bij de deelgemeentenaam</returns>
		/// <remarks>Dit is nogal een omslachtige search voor iets dat eigenlijk weinig zinvol is.</remarks>
		[HandleError]
		public ActionResult PostNrVoorstellen(String gemeente)
		{
			IEnumerable<WoonPlaatsInfo> tags = VeelGebruikt.WoonPlaatsenOphalen().Where(x => x.Naam.Equals(gemeente, StringComparison.CurrentCultureIgnoreCase));

			// Select the tags that match the query, and get the 
			// number or tags specified by the limit.
			var retValue = tags
				.Select(r => r.PostNummer).FirstOrDefault();

			// Return the result set as JSON
			return Json(retValue);
		}

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="groepID">ID van de groep die de pagina oproept, en van wie we dus gegevens moeten tonen</param>
        /// <returns></returns>
		[HandleError]
		public override ActionResult Index(int groepID)
		{
			return RedirectToAction("Index", new { Controller = "Personen" });
		}
	}
}
