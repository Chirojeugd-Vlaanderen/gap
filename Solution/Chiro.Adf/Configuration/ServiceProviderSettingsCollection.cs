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

using System.Configuration;

namespace Chiro.Cdf.ServiceHelper.Configuration
{
	/// <summary>
	/// Represents a collection of ServiceProvider configuration elements.
	/// </summary>
	public class ServiceProviderSettingsCollection : ConfigurationElementCollection
	{
		///<summary>
		///When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.
		/// </summary>
		///
		///<returns>
		///A new <see cref="T:System.Configuration.ConfigurationElement" />.
		///</returns>
		///
		protected override ConfigurationElement CreateNewElement()
		{
			return new ServiceProviderConfigurationElement();
		}

		///<summary>
		///Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		///
		///<returns>
		///An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.
		///</returns>
		///
		///<param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for. </param>
		protected override object GetElementKey(ConfigurationElement element) 
		{
			return ((ServiceProviderConfigurationElement) element).Name;
		}

		/// <summary>
		/// Adds a ServiceProviderConfigurationElement to the ServiceProviderSettingsCollection.
		/// </summary>
		/// <param name="configurationElement"></param>
		public void Add(ServiceProviderConfigurationElement configurationElement)
		{
			BaseAdd(configurationElement);
		}
	}
}