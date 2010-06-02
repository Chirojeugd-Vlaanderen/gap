using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Data.Super;
using Chiro.Gap.Domain;
using Chiro.Gap.InitieleImport.Properties;
using Chiro.Gap.ServiceContracts;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Pdox.Data;

namespace Chiro.Gap.InitieleImport
{
	class Program
	{
		private static IServiceHelper _serviceHelper;

		private struct MogelijkDubbel
		{
			public PersoonDetail Nieuw { get; set; }
			public PersoonDetail Bestaand { get; set; }
		}

		/// <summary>
		/// Hier gebeurt het!
		/// </summary>
		/// <param name="args">Command line argumenten; eerste moet stamnr zijn</param>
		static void Main(string[] args)
		{
			if (args.Count() == 0)
			{
				Console.WriteLine(Properties.Resources.Usage);
			}
			else
			{
				Factory.ContainerInit();  // Init IOC
				_serviceHelper = Factory.Maak<IServiceHelper>();

				var helper = new KipdorpHelper();
				var god = new DataGod();


				string stamNrFile = helper.StamNrNaarBestand(args[0]);
				string aansluitingsBestand = helper.RecentsteAansluitingsBestand(args[0]);


				#region Recentste aansluitingsbestand uitpakken

				Console.WriteLine(Resources.UitpakkenVan, aansluitingsBestand);

				// TODO: temporary directory maken, met unieke naam.  Daar zal wellicht wel iets voor
				// bestaan in .NET.

				// !UitPakDir kort houden, owv de 65-karakterlimiet van paradoxpaden.
				
				string destdir = String.Format("{0}/{1}", Settings.Default.UitPakDir, stamNrFile);

				// pak recentste aansluitingsbestand uit
				Directory.CreateDirectory(destdir);

				string dataDir = helper.Uitpakken(aansluitingsBestand, destdir);

				#endregion

				#region Groep leegmaken en opnieuw creeren

				// alles van de groep verwijderen
                                                             
				Console.WriteLine(Resources.VolledigVerwijderen);
				god.GroepVolledigVerwijderen(args[0]);

				// groep opnieuw aanmaken

				Console.WriteLine(Resources.GroepOpnieuwAanmaken);
				god.GroepUitKipadmin(args[0]);

				#endregion

				#region Gebruikersrecht toekennen
				// rechten toekennen, zodat je als user de groep kan opvullen
				// Via de service komen we te weten wat de gebruikersnaam is :)

				string userName = _serviceHelper.CallService<IGroepenService, string>(svc => svc.WieBenIk());
				Console.WriteLine(Resources.ServiceUser, userName);
				Console.WriteLine(Resources.RechtenToekennen, userName);

				god.RechtenToekennen(args[0], userName);
				#endregion

				// Eerst importeren uit paradox, omdat de import uit paradox
				// bijv. geen telefoonnummers updatet als er al een persoon gevonden is.

				ImporterenUitPdox(dataDir);

				// Aanvullen uit kipadmin.

				god.GroepsPersonenUitKipadmin(args[0]);



				// Kan directory blijkbaar niet verwijderen, omdat dit proces zelf de directory gebruikt :-/
				//Directory.Delete(destdir, true);
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
				dbGroep = _serviceHelper.CallService<IGroepenService, GroepInfo>(svc => svc.InfoOphalenCode(groep.StamNummer));
			}
			catch (FaultException<GapFault> ex)
			{
				if (ex.Detail.FoutNummer == FoutNummer.GeenGav)
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
				IDPersEnGP ids = null;

				ToonPersoonDetail(p.PersoonDetail);

				try
				{
					ids = _serviceHelper.CallService<IGelieerdePersonenService, IDPersEnGP>(svc => svc.Aanmaken(p.PersoonDetail, dbGroep.ID));
					Console.WriteLine(Resources.PersoonAangemaaktAls, ids.PersoonID, ids.GelieerdePersoonID);
				}
				catch (FaultException<BlokkerendeObjectenFault<PersoonDetail>> ex)
				{
					if (!Settings.Default.VermijdDubbels)
					{
						ids =
							_serviceHelper.CallService<IGelieerdePersonenService, IDPersEnGP>(
								svc => svc.GeforceerdAanmaken(p.PersoonDetail, dbGroep.ID, true));
						p.PersoonDetail.GelieerdePersoonID = ids.GelieerdePersoonID;

						mogelijkeDubbels.Add(new MogelijkDubbel {Bestaand = ex.Detail.Objecten.First(), Nieuw = p.PersoonDetail});

						Console.WriteLine(Resources.PersoonAangemaaktAls, ids.PersoonID, ids.GelieerdePersoonID);
					}
				}
				catch (TimeoutException)
				{
					Console.WriteLine(Resources.TimeOut);
					ids = null;
				}

				if (ids != null)
				{
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
							_serviceHelper.CallService<IGelieerdePersonenService>(svc => svc.AdresToevoegenPersonen(
								new List<int> { ids.GelieerdePersoonID },
								pa,
								false));

						}
						catch (FaultException<OngeldigObjectFault>)
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

						try
						{
							_serviceHelper.CallService<IGelieerdePersonenService>(svc => svc.CommunicatieVormToevoegen(
								ids.GelieerdePersoonID,
								ci));
						}
						catch (FaultException<GapFault>)
						{
							Console.WriteLine(Resources.CommunicatieVormFoutFormaat);
						}
						catch (TimeoutException)
						{
							Console.WriteLine(Resources.TimeOut);
						}

					}
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
