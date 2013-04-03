using System.Collections.Generic;
using System.Configuration;

namespace Chiro.Adf.Configuration
{
	/// <summary>
	/// Represents a ServiceProvider configuration element.
	/// </summary>
	public class ServiceProviderConfigurationElement : ConfigurationElement
	{
		private readonly IDictionary<string, string> _constructorParameters = new Dictionary<string, string>();

		/// <summary>
		/// Gets the logical configuration name of the service provider.
		/// </summary>
		[ConfigurationProperty("name")]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		/// <summary>
		/// Gets or sets the fully qualified name of the service provider type.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods"), ConfigurationProperty("type")]
		public string Type
		{
			get { return (string)this["type"]; }
			set { this["type"] = value; }
		}

		/// <summary>
		/// Gets key value pairs representing the configured constructor parameters a service provider.
		/// </summary>
		public IDictionary<string, string> ConstructorParameters
		{
			get { return _constructorParameters; }
		}

		///<summary>
		///Gets a value indicating whether an unknown attribute is encountered during deserialization.
		/// </summary>
		///
		///<returns>
		///true when an unknown attribute is encountered while deserializing; otherwise, false.
		///</returns>
		///
		///<param name="name">The name of the unrecognized attribute.</param>
		///<param name="value">The value of the unrecognized attribute.</param>
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			_constructorParameters.Add(name, value);
			return true;
		}

		
	}
}
