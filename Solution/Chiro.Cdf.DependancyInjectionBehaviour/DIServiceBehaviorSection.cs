using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Configuration;
using System.Configuration;

namespace Chiro.Cdf.DependencyInjection
{
	/// <summary>
	/// Configuration section for the DIServiceBehavior. 
	/// </summary>
	public class DIServiceBehaviorSection : BehaviorExtensionElement
	{
		public DIServiceBehaviorSection()
			: base()
		{
		}

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
			List<TypeMapping> typeMappings = new List<TypeMapping>();
			foreach (TypeMappingElement typeMappingElement in this.TypeMappings)
			{
				TypeMapping typeMapping = new TypeMapping(typeMappingElement.TypeRequested, typeMappingElement.TypeToBuild);
				typeMappings.Add(typeMapping);
			}
			
			return new DIServiceBehavior(typeMappings);
		}
	}
}
