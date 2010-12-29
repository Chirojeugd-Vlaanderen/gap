using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Chiro.Cdf.ServiceHelper;

namespace Chiro.Gap.WebApp.Test
{
  
	[TestClass()]
	public class VeelGebruiktTest
	{
		/// <summary>
		/// Deze dummyservicehelper zal bijhouden hoe vaak de service
		/// aangeroepen wordt.  (Ik kreeg dit niet gearrangeerd met Moq.)
		/// </summary>
		private class DummyServiceHelper : IServiceHelper
		{
			private int _aantalCalls = 0;

			internal int AantalCalls
			{
				get { return _aantalCalls; }
			}

			public T CallService<I, T>(Func<I, T> operation) where I:class
			{
				++_aantalCalls;

				return default(T);
			}

			public void CallService<I>(Action<I> operation) where I : class
			{
				++_aantalCalls;
			}
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
		///A test for FunctieProblemenOphalen
		///</summary>
		[TestMethod()]
		public void FunctieProblemenOphalenTest()
		{
			// Arrange

			int groepID = 0; // TODO: Initialize to an appropriate value
			var serviceHelper = new DummyServiceHelper();
			var veelGebruikt = new VeelGebruikt(serviceHelper);
			
			// Act

			var problemen1 = veelGebruikt.FunctieProblemenOphalen(groepID, serviceHelper);
			var problemen2 = veelGebruikt.FunctieProblemenOphalen(groepID, serviceHelper);

			// Assert

			Assert.AreEqual(serviceHelper.AantalCalls, 1);
		}
	}
}
