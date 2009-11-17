using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Dummies;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts.Mappers;

namespace Chiro.Gap.ServiceContracts.Test
{
	// 

	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class MapperTest
	{
		public MapperTest()
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

		[ClassInitialize]
		public static void Init(TestContext ctx)
		{
			Factory.ContainerInit();

			MappingHelper.MappingsDefinieren();
		}

		/// <summary>
		/// Test die probeert de leden uit huidig groepswerkjaar te mappen op een lijst van LidInfo.
		/// </summary>
		[TestMethod]
		public void MapLijstLeden()
		{
			var LidInfoLijst = Mapper.Map<IEnumerable<Lid>, IList<LidInfo>>(DummyData.HuidigGwj.Lid);

			Assert.IsTrue(LidInfoLijst.Count > 0);
		}
	}
}
