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

namespace Algemeen.DependencyInjectionBehavior
{
    /// <summary>
    /// TODO (#190): documenteren
    /// </summary>
	public class TypeMappingElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new TypeMappingElement();
		}

		protected override Object GetElementKey(ConfigurationElement element)
		{
			return ((TypeMappingElement)element).Name;
		}

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

		new public TypeMappingElement this[string name]
		{
			get
			{
				return (TypeMappingElement)BaseGet(name);
			}
		}

		public int IndexOf(TypeMappingElement typeMapping)
		{
			return BaseIndexOf(typeMapping);
		}

		public void Remove(TypeMappingElement typeMapping)
		{
			if (BaseIndexOf(typeMapping) >= 0)
			{
				BaseRemove(typeMapping.Name);
			}
		}

		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		public void Remove(string name)
		{
			BaseRemove(name);
		}
	}
}
