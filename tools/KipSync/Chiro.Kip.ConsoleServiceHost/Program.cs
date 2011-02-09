using System;
using System.Messaging;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceModel;

namespace Chiro.Kip.ConsoleServiceHost
{
	class Program
	{
		static void Main(string[] args)
		{
			Factory.ContainerInit();

			var host = new ServiceHost<Chiro.Kip.Services.SyncPersoonService>();

			host.Open();

			Console.WriteLine("Hit Enter to stop the service");
			Console.ReadKey();

			host.Close();

		}
	}
}
