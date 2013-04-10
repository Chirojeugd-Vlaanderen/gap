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
﻿using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Moq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Workers.Test
{
	/// <summary>
	/// Dit is een testclass voor Unit Tests van FunctiesManagerTest,
	/// to contain all FunctiesManagerTest Unit Tests
	/// </summary>
	[TestClass]
	public class FunctiesManagerTest
	{
		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();
		}
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup()
		// { 
		// }
		//
		// Use TestInitialize to run code before running each test
		// [TestInitialize]
		// public void MyTestInitialize()
		// {
		// }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup()
		// {
		// }
		//
		#endregion

        /// <summary>
        /// Opsporen functie met te veel aantal members
        /// </summary>
        [TestMethod]
        public void TweeKeerUniekeFunctieToekennenTestVerschillendLid()
        {
            //// Arrange

            //var testData = new DummyData();

            //var fm = Factory.Maak<FunctiesManager>();
            //var gm = Factory.Maak<GroepenManager>();

            //var f = gm.FunctieToevoegen(
            //    testData.DummyGroep,
            //    testData.NieuweFunctieNaam,
            //    testData.NieuweFunctieCode,
            //    1, 0,
            //    LidType.Alles);

            //IEnumerable<Functie> functies = new Functie[] { f };

            //// Act

            //fm.Toekennen(testData.LeiderJos, functies);
            //fm.Toekennen(testData.LidYvonne, functies);

            //// Assert

            //var problemen = fm.AantallenControleren(testData.HuidigGwj, functies);
            //Assert.AreNotEqual(problemen.Count(), 0);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Als een functie maar 1 keer mag voorkomen, maar ze wordt 2 keer toegekend aan dezelfde
        /// persoon, dan moet dat zonder problemen kunnen.  
        /// </summary>
        [TestMethod]
        public void TweeKeerUniekeFunctieToekennenTestZelfdeLid()
        {
            // Arrange

            // Genereer de situatie

            var groep = new ChiroGroep();
            var groepsWerkJaar = new GroepsWerkJaar {Groep = groep};
            groep.GroepsWerkJaar = new List<GroepsWerkJaar> {groepsWerkJaar};

            var leider = new Leiding {GroepsWerkJaar = groepsWerkJaar};
            var functie = new Functie
                              {
                                  MaxAantal = 1,
                                  Type = LidType.Alles,
                                  IsNationaal = true,
                                  Niveau = Niveau.Alles
                              };


            var fm = Factory.Maak<FunctiesManager>();

            // Act

            fm.Toekennen(leider, new[]{functie});
            fm.Toekennen(leider, new[]{functie});

            // Assert

            var problemen = fm.AantallenControleren(groepsWerkJaar, new[]{functie});
            Assert.AreEqual(problemen.Count(), 0);
        }

        /// <summary>
        /// Het toekennen van een functie die niet geldig is in het huidige werkjaar, moet
        /// een exception opleveren
        /// </summary>
        [ExpectedException(typeof(FoutNummerException))]
        [TestMethod]
        public void ToekennenFunctieOngeldigWerkJaar()
        {
            //// Arrange

            //var testData = new DummyData();

            //FunctiesManager fm = Factory.Maak<FunctiesManager>();
            //GroepenManager gm = Factory.Maak<GroepenManager>();

            //Functie f = gm.FunctieToevoegen(
            //    testData.DummyGroep,
            //    testData.NieuweFunctieNaam,
            //    testData.NieuweFunctieCode,
            //    1, 0,
            //    LidType.Alles);

            //f.WerkJaarTot = testData.HuidigGwj.WerkJaar - 1; // vervallen functie

            //IEnumerable<Functie> functies = new Functie[] { f };

            //// Act

            //fm.Toekennen(testData.LeiderJos, functies);

            //// Assert
            //Assert.IsTrue(false);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Functies voor leiding mogen niet aan een kind toegewezen worden.
        /// </summary>
        [ExpectedException(typeof(FoutNummerException))]
        [TestMethod]
        public void ToekennenLidFunctieAanLeiding()
        {
            // Arrange

            var fm = Factory.Maak<FunctiesManager>();

            Groep groep = new ChiroGroep
                          {
                              Functie = new List<Functie>()
                          };

            var functie = new Functie
                            {
                                Groep = groep,
                                MaxAantal = 1,
                                MinAantal = 0,
                                Niveau = Niveau.LidInGroep
                            };
            groep.Functie.Add(functie);

            var leider = new Leiding
                             {
                                 GroepsWerkJaar = new GroepsWerkJaar {Groep = groep}
                             };
            groep.GroepsWerkJaar.Add(leider.GroepsWerkJaar);

            // Act

            fm.Toekennen(leider, new List<Functie>{functie});

            // Assert
            // We hebben geen assertions, want we verwachten dat er een exception optrad.
        }

        /// <summary>
        /// Verplichte functie die niet toegekend wordt
        /// </summary>
        [TestMethod]
        public void NietToegekendeVerplichteFunctie()
        {
            // ARRANGE

            var g = new ChiroGroep();

            // een (eigen) functie waarvan er precies 1 moet zijn
            var f = new Functie
                        {
                            MinAantal = 1,
                            Type = LidType.Alles,
                            ID = 1,
                            IsNationaal = false,
                            Niveau = Niveau.Alles,
                            Groep = g,
                        };

            // groepswerkjaar zonder leden
            var gwj = new GroepsWerkJaar
                          {
                              Lid = new List<Lid>(),
                              Groep = g
                          };

            // Maak een functiesmanager
            var fm = Factory.Maak<FunctiesManager>();


            // ACT
            var problemen = fm.AantallenControleren(gwj, new[] {f});

            // ASSERT
            Assert.IsTrue(problemen.Any(prb => prb.ID == f.ID));
        }

        /// <summary>
        /// Kijkt na of de verplichte aantallen genegeerd worden voor functies die niet geldig zijn
        /// in het gegeven groepswerkjaar.
        /// </summary>
        [TestMethod]
        public void IrrelevanteVerplichteFunctie()
        {
            //// Arrange

            //Factory.ContainerInit();	// Container resetten alvorens dummydata te maken.
            //var testData = new DummyData();

            //var fm = Factory.Maak<FunctiesManager>();
            //var gm = Factory.Maak<GroepenManager>();

            //Functie f = gm.FunctieToevoegen(
            //    testData.DummyGroep,
            //    testData.NieuweFunctieNaam,
            //    testData.NieuweFunctieCode,
            //    1, 1,
            //    LidType.Alles);	// pas volgend jaar geldig

            //f.WerkJaarTot = testData.HuidigGwj.WerkJaar - 1; // vervallen functie

            //f.ID = testData.NieuweFunctieID;

            //// Jos krijgt alle nationaal bepaalde functies, zodat eventuele verplichte
            //// nationaal bepaalde functies OK zijn.
            //fm.Toekennen(testData.LeiderJos, fm.NationaalBepaaldeFunctiesOphalen());

            //// Act

            //var problemen = from p in fm.AantallenControleren(testData.HuidigGwj)
            //                where p.ID == testData.NieuweFunctieID
            //                select p;

            //// Assert

            //Assert.AreEqual(problemen.Count(), 0);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Standaard 'AantallenControleren'.  Nakijken of rekening wordt gehouden
        /// met nationaal bepaalde functies.
        /// </summary>
        [TestMethod]
        public void OntbrekendeNationaalBepaaldeFuncties()
        {
            // ARRANGE

            // een nationale functie waarvan er precies 1 moet zijn
            var f = new Functie
            {
                MinAantal = 1,
                Type = LidType.Alles,
                ID = 1,
                IsNationaal = true,
                Niveau = Niveau.Alles
            };

            // groepswerkjaar zonder leden
            var gwj = new GroepsWerkJaar
            {
                Lid = new List<Lid>()
            };

            // Maak een functiesmanager
            var fm = Factory.Maak<FunctiesManager>();


            // ACT
            var problemen = fm.AantallenControleren(gwj, new[] { f });

            // ASSERT
            Assert.IsTrue(problemen.Any(prb => prb.ID == f.ID));
        }

        /// <summary>
        /// Testfuncties vervangen
        /// </summary>
        [TestMethod]
        public void FunctiesVervangen()
        {
            // Arrange

            // testdata
            var gwj = new GroepsWerkJaar();
            var groep = new ChiroGroep
            {
                GroepsWerkJaar = new List<GroepsWerkJaar> { gwj }
            };
            gwj.Groep = groep;

            var contactPersoon = new Functie
            {
                ID = 1,
                IsNationaal = true,
                Niveau = Niveau.Alles,
                Naam = "Contactpersoon",
                Type = LidType.Leiding
            };
            var finVer = new Functie
            {
                ID = 2,
                IsNationaal = true,
                Niveau = Niveau.Alles,
                Naam = "FinancieelVerantwoordelijke",
                Type = LidType.Leiding
            };
            var vb = new Functie
            {
                ID = 3,
                IsNationaal = true,
                Niveau = Niveau.Alles,
                Naam = "VB",
                Type = LidType.Leiding
            };
            var redactie = new Functie
            {
                ID = 4,
                IsNationaal = false,
                Niveau = Niveau.Groep,
                Naam = "RED",
                Type = LidType.Leiding,
                Groep = groep
            };
            var leiding = new Leiding
            {
                ID = 100,
                GroepsWerkJaar = gwj,
                Functie = new List<Functie> { contactPersoon, redactie },
                GelieerdePersoon = new GelieerdePersoon { Groep = groep }
            };

            var functiesMgr = Factory.Maak<FunctiesManager>();

            // ACT

            var leidingsFuncties = leiding.Functie; // bewaren voor latere referentie
            functiesMgr.Vervangen(leiding, new List<Functie> { finVer, vb, redactie });

            // ASSERT

            Assert.AreEqual(leiding.Functie.Count(), 3);
            Assert.IsTrue(leiding.Functie.Contains(finVer));
            Assert.IsTrue(leiding.Functie.Contains(vb));
            Assert.IsTrue(leiding.Functie.Contains(redactie));

            // om problemen te vermijden met entity framework, mag je bestaande collecties niet zomaar vervangen;
            // je moet entiteiten toevoegen aan/verwijderen uit bestaande collecties.
            Assert.AreEqual(leiding.Functie, leidingsFuncties);
        }


		/// <summary>
		/// Controleert of de nationaal bepaalde functies gecachet worden, door te tellen hoe dikwijls
		/// de DAO opgevraagd wordt.
		/// </summary>
		[TestMethod]
		public void NationaalBepaaldeFunctiesOphalenTest()
		{
            //#region Arrange

            //// Deze DAO's nog eens expliciet registreren, om te vermijden dat wijzigingen in
            //// andere tests een vertekend beeld opleveren.

            //Factory.InstantieRegistreren<IAutorisatieManager>(new AutMgrAltijdGav());
            //Factory.InstantieRegistreren<ILedenDao>(new DummyLedenDao());

            //// Mock voor IFunctieDao, die een lege lijst geeft als de nationaal bepaalde functies
            //// opgevraagd worden.  We willen gewoon tellen hoe dikwijls deze opgevraagd wordt.
            //var funDaoMock = new Mock<IFunctiesDao>();
            //funDaoMock.Setup(dao => dao.NationaalBepaaldeFunctiesOphalen()).Returns(new List<Functie>());
            //Factory.InstantieRegistreren(funDaoMock.Object);

            //var target = Factory.Maak<FunctiesManager>();

            //#endregion

            //#region Act
            //// tweemaal opvragen
            //var resultaat = target.NationaalBepaaldeFunctiesOphalen();
            //resultaat = target.NationaalBepaaldeFunctiesOphalen();
            //#endregion

            //#region Assert

            //funDaoMock.Verify(dao => dao.NationaalBepaaldeFunctiesOphalen(), Times.AtMost(1), "Nationale functies waren niet gecachet.");

            //#endregion
            throw new NotImplementedException(NIEUWEBACKEND.Info);
		}

		/// <summary>
		/// Test voor ticket #890 
		/// </summary>
		[TestMethod]
		public void FunctieOphalenTest()
		{
            //// Arrange

            //var auMgrMock = new Mock<IAutorisatieManager>();
            //var funDaoMock = new Mock<IFunctiesDao>();

            //auMgrMock.Setup(mgr => mgr.IsGavCategorie(It.IsAny<int>())).Returns(false);
            //auMgrMock.Setup(mgr => mgr.IsGavFunctie(It.IsAny<int>())).Returns(true);

            //funDaoMock.Setup(mgr => mgr.Ophalen(It.IsAny<int>())).Returns(new Functie());

            //Factory.InstantieRegistreren<IAutorisatieManager>(auMgrMock.Object);
            //Factory.InstantieRegistreren<IFunctiesDao>(funDaoMock.Object);

            //var funMgr = Factory.Maak<FunctiesManager>();

            //// act

            //var resultaat = funMgr.Ophalen(100, false); // haal functie op zonder iets extra

            //// assert

            //// Aangezien ik de autorisatiemanager gemockt heb, zodat je rechten krijgt op iedere
            //// functie, moet er een functie opgehaald zijn.

            //Assert.IsNotNull(resultaat);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
		}

		/// <summary>
		/// Controleert of een ongebruikte functie probleemloos verwijderd kan worden
		/// </summary>
		[TestMethod]
		public void OngebruikteFunctieVerwijderenTest()
		{
            //// arrange

            //var testData = new DummyData();

            //var veelGebruiktMock = new Mock<IVeelGebruikt>();
            //veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarIDOphalen(testData.DummyGroep.ID)).Returns(testData.HuidigGwj.ID);
            //Factory.InstantieRegistreren(veelGebruiktMock.Object);

            //var mgr = Factory.Maak<FunctiesManager>();

            //// act

            //var result = mgr.Verwijderen(testData.OngebruikteFunctie, false);

            //// assert

            //Assert.IsNull(result);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
		}

		/// <summary>
		/// probeert een functie die dit jaar in gebruik is te verwijderen.  We verwachten een exception.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(BlokkerendeObjectenException<Lid>))]
		public void FunctieDitJaarInGebruikVerwijderenTest()
		{
            // arrange

            // testsituatie creeren
		    var functie = new Functie();
		    var groepswerkjaar = new GroepsWerkJaar
		                             {
                                         ID = 11,
		                                 Groep =
		                                     new ChiroGroep
		                                         {
		                                             ID = 1,
		                                             GroepsWerkJaar = new List<GroepsWerkJaar>()
		                                         }
		                             };
            groepswerkjaar.Groep.GroepsWerkJaar.Add(groepswerkjaar);
		    functie.Groep = groepswerkjaar.Groep;
		    var lid = new Leiding {Functie = new List<Functie>() {functie}, GroepsWerkJaar = groepswerkjaar};
            functie.Lid.Add(lid);

			var mgr = Factory.Maak<FunctiesManager>();

			// act

			var result = mgr.Verwijderen(functie, false);

			// assert

			// Als we hier toekomen zonder exception, dan ging er iets mis.
			Assert.IsTrue(false);
		}

		/// <summary>
		/// probeert een functie die enkel dit jaar in gebruik is, geforceerd te verwijderen. 
		/// We verwachten dat ze definitief weg is.
		/// </summary>
		[TestMethod]
		public void FunctieEnkelDitJaarInGebruikGeforceerdVerwijderenTest()
		{
            //// arrange

            //var testData = new DummyData();

            //var veelGebruiktMock = new Mock<IVeelGebruikt>();
            //veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarIDOphalen(testData.DummyGroep.ID)).Returns(testData.HuidigGwj.ID);
            //Factory.InstantieRegistreren<IVeelGebruikt>(veelGebruiktMock.Object);

            //var mgr = Factory.Maak<FunctiesManager>();

            //// act

            //var result = mgr.Verwijderen(testData.UniekeFunctie, true);

            //// assert

            //Assert.IsNull(result);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
		}

		/// <summary>
		/// probeert een functie die zowel dit jaar als vorig jaar gebruikt is, 
		/// geforceerd te verwijderen.  We verwachten dat het 'werkJaar tot'  wordt
		/// ingevuld.
		/// </summary>
		[TestMethod]
		public void FunctieLangerInGebruikGeforceerdVerwijderenTest()
		{
            //// arrange

            //var testData = new DummyData();

            //Debug.Assert(testData.HuidigGwj != null);

            //var veelGebruiktMock = new Mock<IVeelGebruikt>();
            //veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarIDOphalen(testData.DummyGroep.ID)).Returns(testData.HuidigGwj.ID);
            //Factory.InstantieRegistreren<IVeelGebruikt>(veelGebruiktMock.Object);

            //var mgr = Factory.Maak<FunctiesManager>();

            //// act

            //var result = mgr.Verwijderen(testData.TraditieFunctie, true);

            //// assert

            //// functie niet meer geldig
            //Assert.IsNotNull(result);
            //Assert.AreEqual(result.WerkJaarTot, testData.HuidigGwj.WerkJaar - 1);

            //// enkel het lid van dit werkJaar blijft over
            //Assert.AreEqual(result.Lid.Count, 1);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
		}
	}
}
