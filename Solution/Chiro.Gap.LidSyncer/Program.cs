// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Sync;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

using Moq;

namespace Chiro.Gap.LidSyncer
{
	class Program
	{
		/// <summary>
		/// Een dom programma dat niets anders doet dan de IoC in orde brengen, en vervolgens een LedenManager
		/// aanroept om de leden met voorbije probeerperiode te syncen.
		/// </summary>
		static void Main(string[] args)
		{
			Factory.ContainerInit();
			MappingHelper.MappingsDefinieren();

			// Leden overzetten vereist super-gav-rechten
			// We regelen dit via mocking.

			var auMgrMock = new Mock<IAutorisatieManager>();
			auMgrMock.Setup(mgr => mgr.IsSuperGav()).Returns(true);
			auMgrMock.Setup(mgr => mgr.IsGavGelieerdePersoon(It.IsAny<int>())).Returns(true);
			auMgrMock.Setup(mgr => mgr.EnkelMijnGelieerdePersonen(It.IsAny<IEnumerable<int>>())).Returns<IEnumerable<int>>(bla => bla.ToList());
			// nifty ;-P

			Factory.InstantieRegistreren(auMgrMock.Object);


			var gpMgr = Factory.Maak<GelieerdePersonenManager>();

			//// Fixen dubbelpuntabonnementen die niet goed overgezet zijn
			// gpMgr.FixDubbelPunt();

			//// Manueel verloren dubbelpunt rechttrekken

			//HardCodedDubbelpunt();

			// Overzetten leden na probeerperiode

			var ledenMgr = Factory.Maak<LedenManager>();
			ledenMgr.OverZettenNaProbeerPeriode();
		}

		private static void HardCodedDubbelpunt()
		{
			int[] gpIdLijst = { 15924, 15926, 15929, 15935, 15939, 15953, 15955, 15963, 15972, 15979, 34039, 34050, 34092, 34116, 34122, 34141, 34148, 34190, 34214, 34248, 35395, 35436, 35488, 35507, 35512, 35523, 35533, 35549, 46568, 46593, 46600, 46627, 46633, 46656, 46659, 46662, 46670, 46679, 46707, 46716, 46720, 46725, 46742, 88581, 88592, 88635, 88667, 88700, 93836, 93842, 93860, 94148, 94239, 94254, 94296, 112859, 113022, 138345, 138360, 138374, 138376, 138407, 138421, 138428, 138435, 140493, 140517, 140542, 140593, 140595 };

			var gpMgr = Factory.Maak<GelieerdePersonenManager>();

			foreach (int gpid in gpIdLijst)
			{
				var gp = gpMgr.Ophalen(gpid, PersoonsExtras.Adressen);

				// Eerst abonnement opheffen

				//Debug.Assert(gp.Persoon.DubbelPuntAbonnement);
				gp.Persoon.DubbelPuntAbonnement = false;
				gpMgr.Bewaren(gp, PersoonsExtras.Geen);

				// Opnieuw bestellen

				try
				{
					gpMgr.DubbelpuntBestellen(gp);
				}
				catch (FoutNummerException ex)
				{
					if (ex.FoutNummer == FoutNummer.AdresOntbreekt)
					{
						Console.WriteLine("Geen adres voor gelieerdepersoon {0}", gp.ID);
					}
                                        else throw;
				}
                                
				gpMgr.Bewaren(gp, PersoonsExtras.Geen);
			}
		}
	}
}
