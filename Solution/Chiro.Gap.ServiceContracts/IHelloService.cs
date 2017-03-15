using System.ServiceModel;

namespace Chiro.Gap.ServiceContracts
{
    [ServiceContract]
    public interface IHelloService
    {
        /// <summary>
        /// Erg lelijke hack die direct in de database schrijft om de aangelogde gebruiker
        /// toegang te geven tot een testgroep.
        /// </summary>
        [OperationContract]
        string Hello();
    }
}