using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Chiro.Adf.ServiceModel
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class LocalizationBehaviorExtensionElement : BehaviorExtensionElement
    {
        /// <summary>
        /// 
        /// </summary>
        private ConfigurationPropertyCollection _properties;

        /// <summary>
        /// Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns>The behavior extension.</returns>
        /// <remarks></remarks>
        protected override object CreateBehavior()
        {
            return new LocalizationBehavior();
        }

        /// <summary>
        /// Gets the type of behavior.
        /// </summary>
        /// <returns>A <see cref="T:System.Type"/>.</returns>
        /// <remarks></remarks>
        public override Type BehaviorType
        {
            get { return typeof(LocalizationBehavior); }
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
    }
}