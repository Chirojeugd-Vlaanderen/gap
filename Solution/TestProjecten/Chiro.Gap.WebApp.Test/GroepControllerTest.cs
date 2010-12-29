using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Web.Mvc;

using Moq;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.WebApp.Controllers;

namespace Chiro.Gap.WebApp.Test
{
    
    
    /// <summary>
    /// This is a test class for GroepControllerTest and is intended
    /// to contain all GroepControllerTest Unit Tests
    /// </summary>
	[TestClass]
	public class GroepControllerTest
	{
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

			var serviceHelperMock = new Mock<IServiceHelper>();

			// Verwacht dat de groepenservice aangeroepen wordt.
			serviceHelperMock.Setup(hlpr => hlpr.CallService(It.IsAny<Action<IGroepenService>>()));

			// Met onze servicehelperconstructie kan ik het mockobject blijkbaar niet laten controleren
			// welke service method precies aangeroepen wordt :-(

			IServiceHelper serviceHelper = serviceHelperMock.Object;
			var target = new CategorieenController(serviceHelper, new VeelGebruikt(serviceHelper));

			var actual = target.CategorieVerwijderen(DUMMYGROEPID, DUMMYCATID) as RedirectToRouteResult;

			// Controleer of verwachte service call wel is gebeurd
			serviceHelperMock.VerifyAll();

			// Verwacht de view met de groepsinstellingen.
			Assert.IsNotNull(actual);
			Assert.AreEqual("Index", actual.RouteValues["action"]);
		}

	}
}
