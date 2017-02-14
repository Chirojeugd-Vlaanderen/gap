/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using Microsoft.Practices.Unity;

namespace Chiro.Cdf.Ioc.Factory
{
    /// <summary>
    /// Factory is verantwoordelijk voor de dependency injection
    /// </summary>
	public static class Factory
	{
		private static readonly object ThreadLock = new object();

		private static IDiContainer _container;

        /// <summary>
        /// Returns the underelying unity container.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// FIXME: Deze code stinkt.
        /// Chiro.Cdf.AdnrWcfExtension gebruikt de deze statische klasse om
        /// de authenticator te gebruiken. De api heeft ook dependency injection
        /// nodig, en dan liefst via dezelfde unity container. Vandaar dat ik hem
        /// hier nu ga exposen.
        /// 
        /// Wat ik in de plaats moet doen, is een IDependencyResolver maken die deze
        /// factory gebruikt, en die dan gebruiken in Chiro.Gap.Api.
        /// </remarks>
	    public static IUnityContainer ContainerGet()
        {
            if (_container is UnityDiContainer)
	        {
                return (_container as UnityDiContainer).UnityContainer;
            }
            return null;
        }

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

                _container = new UnityDiContainer();
                _container.InitVolgensConfigFile();
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

			return _container.Maak<T>();
		}

		/// <summary>
		/// Gebruik Unity om een instantie van het gevraagde type te creëren.
		/// </summary>
		/// <param name="t">Van welk type het object moet zijn</param>
		/// <returns>Het geïnstantieerde object</returns>
		public static object Maak(Type t)
		{
			Debug.Assert(_container != null);

			return _container.Maak(t);
		}

		/// <summary>
		/// Zorg ervoor dat voor een (implementatie van) T steeds
		/// hetzelfde bestaande object gebruikt wordt.
		/// </summary>
		/// <typeparam name="T">Type van de interface</typeparam>
		/// <param name="instantie">Te gebruiken object</param>
		public static void InstantieRegistreren<T>(T instantie)
		{
            Debug.Assert(_container != null);   // als deze assert failt, vergat je de container te initialiseren.

        	_container.InstantieRegistreren(instantie);
		}

        /// <summary>
        /// Configureert de IOC-container zodanig dat er telkens een nieuwe instantie van het gegeven
        /// <paramref name="type"/> gebruikt wordt als er een klase van type <typeparamref name="T"/>
        /// gevraagd wordt.
        /// </summary>
        /// <typeparam name="T">Gevraagde type</typeparam>
        /// <param name="type">Type van opgeleverd object</param>
        public static void TypeRegistreren<T>(Type type)
        {
            _container.TypeRegistreren<T>(type);
        }
	}
}
