using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Data.Ef;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using System.Data.Objects;

namespace Cg2.Data.Ef
{
    public class CommunicatieVormDao: Dao<CommunicatieVorm>, ICommunicatieVormDao
    {
        public IList<CommunicatieVorm> ZoekenOpNummer(string zoekString)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.CommunicatieVorm.MergeOption = MergeOption.NoTracking;

                return (
                    from cv in db.CommunicatieVorm
                    where cv.Nummer == zoekString
                    select cv).ToList();
            }
        }
    }
}
