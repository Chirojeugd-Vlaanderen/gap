// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;

namespace Chiro.Cdf.DependencyInjection
{
	/// <summary>
	/// Configuration section for the DIServiceBehavior. 
	/// </summary>
	public class DIServiceBehaviorSection : BehaviorExtensionElement
	{
		[ConfigurationProperty("typeMappings")]
		public TypeMappingElementCollection TypeMappings
		{
			get
			{
				return (TypeMappingElementCollection)base["typeMappings"];
			}
		}

		public override Type BehaviorType
		{
			get { return typeof(DIServiceBehavior); }
		}

		protected override object CreateBehavior()
		{
			var typeMappings = (from TypeMappingElement typeMappingElement in TypeMappings
			                    select new TypeMapping(typeMappingElement.TypeRequested, typeMappingElement.TypeToBuild)).ToList();

			return new DIServiceBehavior(typeMappings);
		}
	}
}
