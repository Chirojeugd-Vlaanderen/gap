using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Chiro.Adf.ServiceModel.Extensions
{
	/// <summary>
	/// TODO (#190): Documenteren!
	/// </summary>
	public class CallContextBehaviorExtensionElement : BehaviorExtensionElement
	{
		private ConfigurationPropertyCollection _properties;

		/// <summary>
		/// Gets the type of behavior.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Type" />.
		/// </returns>
		public override Type BehaviorType
		{
			get { return typeof(CallContextBehavior); }
		}

		/// <summary>
		/// Creates a behavior extension based on the current configuration settings.
		/// </summary>
		/// <returns>
		/// The behavior extension.
		/// </returns>
		protected override object CreateBehavior()
		{
			Type type = Type.GetType(ContextType);

			if (type == null)
				throw new ConfigurationErrorsException(string.Format("The type '{0}' could not be initialized.", ContextType));

			return new CallContextBehavior(type);
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

		/// <summary>
		/// TODO (#190): Documenteren!
		/// </summary>
		[ConfigurationProperty("contextType", IsRequired = true)]
		public string ContextType
		{
			get { return (string)base["contextType"]; }
			set { base["contextType"] = value; }
		}

	}
}