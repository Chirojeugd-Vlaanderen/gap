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
    public class KindDao : Dao<Kind>, IKindDao
    {
        /// <summary>
        /// Creeert een nieuw kind
        /// </summary>
        /// <param name="l">kindobject</param>
        /// <returns>bewaarde kindobject</returns>
        /// <remarks>Wijzigingen in GroepsWerkJaar of
        /// GelieerdePersoon worden niet meegenomen!</remarks>
        public override Kind Creeren(Kind k)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                Kind kindMetContext = db.AttachObjectGraph(k, lid => lid.GroepsWerkJaar, lid => lid.GelieerdePersoon, kind => kind.AfdelingsJaar);
                db.SaveChanges();
            }
            return k;
        }

    }
}
