// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Linq;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor communicatietypes
	/// </summary>
	public class CommunicatieTypeDao : Dao<CommunicatieType, ChiroGroepEntities>, ICommunicatieTypeDao
	{
		/// <summary>
		/// Haalt het communicatietype op met de gegeven ID
		/// </summary>
		/// <param name="communicatieTypeID">ID van het communicatietype in kwestie</param>
		/// <returns>Het communicatietype met de gegeven ID</returns>
		public override CommunicatieType Ophalen(int communicatieTypeID)
		{
			using (var db = new ChiroGroepEntities())
			{
				return (
					from ct in db.CommunicatieType
					where ct.ID == communicatieTypeID
					select ct).FirstOrDefault();
			}
		}
	}
}