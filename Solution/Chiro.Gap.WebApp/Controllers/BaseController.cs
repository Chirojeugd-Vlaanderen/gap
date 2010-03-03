// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

// Met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
	/// <summary>
	/// Deze controller bevat de method 'BaseModelInit', het BaseModel initialiseert.
	/// Verder ga ik hier proberen de IoC te arrangere voor de ServiceHelper
	/// </summary>
	/// <remarks>MasterAttribute helpt de overerving regelen</remarks>
	[Master]
	public abstract class BaseController : Controller
	{
		private IServiceHelper _serviceHelper;

		protected IServiceHelper ServiceHelper { get { return _serviceHelper; } }

		/// <summary>
		/// Constructor voor de BaseController
		/// </summary>
		/// <param name="serviceHelper">De IServiceHelper die de service calls zal uitvoeren
		/// (dependency injection)</param>
		public BaseController(IServiceHelper serviceHelper): base()
		{
			_serviceHelper = serviceHelper;
		}

		/// <summary>
		/// Vult de groepsgegevens in in de base view.
		/// </summary>
		/// <param name="model">Te 'initen' model</param>
		/// <param name="groepID">ID van de gewenste groep</param>
		protected void BaseModelInit(MasterViewModel model, int groepID)
		{
			if (groepID == 0)
			{
				// De Gekozen groep is nog niet gekend, zet defaults
				// TODO: De defaults op een zinvollere plaats definieren.

				model.GroepsNaam = "Nog geen Chirogroep geselecteerd";
				model.Plaats = "geen";
				model.StamNummer = "--";
				// model.GroepsCategorieen = new List<SelectListItem>();
			}
			else
			{
				string cacheKey = "GI" + groepID.ToString();

				System.Web.Caching.Cache c = System.Web.HttpContext.Current.Cache;

				GroepInfo gi = (GroepInfo)c.Get(cacheKey);
				if (gi == null)
				{
					gi = ServiceHelper.CallService<IGroepenService, GroepInfo>(g => g.Ophalen(groepID, GroepsExtras.Geen));
					c.Add(cacheKey, gi, null, Cache.NoAbsoluteExpiration, new TimeSpan(2, 0, 0), CacheItemPriority.Normal, null);
				}

				model.GroepsNaam = gi.Naam;
				model.Plaats = gi.Plaats;
				model.StamNummer = gi.StamNummer;
				model.GroepID = gi.ID;
			}
		}
	}
}
