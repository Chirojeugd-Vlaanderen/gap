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
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Chiro.Cdf.UnityWcfExtensions
{
    /// <summary>
    /// Modifies the creation of a <see cref="System.ServiceModel.InstanceContext"/> by adding an instance of the <see cref="UnityInstanceContextExtension"/> class.
    /// </summary>
    public class UnityInstanceContextInitializer : IInstanceContextInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityInstanceContextInitializer"/> class.
        /// </summary>
        public UnityInstanceContextInitializer()
            : base()
        {
        }

        /// <summary>
        /// Modifies the newly created <see cref="System.ServiceModel.InstanceContext"/> by adding an instance of the <see cref="UnityInstanceContextExtension"/> class.
        /// </summary>
        /// <param name="instanceContext">The system-supplied instance context.</param>
        /// <param name="message">The message that triggered the creation of the instance context.</param>
        public void Initialize(InstanceContext instanceContext, Message message)
        {
            if (instanceContext == null)
            {
                throw new ArgumentNullException("instanceContext");
            }

            instanceContext.Extensions.Add(new UnityInstanceContextExtension());

            // We need to subscribe to the Closing event so we can remove the extension.
            instanceContext.Closing += new System.EventHandler(this.InstanceContextClosing);
        }

        /// <summary>
        /// Occurs when a communication object transitions into the closing state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="System.EventArgs" /> that contains no event data.</param>
        private void InstanceContextClosing(object sender, System.EventArgs e)
        {
            InstanceContext instanceContext = sender as InstanceContext;
            if (instanceContext != null)
            {
                instanceContext.Closing -= new System.EventHandler(this.InstanceContextClosing);

                // We have to get this manually, as the operation context has been disposed by now.
                UnityInstanceContextExtension extension = instanceContext.Extensions.Find<UnityInstanceContextExtension>();
                if (extension != null)
                {
                    instanceContext.Extensions.Remove(extension);
                }
            }
        }
    }
}
