/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
﻿using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Chiro.Adf.ServiceModel.Extensions
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class CallContextInitializer : ICallContextInitializer
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly Type _contextType;


        /// <summary>
        /// Initializes a new instance of the <see cref="CallContextInitializer"/> class.
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        /// <remarks></remarks>
        public CallContextInitializer(Type contextType)
        {
            _contextType = contextType;
        }

        /// <summary>
        /// Implement to participate in the initialization of the operation thread.
        /// </summary>
        /// <param name="instanceContext">The service instance for the operation.</param>
        /// <param name="channel">The client channel.</param>
        /// <param name="message">The incoming message.</param>
        /// <returns>A correlation object passed back as the parameter of the <see cref="M:System.ServiceModel.Dispatcher.ICallContextInitializer.AfterInvoke(System.Object)"/> method.</returns>
        /// <remarks></remarks>
        object ICallContextInitializer.BeforeInvoke(System.ServiceModel.InstanceContext instanceContext, System.ServiceModel.IClientChannel channel, Message message)
        {
            CallContext.Current = Activator.CreateInstance(_contextType) as CallContext;

            if (CallContext.Current == null)
                throw new ConfigurationErrorsException(string.Format("The type '{0}' could not be initialized.", _contextType));

            CallContext.Current.Initialize();

            return null; // we don't need no correlation
        }

        /// <summary>
        /// Afters the invoke.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <remarks></remarks>
        void ICallContextInitializer.AfterInvoke(object state)
        {
            // Dispose the context if it as created
            if (CallContext.Current != null)
                CallContext.Current.Dispose();
        }
    }
}