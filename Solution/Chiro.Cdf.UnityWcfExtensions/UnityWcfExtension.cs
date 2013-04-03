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
﻿/*
 * Thanks to Andrew Oakley
 * Unity Lifetime Managers and WCF
 * http://blogs.msdn.com/b/atoakley/archive/2010/12/29/unity-lifetime-managers-and-wcf.aspx
 * 
 */

using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Chiro.Cdf.UnityWcfExtensions
{
    /// <summary>
    /// Base class for Unity WCF lifetime manager support.
    /// </summary>
    /// <typeparam name="T">IExtensibleObject on which to attach.</typeparam>
    public class UnityWcfExtension<T> : IExtension<T>
        where T : IExtensibleObject<T>
    {
        /// <summary>
        /// Backing store for relating keys to object instances for Unity.
        /// </summary>
        private readonly Dictionary<Guid, object> instances = new Dictionary<Guid, object>();

        /// <summary>
        /// Enables an extension object to find out when it has been aggregated. Called when the extension is added to the IExtensibleObject.Extensions property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Attach(T owner)
        {
        }

        /// <summary>
        /// Enables an object to find out when it is no longer aggregated. Called when an extension is removed from the IExtensibleObject.Extensions property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Detach(T owner)
        {
            // If we are being detached, let's go ahead and clean up, just in case.
            List<Guid> keys = new List<Guid>(this.instances.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                this.RemoveInstance(keys[i]);
            }
        }

        /// <summary>
        /// Registers the given instance with the given key into the backing store.
        /// </summary>
        /// <param name="key">Key to associate with the object instance.</param>
        /// <param name="value">Object instance to associate with the given key in the backing store.</param>
        public void RegisterInstance(Guid key, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.instances.Add(key, value);
        }

        /// <summary>
        /// Finds the object associated with the given key.
        /// </summary>
        /// <param name="key">Key used to find the associated object instance.</param>
        /// <returns>The object instance associated with the supplied key.  If no instance is registered, null is returned.</returns>
        public object FindInstance(Guid key)
        {
            object obj = null;

            // We don't care whether or not this succeeds or fails.
            this.instances.TryGetValue(key, out obj);
            return obj;
        }

        /// <summary>
        /// Removes the given key from the backing store.  This method will also dispose of the associated object instance if it implements <see cref="System.IDisposable"/>.
        /// </summary>
        /// <param name="key">Key to remove from the backing store.</param>
        public void RemoveInstance(Guid key)
        {
            // We don't want to use FindInstance JUST IN CASE somehow a key gets in there with a null object.
            object instance = null;

            if (this.instances.ContainsKey(key))
            {
                // Get the instance.
                instance = this.instances[key];

                // See if it needs disposing.
                IDisposable disposable = instance as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }

                // Remove it from the instances list.
                this.instances.Remove(key);
            }
        }
    }
}
