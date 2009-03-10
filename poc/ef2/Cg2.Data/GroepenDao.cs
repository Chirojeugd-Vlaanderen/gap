using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;
using System.Data.Objects;

namespace Cg2.Data.Ef
{
    public class GroepenDao: Dao<Groep>, IGroepenDao
    {
        public GroepsWerkJaar HuidigWerkJaarGet(int groepID)
        {
            GroepsWerkJaar result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // db.GroepsWerkJaar.MergeOption = MergeOption.NoTracking;

                result = (
                    from wj in db.GroepsWerkJaar
                    where wj.Groep.ID == groepID
                    orderby wj.WerkJaar descending
                    select wj).FirstOrDefault<GroepsWerkJaar>();

                db.Detach(result);

                // Als ik de EntityKey niet op null zet, kan ik dit
                // groepswerkjaar blijkbaar niet gebruiken voor een
                // nieuw lid?
            }
            return result;
        }
    }
}
