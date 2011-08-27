using System;
using Chiro.Poc.Ioc;
using Chiro.Poc.ServiceGedoe;
using Chiro.Poc.WcfService.ServiceContracts;

namespace Chiro.Poc.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Factory.ContainerInit();

            Console.WriteLine(ServiceHelper.CallService<IService1, string>(svc => svc.Hallo()));
            Console.ReadLine();
        }
    }
}
