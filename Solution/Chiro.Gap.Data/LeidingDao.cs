// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor leiding
	/// </summary>
	public class LeidingDao : Dao<Leiding, ChiroGroepEntities>, ILeidingDao
	{
		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor leiding
		/// </summary>
		public LeidingDao()
		{
			connectedEntities = new Expression<Func<Leiding, object>>[] 
			{
				e => e.GroepsWerkJaar.WithoutUpdate(),
				e => e.GelieerdePersoon, 
				e => e.GelieerdePersoon.Persoon, 
				e => e.AfdelingsJaar.First(),
				e => e.AfdelingsJaar.First().Afdeling.WithoutUpdate()
			};
		}
	}
}
