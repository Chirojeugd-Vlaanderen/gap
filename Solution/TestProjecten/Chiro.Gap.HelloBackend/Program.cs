using System;
using Chiro.Gap.ServiceContracts;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Cdf.ServiceHelper;

namespace DummyProject
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Factory.ContainerInit();
			var helper = Factory.Maak<ServiceHelper> ();
			var result = helper.CallService<IGroepenService, string> (svc => svc.TestDatabase());
            Console.WriteLine(result);
		    // 2de call.

			result = helper.CallService<IGroepenService, string> (svc => svc.TestDatabase());
			Console.WriteLine(result);
			Console.ReadLine ();
		}
	}
}
