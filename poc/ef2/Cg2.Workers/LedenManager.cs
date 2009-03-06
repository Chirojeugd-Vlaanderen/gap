using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Data.Ef;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Workers
{
    public class LedenManager
    {
        private IDao<Lid> _dao = new Dao<Lid>();

        public IDao<Lid> Dao
        {
            get { return _dao; }
        }

        /// <summary>
        /// 'Opwaarderen' van een gelieerd persoon tot een lid.
        /// </summary>
        /// <param name="gpid"></param>
        /// <param name="GroepsWerkJaarID"></param>
        /// <returns></returns>
        public Lid LidMaken(GelieerdePersoon gp, GroepsWerkJaar gwj)
        {
            // Het plan:
            //
            // 1. kijken of er al een lid bestaat.
            // 2. Zo ja:
            //    2a Als dat lid verwijderd of nonactief is: opnieuw activeren
            //       (weet niet meer precies wat 'nonactief' wil zeggen; op te zoeken)
            //    2b Was al actief lid: exception
            // 3. Zo nee:
            //    Nieuw lidobject
            //
            // (Niet vergeten: probeerperiode berekenen)

            throw new NotImplementedException();
        }
    }
}
