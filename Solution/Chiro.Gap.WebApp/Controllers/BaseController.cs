// Met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Chiro.Gap.Orm;
using System.Configuration;
using Chiro.Adf.ServiceModel;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.WebApp.Models;
using System.Web.Caching;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Houdt de info bij die in de MasterPage getoond moet worden
    /// </summary>
    /// <remarks>MasterAttribute helpt de overerving regelen</remarks>
    [Master]
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Standaard constructor
        /// </summary>
        public BaseController() : base()
        {
        }

        /// <summary>
        /// Vult de groepsgegevens in in de base view.
        /// </summary>
        /// <param name="model">Te 'initen' model</param>
        /// <param name="groepID">groepID van de gewenste groep</param>
        protected static void BaseModelInit(MasterViewModel model, int groepID)
        { 
            if (groepID == 0)
            {
                // De Gekozen groep is nog niet gekend, zet defaults
                // TODO: De defaults op een zinvollere plaats definieren.

                model.Groepsnaam = "Nog geen Chirogroep geselecteerd";
                model.Plaats = "geen";
                model.StamNummer = "--";
                //model.GroepsCategorieen = new List<SelectListItem>();
            }
            else
            {
                string cacheKey = "GI" + groepID.ToString();

                System.Web.Caching.Cache c = System.Web.HttpContext.Current.Cache;

                GroepInfo gi = (GroepInfo)c.Get(cacheKey);
                if (gi == null)
                {
                    gi = ServiceHelper.CallService<IGroepenService, GroepInfo>(g => g.InfoOphalen(groepID));
                    c.Add(cacheKey, gi, null, Cache.NoAbsoluteExpiration, new TimeSpan(2, 0, 0), CacheItemPriority.Normal, null);
                }

                model.Groepsnaam = gi.Naam;
                model.Plaats = gi.Plaats;
                model.StamNummer = gi.StamNummer;

                /*List<SelectListItem> list = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();
                item.Text = "Kookies";
                item.Value = "10";
                item.Selected = false;
                list.Add(item);
                item = new SelectListItem();
                item.Text = "Alle";
                item.Value = "Alle";
                item.Selected = true;
                list.Add(item);
                model.GroepsCategorieen = list;*/
            }
        }
    }
}
