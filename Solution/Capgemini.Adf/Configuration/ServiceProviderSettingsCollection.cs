using System.Configuration;

namespace Cg2.Adf.Configuration
{
	/// <summary>
	/// Represents a collection of ServiceProvider configuration elements.
	/// </summary>
	public class ServiceProviderSettingsCollection : ConfigurationElementCollection
	{
		///<summary>
		///When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.
		///</summary>
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
		///</summary>
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