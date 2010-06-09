// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp
{
	/// <summary>
	/// Nuttige functionaliteit ivm adressen, die in verschillende controllers van pas komt.
	/// </summary>
	public class AdressenHelper
	{
		private const string WOONPLAATSENCACHEKEY = "WoonPlaatsenKacheKey";

		private IServiceHelper _serviceHelper;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="serviceHelper">Een IServiceHelper die zal instaan voor de service calls.</param>
		public AdressenHelper(IServiceHelper serviceHelper)
		{
			_serviceHelper = serviceHelper;
		}

		/// <summary>
		/// Haalt WoonPlaatsInfo op voor woonplaatsen met gegeven <paramref name="postNummer"/>
		/// </summary>
		/// <param name="postNummer">Postnummer waarvan de woonplaatsen gevraagd zijn</param>
		/// <returns>WoonPlaatsInfo voor woonplaatsen met gegeven <paramref name="postNummer"/></returns>
		public IEnumerable<WoonPlaatsInfo> WoonPlaatsenOphalen(int postNummer)
		{
			return (from g in WoonPlaatsenOphalen()
				where g.PostNummer == postNummer
				orderby g.Naam
				select g).ToArray();
		}

		/// <summary>
		/// Levert een lijst op van alle woonplaatsen
		/// </summary>
		/// <returns>Een lijst met alle beschikbare woonplaatsen</returns>
		public IEnumerable<WoonPlaatsInfo> WoonPlaatsenOphalen()
		{
			string cacheKey = WOONPLAATSENCACHEKEY;

			var cache = System.Web.HttpContext.Current.Cache;
			var result = (IEnumerable<WoonPlaatsInfo>)cache.Get(cacheKey);

			if (result == null)
			{
				// Cache geexpired, opnieuw opzoeken en cachen.

				result = _serviceHelper.CallService<IGroepenService,
					IEnumerable<WoonPlaatsInfo>>(g => g.GemeentesOphalen());

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
	}
}
