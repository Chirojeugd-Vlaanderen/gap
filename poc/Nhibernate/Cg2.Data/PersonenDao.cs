using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;
using Cg2.Core.DataInterfaces;

namespace Cg2.Data.Nh
{
    public class PersonenDao: Dao<Persoon>, IPersonenDao
    {
        public Persoon OphalenMetCommunicatie(int id)
        {
            throw new NotImplementedException();
        }
    }
}
