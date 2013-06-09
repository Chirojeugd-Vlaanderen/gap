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
using System.Globalization;
using System.ServiceModel;

using Microsoft.Practices.Unity;

namespace Chiro.Cdf.UnityWcfExtensions
{
    /// <summary>
    /// Abstract base class for Unity WCF lifetime support.
    /// </summary>
    /// <typeparam name="T">IExtensibleObject for which to register Unity lifetime manager support.</typeparam>
    public abstract class UnityWcfLifetimeManager<T> : LifetimeManager
        where T : IExtensibleObject<T>
    {
        /// <summary>
        /// Key for Unity to use for the associated object's instance.
        /// </summary>
        private readonly Guid key = Guid.NewGuid();

        /// <summary>
        /// Gets the currently registered UnityWcfExtension for this lifetime manager.
        /// </summary>
        private UnityWcfExtension<T> Extension
        {
            get
            {
                UnityWcfExtension<T> extension = this.FindExtension();
                if (extension == null)
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.CurrentCulture, "UnityWcfExtension<{0}> must be registered in WCF.", typeof(T).Name));
                }

                return extension;
            }
        }

        /// <summary>
        /// Gets the object instance for the given key from the currently registered extension.
        /// </summary>
        /// <returns>The object instance associated with the given key.  If no instance is registered, null is returned.</returns>
        public override object GetValue()
        {
            return this.Extension.FindInstance(this.key);
        }

        /// <summary>
        /// Removes the object instance for the given key from the currently registered extension.
        /// </summary>
        public override void RemoveValue()
        {
            // Not called, but just in case.
            this.Extension.RemoveInstance(this.key);
        }

        /// <summary>
        /// Associates the supplied object instance with the given key in the currently registered extension.
        /// </summary>
        /// <param name="newValue">The object to register in the currently registered extension.</param>
        public override void SetValue(object newValue)
        {
            this.Extension.RegisterInstance(this.key, newValue);
        }

        /// <summary>
        /// Finds the currently registered UnityWcfExtension for this lifetime manager.
        /// </summary>
        /// <returns>Currently registered UnityWcfExtension of the given type, or null if no UnityWcfExtension is registered.</returns>
        protected abstract UnityWcfExtension<T> FindExtension();
    }
}
