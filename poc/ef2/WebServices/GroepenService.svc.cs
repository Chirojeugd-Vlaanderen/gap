using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Orm;
using Cg2.Data.Ef;

namespace WebServices
{
    // NOTE: If you change the class name "GroepenService" here, you must also update the reference to "GroepenService" in Web.config.
    public class GroepenService : IGroepenService
    {
        #region IGroepenService Members

        public Groep Updaten(Groep g, Groep origineel)
        {
            Dao<Groep> dao = new Dao<Groep>();

            try
            {
                return dao.Updaten(g, origineel);
            }
            catch (Exception e)
            {
                // TODO: fatsoenlijke exception handling
                throw new FaultException(e.Message, new FaultCode("Optimistic Concurrency Exception"));
            }
        }

        public Groep Ophalen(int groepID)
        {
            return new Dao<Groep>().Ophalen(groepID);
        }

        public string Hallo()
        {
            return "Hallo GroepenService.";
        }

        #endregion
    }
}
