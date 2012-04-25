using Microsoft.VisualStudio.TestTools.UnitTesting;

using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm.Test
{
    /// <summary>
    /// Dit is een testclass voor Unit Tests van FunctieTest,
    /// to contain all FunctieTest Unit Tests
    /// </summary>
	[TestClass]
	public class FunctieTest
	{
    	#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize]
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
		/// Controleert of de nationaal bepaalde functie 'Groepsleiding' wordt herkend.
		/// </summary>
		[TestMethod]
		public void NationaalBepaaldeFunctieTest()
		{
			var f = new Functie {ID = (int) NationaleFunctie.GroepsLeiding}; 
            Assert.IsTrue(f.Is(NationaleFunctie.GroepsLeiding));
		}
	}
}
