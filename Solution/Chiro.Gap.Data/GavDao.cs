using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using System.Data.Objects;

namespace Chiro.Gap.Data.Ef
{
    public class GavDao: Dao<Gav>, IGavDao
    {
        public Gav Ophalen(string login)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.Gav.MergeOption = MergeOption.NoTracking;

                return (from gav in db.Gav
                        where gav.Login == login
                        select gav).FirstOrDefault();
            }
        }
    }
}
