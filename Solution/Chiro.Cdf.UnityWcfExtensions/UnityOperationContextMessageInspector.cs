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
﻿using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Chiro.Cdf.UnityWcfExtensions
{
    /// <summary>
    /// Adds and removes instances of <see cref="UnityOperationContextExtension"/> from the current operation context.
    /// </summary>
    public class UnityOperationContextMessageInspector : IDispatchMessageInspector
    {
        /// <summary>
        /// Adds an instance of the <see cref="UnityOperationContextExtension"/> class to the current operation context after an inbound message has been received but before the message is dispatched to the intended operation.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="channel">The incoming channel.</param>
        /// <param name="instanceContext">The current service instance.</param>
        /// <returns>The object used to correlate state. This object is passed back in the BeforeSendReply method.</returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            OperationContext.Current.Extensions.Add(new UnityOperationContextExtension());
            return null;
        }

        /// <summary>
        /// Removes the registered instance of the <see cref="UnityOperationContextExtension"/> class from the current operation context after the operation has returned but before the reply message is sent.
        /// </summary>
        /// <param name="reply">The reply message. This value is null if the operation is one way.</param>
        /// <param name="correlationState">The correlation object returned from the AfterReceiveRequest method.</param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            OperationContext.Current.Extensions.Remove(UnityOperationContextExtension.Current);
        }
    }
}
