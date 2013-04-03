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
using System.ComponentModel;
using System.Configuration;

namespace Algemeen.DependencyInjectionBehavior
{
    /// <summary>
    /// TODO (#190): documenteren
    /// </summary>
	public class TypeMappingElement : ConfigurationElement
	{
		private AssemblyQualifiedTypeNameConverter typeConverter = new AssemblyQualifiedTypeNameConverter();

		[ConfigurationProperty("name")]
		public string Name
		{
			get
			{
				return (string)base["name"];
			}
		}

		[ConfigurationProperty("typeRequested")]
		public string TypeRequestedName
		{
			get
			{
				return (string)base["typeRequested"];
			}
		}

		[ConfigurationProperty("typeToBuild")]
		public string TypeToBuildName
		{
			get
			{
				return (string)base["typeToBuild"];
			}
		}

		public Type TypeRequested
		{
			get { return (Type)typeConverter.ConvertFrom(TypeRequestedName); }
		}

		public Type TypeToBuild
		{
			get { return (Type)typeConverter.ConvertFrom(TypeToBuildName); }
		}
	}

	/// <summary>
	/// Represents a configuration converter that converts a string to <see cref="Type"/> based on a fully qualified name.
	/// </summary>
	public class AssemblyQualifiedTypeNameConverter : ConfigurationConverterBase
	{
		/// <summary>
		/// Returns the assembly qualified name for the passed in Type.
		/// </summary>
		/// <param name="context">The container representing this System.ComponentModel.TypeDescriptor.</param>
		/// <param name="culture">Culture info for assembly</param>
		/// <param name="value">Value to convert.</param>
		/// <param name="destinationType">Type to convert to.</param>
		/// <returns>Assembly Qualified Name as a string</returns>
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (value != null)
			{
				var typeValue = value as Type;
				if (typeValue == null)
				{
					throw new ArgumentException("The type specified can not be loaded");
				}

				if (typeValue != null)
				{
					return (typeValue).AssemblyQualifiedName;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns a type based on the assembly qualified name passed in as data.
		/// </summary>
		/// <param name="context">The container representing this System.ComponentModel.TypeDescriptor.</param>
		/// <param name="culture">Culture info for assembly.</param>
		/// <param name="value">Data to convert.</param>
		/// <returns>Type of the data</returns>
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var stringValue = (string)value;
			if (!string.IsNullOrEmpty(stringValue))
			{
				Type result = Type.GetType(stringValue, false);
				if (result == null)
				{
					throw new ArgumentException("Invalid type value");
				}

				return result;
			}
			return null;
		}
	}
}
