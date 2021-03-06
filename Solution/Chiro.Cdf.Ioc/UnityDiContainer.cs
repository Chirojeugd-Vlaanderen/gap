/*
 * Copyright 2008-2015, 2017 the GAP developers. See the NOTICE file at the 
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
using System.Configuration;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Chiro.Cdf.Ioc
{
    /// <summary>
    /// Eigenlijk gewoon een wrapper rond een unity container.
    /// </summary>
	public class UnityDiContainer: IDiContainer
    {
		private readonly IUnityContainer _container;
        private static readonly UnityConfigurationSection _configurationSection;

        static UnityDiContainer()
        {
            // We lezen de configurationsection 1 keer in. Want ik heb de indruk dat die telkens leeg is
            // als ik ze opnieuw probeer te lezen voor de volgende configuratie.
            // Normaal gezien is er ook maar 1 unity configuratie per toepassing, en verandert die niet
            // terwijl de toepassing loopt.
            _configurationSection = (UnityConfigurationSection) ConfigurationManager.GetSection("unity");
        }

        public UnityDiContainer()
        {
            _container = new UnityContainer();
        }

        public IUnityContainer UnityContainer { get { return _container; } }

        public void Dispose()
        {
            _container.Dispose();
        }

        /// <summary>
        /// Initialiseert de unity container voor de factory.
        /// </summary>
        public void InitVolgensConfigFile()
        {
            // Als section null is, is er geen unity-configuratie in app.config of web.config.
            // Op zich geen probleem, want de configuratie kan ook at runtime

            if (_configurationSection != null)
            {
                // Als de toepassing hierop crasht, kijk dan je config file na:
                // * Heb je bij je types ook de assembly's vermeld waarin ze gedefinieerd zijn?
                // * Bevat je project referenties naar al die assemblies?
                // * Hernoemde je onlangs interfaces of implementaties?

                // Wat ook kan helpen, is de ongebruikte referenties uit je project verwijderen,
                // alsook de referenties nodig voor dependency injection, en die dan allemaal
                // terugleggen.

                _configurationSection.Configure(_container);
            }
        }

        /// <summary>
		/// Gebruik Unity om een instantie van type/interface T
		/// te creëren.
		/// </summary>
        /// <typeparam name="T">Van welk type het object moet zijn</typeparam>
		/// <returns>Het type</returns>
		public  T Maak<T>()
		{
			Debug.Assert(_container != null);

			return _container.Resolve<T>();
		}

		/// <summary>
		/// Gebruik Unity om een instantie van het gevraagde type te creëren.
		/// </summary>
		/// <param name="t">Van welk type het object moet zijn</param>
		/// <returns>Het geïnstantieerde object</returns>
		public  object Maak(Type t)
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
		public  void InstantieRegistreren<T>(T instantie)
		{
            Debug.Assert(_container != null);   // als deze assert failt, vergat je de container te initialiseren.

		    _container.RegisterInstance(instantie);
		}

        /// <summary>
        /// Configureert de IOC-container zodanig dat er telkens een nieuwe instantie van het gegeven
        /// <paramref name="type"/> gebruikt wordt als er een klase van type <typeparamref name="T"/>
        /// gevraagd wordt.
        /// </summary>
        /// <typeparam name="T">Gevraagde type</typeparam>
        /// <param name="type">Type van opgeleverd object</param>
        public  void TypeRegistreren<T>(Type type)
        {
            _container.RegisterType(type);
        }
	}
}
