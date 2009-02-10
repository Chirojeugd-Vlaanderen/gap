using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;

namespace Cg2.Workers
{
    public class GroepenManager
    {

        #region IGroepenManager Members

        public Groep Updaten(Groep g, Groep origineel)
        {
            IDao<Groep> dao = new Dao<Groep>();
            return dao.Updaten(g, origineel);
        }

        public Groep Ophalen(int groepID)
        {
            IDao<Groep> dao = new Dao<Groep>();

            return dao.Ophalen(groepID);
        }

        #endregion
    }
}
