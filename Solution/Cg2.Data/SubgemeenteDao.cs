using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    public class SubgemeenteDao: Dao<Subgemeente>, ISubgemeenteDao
    {
        public Subgemeente Ophalen(string naam, int postNr)
        {
            Subgemeente resultaat = null;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                resultaat = (
                    from Subgemeente s in db.Subgemeente
                    where s.Naam == naam && s.PostNr == postNr
                    select s).FirstOrDefault<Subgemeente>();

                if (resultaat != null)
                {
                    db.Detach(resultaat);
                }
            }

            return resultaat;
        }
    }
}
