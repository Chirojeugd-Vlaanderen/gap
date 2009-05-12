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
        private static object threadLock = new object();

        private static IUnityContainer _container = null;

        private static IUnityContainer Container
        {
            get
            {
                if (_container == null)
                {
                    InitContainer();
                }
                return _container;
            }
        }


        private static void InitContainer()
        {
            lock (threadLock)
            {
                if (_container == null)
                {
                    _container = new UnityContainer();
                    UnityConfigurationSection section
                        = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                    section.Containers.Default.Configure(_container);
                }
            }
        }
        
        /// <summary>
        /// Gebruik Unity om een instantie van type/interface T
        /// te creeren.
        /// </summary>
        /// <returns></returns>
        public static T Maak()
        {
            return Container.Resolve<T>(); 
        }
    }
}
