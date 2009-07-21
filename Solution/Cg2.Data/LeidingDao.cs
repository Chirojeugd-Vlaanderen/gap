using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using System.Diagnostics;
using System.Data.Objects;
using System.Data;
using Cg2.Orm.DataInterfaces;

using Cg2.EfWrapper.Entity;


namespace Cg2.Data.Ef
{
    public class LeidingDao : Dao<Leiding>, ILeidingDao
    {
        /// <summary>
        /// Creeert een nieuwe leider/leidster
        /// </summary>
        /// <param name="l">Leiding-object</param>
        /// <returns>bewaard Leiding-object</returns>
        /// <remarks>Wijzigingen in GroepsWerkJaar of
        /// GelieerdePersoon worden niet meegenomen!</remarks>
        public override Leiding Creeren(Leiding l)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                Leiding kindMetContext = db.AttachObjectGraph(l, lid => lid.GroepsWerkJaar, lid => lid.GelieerdePersoon, leiding => leiding.AfdelingsJaar);
                db.SaveChanges();
            }
            return l;
        }
    }
}
