using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using System.Web.Http;
using Unity.WebApi;

namespace Chiro.Gap.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config)
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

            // Als section null is, is er geen unity-configuratie in app.config of web.config.
            // Op zich geen probleem, want de configuratie kan ook at runtime

            if (section != null)
            {
                // Als de toepassing hierop crasht, kijk dan je config file na:
                // * Heb je bij je types ook de assembly's vermeld waarin ze gedefinieerd zijn?
                // * Bevat je project referenties naar al die assemblies?
                // * Hernoemde je onlangs interfaces of implementaties?

                // Wat ook kan helpen, is de ongebruikte referenties uit je project verwijderen,
                // alsook de referenties nodig voor dependency injection, en die dan allemaal
                // terugleggen.

                section.Configure(container);
            }

            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}