using System.ServiceModel;

namespace Chiro.Gap.ServiceContracts
{
    [ServiceContract]
    public interface IHelloService
    {
        /// <summary>
        /// Eenvoudig serviceje om te testen.
        /// </summary>
        [OperationContract]
        string Hello();
    }
}