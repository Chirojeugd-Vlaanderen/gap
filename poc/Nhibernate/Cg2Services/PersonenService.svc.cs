using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Core.Domain;
using Cg2.Workers;

namespace Cg2Services
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

            IPersonenManager pm = new PersonenManager();

            var result = pm.Ophalen(persoonID);

            return result;
        }

        public Persoon OphalenMetCommunicatie(int persoonID)
        {
            IPersonenManager pm = new PersonenManager();
            return pm.OphalenMetCommunicatie(persoonID);
        }

        public string Hallo()
        {
            return "Hallo PersonenService.";
        }

        #endregion
    }
}
