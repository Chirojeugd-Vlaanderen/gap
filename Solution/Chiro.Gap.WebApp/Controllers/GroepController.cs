// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Diagnostics;
using System.Web.Caching;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	[HandleError]
	public class GroepController : BaseController
	{
		public GroepController(IServiceHelper serviceHelper) : base(serviceHelper) { }

		/// <summary>
		/// Genereert een view met algemene gegevens over de groep
		/// </summary>
		/// <param name="groepID">ID van de gewenste groep</param>
		/// <returns>View met algemene gegevens over de groep</returns>
		[HandleError]
		public override ActionResult Index(int groepID)
		{
			var c = System.Web.HttpContext.Current.Cache;
			string isLiveCacheKey = Properties.Resources.IsLiveCacheKey;

			var model = new GroepsInstellingenModel
							{
								Titel = Properties.Resources.GroepsInstellingenTitel,
								Detail = ServiceHelper.CallService<IGroepenService, GroepDetail>(
									svc => svc.DetailOphalen(groepID))
							};
			// Ook hier nakijken of we live zijn.
			// TODO: gedupliceerde code uit BaseController.BaseModelInit.  Op te kuisen.

			// Werken we op test of live?
			bool? isLive = (bool?)c.Get(isLiveCacheKey);
			if (isLive == null)
			{
				isLive = ServiceHelper.CallService<IGroepenService, bool>(svc => svc.IsLive());
				c.Add(isLiveCacheKey, isLive, null, Cache.NoAbsoluteExpiration, new TimeSpan(2, 0, 0), CacheItemPriority.High, null);
			}

			Debug.Assert(isLive != null);
			model.IsLive = (bool)isLive;

			return View(model);
		}
	}
}
