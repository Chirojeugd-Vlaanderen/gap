using System;
using System.Linq;
using Chiro.Adf.ServiceModel;
using Chiro.Gap.UpdateSvc.Contracts;
using PersonenMergen.Properties;

namespace PersonenMergen
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Count() != 2)
			{
				Console.WriteLine(Resources.Gebruik, Environment.CommandLine);
				Console.ReadLine();
			}
			else
			{
			    ServiceHelper.CallService<IUpdateService>(svc => svc.AdNummerVervangen(int.Parse(args[0]), int.Parse(args[1])));
			}
		}
	}
}
