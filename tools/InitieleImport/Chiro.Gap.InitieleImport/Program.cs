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

				var personen = lezer.PersonenOphalen();

				foreach (var p in personen)
				{
					Console.WriteLine(
						Properties.Resources.PersoonInfo,
						p.PersoonDetail.VoorNaam,
						p.PersoonDetail.Naam,
						p.PersoonDetail.GeboorteDatum,
						p.PersoonDetail.Geslacht,
						p.PersoonDetail.AdNummer,
						p.LidInfo == null ? String.Empty : p.LidInfo.Type.ToString());

					foreach (var pa in p.PersoonsAdresInfo)
					{
						Console.WriteLine(
							Properties.Resources.AdresInfo,
							pa.StraatNaamNaam,
							pa.HuisNr,
							pa.Bus ?? "/",
							pa.PostNr,
							pa.WoonPlaatsNaam,
							pa.AdresType);
					}
				}

				Console.WriteLine(Properties.Resources.TotaalInfo, personen.Count());
			}
			

#if DEBUG
			Console.ReadLine();
#endif
		}
	}
}
