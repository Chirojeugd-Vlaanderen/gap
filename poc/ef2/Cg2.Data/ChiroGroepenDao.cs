using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;

namespace Cg2.Data.Ef
{
    public class ChiroGroepenDao: Dao<ChiroGroep>, IDao<ChiroGroep>
    {

        #region IDao<ChiroGroep> Members

        ChiroGroep IDao<ChiroGroep>.Ophalen(int id)
        {
            ChiroGroep result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                result = (
                    from g in db.Groep.OfType<ChiroGroep>()
                    where g.ID == id
                    select g).FirstOrDefault<ChiroGroep>();
            }
            return result;
        }

        List<ChiroGroep> IDao<ChiroGroep>.AllesOphalen()
        {
            List<ChiroGroep> result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                result = (
                    from g in db.Groep.OfType<ChiroGroep>()
                    select g).ToList<ChiroGroep>();
            }
            return result;
        }

        #endregion
    }
}
