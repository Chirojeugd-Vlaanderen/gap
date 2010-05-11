using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.InitieleImport.Properties;
using Chiro.Gap.ServiceContracts;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts.FaultContracts;
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
				Factory.ContainerInit();  // Init IOC

				#region Connectie met services
				var serviceHelper = Factory.Maak<IServiceHelper>();
				string userName = serviceHelper.CallService<IGroepenService, string>(svc => svc.WieBenIk());
				Console.WriteLine(Resources.ServiceUser, userName);
				#endregion

				var lezer = new Lezer(args[0]);

				#region Groep Ophalen
				var groep = lezer.GroepOphalen();
				GroepInfo dbGroep;

				Console.WriteLine(
					Properties.Resources.GroepsInfo,
					groep.StamNummer,
					groep.Naam,
					groep.Plaats);

				try
				{
					dbGroep = serviceHelper.CallService<IGroepenService, GroepInfo>(svc => svc.InfoOphalenCode(groep.StamNummer));
				}
				catch (FaultException<GapFault> ex)
				{
					if (ex.Detail.FoutNummer == FoutNummers.GeenGav)
					{
						Console.WriteLine(Resources.GeenGav);
					}
					return;
				}

				Console.WriteLine(Resources.GroepId, dbGroep.ID);

				#endregion

				#region Personen Ophalen
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

					foreach (var ci in p.CommunicatieInfo)
					{
						Console.WriteLine(
							Properties.Resources.CommunicatieInfo,
							ci.Nummer,
							ci.CommunicatieTypeID,
							ci.Voorkeur ? "*" : " ");
					}
				}
				#endregion


				Console.WriteLine(Properties.Resources.TotaalInfo, personen.Count());
			}
			

#if DEBUG
			Console.ReadLine();
#endif
		}
	}
}
