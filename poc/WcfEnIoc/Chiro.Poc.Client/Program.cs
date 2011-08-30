using System;
using Chiro.Poc.Ioc;
using Chiro.Poc.ServiceGedoe;
using Chiro.Poc.WcfService.ServiceContracts;

namespace Chiro.Poc.Client
{
    /// <summary>
    /// Voorbeeldprogramma dat ServiceHelper gebruikt om de webservice met cintract IService1 aan te roepen.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Initialiseer de IOC-container.  In App.Config staat gedefinieerd dat ServiceHelper de OnlineServiceClient
            // moet gebruiken om de service calls uit te voeren.

            Factory.ContainerInit();

            Console.WriteLine(ServiceHelper.CallService<IService1, string>(svc => svc.Hallo()));
            Console.ReadLine();
        }
    }
}
