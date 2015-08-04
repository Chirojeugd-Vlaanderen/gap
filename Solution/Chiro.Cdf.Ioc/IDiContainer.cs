/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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

namespace Chiro.Cdf.Ioc
{
    /// <summary>
    /// Wrapper rond een echte dependency injection container.
    /// </summary>
    public interface IDiContainer : IDisposable
    {
        /// <summary>
        /// Initialiseert de container op basis van de config file.
        /// </summary>
        void InitVolgensConfigFile();

        /// <summary>
        /// Creeert een instantie van type/interface <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type voor het gevraagde object.</typeparam>
        /// <returns>Object van het gegeven type.</returns>
        T Maak<T>();

        /// <summary>
        /// Creeert een instantie van type/interface <paramref name="t"/>.
        /// </summary>
        /// <param name="t">Type voor het gevraagde object.</param>
        /// <returns>Object van het gegeven type.</returns>
        object Maak(Type t);

        /// <summary>
        /// Registreer een object dat opgeleverd moet worden als een gegeven
        /// interface/type <typeparamref name="T"/> gevraagd is.
        /// </summary>
        /// <typeparam name="T">Type of interface.</typeparam>
        /// <param name="instantie">Op te leveren object object</param>
        void InstantieRegistreren<T>(T instantie);

        /// <summary>
        /// Configureert de IOC-container zodanig dat er telkens een nieuwe instantie van het gegeven
        /// <paramref name="type"/> gebruikt wordt als er een klasse van of interface <typeparamref name="T"/>
        /// gevraagd wordt.
        /// </summary>
        /// <typeparam name="T">Gevraagde type.</typeparam>
        /// <param name="type">Type van op te leveren object.</param>
        void TypeRegistreren<T>(Type type);
    }
}