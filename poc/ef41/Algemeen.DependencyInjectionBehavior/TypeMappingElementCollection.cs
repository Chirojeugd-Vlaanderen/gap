// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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