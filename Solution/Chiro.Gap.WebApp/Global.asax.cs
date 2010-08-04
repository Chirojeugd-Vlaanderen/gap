// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.WebApp
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		private static IUnityContainer _container;

		protected void Application_Start()
		{
			InitializeContainer();

			RegisterRoutes(RouteTable.Routes);

			// Registreer de nieuwe adapters voor validatie via client side scripting
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(VerplichtAttribute), typeof(VerplichtAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(StringLengteAttribute), typeof(StringLengteAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(StringMinimumLengteAttribute), typeof(StringMinimumLengteAttributeAdapter));

			// Dit mogen we niet op true zetten, want dan zijn de booleans / checkboxen allemaal aan te kruisen
			// Dit mag toch niet weg ook niet!
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

			DefaultModelBinder.ResourceClassKey = "MyResources";
			ValidationExtensions.ResourceClassKey = "MyResources";
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Route voor ~/: Kies GAV",
				string.Empty,
				new
				{
					controller = "Gav",
					action = "Index"
				});

			routes.MapRoute(
				"Actions zonder paging",
				"{groepID}/{controller}/{action}/{id}",
				new
				{
					controller = "Handleiding",
					action = "Index"
				},
				new
				{
					groepID = @"\d+",
					id = @"\d+"
				});

			// (Handleiding indien geen controller meegegeven)
			routes.MapRoute(
				"Default",
				"{groepID}/{controller}/{action}/{id}/{page}",
				new
				{
					controller = "Handleiding",
					action = "Index",
					page = "1",
					id = "0"
				},
				new
					{
						groepID = @"\d+",
						id = @"\d+",
						page = @"\d+" // groepID, ID en page moeten een getal zijn
					});

			// Handleiding
			routes.MapRoute(
				"Handleiding",
				"{controller}/{action}",
				new
				{
					controller = "Handleiding",
					action = "Index"
				},
				new
				{
					controller = "Handleiding"
				});

			// Foutpagina's
			routes.MapRoute(
				"Foutpagina",
				"{controller}/{action}/{*path}",
				new
				{
					controller = "Error",
					action = "Index"		// Defaults
				},
				new
				{
					controller = "Error"	// Constraint
				});

			// Opvang voor url's die niet aan de opgelegde patronen voldoen
			routes.MapRoute(
				"Catch All",
				"{*path}",
				new { controller = "Error", action = "NietGevonden" }
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
	}
}