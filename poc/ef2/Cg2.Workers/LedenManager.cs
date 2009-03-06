using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Data.Ef;

namespace Cg2.Workers
{
    public class LedenManager
    {
        private IDao<Lid> _dao = new Dao<Lid>();

        public IDao<Lid> Dao
        {
            get { return _dao; }
        }

        public Lid LidMaken(GelieerdePersoon p)
        {
            // Het plan:
            //
            // 1. kijken of er al een lid bestaat.
            // 2. Zo ja:
            //    2a Als dat lid verwijderd of nonactief is: opnieuw activeren
            //       (weet niet meer precies wat 'nonactief' wil zeggen; op te zoeken)
            //    2b Was al actief lid: exception
            // 3. Zo nee:
            //    Upgrade gelieerdPersoon naar lid via
            //    (nog te schrijven) stored procedure
            //
            // (Niet vergeten: probeerperiode berekenen)

            throw new NotImplementedException();
        }
    }
}
