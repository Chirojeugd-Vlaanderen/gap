using System;
using System.Linq;
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
				using (var svc = new GapUpdateService.UpdateServiceClient())
				{
					svc.AdNummerVervangen(int.Parse(args[0]), int.Parse(args[1]));
				}
			}
		}
	}
}
