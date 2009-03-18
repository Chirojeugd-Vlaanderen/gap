using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Data.Ef
{
    public class AuthorisatieDao: Dao<GebruikersRecht>, IAuthorisatieDao
    {
        public GebruikersRecht GebruikersRechtOphalen(string login, int groepID)
        {
            GebruikersRecht resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                var query
                    = from r in db.GebruikersRecht
                      where r.Groep.ID == groepID && r.Gav.Login == login
                      select r;

                resultaat = query.FirstOrDefault();
            }

            return resultaat;
        }
    }
}
