// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.ComponentModel;
using System.Configuration;

namespace Chiro.Cdf.DependencyInjection
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
