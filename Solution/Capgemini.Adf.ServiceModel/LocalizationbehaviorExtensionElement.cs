using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using Capgemini.Adf.ServiceModel.Extensions;

namespace Capgemini.Adf.ServiceModel
{
	/// <summary>
	/// Represents the configuration element that registers the LocalizationBehavior.
	/// </summary>
	public class LocalizationBehaviorExtensionElement : BehaviorExtensionElement
	{
		private ConfigurationPropertyCollection properties;

		protected override object CreateBehavior()
		{
			return new LocalizationBehavior();
		}

		public override Type BehaviorType
		{
			get { return typeof (LocalizationBehavior); }
		}

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