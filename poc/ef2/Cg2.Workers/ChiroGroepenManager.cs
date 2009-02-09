using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;
using Cg2.Core.DataInterfaces;
using Cg2.Data.Ef;

namespace Cg2.Workers
{
    public class ChiroGroepenManager: IChiroGroepenManager
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
