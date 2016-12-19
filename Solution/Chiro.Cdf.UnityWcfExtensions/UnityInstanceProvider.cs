/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
﻿using System;
﻿using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Chiro.Cdf.Ioc;
﻿using Chiro.Cdf.Ioc.Factory;

namespace Chiro.Cdf.UnityWcfExtensions
{
    /// <summary>
    /// Declares methods that provide a service object or recycle a service object for a Windows Communication Foundation (WCF) service.
    /// </summary>
    public class UnityInstanceProvider : IInstanceProvider
    {
        /// <summary>
        /// The <see cref="System.Type"/> to create.
        /// </summary>
        private readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityInstanceProvider"/> class with the <see cref="Type"/>
        /// to create and the name of the container to use.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to create.</param>
        /// <param name="containerName">The name of the container to use.</param>
        /// <remarks>
        /// If <paramref name="containerName"/> is null then the default configuration is used. If the Unity configuration section
        /// is not found, the container will just try and resolve the type.
        /// </remarks>
        public UnityInstanceProvider(Type type, string containerName)
        {
            this.type = type;
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="InstanceContext" /> object.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="InstanceContext" /> object.</param>
        /// <returns>
        /// A user-defined service object.
        /// </returns>
        /// <remarks>
        /// Uses the configured Unity container to resolve the service object.
        /// </remarks>
        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext" /> object.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext" /> object.</param>
        /// <param name="message">The message that triggered the creation of a service object.</param>
        /// <returns>
        /// The service object.
        /// </returns>
        /// <remarks>
        /// Uses the configured Unity container to resolve the service object.
        /// </remarks>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return Factory.Maak(this.type);
        }

        /// <summary>
        /// Called when an <see cref="T:System.ServiceModel.InstanceContext" /> object recycles a service object.
        /// </summary>
        /// <param name="instanceContext">The service's instance context.</param>
        /// <param name="instance">The service object to be recycled.</param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            // Since we created the service instance, we need to dispose of it, if needed.
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
