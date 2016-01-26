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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Chiro.Cdf.UnityWcfExtensions
{
    /// <summary>
    /// Initializes and cleans up thread-local storage for the thread that invokes user code to support the <see cref="UnityContextChannelLifetimeManager"/>.
    /// </summary>
    public class UnityCallContextInitializer : ICallContextInitializer
    {
        /// <summary>
        /// Cleans up the thread that invoked the operation by removing the <see cref="UnityContextChannelExtension"/>.
        /// </summary>
        /// <param name="correlationState">The correlation object returned from the BeforeInvoke method.</param>
        public void AfterInvoke(object correlationState)
        {
            // It feels wrong going through the OperationContext to get to the channel, but since it's not passed as a parameter
            // to this method, like BeforeInvoke(), we have to do it this way.  Should we return a correlation state
            // from BeforeInvoke() to get to this?
            OperationContext.Current.Channel.Extensions.Remove(UnityContextChannelExtension.Current);
        }

        /// <summary>
        /// Initializes the operation thread by adding the <see cref="UnityContextChannelExtension"/> to the WCF client channel.
        /// </summary>
        /// <param name="instanceContext">The service instance for the operation.</param>
        /// <param name="channel">The client channel.</param>
        /// <param name="message">The incoming message.</param>
        /// <returns>A correlation object passed back as a parameter of the AfterInvoke method.</returns>
        public object BeforeInvoke(InstanceContext instanceContext, IClientChannel channel, Message message)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("channel");
            }

            channel.Extensions.Add(new UnityContextChannelExtension());
            return null;
        }
    }
}
