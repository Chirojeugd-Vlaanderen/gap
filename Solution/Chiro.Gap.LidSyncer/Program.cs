using Chiro.Cdf.Ioc;
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

			// Leden overzetten vereist super-gav-rechten
			// We regelen dit via mocking.

			var auMgrMock = new Mock<IAutorisatieManager>();
			auMgrMock.Setup(mgr => mgr.IsSuperGav()).Returns(true);
			Factory.InstantieRegistreren(auMgrMock.Object);

			// Creeer nu de ledenmanager

			var ledenMgr = Factory.Maak<LedenManager>();

			ledenMgr.LedenOverZetten();
		}
	}
}
