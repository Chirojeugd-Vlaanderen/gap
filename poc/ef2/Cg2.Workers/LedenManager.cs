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
            // Kijken of er al een lid bestaat, moet nog gebeuren!

            Lid l = new Lid();

            l.GelieerdePersoon = gp;
            l.GroepsWerkJaar = gwj;

            gp.Lid.Add(l);
            gwj.Lid.Add(l);

            // Einde instapperiode moet ook nog berekend worden.
            l.EindeInstapPeriode = DateTime.Now;

            // Ik denk dat in deze method geen databasecall mag gebeuren.
            // Dit moet via de Dao.

            return l;
        }
    }
}
