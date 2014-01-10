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
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Messaging;

using Microsoft.Practices.Unity;

// LET OP! De klasses in dit project hebben niets te maken met ServiceHelper, wel met de
// implementatie van onze message queue services.

namespace Chiro.Cdf.ServiceModel
{
    /// <summary>
    /// Generic ServiceHost with extra features 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceHost<T> : ServiceHost
    {
        public ServiceHost()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// Constructor voor dependency injection.  Hier wordt DI gebruikt om de instantie
        /// aan te maken.  (Hoop ik.)
        /// </summary>
        /// <param name="instantie"></param>
        [InjectionConstructor]
        public ServiceHost(T instantie)
            : base(instantie)
        {
        }

        public ServiceHost(params string[] baseAdresses)
            : base(typeof(T), Convert(baseAdresses))
        {
        }

        public ServiceHost(params Uri[] baseAdresses)
            : base(typeof(T), baseAdresses)
        {
        }

        static Uri[] Convert(string[] baseAddresses)
        {
            Converter<string, Uri> convert = address => new Uri(address);
            return baseAddresses.ConvertAll(convert);
        }
    }

    public static class QueuedServiceHelper
    {
        public static void VerifyQueue(this ServiceEndpoint endpoint)
        {
            if (endpoint.Binding is NetMsmqBinding)
            {
                string queue = GetQueueFromUri(endpoint.Address.Uri);
                if (MessageQueue.Exists(queue) == false)
                {
                    MessageQueue.Create(queue, true);
                }
            }
        }

        private static string GetQueueFromUri(Uri uri)
        {
            string path = uri.PathAndQuery;
            string queue = "." + path.Replace(@"/", @"\").Replace("private", "private$");
            return queue;
        }
    }
}
