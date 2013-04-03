/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
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