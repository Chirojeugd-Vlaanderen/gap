using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Cg2.Ioc
{
    public static class Factory<T>
    {
        /// <summary>
        /// Gebruik Unity om een instantie van type/interface T
        /// te creeren.
        /// </summary>
        /// <returns></returns>
        public static T Maak()
        {
            T resultaat;

            using (IUnityContainer container = new UnityContainer())
            {
                UnityConfigurationSection section
                    = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                section.Containers.Default.Configure(container);

                resultaat = container.Resolve<T>();
            }

            return resultaat;
        }
    }
}
