using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Diagnostics;

namespace Cg2.Ioc
{
    public static class Factory
    {
        private static object threadLock = new object();

        private static IUnityContainer _container = null;

        private static IUnityContainer Container
        {
            get
            {
                Debug.Assert(_container != null);
                return _container;
            }
        }

        public static void InitContainer()
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

        public static void DisposeContainer()
        {
            lock (threadLock)
            {
                if (_container != null)
                {
                    _container.Dispose();
                    _container = null;
                }
            }
        }
        
        /// <summary>
        /// Gebruik Unity om een instantie van type/interface T
        /// te creeren.
        /// </summary>
        /// <returns></returns>
        public static T Maak<T>()
        {
            return Container.Resolve<T>(); 
        }
    }
}
