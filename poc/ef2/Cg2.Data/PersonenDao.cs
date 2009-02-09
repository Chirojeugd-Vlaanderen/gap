using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Data.Ef
{
    public class PersonenDao: Dao<Persoon>, IPersonenDao
    {
        public Persoon OphalenMetCommunicatie(int id)
        {
            Persoon result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                result = (
                    from p in db.Persoon.Include("Communicatie")
                    where p.ID == id
                    select p).FirstOrDefault<Persoon>();
            }
            return result;
        }
    }
}
