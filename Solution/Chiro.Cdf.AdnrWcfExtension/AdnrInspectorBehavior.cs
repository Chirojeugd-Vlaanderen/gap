/*
 * Copyright 2017 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Chiro.Cdf.Authentication;

namespace Chiro.Cdf.AdnrWcfExtension
{
    // Thank you http://trycatch.me/adding-custom-message-headers-to-a-wcf-service-using-inspectors-behaviors
    // Eigenlijk zijn messageinspectors, endpointbehaviors en servicebehaviors verschillende 
    // zeken, maar hier hebben we een klasse die alles tegelijk is. Ik nam dat gewoon over van 
    // de blogpost. Misschien moeten we dat nog opsplitsen.
    [AttributeUsage(AttributeTargets.Class)]
    public class AdnrInspectorBehavior : Attribute, IDispatchMessageInspector, IClientMessageInspector, IEndpointBehavior, IServiceBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // empty.
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            // empty.
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            // empty.
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            //Retrieve Inbound Object from Request
            var header = request.Headers.GetHeader<UserInfo>("adnr-header", "s");
            if (header != null)
            {
                OperationContext.Current.IncomingMessageProperties.Add("UserInfo", header);
            }
            return null;
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var inspector = new AdnrInspectorBehavior();
            clientRuntime.MessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (var eDispatcher in cDispatcher.Endpoints)
                {
                    eDispatcher.DispatchRuntime.MessageInspectors.Add(new AdnrInspectorBehavior());
                }
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var channelDispatcher = endpointDispatcher.ChannelDispatcher;
            if (channelDispatcher == null) return;
            foreach (var ed in channelDispatcher.Endpoints)
            {
                var inspector = new AdnrInspectorBehavior();
                ed.DispatchRuntime.MessageInspectors.Add(inspector);
            }
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            // empty.
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            //Instantiate new HeaderObject with values from ClientContext;
            var dataToSend = new UserInfo
            {
                // FIXME: Voorlopig hardgecodeerd test-ad-nummer.
                AdNr = 39198
            };

            var typedHeader = new MessageHeader<UserInfo>(dataToSend);
            var untypedHeader = typedHeader.GetUntypedHeader("adnr-header", "s");

            request.Headers.Add(untypedHeader);
            return null;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            // empty.
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // empty.
        }
    }
}
