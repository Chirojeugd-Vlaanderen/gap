using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Chiro.Kip.ResubmitDeadMessages
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("<ENTER> to start...");
			Console.ReadLine();

			using (var serviceHost = new ServiceHost(typeof(DlqSyncPersoonService)))
			{
				serviceHost.Open();

				Console.WriteLine("The dead letter service is ready.");
				Console.WriteLine("Press <ENTER> to terminate service.");

				Console.ReadLine();

				serviceHost.Close();
			}
		}
	}
}
