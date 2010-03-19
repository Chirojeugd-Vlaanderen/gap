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
	/// <summary>
	/// Deze controller zal in eerste instantie voornamelijk gegevens opleveren voor de autosuggestie.
	/// </summary>
	public class AdressenController : BaseController
	{
		public AdressenController(IServiceHelper serviceHelper) : base(serviceHelper) { }

		/// <summary>
		/// Levert een lijst op van alle deelgemeentes
		/// </summary>
		/// <returns>Een lijst met alle beschikbare deelgemeentes</returns>
		public IEnumerable<GemeenteInfo> DeelGemeentesOphalen()
		{
			string cacheKey = Properties.Settings.Default.DeelGemeentesCacheKey;

			var cache = System.Web.HttpContext.Current.Cache;
			var result = (IEnumerable<GemeenteInfo>)cache.Get(cacheKey);

			if (result == null)
			{
				// Cache geexpired, opnieuw opzoeken en cachen.

				result = ServiceHelper.CallService<IGroepenService,
					IEnumerable<GemeenteInfo>>(g => g.GemeentesOphalen());

				cache.Add(
					cacheKey,
					result,
					null,
					System.Web.Caching.Cache.NoAbsoluteExpiration,
					new TimeSpan(1, 0, 0, 0) /* bewaar 1 dag */,
					System.Web.Caching.CacheItemPriority.High /* niet te gauw wissen; grote kost */,
					null);
			}
			return result;
		}

		/// <summary>
		/// Genereert een lijst tags (Json) met namen van deelgemeentes voor het autosuggestiescript
		/// </summary>
		/// <param name="q">Eerste letters van de te zoeken deelgemeente</param>
		/// <param name="limit">Maximale lengte van de lijst.  Indien 0, wordt de standaardlengte gekozen.</param>
		/// <returns></returns>
		public ActionResult GemeentesVoorstellen(String q, int limit)
		{
			if (limit == 0)
			{
				limit = Properties.Settings.Default.AutoSuggestieStandaardLimiet;
			}

			var gemeenteLijst = DeelGemeentesOphalen();

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
		/// Stelt op basis van een <paramref name="gedeeltelijkeStraatNaam"/> en een 
		/// <paramref name="gemeenteNaam"/> een lijst suggesties samen met straatnamen die de
		/// gebruiker mogelijk zinnes is in te vullen.
		/// </summary>
		/// <param name="gedeeltelijkeStraatNaam">Wat de gebruiker al intikte</param>
		/// <param name="gemeenteNaam">Naam van gemeente waarin de straat moet liggen</param>
		/// <returns>Json-lijst voor autosuggestie met voorgestelde straatnamen</returns>
		public ActionResult StratenVoorstellen(String gedeeltelijkeStraatNaam, String gemeenteNaam)
		{
			// Opm. van Johan:
			// TODO: Moet die gemeentelijst niet in cache zitten, ipv op het niveau van de app?
			// Na te kijken in het verslag over state
			var postNrs = (from gemeente in DeelGemeentesOphalen()
						   where String.Compare(gemeente.Naam, gemeenteNaam) == 0
						   select gemeente.PostNummer).Distinct().ToList();

			IEnumerable<StraatInfo> mogelijkeStraten =
				ServiceHelper.CallService<IGroepenService, IEnumerable<StraatInfo>>(
					x => x.StratenOphalenMeerderePostNrs(gedeeltelijkeStraatNaam, postNrs));

			var namen = (from straat in mogelijkeStraten
						 orderby straat.Naam
						 select straat.Naam).Distinct();

			var resultaat = from tag in namen
							select new { Tag = tag };

			// Return the result set as JSON
			return Json(resultaat);
		}

		/// <summary>
		/// Zoekt een postnummer dat correspondeert met de gegeven deelgemeentenaam.
		/// </summary>
		/// <param name="gemeente">Naam van de deelgemeente</param>
		/// <returns>Een willekeurig postnummer dat hoort bij de deelgemeentenaam</returns>
		/// <remarks>Dit is nogal een omslachtige search voor iets dat eigenlijk weinig zinvol is.</remarks>
		public ActionResult PostNrVoorstellen(String gemeente)
		{
			IEnumerable<GemeenteInfo> tags = DeelGemeentesOphalen().Where(x => x.Naam.Equals(gemeente, StringComparison.CurrentCultureIgnoreCase));

			// Select the tags that match the query, and get the 
			// number or tags specified by the limit.
			var retValue = tags
				.Select(r => r.PostNummer).FirstOrDefault();

			// Return the result set as JSON
			return Json(retValue);
		}
	}
}
