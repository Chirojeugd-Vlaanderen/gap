// Met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Cg2.Orm;
using System.Configuration;
using Cg2.Adf.ServiceModel;
using Cg2.ServiceContracts;
using MvcWebApp2.Models;
using System.Web.Caching;

namespace MvcWebApp2.Controllers
{
    /// <summary>
    /// Houdt de info bij die in de MasterPage getoond moet worden
    /// </summary>
    /// <remarks>MasterAttribute helpt de overerving regelen</remarks>
    [Master]
    public abstract class BaseController : Controller
    {
        protected MasterViewModel model = new MasterViewModel();

        /// <summary>
        /// Standaard constructor
        /// </summary>
        public BaseController() : base()
        {
        }

        /// <summary>
        /// Geeft properties van het MasterViewModel door aan het overervend model
        /// </summary>
        /// <param name="childViewModel">Model dat overerft, gecast als MasterViewModel</param>
        /// <remarks>Wordt aangeroepen door MasterAttribute na elke request</remarks>
        public void SetModel(MasterViewModel childViewModel)
        {
            GegevensVanDeGroepInvullen();

            childViewModel.Groepsnaam = model.Groepsnaam;
            childViewModel.Plaats = model.Plaats;
            childViewModel.StamNummer = model.StamNummer;
            childViewModel.Title += model.Title;
        }

        private void GegevensVanDeGroepInvullen()
        { 

            if (Sessie.GroepID == 0)
            {
                // De Gekozen groep is nog niet gekend, zet defaults
                // TODO: De defaults op een zinvollere plaats definieren.

                model.Groepsnaam = "Nog geen Chirogroep geselecteerd";
                model.Plaats = "geen";
                model.StamNummer = "--";
            }
            else
            {
                string cacheKey = "GI" + Sessie.GroepID.ToString();

                System.Web.Caching.Cache c = System.Web.HttpContext.Current.Cache;

                GroepInfo gi = (GroepInfo)c.Get(cacheKey);
                if (gi == null)
                {
                    gi = ServiceHelper.CallService<IGroepenService, GroepInfo>(g => g.OphalenInfo(Sessie.GroepID));
                    c.Add(cacheKey, gi, null, Cache.NoAbsoluteExpiration, new TimeSpan(2, 0, 0), CacheItemPriority.Normal, null);
                }

                model.Groepsnaam = gi.Groepsnaam;
                model.Plaats = gi.Plaats;
                model.StamNummer = gi.StamNummer;
                model.Title = " - .: Kakajo :. ";
            }
        }
    }
}
