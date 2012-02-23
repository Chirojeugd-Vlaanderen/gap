// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Chiro.Cdf.Ioc
{
    /// <summary>
    /// Factory is verantwoordelijk voor de dependency injection
    /// </summary>
	public static class Factory
	{
		private static readonly object ThreadLock = new object();

		private static IUnityContainer _container;

		/// <summary>
		/// Initialiseert de unity container voor de factory.
		/// </summary>
		public static void ContainerInit()
		{
            // Kunnen we dit niet gewoon in de constructor doen, en zo een 
            // singleton implementeren?

            lock (ThreadLock)
            {
                if (_container != null)
                {
                    // Andere container in gebruik: geef eerst vrij.

                    _container.Dispose();
                    _container = null;
                }

                _container = new UnityContainer();

                var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

                // Als section null is, is er geen unity-configuratie in app.config of web.config.
                // Op zich geen probleem, want de configuratie kan ook at runtime

                if (section != null)
                {
                    // Als je hier een IoException of zoiets krijgt, dan mis je waarschijnlijk referenties
                    // naar assemblies die in je untiy-configuratie voorkomen.
                    // Typisch voor services: Chiro.Cdf.DependencyInjectionBehavior.

                    section.Configure(_container);
                }
            }
		}

		/// <summary>
		/// Gebruik Unity om een instantie van type/interface T
		/// te creëren.
		/// </summary>
        /// <typeparam name="T">Van welk type het object moet zijn</typeparam>
		/// <returns>Het type</returns>
		public static T Maak<T>()
		{
			Debug.Assert(_container != null);

			return _container.Resolve<T>();
		}

		/// <summary>
		/// Gebruik Unity om een instantie van het gevraagde type te creëren.
		/// </summary>
		/// <param name="t">Van welk type het object moet zijn</param>
		/// <returns>Het geïnstantieerde object</returns>
		public static object Maak(Type t)
		{
			Debug.Assert(_container != null);

			return _container.Resolve(t);
		}

		/// <summary>
		/// Zorg ervoor dat voor een (implementatie van) T steeds
		/// hetzelfde bestaande object gebruikt wordt.
		/// </summary>
		/// <typeparam name="T">Type van de interface</typeparam>
		/// <param name="instantie">Te gebruiken object</param>
		public static void InstantieRegistreren<T>(T instantie)
		{
			// Als de debugger hier een Exception genereert bij het unittesten, dan is
			// dat mogelijk geen probleem:
			// http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=7019
			// Maar ik ben daar toch niet zo zeker van :-/

			try
			{
				_container.RegisterInstance(instantie);
			}
			catch (SynchronizationLockException)
			{
				// Doe niets. :-/
			}	
		}
	}
}
