using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Chiro.Gap.Data.Ef;
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

        // Nog geen interessante functionaliteit
    }
}
