using System;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    /// <summary>
    /// 
    /// </summary>
    class PlaatsenDao : Dao<Plaats, ChiroGroepEntities>, IPlaatsenDao
    {
        public Plaats Zoeken(int groepID, string plaatsNaam, int adresID, params Expression<Func<Plaats, object>>[] paths)
        {
            Plaats result;

            using (var db = new ChiroGroepEntities())
            {
                var query = (from p in db.Plaats
                             where p.Groep.ID == groepID && string.Equals(p.Naam, plaatsNaam) && p.Adres.ID == adresID
                             select p) as ObjectQuery<Plaats>;
                query = IncludesToepassen(query, paths);

                result = query.FirstOrDefault();
            }
            return result == null ? null : Utility.DetachObjectGraph(result);
        }
    }
}
