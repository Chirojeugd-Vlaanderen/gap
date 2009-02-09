using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;
using Cg2.Core.DataInterfaces;
using NHibernate;

namespace Cg2.Data.Nh
{
    public class PersonenDao: Dao<Persoon>, IPersonenDao
    {
        public Persoon OphalenMetCommunicatie(int id)
        {
            Persoon result;
            using (ISession sessie = SessionFactory.Factory.OpenSession())
            {
                using (sessie.BeginTransaction())
                {
                    result = sessie.Get<Persoon>(id);
                    NHibernateUtil.Initialize(result.Communicatie);
                    sessie.Transaction.Commit();
                }
            }
            return result;
        }
    }
}
