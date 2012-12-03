// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    /// Dit is een testclass voor Unit Tests van GroepsWerkJaarManagerTest
    /// </summary>
	[TestClass]
	public class GroepsWerkJaarManagerTest
	{


		private TestContext _testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext
		{
			get
			{
				return _testContextInstance;
			}
			set
			{
				_testContextInstance = value;
			}
		}

		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();
		}


		///<summary>
		///Controleert of groepswerkjaarcache gecleard wordt bij bewaren groepswerkjaar
		/// </summary>
		[TestMethod]
		public void BewarenTest()
		{
            //// Arrange

            //var testData = new DummyData();

            //var veelGebruiktMock = new Mock<IVeelGebruikt>();
            //veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarResetten(testData.DummyGroep.ID)).Verifiable();
            //Factory.InstantieRegistreren(veelGebruiktMock.Object);

            //var groepsWerkJaarDaoMock = new Mock<IGroepsWerkJaarDao>();
            //groepsWerkJaarDaoMock.Setup(
            //    dao => dao.Bewaren(testData.HuidigGwj, It.IsAny<Expression<Func<GroepsWerkJaar, object>>[]>())).Returns(
            //        testData.HuidigGwj);
            //Factory.InstantieRegistreren(groepsWerkJaarDaoMock.Object);

            //var target = Factory.Maak<GroepsWerkJaarManager>();

            //// Act

            //target.Bewaren(testData.HuidigGwj, GroepsWerkJaarExtras.Geen);

            //// Assert
            //veelGebruiktMock.Verify(vgb => vgb.GroepsWerkJaarResetten(testData.DummyGroep.ID), Times.Once());
            throw new NotImplementedException(NIEUWEBACKEND.Info);
		}

        /// <summary>
        /// Snelle test die nakijkt of GroepsWerkJaarManager.AfdelingsJarenVoorstellen daadwerkelijk de geboortedata
        /// voor de nieuwe afdelingsjaren bijwerkt.
        ///</summary>
        [TestMethod()]
        public void AfdelingsJarenVoorstellenTest()
        {
            var target = Factory.Maak<GroepsWerkJarenManager>();

            var groep = new ChiroGroep();
            var groepsWerkJaar = new GroepsWerkJaar {WerkJaar = 2010, ID = 2971, Groep = groep};
            var afdeling = new Afdeling {ID = 2337, ChiroGroep = groep};
            var afdelingsJaar = new AfdelingsJaar
                {GroepsWerkJaar = groepsWerkJaar, Afdeling = afdeling, GeboorteJaarVan = 2003, GeboorteJaarTot = 2004};

            const int NIEUW_WERKJAAR = 2011;

            var actual = target.AfdelingsJarenVoorstellen(groep, groep.Afdeling.ToArray(), NIEUW_WERKJAAR);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual[0]);
            Assert.AreEqual(actual[0].GeboorteJaarVan, afdelingsJaar.GeboorteJaarVan + 1);
            Assert.AreEqual(actual[0].GeboorteJaarTot, afdelingsJaar.GeboorteJaarTot + 1);
        }

        /// <summary>
        /// Deze test moet nagaan of GroepsWerkjaarManager.AfdelingsJarenVoorstellen ook een
        /// officiele afdeling koppelt aan een afdelingsjaar voor een afdeling die vorig jaar
        /// niet actief was.
        ///</summary>
        [TestMethod()]
        public void AfdelingsJarenVoorstellenTest1()
        {
            //// -- Arrange --

            //var afdelingenDaoMock = new Mock<IAfdelingenDao>();
            //afdelingenDaoMock.Setup(dao => dao.OfficieleAfdelingOphalen((int)NationaleAfdeling.Ribbels)).Returns(
            //    new OfficieleAfdeling {ID = (int) NationaleAfdeling.Ribbels, LeefTijdVan = 6, LeefTijdTot = 7});
            //Factory.InstantieRegistreren(afdelingenDaoMock.Object);

            //var target = Factory.Maak<GroepsWerkJaarManager>();

            //// Een Chirogroep met een oud groepswerkjaar. Zonder afdelingen, why not.
            //var groep = new ChiroGroep
            //    {GroepsWerkJaar = new EntityCollection<GroepsWerkJaar> {new GroepsWerkJaar()}};

            //// Dit jaar willen we een groep met 1 afdeling.
            //var afdelingen = new[] {new Afdeling {ID = 1, ChiroGroep = groep}};

            //const int NIEUW_WERKJAAR = 2012; // Jaartal is eigenlijk irrelevant voor deze test.

            //// -- Act -- 
            //var actual = target.AfdelingsJarenVoorstellen(groep, afdelingen, NIEUW_WERKJAAR);
            //var afdelingsJaar = actual.FirstOrDefault();

            //// -- Assert --

            //Assert.IsNotNull(afdelingsJaar);
            //Assert.IsNotNull(afdelingsJaar.OfficieleAfdeling);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }
    }
}
