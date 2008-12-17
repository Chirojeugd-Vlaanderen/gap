using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Core.Domain;
using Cg2.Util;

namespace WebServices
{
    // NOTE: If you change the class name "GroepenService" here, you must also update the reference to "GroepenService" in Web.config.
    public class GroepenService : IGroepenService
    {
        IServiceFactory serviceFactory;

        #region IGroepenService Members

        public Groep Updaten(Groep g, Groep origineel)
        {
            serviceFactory = new ClassServiceFactory();
            IGroepenManager gm = (IGroepenManager)serviceFactory
                .FindByServiceName("Cg2/Core/Domain/IGroepenManager");
            return gm.Updaten(g, origineel);
        }

        public Groep Ophalen(int groepID)
        {
            serviceFactory = new ClassServiceFactory();
            IGroepenManager gm = (IGroepenManager)serviceFactory
                .FindByServiceName("Cg2/Core/Domain/IGroepenManager");

            var result = gm.Ophalen(groepID);
            return result;
        }

        #endregion
    }
}
