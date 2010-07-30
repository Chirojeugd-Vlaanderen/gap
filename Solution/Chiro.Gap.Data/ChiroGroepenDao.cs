// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor Chirogroepen
	/// </summary>
	public class ChiroGroepenDao : Dao<ChiroGroep, ChiroGroepEntities>, IDao<ChiroGroep>
	{
		/// <summary>
		/// Haalt de ChiroGroep op met de gegeven ID
		/// </summary>
		/// <param name="id">ID van de Chirogroep in kwestie</param>
		/// <returns>De ChiroGroep met de gegeven ID</returns>
		ChiroGroep IDao<ChiroGroep>.Ophalen(int id)
		{
			ChiroGroep result;

			using (var db = new ChiroGroepEntities())
			{
				result = (
					from g in db.Groep.OfType<ChiroGroep>()
					where g.ID == id
					select g).FirstOrDefault<ChiroGroep>();

				db.Detach(result);
			}
			return result;
		}

		/// <summary>
		/// Haalt alle Chirogroepen op
		/// </summary>
		/// <returns>Een lijst met alle Chirogroepen</returns>
		IList<ChiroGroep> IDao<ChiroGroep>.AllesOphalen()
		{
			List<ChiroGroep> result;

			using (var db = new ChiroGroepEntities())
			{
				db.Groep.MergeOption = MergeOption.NoTracking;

				result = (
					from g in db.Groep.OfType<ChiroGroep>()
					select g).ToList<ChiroGroep>();
			}
			return result;
		}
	}
}
