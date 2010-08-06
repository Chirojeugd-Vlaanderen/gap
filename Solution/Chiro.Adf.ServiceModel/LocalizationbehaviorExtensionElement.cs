using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Chiro.Adf.ServiceModel
{
	/// <summary>
	/// Represents the configuration element that registers the LocalizationBehavior.
	/// </summary>
	public class LocalizationBehaviorExtensionElement : BehaviorExtensionElement
	{
		private ConfigurationPropertyCollection _properties;

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <returns></returns>
		protected override object CreateBehavior()
		{
			return new LocalizationBehavior();
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
		public override Type BehaviorType
		{
			get { return typeof (LocalizationBehavior); }
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (_properties == null)
				{
					_properties = new ConfigurationPropertyCollection { new ConfigurationProperty("contextType", typeof(string)) };
				}
				return _properties;
			}
		}
	}
}