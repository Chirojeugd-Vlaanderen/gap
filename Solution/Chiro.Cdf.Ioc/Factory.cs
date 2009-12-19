using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Diagnostics;

namespace Chiro.Cdf.Ioc
{
    public static class Factory
    {
        private static object threadLock = new object();

        private static IUnityContainer _container = null;
        private static string _containerNaam = "";

        public static IUnityContainer Container
        {
            get
            {
                return _container;
            }
        }

        public static string ContainerNaam
        {
            get
            {
                return _containerNaam;
            }
        }

        /// <summary>
        /// Initialiseert de unity container voor de factory.
        /// Gebruikt de standaardcontainer.
        /// </summary>
        public static void ContainerInit()
        {
            ContainerInit(Properties.Settings.Default.StandaardContainer);
        }

        /// <summary>
        /// Initialiseert de unity container voor de factory
        /// </summary>
        /// <param name="containerNaam">naam van de te gebruiken container</param>
        public static void ContainerInit(string containerNaam)
        {
            lock (threadLock)
            {
                if (_container != null /*&& _containerNaam != containerNaam*/)   // zie ticket #155
                {
                    // Andere container in gebruik: geef eerst vrij.

                    _container.Dispose();
                    _container = null;
                    _containerNaam = "";
                }

                if (_container == null)
                {
                    var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

                    _container = new UnityContainer();
                    if (section != null) 
                        section.Containers[containerNaam].Configure(_container);
                    _containerNaam = containerNaam;
                }
            }
        }

        public static void Dispose()
        {
            lock (threadLock)
            {
                if (_container != null)
                {
                    _container.Dispose();
                    _container = null;
                }
                _containerNaam = "";
            }
        }
        
        /// <summary>
        /// Gebruik Unity om een instantie van type/interface T
        /// te creeren.
        /// </summary>
        /// <returns></returns>
        public static T Maak<T>()
        {
            Debug.Assert(_container != null);
            
            return Container.Resolve<T>(); 
        }

        /// <summary>
        /// Gebruik Unity om een instantie van het gevraagde type te creeren.
        /// </summary>
        /// <returns></returns>
        public static object Maak(Type t)
        {
            Debug.Assert(_container != null);

            return Container.Resolve(t);
        }

        /// <summary>
        /// Zorg ervoor dat voor een (implementatie van) T steeds
        /// hetzelfde bestaande object gebruikt wordt.
        /// </summary>
        /// <typeparam name="T">type of interface</typeparam>
        /// <param name="instantie">te gebruiken object</param>
        public static void InstantieRegistreren<T>(T instantie)
        {
            Container.RegisterInstance<T>(instantie);
        }
    }
}
