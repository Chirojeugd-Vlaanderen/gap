// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	public class ChiroGroepenDao : Dao<ChiroGroep, ChiroGroepEntities>, IDao<ChiroGroep>
	{
		ChiroGroep IDao<ChiroGroep>.Ophalen(int id)
		{
			ChiroGroep result;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				result = (
					from g in db.Groep.OfType<ChiroGroep>()
					where g.ID == id
					select g).FirstOrDefault<ChiroGroep>();

				db.Detach(result);
			}
			return result;
		}

		IList<ChiroGroep> IDao<ChiroGroep>.AllesOphalen()
		{
			List<ChiroGroep> result;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
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
