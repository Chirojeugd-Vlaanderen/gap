// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
