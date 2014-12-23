/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
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
using System.Collections;
using System.Collections.Generic;
using Chiro.Cdf.Ioc;
using Microsoft.Practices.Unity;

namespace Chiro.Cdf.ServiceHelper
{
    /// <summary>
    /// Service provider die IOC gebruikt om implementaties van de service-interface te genereren.
    /// </summary>
    /// <remarks></remarks>
    public class IocServiceProvider: IServiceProvider
    {
        /// <summary>
        /// Gebruikt IoC om de service met interface I te instantieren.
        /// </summary>
        /// <typeparam name="I">Service interface</typeparam>
        /// <returns>Een instantie van <typeparamref name="I"/>.  Als die niet geresolved kon worden, dan <c>null</c></returns>
        /// <remarks></remarks>
        public I GetService<I>() where I : class
        {
            try
            {
                return Factory.Maak<I>();
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the configured service implementation instance with the specified name.
        /// </summary>
        /// <typeparam name="I">The interface or type to get the service for.</typeparam>
        /// <param name="instanceName">The name of the service instance.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public I GetService<I>(string instanceName) where I : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Try and get the requested service
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool TryGetService<I>(out I service) where I : class
        {
            throw new NotImplementedException();
        }
    }
}
