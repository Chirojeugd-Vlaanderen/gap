/*
 * Copyright 2013 Ben Bridts.
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

using System.Web.Http;
using System.Web.Http.OData.Builder;
using Chiro.Gap.WebApi.Models;
using Microsoft.Data.Edm;

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
            modelBuilder.EntitySet<AfdelingModel>("Afdeling");
            modelBuilder.EntitySet<PersoonModel>("Persoon");
            modelBuilder.EntitySet<ContactgegevenModel>("Contactgegeven");
            modelBuilder.EntitySet<AdresModel>("Adres");

            IEdmModel model = modelBuilder.GetEdmModel();
            config.Routes.MapODataRoute("ODataRoute", "api", model);

            // default routes for apicontrollers
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );
        }
    }
}