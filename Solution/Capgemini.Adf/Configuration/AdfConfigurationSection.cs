using System.Configuration;

namespace Capgemini.Adf.Configuration
{
	/// <summary>
	/// Represents the Capgemini.Adf confgiuration section named 'capgemini.Adf'.
	/// </summary>
	public class AdfConfigurationSection : ConfigurationSection
	{
		internal const string DefaultAdfConfigurationSectionName = "capgemini.Adf";
		internal const string ServiceProvidersElementName = "serviceProviders";
        
		/// <summary>
		/// Gets the configured service providers.
		/// </summary>
		[ConfigurationProperty(ServiceProvidersElementName, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ServiceProviderSettingsCollection ServiceProviders
		{
			get { return (ServiceProviderSettingsCollection)base[ServiceProvidersElementName]; }
		}
	}
}
