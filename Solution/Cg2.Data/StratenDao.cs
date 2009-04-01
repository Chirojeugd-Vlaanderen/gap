using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Data.Ef
{
    public class StratenDao: Dao<Straat>, IStratenDao
    {
        public Straat Ophalen(string naam, int postNr)
        {
            Straat resultaat = null;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                resultaat = (
                    from Straat s in db.Straat
                    where s.Naam == naam && s.PostNr == postNr
                    select s).FirstOrDefault<Straat>();

                db.Detach(resultaat);
            }

            return resultaat;
        }
    }
}
