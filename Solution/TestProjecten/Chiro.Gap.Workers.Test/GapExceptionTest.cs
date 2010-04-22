using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for GapExceptionTest and is intended
    ///to contain all GapExceptionTest Unit Tests
    ///</summary>
	[TestClass()]
	public class GapExceptionTest
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
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
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
		///Test de (de)serializatie van de GapException
		///</summary>
		[TestMethod()]
		public void GapExceptionConstructorTest()
		{
			// Arrange

			var target = new GapException {FoutNummer = 11, Items = new string[] {"een", "twee"}};


			// Act

			using (Stream s = new MemoryStream())
			{
				var formatter = new BinaryFormatter();

				formatter.Serialize(s, target);
				s.Position = 0;

				target = (GapException)formatter.Deserialize(s);
			}

			Assert.AreEqual(target.FoutNummer, 11);
			Assert.AreEqual(String.Compare(target.Items.First(), "een"), 0);
			Assert.AreEqual(String.Compare(target.Items.Last(), "twee"), 0);

		}
	}
}
