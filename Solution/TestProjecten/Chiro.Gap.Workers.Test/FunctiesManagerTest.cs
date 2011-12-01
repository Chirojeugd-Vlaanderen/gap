using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers.Test
{


	/// <summary>
	/// This is a test class for FunctiesManagerTest and is intended
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
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		/// <summary>
		/// Opsporen functie met te veel aantal members
		/// </summary>
		[TestMethod]
		public void TweeKeerUniekeFunctieToekennenTestVerschillendLid()
		{
			// Arrange

			var testData = new DummyData();

			var fm = Factory.Maak<FunctiesManager>();
			var gm = Factory.Maak<GroepenManager>();

			var f = gm.FunctieToevoegen(
				testData.DummyGroep,
				testData.NieuweFunctieNaam,
				testData.NieuweFunctieCode,
				1, 0,
				LidType.Alles);

			IEnumerable<Functie> functies = new Functie[] { f };

			// Act

			fm.Toekennen(testData.LeiderJos, functies);
			fm.Toekennen(testData.LidYvonne, functies);

			// Assert

			var problemen = fm.AantallenControleren(testData.HuidigGwj, functies);
			Assert.AreNotEqual(problemen.Count(), 0);
		}

		/// <summary>
		/// Als een functie maar 1 keer mag voorkomen, maar ze wordt 2 keer toegekend aan dezelfde
		/// persoon, dan moet dat zonder problemen kunnen.  
		/// </summary>
		[TestMethod]
		public void TweeKeerUniekeFunctieToekennenTestZelfdeLid()
		{
			// Arrange

			var testData = new DummyData();

			FunctiesManager fm = Factory.Maak<FunctiesManager>();
			GroepenManager gm = Factory.Maak<GroepenManager>();

			Functie f = gm.FunctieToevoegen(
				testData.DummyGroep,
				testData.NieuweFunctieNaam,
				testData.NieuweFunctieCode,
				1, 0,
				LidType.Alles);

			IEnumerable<Functie> functies = new Functie[] { f };

			// Act

			fm.Toekennen(testData.LeiderJos, functies);
			fm.Toekennen(testData.LeiderJos, functies);

			// Assert

			var problemen = fm.AantallenControleren(testData.HuidigGwj, functies);
			Assert.AreEqual(problemen.Count(), 0);
		}

		/// <summary>
		/// Het toekennen van een functie die niet geldig is in het huidige werkjaar, moet
		/// een exception opleveren
		/// </summary>
		[ExpectedException(typeof(FoutNummerException))]
		[TestMethod]
		public void ToekennenFunctieOngeldigWerkJaar()
		{
			// Arrange

			var testData = new DummyData();

			FunctiesManager fm = Factory.Maak<FunctiesManager>();
			GroepenManager gm = Factory.Maak<GroepenManager>();

			Functie f = gm.FunctieToevoegen(
				testData.DummyGroep,
				testData.NieuweFunctieNaam,
				testData.NieuweFunctieCode,
				1, 0,
				LidType.Alles);

		    f.WerkJaarTot = testData.HuidigGwj.WerkJaar - 1; // vervallen functie

			IEnumerable<Functie> functies = new Functie[] { f };

			// Act

			fm.Toekennen(testData.LeiderJos, functies);

			// Assert
			Assert.IsTrue(false);
		}

		/// <summary>
		/// Functies voor leiding mogen niet aan een kind toegewezen worden.
		/// </summary>
		[ExpectedException(typeof(InvalidOperationException))]
		[TestMethod]
		public void ToekennenLidFunctieAanLeiding()
		{
			// Arrange

			var testData = new DummyData();

			FunctiesManager fm = Factory.Maak<FunctiesManager>();
			GroepenManager gm = Factory.Maak<GroepenManager>();

			Functie f = gm.FunctieToevoegen(
				testData.DummyGroep,
				testData.NieuweFunctieNaam,
				testData.NieuweFunctieCode,
				1, 0,
				LidType.Kind);

			IEnumerable<Functie> functies = new Functie[] { f };

			// Act
			fm.Toekennen(testData.LeiderJos, functies);

			// Assert
			Assert.IsTrue(false);
		}

		/// <summary>
		/// Verplichte functie die niet toegekend wordt
		/// </summary>
		[TestMethod]
		public void NietToegekendeVerplichteFunctie()
		{
			// Arrange

			var testData = new DummyData();

			FunctiesManager fm = Factory.Maak<FunctiesManager>();
			GroepenManager gm = Factory.Maak<GroepenManager>();

			Functie f = gm.FunctieToevoegen(
				testData.DummyGroep,
				testData.NieuweFunctieNaam,
				testData.NieuweFunctieCode,
				1, 1,
				LidType.Alles);

			// Functie bewaren, zodat dummydao een ID toekent.

			f = fm.Bewaren(f);

			IEnumerable<Functie> functies = new Functie[] { f };

			// Act

			fm.Toekennen(testData.LeiderJos, functies);
			fm.Toekennen(testData.LidYvonne, functies);

			// Assert

			var problemen = fm.AantallenControleren(testData.HuidigGwj, functies);
			Assert.AreNotEqual(problemen.Count(), 0);
		}

		/// <summary>
		/// Kijkt na of de verplichte aantallen genegeerd worden voor functies die niet geldig zijn
		/// in het gegeven groepswerkjaar.
		/// </summary>
		[TestMethod]
		public void IrrelevanteVerplichteFunctie()
		{
			// Arrange

			Factory.ContainerInit();	// Container resetten alvorens dummydata te maken.
			var testData = new DummyData();

			var fm = Factory.Maak<FunctiesManager>();
			var gm = Factory.Maak<GroepenManager>();

			Functie f = gm.FunctieToevoegen(
				testData.DummyGroep,
				testData.NieuweFunctieNaam,
				testData.NieuweFunctieCode,
				1, 1,
				LidType.Alles);	// pas volgend jaar geldig

		    f.WerkJaarTot = testData.HuidigGwj.WerkJaar - 1; // vervallen functie

			f.ID = testData.NieuweFunctieID;

			// Jos krijgt alle nationaal bepaalde functies, zodat eventuele verplichte
			// nationaal bepaalde functies OK zijn.
			fm.Toekennen(testData.LeiderJos, fm.NationaalBepaaldeFunctiesOphalen());

			// Act

			var problemen = from p in fm.AantallenControleren(testData.HuidigGwj)
			                where p.ID == testData.NieuweFunctieID
			                select p;

			// Assert

			Assert.AreEqual(problemen.Count(), 0);
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

			FunctiesManager fm = Factory.Maak<FunctiesManager>();

			// Act

			var problemen = fm.AantallenControleren(testData.HuidigGwj);
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
			// Arrange

			var testData = new DummyData();

			FunctiesManager fm = Factory.Maak<FunctiesManager>();
			GroepenManager gm = Factory.Maak<GroepenManager>();

			Functie f = gm.FunctieToevoegen(
				testData.DummyGroep,
				testData.NieuweFunctieNaam,
				testData.NieuweFunctieCode,
				1, 0,
				0);

			// Act

			f = fm.Bewaren(f);

			// Assert

			Assert.AreNotEqual(f.ID, 0);
		}

		/// <summary>
		/// Test op exception bij poging tot bewaren van nationaal bepaalde functie.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(GeenGavException))]
		public void NationaalBepaaldeFunctieBewarenTest()
		{
			// Arrange

			var testData = new DummyData();

			var fm = Factory.Maak<FunctiesManager>();

			var f = fm.NationaalBepaaldeFunctiesOphalen().First();

			// Act

			f = fm.Bewaren(f);

			// Assert

			// Dit mogen we niet halen.
			Assert.IsTrue(false);
		}

		/// <summary>
		/// Testfuncties vervangen
		/// </summary>
		[TestMethod]
		public void FunctiesVervangen()
		{
			// Arrange

			var testData = new DummyData();

			var fm = Factory.Maak<FunctiesManager>();
			var gm = Factory.Maak<GroepenManager>();

			Functie f = gm.FunctieToevoegen(
				testData.DummyGroep,
				testData.NieuweFunctieNaam,
				testData.NieuweFunctieCode,
				1, 0,
				LidType.Alles);

			// Het DummyDao kent een ID toe aan f.  (Voor DummyDao is dat OK, maar in echte situaties
			// niet, omdat de nieuwe f niet gekoppeld zou zijn aan de groep.  Eigenlijk moeten we
			// de groep bewaren, samen met zijn functies.)

			f = fm.Bewaren(f);

			var natBepFuncties = fm.NationaalBepaaldeFunctiesOphalen();
			// we weten dat er minstens 2 nat. bepaalde functies zijn.
			IEnumerable<Functie> functies = new Functie[] { f, natBepFuncties.First() };
			fm.Toekennen(testData.LeiderJos, functies);

			// Act

			fm.Vervangen(testData.LeiderJos, natBepFuncties);

			// Assert

			Assert.IsTrue(testData.LeiderJos.Functie.Contains(natBepFuncties.First()));
			Assert.IsTrue(testData.LeiderJos.Functie.Contains(natBepFuncties.Last()));
			Assert.IsFalse(testData.LeiderJos.Functie.Contains(f));
		}

	}

}
