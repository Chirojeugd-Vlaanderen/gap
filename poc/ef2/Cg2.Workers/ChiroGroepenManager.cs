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
        #region IChiroGroepenManager Members

        public ChiroGroep Ophalen(int groepID)
        {
            ChiroGroepenDao dao = new ChiroGroepenDao();

            return dao.Ophalen(groepID);
        }

        #endregion
    }
}
