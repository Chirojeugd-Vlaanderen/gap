using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using Chiro.Gap.WebApi.Models;

namespace Chiro.Gap.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // specific routes for odata
            // see http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/getting-started-with-odata-in-web-api/create-a-read-only-odata-endpoint
            ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<GroepModel>("Groep");
            modelBuilder.EntitySet<PersoonModel>("Persoon");
            modelBuilder.EntitySet<WerkjaarModel>("Werkjaar");

            Microsoft.Data.Edm.IEdmModel model = modelBuilder.GetEdmModel();
            config.Routes.MapODataRoute("ODataRoute", "api", model);

            // default routes for apicontrollers
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
