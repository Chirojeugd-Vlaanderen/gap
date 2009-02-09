using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.DataInterfaces;
using Cg2.Core.Domain;

namespace Cg2.Data.Ef
{
    public class PersonenDao: Dao<Persoon>, IPersonenDao
    {
        public Persoon OphalenMetCommunicatie(int id)
        {
            Persoon result;

            using (Cg2ObjectContext db = new Cg2ObjectContext())
            {
                result = (
                    from p in db.Personen.Include("Communicatie")
                    where p.ID == id
                    select p).FirstOrDefault<Persoon>();
            }
            return result;
        }
    }
}
