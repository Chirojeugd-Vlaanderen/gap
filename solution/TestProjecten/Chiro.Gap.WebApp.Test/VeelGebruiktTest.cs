using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

using Moq;

namespace Chiro.Gap.WebApp.Test
{

	[TestClass]
	public class VeelGebruiktTest
	{
		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();

			// zorgt ervoor dat de dummy service helper een lege lijst
			// landinfo oplevert als er IEnumerable<LandInfo> gevraagd wordt.
			// (een beetje een hack)
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		///<summary>
		///Kijkt na of de lijst met landen goed wordt gecachet
		/// </summary>
		[TestMethod]
		public void LandenOphalenTest()
		{
			// Arrange

		    var groepenServiceMock = new Mock<IGroepenService>();
		    groepenServiceMock.Setup(mock => mock.LandenOphalen()).Returns(new List<LandInfo>());
            Factory.InstantieRegistreren(groepenServiceMock.Object);

			var veelGebruikt = new VeelGebruikt();

			// Act

			var res1 = veelGebruikt.LandenOphalen();
			var res2 = veelGebruikt.LandenOphalen();

			// Assert

			groepenServiceMock.Verify(mock => mock.LandenOphalen(), Times.Exactly(1));
		}
	}
}
