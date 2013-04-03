using System.Configuration;

namespace Chiro.Adf.Configuration
{
	/// <summary>
	/// Represents the confgiuration section named 'chiroGroep'.
	/// </summary>
	public class AdfConfigurationSection : ConfigurationSection
	{
		internal const string DEFAULT_ADF_CONFIGURATION_SECTION_NAME = "serviceHelper";
		internal const string SERVICE_PROVIDERS_ELEMENT_NAME = "serviceProviders";
        
		/// <summary>
		/// Gets the configured service providers.
		/// </summary>
		[ConfigurationProperty(SERVICE_PROVIDERS_ELEMENT_NAME, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ServiceProviderSettingsCollection ServiceProviders
		{
			get { return (ServiceProviderSettingsCollection)base[SERVICE_PROVIDERS_ELEMENT_NAME]; }
		}
	}
}
