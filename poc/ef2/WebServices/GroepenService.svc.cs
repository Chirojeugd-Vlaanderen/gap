using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Core.Domain;
using Cg2.Util;
using Cg2.Workers;

namespace WebServices
{
    // NOTE: If you change the class name "GroepenService" here, you must also update the reference to "GroepenService" in Web.config.
    public class GroepenService : IGroepenService
    {
        #region IGroepenService Members

        public Groep Updaten(Groep g, Groep origineel)
        {
            IGroepenManager gm = new GroepenManager();

            try
            {
                return gm.Updaten(g, origineel);
            }
            catch (Exception e)
            {
                // TODO: fatsoenlijke exception handling

                throw new FaultException(e.Message, new FaultCode("Optimistic Concurrency Exception"));
            }
        }

        public Groep Ophalen(int groepID)
        {
            IGroepenManager gm = new GroepenManager();

            var result = gm.Ophalen(groepID);
            return result;
        }

        public string Hallo()
        {
            return "Hallo GroepenService.";
        }

        #endregion
    }
}
