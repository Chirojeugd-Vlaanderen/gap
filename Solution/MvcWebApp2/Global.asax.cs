using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Chiro.Gap.WebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // FIXME: startpagina moet uitmaken van hoeveel groepen
            // de gebruiker GAV is, en eventueel een keuze laten maken.

            routes.MapRoute(
                "Route voor ~/: Kies GAV"
                , ""
                , new { controller = "Gav", action = "Index" });
            
            routes.MapRoute(
                "PaginatedPersonenList",
                "{groepID}/Personen/List/{page}",
                new { controller = "Personen", action = "List" }
            );

            routes.MapRoute(
                "PaginatedLedenList",
                "{groepID}/Leden/List/{groepsWerkJaarId}",
                new { controller = "Leden", action = "List" }
            );

            routes.MapRoute(
                "Default",
                "{groepID}/{controller}/{action}/{id}",
                new { controller = "Personen", action = "Index", id = "" }
                // (personencontroller indien geen controller meegegeven)
            );

        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}