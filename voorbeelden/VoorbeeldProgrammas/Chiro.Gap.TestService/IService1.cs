using System;
using System.ServiceModel;

namespace Chiro.Gap.TestService
{
    /// <summary>
    /// Deze service heeft in eerste instantie gewoon tot doel om de combinatie
    /// WCF, Unity en IDisposable te testen.
    /// </summary>
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string Hello();
    }
}
