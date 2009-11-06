using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Data.Ef
{
    public class StratenDao: Dao<Straat, ChiroGroepEntities>, IStratenDao
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

                if (resultaat != null)
                {
                    db.Detach(resultaat);
                }
            }

            return resultaat;
        }
    }
}
