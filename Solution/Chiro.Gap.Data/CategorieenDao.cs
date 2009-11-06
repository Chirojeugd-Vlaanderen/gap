using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using System.Data.Objects;
using System.Linq.Expressions;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Data.Ef
{
	public class CategorieenDao : Dao<Categorie, ChiroGroepEntities>, ICategorieenDao
	{
		public CategorieenDao()
		{
			connectedEntities = new Expression<Func<Categorie, object>>[2] { 
                                        e => e.Groep, 
                                        e => e.GelieerdePersoon };
		}

		public IEnumerable<Categorie> OphalenVanGroep(int groepID)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Categorie.MergeOption = MergeOption.NoTracking;

				return (
				    from cv in db.Categorie
				    where cv.Groep.ID == groepID
				    select cv).ToList();
			}
		}
	}
}
