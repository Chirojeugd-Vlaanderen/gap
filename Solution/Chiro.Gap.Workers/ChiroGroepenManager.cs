using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Workers
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
