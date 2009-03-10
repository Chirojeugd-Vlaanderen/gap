using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.ServiceContracts;
using Cg2.Workers;
using Cg2.Orm;

namespace Cg2.Services
{
    // NOTE: If you change the class name "LedenService" here, you must also update the reference to "LedenService" in Web.config.
    public class LedenService : ILedenService
    {
        public Lid LidMakenEnBewaren(int gelieerdePersoonID)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();
            LedenManager lm = new LedenManager();

            Lid l = lm.LidMaken(pm.Dao.Ophalen(gelieerdePersoonID));
            
            return lm.Dao.Bewaren(l);
        }
    }
}
