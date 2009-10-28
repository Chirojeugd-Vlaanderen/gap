using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using Chiro.Adf.ServiceModel.Extensions;

namespace Chiro.Adf.ServiceModel
{
	/// <summary>
	/// Represents the configuration element that registers the LocalizationBehavior.
	/// </summary>
	public class LocalizationBehaviorExtensionElement : BehaviorExtensionElement
	{
		private ConfigurationPropertyCollection properties;

        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
        /// <returns></returns>
		protected override object CreateBehavior()
		{
			return new LocalizationBehavior();
		}

        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
		public override Type BehaviorType
		{
			get { return typeof (LocalizationBehavior); }
		}

        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (properties == null)
				{
					properties = new ConfigurationPropertyCollection { new ConfigurationProperty("contextType", typeof(string)) };
				}
				return properties;
			}
		}
	}
}