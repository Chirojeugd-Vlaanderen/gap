/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Workers.Test
{


	/// <summary>
	/// Dit is een testclass voor Unit Tests van GelieerdePersonenManagerTest,
	/// to contain all GelieerdePersonenManagerTest Unit Tests
	/// </summary>
	[TestClass]
	public class CategorieTest
	{



        [ClassInitialize]
        static public void InitialiseerTests(TestContext tc)
        {
            Factory.ContainerInit();
        }

        [ClassCleanup]
        static public void AfsluitenTests()
        {
            
        }

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

		// Tests die niets deden meteen weggegooid.


		/// <summary>
		/// Controleert of een categorie niet per ongeluk dubbel gekoppeld kan worden aan eenzelfde persoon.
		/// GelieerdeJos zit in de categorie Vervelend, we proberen hem daar nog eens aan toe te voegen, en
		/// hopen dat het mislukt.
		/// </summary>
		[TestMethod]
		public void DubbeleKoppelingCategorie()
		{
            //// Arrange

            //var testData = new DummyData();

            //var gpMgr = Factory.Maak<GelieerdePersonenManager>();

            //if (!testData.GelieerdeJos.Categorie.Contains(testData.Vervelend))
            //{
            //    // Zeker zijn dat Jos al een keer in de categorie zit.
            //    gpMgr.CategorieKoppelen(
            //        new GelieerdePersoon[] { testData.GelieerdeJos }, 
            //        testData.Vervelend);
            //}

            //int aantalCategorieen = testData.GelieerdeJos.Categorie.Count;
		
            //// Act

            //gpMgr.CategorieKoppelen(new GelieerdePersoon[] { testData.GelieerdeJos }, testData.Vervelend);
			
            //// Assert		

            //Assert.IsTrue(testData.GelieerdeJos.Categorie.Count == aantalCategorieen);
            throw new NotImplementedException(Nieuwebackend.Info);
		}

	}
}
