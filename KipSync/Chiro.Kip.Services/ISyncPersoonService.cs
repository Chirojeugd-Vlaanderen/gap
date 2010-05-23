using System.Collections.Generic;
using System.ServiceModel;
using Chiro.Kip.Services.DataContracts;

namespace Chiro.Kip.Services
{
    
    [ServiceContract()]
    public interface ISyncPersoonService
    {

        [OperationContract(IsOneWay = true)]
        void PersoonUpdated(Persoon persoon);

        [OperationContract(IsOneWay = true)]
        void AdresUpdated(Persoon persoon, IEnumerable<Adres> adressen);

        [OperationContract(IsOneWay = true)]
        void CommunicatieUpdated(Persoon persoon, IEnumerable<Communicatiemiddel> communicatiemiddelen);


    }

    
}
