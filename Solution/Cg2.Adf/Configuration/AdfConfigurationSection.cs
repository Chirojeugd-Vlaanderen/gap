using System.Configuration;

namespace Cg2.Adf.Configuration
{
	/// <summary>
	/// Represents the confgiuration section named 'chiroGroep'.
	/// </summary>
	public class AdfConfigurationSection : ConfigurationSection
	{
		internal const string DefaultAdfConfigurationSectionName = "serviceHelper";
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
