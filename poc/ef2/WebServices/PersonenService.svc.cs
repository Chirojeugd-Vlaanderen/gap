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
    // NOTE: If you change the class name "PersonenService" here, you must also update the reference to "PersonenService" in Web.config.
    public class PersonenService : IPersonenService
    {
        #region IPersonenService Members

        public Persoon Ophalen(int persoonID)
        {
            // Het feit dat hier nog IPersonenManager als type
            // staat, is nog een overblijfsel van de IOC-achtige
            // implementatie uit de 'service factory'.

            return new PersonenDao().Ophalen(persoonID);
        }

        public int Bewaren(Persoon p)
        {
            return new PersonenDao().Bewaren(p).ID;
        }

        public void Verwijderen(Persoon p)
        {
            new PersonenDao().Verwijderen(p);
        }


        public Persoon OphalenMetCommunicatie(int persoonID)
        {
            return new PersonenDao().OphalenMetCommunicatie(persoonID);
        }

        public string Hallo()
        {
            return "Hallo PersonenService.";
        }

        #endregion
    }
}
