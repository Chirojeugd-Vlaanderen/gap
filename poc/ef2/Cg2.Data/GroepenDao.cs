using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Data.Ef
{
    public class GroepenDao: Dao<Groep>, IGroepenDao
    {
        public GroepsWerkJaar HuidigWerkJaarGet(int groepID)
        {
            GroepsWerkJaar result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                result = (
                    from wj in db.GroepsWerkJaar
                    where wj.Groep.ID == groepID
                    orderby wj.WerkJaar descending
                    select wj).FirstOrDefault<GroepsWerkJaar>();

                db.Detach(result);
            }
            return result;
        }
    }
}
