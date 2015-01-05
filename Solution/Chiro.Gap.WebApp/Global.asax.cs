/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.WebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            InitializeContainer();

            RegisterRoutes(RouteTable.Routes);

            // Registreer de nieuwe adapters voor validatie via client side scripting
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(VerplichtAttribute), typeof(VerplichtAttributeAdapter));

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
                "Handleiding",
                "{groepID}/{controller}/{helpBestand}",
                new
                {
                    action = "ViewTonen",
                    helpBestand = "Index"
                },
                new
                {
                    groepID = @"\d+",
                    controller = "Handleiding"
                });

            routes.MapRoute(
                "Handleiding zonder groepID",
                "{controller}/{helpBestand}",
                new
                {
                    action = "ViewTonen",
                    helpBestand = "Index"
                },
                new
                {
                    controller = "Handleiding"
                });

            // Defaults
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

            // Foutpagina's
            routes.MapRoute(
                "Foutpagina",
                "{controller}/{action}/{*path}",
                new
                {
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
                new { controller = "Error", action = "NietGevonden" });
        }

        private static void InitializeContainer()
        {
            Factory.ContainerInit();

            IControllerFactory controllerFactory =
                new UnityControllerFactory();

            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}