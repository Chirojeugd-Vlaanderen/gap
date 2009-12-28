using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Chiro.Cdf.Ioc;
using Chiro.Adf.ServiceModel;
using System.Web.Mvc.Html;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
		private static IUnityContainer _container;
		private static IEnumerable<GemeenteInfo> GemeenteLijst;
		private static IEnumerable<StraatInfo> StratenLijst;

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);

            InitializeContainer();
			InitializeAdressDatabase();

            DefaultModelBinder.ResourceClassKey = "MyResources";
            ValidationExtensions.ResourceClassKey = "MyResources";
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // FIXME: startpagina moet uitmaken van hoeveel groepen
            // de gebruiker GAV is, en eventueel een keuze laten maken.

            routes.MapRoute(
                "Route voor ~/: Kies GAV"
                , ""
                , new { controller = "Gav", action = "Index" });
            /*
            routes.MapRoute(
                "LedenCategorie",
                "{groepID}/Leden/Categorie/{categorieId}/{page}",
                new { controller = "Leden", action = "CatList" }
            );

            routes.MapRoute(
                "PersonenCategorie",
                "{groepID}/Personen/Categorie/{categorieId}/{page}",
                new { controller = "Personen", action = "CatList" }
            );
            */
            routes.MapRoute(
                "Default",
                "{groepID}/{controller}/{action}/{id}/{page}",
                new { controller = "Personen", action = "Index", page = "1", id = "0"}
                // (personencontroller indien geen controller meegegeven)
            );
        }

        private static void InitializeContainer()
        {

            if (_container == null)
            {
                Factory.ContainerInit();
                _container = Factory.Container;
            }

            IControllerFactory controllerFactory = 
                new UnityControllerFactory(_container);

            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

        }

		private static void InitializeAdressDatabase()
		{
			GemeenteLijst = ServiceHelper.CallService<IGroepenService, IEnumerable<GemeenteInfo>>(g => g.GemeentesOphalen());
			//StratenLijst = ServiceHelper.CallService<IGroepenService, IEnumerable<StraatInfo>>(g => g.StratenOphalen());
		}

		public static IEnumerable<GemeenteInfo> getGemeentes()
		{
			return GemeenteLijst;
		}

		public static IEnumerable<StraatInfo> getStraten()
		{
			return StratenLijst;
		}

    }
}