using System;
using System.Messaging;
using Chiro.Cdf.ServiceModel;

namespace Chiro.Kip.ConsoleServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {

            var host = new ServiceHost<Chiro.Kip.Services.SyncPersoonService>();

            host.Open();

            Console.WriteLine("Hit Enter to stop the service");
            Console.ReadKey();

            host.Close();

        }
    }
}
