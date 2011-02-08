using System;

using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceModel;
using Chiro.Gap.UpdateSvc.Contracts;
using Chiro.Gap.UpdateSvc.Service;

namespace Chiro.Gap.UpdateSvc.ConsoleServiceHost
{
	class Program
	{
		static void Main(string[] args)
		{
			Factory.ContainerInit();

			// ServiceHost<T> is een type dat we zelf declareren in Chiro.Cdf.ServiceModel, maar
			// eigenlijk is onderstaande lijn min of meer gelijkaardig aan
			// var host = new ServiceHost(typeof(UpdateService));
			//
			// TODO: Nakijken of die custom ServiceHost<T> wel de moeite is om te behouden.

			var host = Factory.Maak<ServiceHost<UpdateService>>();

			host.Open();
			Console.WriteLine("Hit enter to stop the service.");
			Console.ReadKey();

			host.Close();
		}
	}
}
