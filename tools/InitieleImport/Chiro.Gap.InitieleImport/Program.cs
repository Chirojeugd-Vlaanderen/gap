using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Chiro.Pdox.Data;

namespace Chiro.Gap.InitieleImport
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Count() == 0)
			{
				Console.WriteLine(Properties.Resources.Usage);
			}
			else
			{
				var lezer = new Lezer(args[0]);
				var groep = lezer.GroepOphalen();

				Console.WriteLine(
					Properties.Resources.GroepsInfo,
					groep.StamNummer,
					groep.Naam,
					groep.Plaats);
			}
			

#if DEBUG
			Console.ReadLine();
#endif
		}
	}
}
