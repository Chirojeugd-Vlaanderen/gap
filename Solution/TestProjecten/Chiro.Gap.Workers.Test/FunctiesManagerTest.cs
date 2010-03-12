using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

using Chiro.Gap.Dummies;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;

namespace Chiro.Gap.Workers.Test
{


	/// <summary>
	///This is a test class for FunctiesManagerTest and is intended
	///to contain all FunctiesManagerTest Unit Tests
	///</summary>
	[TestClass()]
	public class FunctiesManagerTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
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

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize()]
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
		//[TestInitialize()]
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
		/// Als een functie maar 1 keer mag voorkomen, maar ze wordt aan 2 personen toegekend, 
		/// verwachten we een exception.  
		/// </summary>
		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TweeKeerUniekeFunctieToekennenTestVerschillendLid()
		{
			// Arrange

			var testData = new DummyData();

			FunctiesManager fm = Factory.Maak<FunctiesManager>();
			Lid lid = testData.LidYvonne;
			IEnumerable<Functie> functies = new Functie[] { testData.UniekeFunctie };

			// Act

			fm.Toekennen(lid, functies);

			// Assert

			// Normaal komen we hier niet meer.

			Assert.IsTrue(false);
		}

		/// <summary>
		/// Als een functie maar 1 keer mag voorkomen, maar ze wordt 2 keer toegekend aan dezelfde
		/// persoon, dan moet dat zonder problemen kunnen.  (Uiteraard blijft de functie 1 keer toegekend,
		/// maar we zijn er vooral in geinteresseerd dat er geen exception opgeworpen wordt.)
		/// </summary>
		[TestMethod()]
		public void TweeKeerUniekeFunctieToekennenTestZelfdeLid()
		{
			// Arrange

			var testData = new DummyData();
			FunctiesManager fm = Factory.Maak<FunctiesManager>();
			Lid lid = testData.LidJos;
			IEnumerable<Functie> functies = new Functie[] { testData.UniekeFunctie };

			// Act

			fm.Toekennen(lid, functies);

			// Assert

			// Kijk even na of jos nog steeds 1 keer de functie UniekeFunctie heeft.

			int aantal = (from fun in lid.Functie
				      where fun.ID == testData.UniekeFunctie.ID 
				     select fun).Count();

			Assert.AreEqual(aantal, 1);
		}
	}
}
