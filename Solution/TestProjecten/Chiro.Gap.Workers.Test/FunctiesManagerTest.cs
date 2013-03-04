using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Moq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Workers.Test
{
	/// <summary>
	/// Dit is een testclass voor Unit Tests van FunctiesManagerTest,
	/// to contain all FunctiesManagerTest Unit Tests
	/// </summary>
	[TestClass]
	public class FunctiesManagerTest
	{
		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();
		}
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup()
		// { 
		// }
		//
		// Use TestInitialize to run code before running each test
		// [TestInitialize]
		// public void MyTestInitialize()
		// {
		// }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup()
		// {
		// }
		//
		#endregion

        /// <summary>
        /// Opsporen functie met te veel aantal members
        /// </summary>
        [TestMethod]
        public void TweeKeerUniekeFunctieToekennenTestVerschillendLid()
        {
            //// Arrange

            //var testData = new DummyData();

            //var fm = Factory.Maak<FunctiesManager>();
            //var gm = Factory.Maak<GroepenManager>();

            //var f = gm.FunctieToevoegen(
            //    testData.DummyGroep,
            //    testData.NieuweFunctieNaam,
            //    testData.NieuweFunctieCode,
            //    1, 0,
            //    LidType.Alles);

            //IEnumerable<Functie> functies = new Functie[] { f };

            //// Act

            //fm.Toekennen(testData.LeiderJos, functies);
            //fm.Toekennen(testData.LidYvonne, functies);

            //// Assert

            //var problemen = fm.AantallenControleren(testData.HuidigGwj, functies);
            //Assert.AreNotEqual(problemen.Count(), 0);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Als een functie maar 1 keer mag voorkomen, maar ze wordt 2 keer toegekend aan dezelfde
        /// persoon, dan moet dat zonder problemen kunnen.  
        /// </summary>
        [TestMethod]
        public void TweeKeerUniekeFunctieToekennenTestZelfdeLid()
        {
            //// Arrange

            //var testData = new DummyData();

            //FunctiesManager fm = Factory.Maak<FunctiesManager>();
            //GroepenManager gm = Factory.Maak<GroepenManager>();

            //Functie f = gm.FunctieToevoegen(
            //    testData.DummyGroep,
            //    testData.NieuweFunctieNaam,
            //    testData.NieuweFunctieCode,
            //    1, 0,
            //    LidType.Alles);

            //IEnumerable<Functie> functies = new Functie[] { f };

            //// Act

            //fm.Toekennen(testData.LeiderJos, functies);
            //fm.Toekennen(testData.LeiderJos, functies);

            //// Assert

            //var problemen = fm.AantallenControleren(testData.HuidigGwj, functies);
            //Assert.AreEqual(problemen.Count(), 0);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Het toekennen van een functie die niet geldig is in het huidige werkjaar, moet
        /// een exception opleveren
        /// </summary>
        [ExpectedException(typeof(FoutNummerException))]
        [TestMethod]
        public void ToekennenFunctieOngeldigWerkJaar()
        {
            //// Arrange

            //var testData = new DummyData();

            //FunctiesManager fm = Factory.Maak<FunctiesManager>();
            //GroepenManager gm = Factory.Maak<GroepenManager>();

            //Functie f = gm.FunctieToevoegen(
            //    testData.DummyGroep,
            //    testData.NieuweFunctieNaam,
            //    testData.NieuweFunctieCode,
            //    1, 0,
            //    LidType.Alles);

            //f.WerkJaarTot = testData.HuidigGwj.WerkJaar - 1; // vervallen functie

            //IEnumerable<Functie> functies = new Functie[] { f };

            //// Act

            //fm.Toekennen(testData.LeiderJos, functies);

            //// Assert
            //Assert.IsTrue(false);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Functies voor leiding mogen niet aan een kind toegewezen worden.
        /// </summary>
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ToekennenLidFunctieAanLeiding()
        {
            //// Arrange

            //var testData = new DummyData();

            //FunctiesManager fm = Factory.Maak<FunctiesManager>();
            //GroepenManager gm = Factory.Maak<GroepenManager>();

            //Functie f = gm.FunctieToevoegen(
            //    testData.DummyGroep,
            //    testData.NieuweFunctieNaam,
            //    testData.NieuweFunctieCode,
            //    1, 0,
            //    LidType.Kind);

            //IEnumerable<Functie> functies = new Functie[] { f };

            //// Act
            //fm.Toekennen(testData.LeiderJos, functies);

            //// Assert
            //Assert.IsTrue(false);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Verplichte functie die niet toegekend wordt
        /// </summary>
        [TestMethod]
        public void NietToegekendeVerplichteFunctie()
        {
            //// Arrange

            //var testData = new DummyData();

            //FunctiesManager fm = Factory.Maak<FunctiesManager>();
            //GroepenManager gm = Factory.Maak<GroepenManager>();

            //Functie f = gm.FunctieToevoegen(
            //    testData.DummyGroep,
            //    testData.NieuweFunctieNaam,
            //    testData.NieuweFunctieCode,
            //    1, 1,
            //    LidType.Alles);

            //// Functie bewaren, zodat dummydao een ID toekent.

            //f = fm.Bewaren(f);

            //IEnumerable<Functie> functies = new Functie[] { f };

            //// Act

            //fm.Toekennen(testData.LeiderJos, functies);
            //fm.Toekennen(testData.LidYvonne, functies);

            //// Assert

            //var problemen = fm.AantallenControleren(testData.HuidigGwj, functies);
            //Assert.AreNotEqual(problemen.Count(), 0);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Kijkt na of de verplichte aantallen genegeerd worden voor functies die niet geldig zijn
        /// in het gegeven groepswerkjaar.
        /// </summary>
        [TestMethod]
        public void IrrelevanteVerplichteFunctie()
        {
            //// Arrange

            //Factory.ContainerInit();	// Container resetten alvorens dummydata te maken.
            //var testData = new DummyData();

            //var fm = Factory.Maak<FunctiesManager>();
            //var gm = Factory.Maak<GroepenManager>();

            //Functie f = gm.FunctieToevoegen(
            //    testData.DummyGroep,
            //    testData.NieuweFunctieNaam,
            //    testData.NieuweFunctieCode,
            //    1, 1,
            //    LidType.Alles);	// pas volgend jaar geldig

            //f.WerkJaarTot = testData.HuidigGwj.WerkJaar - 1; // vervallen functie

            //f.ID = testData.NieuweFunctieID;

            //// Jos krijgt alle nationaal bepaalde functies, zodat eventuele verplichte
            //// nationaal bepaalde functies OK zijn.
            //fm.Toekennen(testData.LeiderJos, fm.NationaalBepaaldeFunctiesOphalen());

            //// Act

            //var problemen = from p in fm.AantallenControleren(testData.HuidigGwj)
            //                where p.ID == testData.NieuweFunctieID
            //                select p;

            //// Assert

            //Assert.AreEqual(problemen.Count(), 0);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Standaard 'AantallenControleren'.  Nakijken of rekening wordt gehouden
        /// met nationaal bepaalde functies.
        /// </summary>
        [TestMethod]
        public void OntbrekendeNationaalBepaaldeFuncties()
        {
            // Arrange

            var testData = new DummyData();

            var fm = Factory.Maak<FunctiesManager>();

            // Act

            var problemen = fm.AantallenControleren(testData.HuidigGwj, testData.HuidigGwj.Groep.Functie);
            // Het DummyFunctieDao voorziet nationaal bepaalde verplichte functies.

            // Assert

            Assert.AreNotEqual(problemen.Count(), 0);
        }



        /// <summary>
        /// Test of regelementaire 'FunctieBewaren' geen exception oplevert.
        /// </summary>
        [TestMethod]
        public void FunctieBewarenTest()
        {
            //// Arrange

            //var testData = new DummyData();

            //FunctiesManager fm = Factory.Maak<FunctiesManager>();
            //GroepenManager gm = Factory.Maak<GroepenManager>();

            //Functie f = gm.FunctieToevoegen(
            //    testData.DummyGroep,
            //    testData.NieuweFunctieNaam,
            //    testData.NieuweFunctieCode,
            //    1, 0,
            //    0);

            //// Act

            //f = fm.Bewaren(f);

            //// Assert

            //Assert.AreNotEqual(f.ID, 0);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Testfuncties vervangen
        /// </summary>
        [TestMethod]
        public void FunctiesVervangen()
        {
            //// Arrange

            //var testData = new DummyData();

            //var fm = Factory.Maak<FunctiesManager>();
            //var gm = Factory.Maak<GroepenManager>();

            //Functie f = gm.FunctieToevoegen(
            //    testData.DummyGroep,
            //    testData.NieuweFunctieNaam,
            //    testData.NieuweFunctieCode,
            //    1, 0,
            //    LidType.Alles);

            //// Het DummyDao kent een ID toe aan f.  (Voor DummyDao is dat OK, maar in echte situaties
            //// niet, omdat de nieuwe f niet gekoppeld zou zijn aan de groep.  Eigenlijk moeten we
            //// de groep bewaren, samen met zijn functies.)

            //f = fm.Bewaren(f);

            //var natBepFuncties = fm.NationaalBepaaldeFunctiesOphalen();
            //// we weten dat er minstens 2 nat. bepaalde functies zijn.
            //IEnumerable<Functie> functies = new Functie[] { f, natBepFuncties.First() };
            //fm.Toekennen(testData.LeiderJos, functies);

            //// Act

            //fm.Vervangen(testData.LeiderJos, natBepFuncties);

            //// Assert

            //Assert.IsTrue(testData.LeiderJos.Functie.Contains(natBepFuncties.First()));
            //Assert.IsTrue(testData.LeiderJos.Functie.Contains(natBepFuncties.Last()));
            //Assert.IsFalse(testData.LeiderJos.Functie.Contains(f));
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }


		/// <summary>
		/// Controleert of de nationaal bepaalde functies gecachet worden, door te tellen hoe dikwijls
		/// de DAO opgevraagd wordt.
		/// </summary>
		[TestMethod]
		public void NationaalBepaaldeFunctiesOphalenTest()
		{
            //#region Arrange

            //// Deze DAO's nog eens expliciet registreren, om te vermijden dat wijzigingen in
            //// andere tests een vertekend beeld opleveren.

            //Factory.InstantieRegistreren<IAutorisatieManager>(new AutMgrAltijdGav());
            //Factory.InstantieRegistreren<ILedenDao>(new DummyLedenDao());

            //// Mock voor IFunctieDao, die een lege lijst geeft als de nationaal bepaalde functies
            //// opgevraagd worden.  We willen gewoon tellen hoe dikwijls deze opgevraagd wordt.
            //var funDaoMock = new Mock<IFunctiesDao>();
            //funDaoMock.Setup(dao => dao.NationaalBepaaldeFunctiesOphalen()).Returns(new List<Functie>());
            //Factory.InstantieRegistreren(funDaoMock.Object);

            //var target = Factory.Maak<FunctiesManager>();

            //#endregion

            //#region Act
            //// tweemaal opvragen
            //var resultaat = target.NationaalBepaaldeFunctiesOphalen();
            //resultaat = target.NationaalBepaaldeFunctiesOphalen();
            //#endregion

            //#region Assert

            //funDaoMock.Verify(dao => dao.NationaalBepaaldeFunctiesOphalen(), Times.AtMost(1), "Nationale functies waren niet gecachet.");

            //#endregion
            throw new NotImplementedException(NIEUWEBACKEND.Info);
		}

		/// <summary>
		/// Test voor ticket #890 
		/// </summary>
		[TestMethod]
		public void FunctieOphalenTest()
		{
            //// Arrange

            //var auMgrMock = new Mock<IAutorisatieManager>();
            //var funDaoMock = new Mock<IFunctiesDao>();

            //auMgrMock.Setup(mgr => mgr.IsGavCategorie(It.IsAny<int>())).Returns(false);
            //auMgrMock.Setup(mgr => mgr.IsGavFunctie(It.IsAny<int>())).Returns(true);

            //funDaoMock.Setup(mgr => mgr.Ophalen(It.IsAny<int>())).Returns(new Functie());

            //Factory.InstantieRegistreren<IAutorisatieManager>(auMgrMock.Object);
            //Factory.InstantieRegistreren<IFunctiesDao>(funDaoMock.Object);

            //var funMgr = Factory.Maak<FunctiesManager>();

            //// act

            //var resultaat = funMgr.Ophalen(100, false); // haal functie op zonder iets extra

            //// assert

            //// Aangezien ik de autorisatiemanager gemockt heb, zodat je rechten krijgt op iedere
            //// functie, moet er een functie opgehaald zijn.

            //Assert.IsNotNull(resultaat);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
		}

		/// <summary>
		/// Controleert of een ongebruikte functie probleemloos verwijderd kan worden
		/// </summary>
		[TestMethod]
		public void OngebruikteFunctieVerwijderenTest()
		{
			// arrange

			var testData = new DummyData();

			var veelGebruiktMock = new Mock<IVeelGebruikt>();
			veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarOphalen(testData.DummyGroep.ID)).Returns(testData.HuidigGwj);
			Factory.InstantieRegistreren<IVeelGebruikt>(veelGebruiktMock.Object);

			var mgr = Factory.Maak<FunctiesManager>();

			// act

			var result = mgr.Verwijderen(testData.OngebruikteFunctie, false);

			// assert

			Assert.IsNull(result);
		}

		/// <summary>
		/// probeert een functie die dit jaar in gebruik is te verwijderen.  We verwachten een exception.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(BlokkerendeObjectenException<Lid>))]
		public void FunctieDitJaarInGebruikVerwijderenTest()
		{
			// arrange

			var testData = new DummyData();

			var veelGebruiktMock = new Mock<IVeelGebruikt>();
			veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarOphalen(testData.DummyGroep.ID)).Returns(testData.HuidigGwj);
			Factory.InstantieRegistreren<IVeelGebruikt>(veelGebruiktMock.Object);

			var mgr = Factory.Maak<FunctiesManager>();

			// act

			var result = mgr.Verwijderen(testData.UniekeFunctie, false);

			// assert

			// Als we hier toekomen zonder exception, dan ging er iets mis.
			Assert.IsTrue(false);
		}

		/// <summary>
		/// probeert een functie die enkel dit jaar in gebruik is, geforceerd te verwijderen. 
		/// We verwachten dat ze definitief weg is.
		/// </summary>
		[TestMethod]
		public void FunctieEnkelDitJaarInGebruikGeforceerdVerwijderenTest()
		{
			// arrange

			var testData = new DummyData();

			var veelGebruiktMock = new Mock<IVeelGebruikt>();
			veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarOphalen(testData.DummyGroep.ID)).Returns(testData.HuidigGwj);
			Factory.InstantieRegistreren<IVeelGebruikt>(veelGebruiktMock.Object);

			var mgr = Factory.Maak<FunctiesManager>();

			// act

			var result = mgr.Verwijderen(testData.UniekeFunctie, true);

			// assert

			Assert.IsNull(result);
		}

		/// <summary>
		/// probeert een functie die zowel dit jaar als vorig jaar gebruikt is, 
		/// geforceerd te verwijderen.  We verwachten dat het 'werkJaar tot'  wordt
		/// ingevuld.
		/// </summary>
		[TestMethod]
		public void FunctieLangerInGebruikGeforceerdVerwijderenTest()
		{
			// arrange

			var testData = new DummyData();

			Debug.Assert(testData.HuidigGwj != null);

			var veelGebruiktMock = new Mock<IVeelGebruikt>();
			veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarOphalen(testData.DummyGroep.ID)).Returns(testData.HuidigGwj);
			Factory.InstantieRegistreren<IVeelGebruikt>(veelGebruiktMock.Object);

			var mgr = Factory.Maak<FunctiesManager>();

			// act

			var result = mgr.Verwijderen(testData.TraditieFunctie, true);

			// assert

			// functie niet meer geldig
			Assert.IsNotNull(result);
			Assert.AreEqual(result.WerkJaarTot, testData.HuidigGwj.WerkJaar - 1);

			// enkel het lid van dit werkJaar blijft over
			Assert.AreEqual(result.Lid.Count, 1);
		}
	}
}
