// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Reflection;

namespace Chiro.Cdf.Data.Entity
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class AssociationEndBehaviorAttribute : Attribute
	{
		private static readonly AssociationEndBehaviorAttribute DefaultInstance = new AssociationEndBehaviorAttribute(String.Empty);

		public AssociationEndBehaviorAttribute(string endName)
		{
			EndName = endName;
		}

		public string EndName { get; private set; }
		public bool Owned { get; set; }

		public static AssociationEndBehaviorAttribute GetAttribute(PropertyInfo property)
		{
			return GetAttribute(property.DeclaringType, property.Name);
		}

		public static AssociationEndBehaviorAttribute GetAttribute(Type type, string endName)
		{
			// Loop over attributes and return matching one:
			foreach (AssociationEndBehaviorAttribute item in type.GetCustomAttributes(typeof(AssociationEndBehaviorAttribute), true))
			{
				if (item.EndName == endName)
				{
					return item;
				}
			}
			// If none found, return default one:
			return DefaultInstance;
		}
	}
}
