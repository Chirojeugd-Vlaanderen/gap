using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Workers;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace WebServices
{
    // NOTE: If you change the class name "PersonenService" here, you must also update the reference to "PersonenService" in Web.config.
    public class PersonenService : IPersonenService
    {
        #region IPersonenService Members

        public Persoon Ophalen(int persoonID)
        {
            // Het feit dat hier nog IPersonenManager als type
            // staat, is nog een overblijfsel van de IOC-achtige
            // implementatie uit de 'service factory'.

            PersonenManager pm = new PersonenManager();

            var result = pm.Dao.Ophalen(persoonID);

            return result;
        }

        public int Bewaren(Persoon p)
        {
            PersonenManager pm = new PersonenManager();

            return pm.Dao.Bewaren(p).ID;
        }

        public void Verwijderen(Persoon p)
        {
            PersonenManager pm = new PersonenManager();
            pm.Dao.Verwijderen(p);
        }

        public Persoon OphalenMetCommunicatie(int persoonID)
        {
            PersonenManager pm = new PersonenManager();
            return pm.Dao.OphalenMetCommunicatie(persoonID);
        }

        public string Hallo()
        {
            return "Hallo PersonenService.";
        }

        #endregion
    }
}
