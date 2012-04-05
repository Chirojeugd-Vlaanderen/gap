using System;
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

			// Als je hieronder een foutmelding krijgt, dan moet je je gebruiker de toestemming geven 
			// om iets open te zetten op poort 8000 (voor metadata exchange)
			// Voor Windows 7 en aanverwanten gaat dat als volgt:
			// netsh http add urlacl url=http://+:8000/ user=DOMAIN\user
			// zie http://go.microsoft.com/fwlink/?LinkId=70353
			host.Open();

			Console.WriteLine("Hit Enter to stop the service");
			Console.ReadKey();

			host.Close();

		}
	}
}
