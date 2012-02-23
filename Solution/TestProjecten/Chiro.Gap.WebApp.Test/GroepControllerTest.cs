using Chiro.Cdf.Ioc;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Web.Mvc;

using Moq;

using Chiro.Gap.ServiceContracts;
using Chiro.Gap.WebApp.Controllers;

namespace Chiro.Gap.WebApp.Test
{
    
    
    /// <summary>
    /// Dit is een testclass voor Unit Tests van GroepControllerTest,
    /// to contain all GroepControllerTest Unit Tests
    /// </summary>
	[TestClass]
	public class GroepControllerTest
	{
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Factory.ContainerInit();
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
		/// Controleert of de action Groep/CategorieVerwijderen de categorieenservice aanroept,
		/// en of achteraf terug de view met de groepsinstellingen getoond wordt.
		/// </summary>
		[TestMethod]
		public void CategorieVerwijderenTest()
		{
			const int DUMMYCATID = 9;
			const int DUMMYGROEPID = 1;

			var groepenServiceMock = new Mock<IGroepenService>();

			// Verwacht dat de groepenservice aangeroepen wordt.
			groepenServiceMock.Setup(mock=>mock.CategorieVerwijderen(It.IsAny<int>(), It.IsAny<bool>()));

		    Factory.InstantieRegistreren(groepenServiceMock.Object);

			var target = new CategorieenController(new VeelGebruikt());

			var actual = target.CategorieVerwijderen(DUMMYGROEPID, DUMMYCATID) as RedirectToRouteResult;

			// Controleer of verwachte service call wel is gebeurd
			groepenServiceMock.VerifyAll();

			// Verwacht de view met de groepsinstellingen.
			Assert.IsNotNull(actual);
			Assert.AreEqual("Index", actual.RouteValues["action"]);
		}

	}
}
