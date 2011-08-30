using System;
using System.Configuration;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Chiro.Poc.Ioc
{
    /// <summary>
    /// IOC-container, die eigenlijk gewoon een wrapper is rond de unity container.
    /// </summary>
    public static class Factory
    {
        private static IUnityContainer _container;

        /// <summary>
        /// Initialiseert de IOC-container op basis van de App.Config van de toepassing.
        /// </summary>
        public static void ContainerInit()
        {
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            _container = new UnityContainer();
            Debug.Assert(_container != null);	// Als _container wel null is, dan ontbreekt de unityconfiguratie in Web.config
            section.Configure(_container);
        }

        /// <summary>
        /// Registreert een object <paramref name="obj"/> dat de IOC-container moet gebruiken als een object van het type
        /// <typeparamref name="T"/> gevraagd wordt.
        /// </summary>
        /// <typeparam name="T">Type van <paramref name="obj"/></typeparam>
        /// <param name="obj">Het te registreren object</param>
        public static void InstantieRegistreren<T>(T obj)
        {
            Debug.Assert(_container != null);   // Als _container null is, is hij waarschijnlijk niet geinitialiseerd
            _container.RegisterInstance(obj);
        }

        /// <summary>
        /// Gebruikt dependency injection om een object van het type <typeparamref name="T"/> op te leveren.
        /// </summary>
        /// <typeparam name="T">Type van het te op te leveren object.</typeparam>
        /// <returns>Een object van het type <typeparamref name="T"/>.</returns>
        public static T Maak<T>()
        {
            Debug.Assert(_container != null);   // Als _container null is, is hij waarschijnlijk niet geinitialiseerd
            return _container.Resolve<T>();
        }
    }
}
