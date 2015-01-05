using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.WebApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            InitializeContainer();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// Use the unity dependency injection container to generate controllers.
        /// </summary>
        private static void InitializeContainer()
        {
            Factory.ContainerInit();
        }
    }
}