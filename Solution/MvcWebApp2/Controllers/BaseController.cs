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
            GegevensVanDeGroepInvullen();
        }

        /// <summary>
        /// Geeft properties van het MasterViewModel door aan het overervend model
        /// </summary>
        /// <param name="childViewModel">Model dat overerft, gecast als MasterViewModel</param>
        /// <remarks>Wordt aangeroepen door MasterAttribute na elke request</remarks>
        public void SetModel(MasterViewModel childViewModel)
        {
            childViewModel.Groepsnaam = model.Groepsnaam;
            childViewModel.Gemeente = model.Gemeente;
            childViewModel.StamNummer = model.StamNummer;
        }

        private void GegevensVanDeGroepInvullen()
        { 
            // TODO: groepID doorgeven en dan op een juiste manier de gegevens opvragen
            model.Groepsnaam = "In BaseController gevulde groepsnaam";
            model.Gemeente = "Antwerpen";
            model.StamNummer = "XXX/0000";
        }
    }
}
