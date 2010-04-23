// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

namespace Chiro.Cdf.DependencyInjection
{
	/// <summary>
	/// Mapping between a requested type and the type to build by the object builder. 
	/// Requested type is usually an interface whereas  typeToBuild is a concrete implementation of that interface.
	/// If this type mapping is not specified and the requested type is an interface, 
	/// object builder will not know how to create a new instance of that type.
	/// </summary>
	public class TypeMapping
	{
		private Type typeRequested;
		private Type typeToBuild;

		public TypeMapping(Type typeRequested, Type typeToBuild)
		{
			this.typeRequested = typeRequested;
			this.typeToBuild = typeToBuild;
		}

		public Type TypeRequested
		{
			get { return typeRequested; }
		}

		public Type TypeToBuild
		{
			get { return typeToBuild; }
		}
	}
}
