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
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Workers.Test
{
	/// <summary>
	/// Tests op de security van de workers.
	/// </summary>
	/// <remarks>Autorisatie moet met de nieuwe backend in de services
	/// gebeuren i.p.v. in de workers. Ik heb de tests hier allemaal
	/// NotImplemented gemaakt; de bedoeling is dat ze vervangen worden
	/// door gelijkaardige tests in Chiro.Gap.Services.Autorisatie.Test.
	/// Dit zal stelselmatisch gebeuren.
	/// </remarks>
	[TestClass]
	public class AutorisatieTest
	{
		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[ClassInitialize]
		static public void InitialiseerTests(TestContext tc)
		{
		}

		[ClassCleanup]
		static public void AfsluitenTests()
		{
		}

		/// <summary>
		/// Probeert ledenlijst op te halen voor niet-GAV.
		/// Verwacht een exception.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(GeenGavException))]
		public void LijstLedenGeenGav()
		{
            //// Arrange

            //var testData = new DummyData();
            //var ledenDaoMock = new Mock<ILedenDao>();
            //var autorisatieMgrMock = new Mock<IAutorisatieManager>();

            //ledenDaoMock.Setup(foo => foo.OphalenUitGroepsWerkJaar(testData.HuidigGwj.ID, false)).Returns(new List<Lid>());
            //autorisatieMgrMock.Setup(foo => foo.IsGavGroepsWerkJaar(testData.HuidigGwj.ID)).Returns(false);

            //Factory.InstantieRegistreren(ledenDaoMock.Object);
            //Factory.InstantieRegistreren(autorisatieMgrMock.Object);

            //var daos = Factory.Maak<LedenDaoCollectie>();
            //var lm = Factory.Maak<LedenManager>();

            //// Act

            //lm.Zoeken(new LidFilter { GroepsWerkJaarID = testData.HuidigGwj.ID, LidType = LidType.Alles }, LidExtras.Geen);

            //// Verwacht exception
            throw new NotImplementedException(NIEUWEBACKEND.Info);
		}

		/// <summary>
		/// Probeert aantal groepswerkjaren van een groep op te halen waarvoor
		/// men geen GAV is, via LedenManager.PaginaOphalen.
		/// Verwacht een exception.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(GeenGavException))]
		public void LijstLedenGeenGavAantalGwj()
		{
            throw new NotImplementedException();
        //    // Arrange

        //    var testData = new DummyData();

        //    var ledenDaoMock = new Mock<ILedenDao>();
        //    var autorisatieMgrMock = new Mock<IAutorisatieManager>();
        //    var groepenDaoMock = new Mock<IGroepenDao>();
        //    var groepsWerkJaarDaoMock = new Mock<IGroepsWerkJaarDao>();

        //    ledenDaoMock.Setup(foo => foo.OphalenUitGroepsWerkJaar(testData.HuidigGwj.ID, false)).Returns(new List<Lid>());
        //    autorisatieMgrMock.Setup(foo => foo.IsGavGroepsWerkJaar(testData.HuidigGwj.ID)).Returns(false);
        //    groepsWerkJaarDaoMock.Setup(foo => foo.Ophalen(testData.HuidigGwj.ID)).Returns(testData.HuidigGwj);
        //    groepenDaoMock.Setup(foo => foo.Ophalen(testData.DummyGroep.ID)).Returns(testData.DummyGroep);

        //    Factory.InstantieRegistreren(ledenDaoMock.Object);
        //    Factory.InstantieRegistreren(groepenDaoMock.Object);
        //    Factory.InstantieRegistreren(autorisatieMgrMock.Object);
        //    Factory.InstantieRegistreren(groepsWerkJaarDaoMock.Object);

        //    Factory.Maak<LedenDaoCollectie>();
        //    var lm = Factory.Maak<LedenManager>();

        //    // Act
        //    lm.Zoeken(new LidFilter { GroepsWerkJaarID = testData.HuidigGwj.ID, LidType = LidType.Alles }, LidExtras.Geen);

        //    // Verwacht exception
        }

		/// <summary>
		/// Probeer een afdelingsjaar van een eigen afdeling te maken in een groep waar je
		/// geen GAV van bent
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(GeenGavException))]
		public void AfdelingsJaarMakenAndereGroep()
		{
			var gwj = new GroepsWerkJaar {ID = 1};
			var afd = new Afdeling();
			var oa = new OfficieleAfdeling();

			// Creer een AutorisatieManagerMock die zegt dat de gebruiker geen GAV is
			// van het gegeven groepswerkjaar.

			var auMgrMock = new Mock<IAutorisatieManager>();
			auMgrMock.Setup(mgr => mgr.IsGav(gwj)).Returns(false);

			Factory.InstantieRegistreren(auMgrMock.Object);

			var ajMgr = Factory.Maak<IAfdelingsJaarManager>();

			ajMgr.Aanmaken(afd, oa, gwj, 200, 2001, GeslachtsType.Gemengd);

			// Als we hier nog komen, heeft de verwachte exception zich niet voorgedaan.

			Assert.IsTrue(false);
		}
	}
}
