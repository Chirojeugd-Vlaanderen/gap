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

using System;
using System.Configuration;

namespace Chiro.Cdf.DependencyInjection
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
	public class TypeMappingElementCollection : ConfigurationElementCollection
	{
        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>A new <see cref="T:System.Configuration.ConfigurationElement"/>.</returns>
        /// <remarks></remarks>
		protected override ConfigurationElement CreateNewElement()
		{
			return new TypeMappingElement();
		}

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.</returns>
        /// <remarks></remarks>
		protected override Object GetElementKey(ConfigurationElement element)
		{
			return ((TypeMappingElement)element).Name;
		}

        /// <summary>
        /// Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <returns>The specified property, attribute, or child element</returns>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">
        ///   <paramref name="prop"/> is read-only or locked.</exception>
        /// <remarks></remarks>
		public TypeMappingElement this[int index]
		{
			get
			{
				return (TypeMappingElement)BaseGet(index);
			}
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

        /// <summary>
        /// Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <returns>The specified property, attribute, or child element</returns>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">
        ///   <paramref name="prop"/> is read-only or locked.</exception>
        /// <remarks></remarks>
		new public TypeMappingElement this[string name]
		{
			get
			{
				return (TypeMappingElement)BaseGet(name);
			}
		}

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="typeMapping">The type mapping.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public int IndexOf(TypeMappingElement typeMapping)
		{
			return BaseIndexOf(typeMapping);
		}

        /// <summary>
        /// Removes the specified type mapping.
        /// </summary>
        /// <param name="typeMapping">The type mapping.</param>
        /// <remarks></remarks>
		public void Remove(TypeMappingElement typeMapping)
		{
			if (BaseIndexOf(typeMapping) >= 0)
			{
				BaseRemove(typeMapping.Name);
			}
		}

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <remarks></remarks>
		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <remarks></remarks>
		public void Remove(string name)
		{
			BaseRemove(name);
		}
	}
}
