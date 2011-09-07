// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
	/// Deze controller zal in eerste instantie voornamelijk gegevens opleveren voor de autosuggestie.
	/// </summary>
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
		public ActionResult WoonPlaatsenOphalen(int postNummer)
		{
			var resultaat = (from g in VeelGebruikt.WoonPlaatsenOphalen()
							 where g.PostNummer == postNummer
							 orderby g.Naam
							 select g);

			return Json(resultaat, JsonRequestBehavior.AllowGet);
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
		public ActionResult StratenVoorstellen(String q, int postNummer)
		{
			IEnumerable<StraatInfo> mogelijkeStraten =
				ServiceHelper.CallService<IGroepenService, IEnumerable<StraatInfo>>(
					x => x.StratenOphalen(q, postNummer));

			var namen = (from straat in mogelijkeStraten
						 orderby straat.Naam
						 select straat.Naam).Distinct();

			return Content(String.Join("\n", namen.ToArray()));
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
