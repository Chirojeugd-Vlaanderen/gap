// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Ioc;
using Chiro.Gap.Sync;
using Chiro.Gap.Workers;

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
			Factory.InstantieRegistreren(auMgrMock.Object);


			var gpMgr = Factory.Maak<GelieerdePersonenManager>();

			// Fixen dubbelpuntabonnementen die niet goed overgezet zijn

			gpMgr.FixDubbelPunt();

			// Overzetten leden na probeerperiode

			// var ledenMgr = Factory.Maak<LedenManager>();
			// ledenMgr.OverZettenNaProbeerPeriode();
		}
	}
}
