/*
 * Copyright 2008 Capgemini - Accelerated Delivery Framework - http://www.be.capgemini.com/
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

using System.Collections.Generic;
using System.Configuration;

namespace Chiro.Cdf.ServiceHelper.Configuration
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
