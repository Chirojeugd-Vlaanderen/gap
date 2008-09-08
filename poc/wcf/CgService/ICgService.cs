using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CgDal;

namespace CgService
{
    // NOTE: If you change the interface name "ICgService" here, you must also update the reference to "ICgService" in Web.config.
    [ServiceContract]
    public interface ICgService
    {
        [OperationContract]
        Persoon PersoonGet(int persoonID);
        [OperationContract]
        String Hello();
    }

}
