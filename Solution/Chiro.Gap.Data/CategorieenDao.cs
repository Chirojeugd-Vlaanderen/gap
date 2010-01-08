using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	public class CategorieenDao : Dao<Categorie, ChiroGroepEntities>, ICategorieenDao
	{
		public CategorieenDao()
		{
			connectedEntities = new Expression<Func<Categorie, object>>[] { 
                                        e => e.Groep.WithoutUpdate(), 
                                        e => e.GelieerdePersoon.First().WithoutUpdate() };
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

		/// <summary>
		/// Zoekt een categorie op op basis van <paramref name="groepID"/> en
		/// <paramref name="code"/>.
		/// </summary>
		/// <param name="groepID">ID van groep waaraan de te zoeken categorie gekoppeld moet zijn</param>
		/// <param name="code">code van de te zoeken categorie</param>
		/// <returns>de gevonden categorie; <c>null</c> indien niet gevonden</returns>
		public Categorie Ophalen(int groepID, string code)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Categorie.MergeOption = MergeOption.NoTracking;

				return (from cat in db.Categorie
					where cat.Groep.ID == groepID && string.Compare(cat.Code, code) == 0
					select cat).FirstOrDefault();
			}
		}
	}
}
