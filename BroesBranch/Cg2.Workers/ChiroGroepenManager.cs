using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;

namespace Cg2.Workers
{
    public class ChiroGroepenManager
    {
        private IDao<ChiroGroep> _dao = null;

        public ChiroGroepenManager(IDao<ChiroGroep> dao)
        {
            _dao = dao;
        }

        public IDao<ChiroGroep> Dao
        {
            get { return _dao; }
        }

        // Nog geen interessante functionaliteit
    }
}
