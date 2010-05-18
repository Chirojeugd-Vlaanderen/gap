using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
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
		private struct MogelijkDubbel
		{
			public PersoonDetail Nieuw { get; set; }
			public PersoonDetail Bestaand { get; set; }
		}

		static void Main(string[] args)
		{
			if (args.Count() == 0)
			{
				Console.WriteLine(Properties.Resources.Usage);
			}
			else
			{
				var helper = new KipdorpHelper();
				string stamNrFile = helper.StamNrNaarBestand(args[0]);
				string aansluitingsBestand = helper.RecentsteAansluitingsBestand(args[0]);

				Console.WriteLine(Resources.UitpakkenVan, aansluitingsBestand);

				// TODO: temporary directory maken, met unieke naam.  Daar zal wellicht wel iets voor
				// bestaan in .NET.

				// !UitPakDir kort houden, owv de 65-karakterlimiet van paradoxpaden.
				
				string destdir = String.Format("{0}/{1}", Settings.Default.UitPakDir, stamNrFile);

				// pak recentste aansluitingsbestand uit
				Directory.CreateDirectory(destdir);

				string dataDir = helper.Uitpakken(aansluitingsBestand, destdir);
				ImporterenUitPdox(dataDir);

				Directory.Delete(destdir, true);
			}


#if DEBUG
			Console.ReadLine();
#endif
		}

		/// <summary>
		/// Importeert de gegevens uit paradoxdatabase in <paramref name="path"/>
		/// </summary>
		/// <param name="path">directory waar de paradoxtabellen zich bevinden</param>
		private static void ImporterenUitPdox(string path)
		{
			var mogelijkeDubbels = new List<MogelijkDubbel>();

			Factory.ContainerInit();  // Init IOC

			#region Connectie met services
			var serviceHelper = Factory.Maak<IServiceHelper>();
			string userName = serviceHelper.CallService<IGroepenService, string>(svc => svc.WieBenIk());
			Console.WriteLine(Resources.ServiceUser, userName);
			#endregion

			var lezer = new Lezer(path);

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

			#region Personen overzetten
			var personen = lezer.PersonenOphalen();

			foreach (var p in personen)
			{
				int persoonID;

				ToonPersoonDetail(p.PersoonDetail);

				try
				{
					persoonID = serviceHelper.CallService<IGelieerdePersonenService, int>(svc => svc.Aanmaken(p.PersoonDetail, dbGroep.ID));
					Console.WriteLine(Resources.PersoonAangemaaktAls, persoonID);
				}
				catch (FaultException<BlokkerendeObjectenFault<PersoonDetail>> ex)
				{
					persoonID = serviceHelper.CallService<IGelieerdePersonenService, int>(svc => svc.GeforceerdAanmaken(p.PersoonDetail, dbGroep.ID, true));
					p.PersoonDetail.GelieerdePersoonID = persoonID;

					mogelijkeDubbels.Add(new MogelijkDubbel { Bestaand = ex.Detail.Objecten.First(), Nieuw = p.PersoonDetail });
				}

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
					try
					{
						serviceHelper.CallService<IGelieerdePersonenService>(svc => svc.AdresToevoegenGelieerdePersonen(
							new List<int> { persoonID },
							pa));

					}
					catch (Exception)
					{
						Console.WriteLine(Resources.OnbekendAdres);
					}
				}

				foreach (var ci in p.CommunicatieInfo)
				{
					Console.WriteLine(
						Properties.Resources.CommunicatieInfo,
						ci.Nummer,
						ci.CommunicatieTypeID,
						ci.Voorkeur ? "*" : " ");

					serviceHelper.CallService<IGelieerdePersonenService>(svc => svc.CommunicatieVormToevoegen(
						persoonID,
						ci));
				}

			}
			#endregion

			Console.WriteLine(Properties.Resources.TotaalInfo, personen.Count());

			foreach (var d in mogelijkeDubbels)
			{
				Console.WriteLine(Resources.MogelijkDubbel);
				ToonPersoonDetail(d.Nieuw);
				ToonPersoonDetail(d.Bestaand);
			}
		}

		/// <summary>
		/// PersoonDetail naar output
		/// </summary>
		/// <param name="p">'Te outputten' PersoonDetail</param>
		private static void ToonPersoonDetail(PersoonDetail p)
		{
			if (p == null) throw new ArgumentNullException("p");

			Console.WriteLine(
				Properties.Resources.PersoonInfo,
				p.VoorNaam,
				p.Naam,
				p.GeboorteDatum,
				p.Geslacht,
				p.AdNummer);
		}
	}
}
