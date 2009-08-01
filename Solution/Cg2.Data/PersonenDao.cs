using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;
using System.Data;
using System.Diagnostics;
using System.Data.Objects;

using Cg2.EfWrapper.Entity;
using Cg2.EfWrapper;

namespace Cg2.Data.Ef
{
    public class PersonenDao: Dao<Persoon>, IPersonenDao
    {
        #region IPersonenDao Members

        // TODO: onderstaande misschien doen via GroepsWerkJaar ipv via aparte persoon- en groepID?

        public IList<Persoon> LijstOphalen(IList<int> personenIDs)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.Persoon.MergeOption = MergeOption.NoTracking;

                return (
                    from p in db.Persoon.Where(Utility.BuildContainsExpression<Persoon, int>(p => p.ID, personenIDs))
                    select p).ToList();
            }
        }

        public override Persoon Bewaren(Persoon p)
        {
            if (p.ID == 0)
            {
                // creeer nieuwe gelieerde persoon

                return Creeren(p);
            }
            else
            {
                using (ChiroGroepEntities db = new ChiroGroepEntities())
                {
                    // Bestaande objecten zijn hun entity key vaak kwijt (omdat die
                    // verloren gaat in de MVC-cyclus.)  Opnieuw genereren dus.

                    p.EntityKey = db.CreateEntityKey("Persoon", p);

                    Persoon geattacht = db.AttachObjectGraph(p);

                    db.SaveChanges();
                }
                return p;
            }
        }

        #endregion
    }
}
