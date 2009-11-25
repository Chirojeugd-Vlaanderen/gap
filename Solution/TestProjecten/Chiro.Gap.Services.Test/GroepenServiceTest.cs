using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.ServiceContracts;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using Moq;
using Chiro.Gap.Dummies;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Services.Test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class GroepenServiceTest
	{
		public GroepenServiceTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

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
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext) 
		{
			Factory.ContainerInit();
		}

		/// <summary>
		/// Deze test kijkt enkel na of de method IGroepenservice.Ophalen(groepID) de groepenDAO
		/// aanspreekt.
		/// </summary>
		[TestMethod]
		public void GroepOphalen()
		{
			#region Arrange

			// Mock creeren voor IGroepenDao.
			var groepenDaoMock = new Mock<IGroepenDao>();

			// Als Ophalen(testGroepID) opgeropen wordt, testgroep opleveren
			groepenDaoMock.Setup(dao => dao.Ophalen(DummyData.DummyGroep.ID)).Returns(DummyData.DummyGroep);

			// IOC container de mock laten gebruiken
			Factory.InstantieRegistreren<IGroepenDao>(groepenDaoMock.Object);

			// Service creeren
			IGroepenService svc = Factory.Maak<GroepenService>();

			#endregion

			#region Act

			Groep g = svc.Ophalen(DummyData.DummyGroep.ID);

			#endregion

			#region Assert

			Assert.IsTrue(g.ID == DummyData.DummyGroep.ID);
			groepenDaoMock.Verify(dao => dao.Ophalen(DummyData.DummyGroep.ID)); // is Ophalen wel opgeroepen?

			#endregion
		}
	}
}
