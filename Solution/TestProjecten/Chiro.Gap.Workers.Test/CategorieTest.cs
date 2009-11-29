using Chiro.Gap.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;
using System.Collections.Generic;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for GelieerdePersonenManagerTest and is intended
    ///to contain all GelieerdePersonenManagerTest Unit Tests
    ///</summary>
	 [TestClass()]
	 public class CategorieTest
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

		  //Is alleen maar auto gecreeerd

		  /// <summary>
		  ///A test for CategorieKoppelen
		  ///</summary>
		  [TestMethod()]
		  public void CategorieKoppelenTest()
		  {
				/*IGelieerdePersonenDao dao = null; // TODO: Initialize to an appropriate value
				IGroepenDao groepenDao = null; // TODO: Initialize to an appropriate value
				ICategorieenDao categorieenDao = null; // TODO: Initialize to an appropriate value
				IAutorisatieManager autorisatieMgr = null; // TODO: Initialize to an appropriate value
				GelieerdePersonenManager target = new GelieerdePersonenManager(dao, groepenDao, categorieenDao, autorisatieMgr); // TODO: Initialize to an appropriate value
				IList<int> gelieerdePersoonIDs = null; // TODO: Initialize to an appropriate value
				int categorieID = 0; // TODO: Initialize to an appropriate value
				bool koppelen = false; // TODO: Initialize to an appropriate value
				target.CategorieKoppelen(gelieerdePersoonIDs, categorieID, koppelen);
				Assert.Inconclusive("A method that does not return a value cannot be verified.");*/
		  }

		  /// <summary>
		  ///A test for OphalenCategorie
		  ///</summary>
		  [TestMethod()]
		  public void OphalenCategorieTest()
		  {
				/*IGelieerdePersonenDao dao = null; // TODO: Initialize to an appropriate value
				IGroepenDao groepenDao = null; // TODO: Initialize to an appropriate value
				ICategorieenDao categorieenDao = null; // TODO: Initialize to an appropriate value
				IAutorisatieManager autorisatieMgr = null; // TODO: Initialize to an appropriate value
				GelieerdePersonenManager target = new GelieerdePersonenManager(dao, groepenDao, categorieenDao, autorisatieMgr); // TODO: Initialize to an appropriate value
				int catID = 0; // TODO: Initialize to an appropriate value
				Categorie expected = null; // TODO: Initialize to an appropriate value
				Categorie actual;
				actual = target.OphalenCategorie(catID);
				Assert.AreEqual(expected, actual);
				Assert.Inconclusive("Verify the correctness of this test method.");*/
		  }

		  		  /// <summary>
		  ///A test for CategorieToevoegen
		  ///</summary>
		  [TestMethod()]
		  public void CategorieToevoegenTest()
		  {
				/*IGroepenDao dao = null; // TODO: Initialize to an appropriate value
				IDao<AfdelingsJaar> afdao = null; // TODO: Initialize to an appropriate value
				IAutorisatieManager autorisatieMgr = null; // TODO: Initialize to an appropriate value
				GroepenManager target = new GroepenManager(dao, afdao, autorisatieMgr); // TODO: Initialize to an appropriate value
				Categorie c = null; // TODO: Initialize to an appropriate value
				int gID = 0; // TODO: Initialize to an appropriate value
				target.CategorieToevoegen(c, gID);
				Assert.Inconclusive("A method that does not return a value cannot be verified.");*/
		  }

		  /// <summary>
		  ///A test for CategorieVerwijderen
		  ///</summary>
		  [TestMethod()]
		  public void CategorieVerwijderenTest()
		  {
				/*IGroepenDao dao = null; // TODO: Initialize to an appropriate value
				IDao<AfdelingsJaar> afdao = null; // TODO: Initialize to an appropriate value
				IAutorisatieManager autorisatieMgr = null; // TODO: Initialize to an appropriate value
				GroepenManager target = new GroepenManager(dao, afdao, autorisatieMgr); // TODO: Initialize to an appropriate value
				int categorieID = 0; // TODO: Initialize to an appropriate value
				target.CategorieVerwijderen(categorieID);
				Assert.Inconclusive("A method that does not return a value cannot be verified.");*/
		  }

		  /// <summary>
		  ///A test for OphalenMetCategorieen
		  ///</summary>
		  [TestMethod()]
		  public void OphalenMetCategorieenTest()
		  {
				/*IGroepenDao dao = null; // TODO: Initialize to an appropriate value
				IDao<AfdelingsJaar> afdao = null; // TODO: Initialize to an appropriate value
				IAutorisatieManager autorisatieMgr = null; // TODO: Initialize to an appropriate value
				GroepenManager target = new GroepenManager(dao, afdao, autorisatieMgr); // TODO: Initialize to an appropriate value
				int groepID = 0; // TODO: Initialize to an appropriate value
				Groep expected = null; // TODO: Initialize to an appropriate value
				Groep actual;
				actual = target.OphalenMetCategorieen(groepID);
				Assert.AreEqual(expected, actual);
				Assert.Inconclusive("Verify the correctness of this test method.");*/
		  }
	 }
}
