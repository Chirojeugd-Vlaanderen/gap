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
        public GebruikersRecht RechtenMbtGroepGet(string login, int groepID)
        {
            GebruikersRecht resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                var query
                    = from r in db.GebruikersRecht
                      where r.Groep.ID == groepID && r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
                      select r;

                resultaat = query.FirstOrDefault();
            }

            return resultaat;
        }

        public GebruikersRecht RechtenMbtGelieerdePersoonGet(string login, int gelieerdePersoonID)
        {
            GebruikersRecht resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                var query
                    = from r in db.GebruikersRecht
                      from gp in db.GelieerdePersoon
                      where r.Groep.ID == gp.Groep.ID && r.Gav.Login == login && gp.ID == gelieerdePersoonID && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
                      select r;

                resultaat = query.FirstOrDefault();
            }

            return resultaat;
        }

        public IList<int> GekoppeldeGroepenGet(string login)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                return
                    (from r in db.GebruikersRecht
                     where r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
                     select r.Groep.ID).ToList<int>();
            }
        }
    }
}
