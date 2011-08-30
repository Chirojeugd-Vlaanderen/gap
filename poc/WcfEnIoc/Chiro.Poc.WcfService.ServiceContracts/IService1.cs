using System.Runtime.Serialization;
using System.ServiceModel;

namespace Chiro.Poc.WcfService.ServiceContracts
{
    /// <summary>
    /// Dom servicecontract, ter illustratie
    /// </summary>
    [ServiceContract]
    public interface IService1
    {
        /// <summary>
        /// Deze method levert gewoon een hello world-achtige string op.
        /// </summary>
        /// <returns>Een hello-world string</returns>
        [OperationContract]
        string Hallo();
    }
}
