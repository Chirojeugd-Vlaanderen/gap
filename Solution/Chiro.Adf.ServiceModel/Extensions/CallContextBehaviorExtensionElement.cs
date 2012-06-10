using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Chiro.Adf.ServiceModel.Extensions
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
	public class CallContextBehaviorExtensionElement : BehaviorExtensionElement
	{
        /// <summary>
        /// 
        /// </summary>
		private ConfigurationPropertyCollection _properties;

        /// <summary>
        /// Gets the type of behavior.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/>.
        ///   </returns>
        /// <remarks></remarks>
		public override Type BehaviorType
		{
			get { return typeof(CallContextBehavior); }
		}

        /// <summary>
        /// Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns>The behavior extension.</returns>
        /// <remarks></remarks>
		protected override object CreateBehavior()
		{
			var type = Type.GetType(this.ContextType);

			if (type == null)
				throw new ConfigurationErrorsException(string.Format("The type '{0}' could not be initialized.", this.ContextType));

			return new CallContextBehavior(type);
		}


        /// <summary>
        /// Gets the collection of properties.
        /// </summary>
        /// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection"/> of properties for the element.</returns>
        /// <remarks></remarks>
		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this._properties == null)
				{
					this._properties = new ConfigurationPropertyCollection { new ConfigurationProperty("contextType", typeof(string)) };
				}
				return this._properties;
			}
		}

        /// <summary>
        /// Gets or sets the type of the context.
        /// </summary>
        /// <value>The type of the context.</value>
        /// <remarks></remarks>
		[ConfigurationProperty("contextType", IsRequired = true)]
		public string ContextType
		{
			get { return (string)base["contextType"]; }
			set { base["contextType"] = value; }
		}

	}
}