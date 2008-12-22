using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;
using Cg2.Util;
using Cg2.Core.DataInterfaces;
using Cg2.Data.Ef;

namespace Cg2.Workers
{
    public class GroepenManager: IGroepenManager
    {

        #region IGroepenManager Members

        public Groep Updaten(Groep g, Groep origineel)
        {
            IDaoFactory df = new EfDaoFactory();
            IGroepenDao gd = df.GroepenDaoGet();
            return gd.Updaten(g, origineel);
        }

        public Groep Ophalen(int groepID)
        {
            IDaoFactory df = new EfDaoFactory();
            IGroepenDao gd = df.GroepenDaoGet();
            return gd.Ophalen(groepID);
        }

        #endregion
    }
}
