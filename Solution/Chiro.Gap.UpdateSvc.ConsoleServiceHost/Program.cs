// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

using Chiro.Cdf.Ioc;
using Chiro.Gap.UpdateSvc.Service;
using Chiro.Cdf.ServiceModel;

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
            // TODO (#1059): Nakijken of die custom ServiceHost<T> wel de moeite is om te behouden.

			var host = new ServiceHost<UpdateService>();

			host.Open();
			Console.WriteLine("Hit enter to stop the service.");
			Console.ReadKey();

			host.Close();
		}
	}
}
