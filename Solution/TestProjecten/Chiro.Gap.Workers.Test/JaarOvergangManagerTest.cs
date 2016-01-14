/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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

using System;
using System.Collections.Generic;
using System.Linq;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    ///This is a test class for JaarOvergangManagerTest and is intended
    ///to contain all JaarOvergangManagerTest Unit Tests
    ///</summary>
	[TestClass()]
	public class JaarOvergangManagerTest
	{

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
		/// Voert een jaarovergang uit, en kijkt na of het nieuwe werkjaar het jusite aantal afdelingsjaren bevat.
		/// </summary>
		[TestMethod()]                    
		public void JaarOvergangUitvoerenTest()
		{
            // ARRANGE

            // Vrij veel voorbereiding voor een vrij flauwe test.

            var ribbeloff = new OfficieleAfdeling { ID = 1, LeefTijdTot = 7, LeefTijdVan = 6, Naam = "Ribbel" };

            var groep = new ChiroGroep { ID = 10, GroepsWerkJaar = new List<GroepsWerkJaar>()};
            var gwj = new GroepsWerkJaar { WerkJaar = 2010, Groep = groep };
            groep.GroepsWerkJaar.Add(gwj);
            var afdjaar1 = new AfdelingsJaar { ID = 1, GeboorteJaarVan = 2003, GeboorteJaarTot = 2004, OfficieleAfdeling = ribbeloff };
            var afd1 = new Afdeling { ID = 2, AfdelingsJaar = new List<AfdelingsJaar> { afdjaar1 }, ChiroGroep = groep};
            afdjaar1.Afdeling = afd1;
            groep.Afdeling.Add(afd1);
            gwj.AfdelingsJaar.Add(afdjaar1);

            var newafdjaar = new AfdelingDetail { AfdelingID = afd1.ID, AfdelingsJaarID = afdjaar1.ID, GeboorteJaarVan = DateTime.Today.Year - 10, GeboorteJaarTot = DateTime.Today.Year - 8, OfficieleAfdelingID = ribbeloff.ID, Geslacht = GeslachtsType.Gemengd };

		    var officieleAfdelingenRepo = new DummyRepo<OfficieleAfdeling>(new[] {ribbeloff});

            // ACT

		    var target = Factory.Maak<JaarOvergangManager>();
            var teActiveren = new List<AfdelingsJaarDetail> { newafdjaar };
            target.JaarOvergangUitvoeren(teActiveren, groep, officieleAfdelingenRepo);

            // ASSERT

            Assert.AreEqual(groep.GroepsWerkJaar.Count, 2); 
            Assert.AreEqual(groep.GroepsWerkJaar.OrderByDescending(gwjr => gwjr.WerkJaar).First().AfdelingsJaar.Count, 1);
		}
	}
}
