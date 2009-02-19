using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Orm;

namespace Cg2.ServiceContracts
{
    // NOTE: If you change the interface name "IGelieerdePersonenService" here, you must also update the reference to "IGelieerdePersonenService" in Web.config.
    [ServiceContract]
    public interface IGelieerdePersonenService
    {
        [OperationContract]
        IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);
    }
}
