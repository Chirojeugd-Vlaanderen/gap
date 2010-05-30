// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Web.Caching;
using System.Web.Mvc;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.WebApp.Models;
using System.Runtime.InteropServices;

namespace Chiro.Gap.WebApp.Controllers
{
	/// <summary>
	/// Deze controller bevat de method 'BaseModelInit', het BaseModel initialiseert.
	/// Verder ga ik hier proberen de IoC te arrangere voor de ServiceHelper
	/// </summary>
	/// <remarks>
	/// MasterAttribute helpt de overerving regelen
	/// Met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc
	/// </remarks>
	[Master]
	public abstract class BaseController : Controller
	{
		private readonly IServiceHelper _serviceHelper;

		protected IServiceHelper ServiceHelper
		{
			get
			{
				return _serviceHelper;
			}
		}

		/// <summary>
		/// Constructor voor de BaseController
		/// </summary>
		/// <param name="serviceHelper">De IServiceHelper die de service calls zal uitvoeren
		/// (dependency injection)</param>
		protected BaseController(IServiceHelper serviceHelper)
		{
			_serviceHelper = serviceHelper;
		}

        /// <summary>
        /// Een standaard index pagina die door elke controller geimplementeerd moet zijn om de "terug naar vorige" te kunnen implementeren.
        /// </summary>
        /// <param name="groepID"></param>
        /// <returns></returns>
        public abstract ActionResult Index([Optional]int groepID);

        /// <summary>
        /// Methode probeert terug te keren naar de vorige (in cookie) opgeslagen lijst. Als dit niet lukt gaat hij naar de indexpagina van de controller terug.
        /// </summary>
        /// <returns></returns>
        public ActionResult TerugNaarVorigeLijst()
        {
            ActionResult r;

            string url = ClientState.VorigeLijst;
            if (url == null)
            {
// ReSharper disable Asp.NotResolved
                r = RedirectToAction("Index");
// ReSharper restore Asp.NotResolved
            }
            else
            {
                r = Redirect(url);
            }

            return r;
        }

        /// <summary>
        /// Methode probeert terug te keren naar de vorige (in cookie) opgeslagen fiche. In tweede instantie probeert hij de vorige lijst, 
        /// in laatste instantie gaat hij naar de indexpagina van de controller terug.
        /// </summary>
        /// <returns></returns>
        public ActionResult TerugNaarVorigeFiche()
        {
            ActionResult r;

            string url = ClientState.VorigeFiche ?? ClientState.VorigeLijst;
            if (url == null)
            {
                // ReSharper disable Asp.NotResolved
                r = RedirectToAction("Index");
                // ReSharper restore Asp.NotResolved   
            }
            else
            {
                r = Redirect(url);
            }

            return r;
        }

		/// <summary>
		/// Vult de groepsgegevens in in de base view.
		/// </summary>
		/// <param name="model">Te initialiseren model</param>
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
			    string cacheKey = "GI" + groepID;

				var c = System.Web.HttpContext.Current.Cache;

				var gi = (GroepInfo)c.Get(cacheKey);
				if (gi == null)
				{
					gi = ServiceHelper.CallService<IGroepenService, GroepInfo>(g => g.InfoOphalen(groepID));
					c.Add(cacheKey, gi, null, Cache.NoAbsoluteExpiration , new TimeSpan(2, 0, 0), CacheItemPriority.Normal, null);
				}

				model.GroepsNaam = gi.Naam;
				model.Plaats = gi.Plaats;
				model.StamNummer = gi.StamNummer;
				model.GroepID = gi.ID;

				var problemen = ServiceHelper.CallService<
					IGroepenService,
					IEnumerable<FunctieProbleemInfo>>(svc => svc.FunctiesControleren(gi.ID));

				foreach (var p in problemen)
				{
					// Eerst een paar specifieke en veelvoorkomende problemen apart behandelen.

					if (p.MinAantal > 0 && p.EffectiefAantal == 0)
					{
						model.Mededelingen.Add(new Mededeling
						{
							Type = MededelingsType.Probleem,
							Info = String.Format(Properties.Resources.FunctieOntbreekt, p.Naam, p.Code)
						});
					}
					else if (p.MaxAantal == 1 && p.EffectiefAantal > 1)
					{
						model.Mededelingen.Add(new Mededeling
						{
							Type = MededelingsType.Probleem,
							Info = String.Format(Properties.Resources.FunctieMeerdereKeren, p.Naam, p.Code, p.EffectiefAantal)
						});
					}

					// Dan de algemene foutmeldingen

					else if (p.MinAantal > p.EffectiefAantal)
					{
						model.Mededelingen.Add(new Mededeling
						{
							Type = MededelingsType.Probleem,
							Info = String.Format(Properties.Resources.FunctieTeWeinig, p.Naam, p.Code, p.EffectiefAantal, p.MinAantal)
						});
					}
					else if (p.EffectiefAantal > p.MaxAantal)
					{
						model.Mededelingen.Add(new Mededeling
						{
							Type = MededelingsType.Probleem,
							Info = String.Format(Properties.Resources.FunctieTeVeel, p.Naam, p.Code, p.EffectiefAantal, p.MinAantal)
						});
					}
				}
			}
		}
	}
}
