using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Orm;
using Chiro.Gap.Domain;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers.Test
{


	/// <summary>
	///This is a test class for AfdelingsJaarManagerTest and is intended
	///to contain all AfdelingsJaarManagerTest Unit Tests
	///</summary>
	[TestClass]
	public class AfdelingsJaarManagerTest
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


		//Use ClassCleanup to run code after all tests in a class have run
		[ClassCleanup()]
		public static void MyClassCleanup()
		{
			Factory.Dispose();
		}

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


		///<summary>
		///Probeer een afdeling te maken in een groepswerkjaar van een andere groep
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(FoutNummerException))]
		public void AfdelingGroepsWerkJaarMismatch()
		{
			var groep1 = new ChiroGroep { ID = 1 };
			var groep2 = new ChiroGroep { ID = 2 };

			var gwj = new GroepsWerkJaar { ID = 1, Groep = groep1 };
			var a = new Afdeling {ChiroGroep = groep2};
			var oa = new OfficieleAfdeling();

			AfdelingsJaarManager target = Factory.Maak<AfdelingsJaarManager>();

			target.Aanmaken(a, oa, gwj, 2001, 2002, GeslachtsType.Gemengd);

			// Als we hier geraken, is de verwachte exception niet opgeworpen

			Assert.IsTrue(false);
		}
	}
}
