using Chiro.Cdf.Ioc.Factory;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;

namespace Chiro.Gap.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config)
        {
            // FIXME! Code smell. We moeten hier een IDependencyResolver maken die werkt met onze Factory.
            Factory.ContainerInit();
            var container = Factory.ContainerGet();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            
            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}