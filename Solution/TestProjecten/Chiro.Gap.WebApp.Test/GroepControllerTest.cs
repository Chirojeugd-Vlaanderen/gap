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
﻿using Chiro.Cdf.Ioc;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            Factory.ContainerInit();
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
