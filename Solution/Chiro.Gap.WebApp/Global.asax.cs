using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Chiro.Cdf.Ioc;
using System.Web.Mvc.Html;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		private static IUnityContainer _container;

		protected void Application_Start()
		{
			RegisterRoutes(RouteTable.Routes);

			InitializeContainer();

			DefaultModelBinder.ResourceClassKey = "MyResources";
			ValidationExtensions.ResourceClassKey = "MyResources";
		}

		private static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
			    "Route voor ~/: Kies GAV"
			    , ""
			    , new { controller = "Gav", action = "Index" });

			routes.MapRoute(
			    "Default",
			    "{groepID}/{controller}/{action}/{id}/{page}",
			    new { controller = "Personen", action = "Index", page = "1", id = "0" }
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

		/*public static IEnumerable<StraatInfo> getStraten()
		{
			return StratenLijst;
		}*/
	}
}