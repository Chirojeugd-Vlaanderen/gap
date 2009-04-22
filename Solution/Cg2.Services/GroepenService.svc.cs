using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Workers;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace Cg2.Services
{
    // NOTE: If you change the class name "GroepenService" here, you must also update the reference to "GroepenService" in Web.config.
    public class GroepenService : IGroepenService
    {
        #region IGroepenService Members

        public Groep Updaten(Groep g)
        {
            GroepenManager gm = new GroepenManager();

            try
            {
                return gm.Dao.Bewaren(g);
            }
            catch (Exception e)
            {
                // TODO: fatsoenlijke exception handling

                throw new FaultException(e.Message, new FaultCode("Optimistic Concurrency Exception"));
            }
        }

        public Groep Ophalen(int groepID)
        {
            GroepenManager gm = new GroepenManager();

            var result = gm.Dao.Ophalen(groepID);
            return result;
        }

        public string Hallo()
        {
            return "Hallo";
        }

        #endregion
    }
}
