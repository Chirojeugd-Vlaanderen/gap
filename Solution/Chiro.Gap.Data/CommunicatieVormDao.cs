// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor communicatievormen
	/// </summary>
	public class CommunicatieVormDao : Dao<CommunicatieVorm, ChiroGroepEntities>, ICommunicatieVormDao
	{
		/// <summary>
		/// TODO: documenteren
		/// </summary>
		/// <param name="zoekString"></param>
		/// <returns></returns>
		public IList<CommunicatieVorm> ZoekenOpNummer(string zoekString)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.CommunicatieVorm.MergeOption = MergeOption.NoTracking;

				return (
					from cv in db.CommunicatieVorm
					where cv.Nummer == zoekString
					select cv).ToList();
			}
		}
	}
}
