using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;
using Cg2.Util;
using Cg2.Core.DataInterfaces;

namespace Cg2.Workers
{
    public class GroepenManager: IGroepenManager
    {

        #region IGroepenManager Members

        public Groep Updaten(Groep g, Groep origineel)
        {
            IServiceFactory serviceFactory = new ClassServiceFactory();
            IDaoFactory df = (IDaoFactory)serviceFactory
                .FindByServiceName("Cg2/Core/DataInterfaces/IDaoFactory");
            IGroepenDao gd = df.GroepenDaoGet();
            return gd.Updaten(g, origineel);
        }

        public Groep Ophalen(int groepID)
        {
            IServiceFactory serviceFactory = new ClassServiceFactory();
            IDaoFactory df = (IDaoFactory)serviceFactory
                .FindByServiceName("Cg2/Core/DataInterfaces/IDaoFactory");
            IGroepenDao gd = df.GroepenDaoGet();
            return gd.Ophalen(groepID);
        }

        #endregion
    }
}
