using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;
using System.Data.Objects;
using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

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

		List<ChiroGroep> IDao<ChiroGroep>.AllesOphalen()
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
