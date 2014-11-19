using Nancy.Bootstrappers.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Bootstrapper;

using Microsoft.Practices.Unity;
using Nancy;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;

using System.Diagnostics;

namespace Chiro.Gap.UpdateApi
{
    public class Bootstrapper : UnityNancyBootstrapper
    {
        protected override void ApplicationStartup(IUnityContainer container, IPipelines pipelines)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during application startup.
        }

        protected override void ConfigureApplicationContainer(IUnityContainer existingContainer)
        {
            // Perform registation that should have an application lifetime

            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

            // Als section null is, is er geen unity-configuratie in app.config of web.config.
            // Op zich geen probleem, want de configuratie kan ook at runtime

            if (section != null)
            {
                // Als je hier een IoException of zoiets krijgt, dan mis je waarschijnlijk referenties
                // naar assemblies die in je untiy-configuratie voorkomen.
                // Typisch voor services: Chiro.Cdf.DependencyInjectionBehavior.
                // Kijk ook eens na of alle assembly's in de types van de unity-configuratie
                // bij de 'References' van je project staan.
                // Ook een mogelijke bron van problemen, is als interfaces van assembly zijn veranderd,
                // maar als dat niet is aangepast in de configuratiefile :)

                section.Configure(existingContainer);
            }
        }

        protected override void ConfigureRequestContainer(IUnityContainer container, NancyContext context)
        {
            // Perform registrations that should have a request lifetime
        }

        protected override void RequestStartup(IUnityContainer container, IPipelines pipelines, NancyContext context)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during request startup.
        }
    }
}