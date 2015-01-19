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

using ﻿System.Collections.Generic;
using System.Linq;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.TestAttributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

		    Chiro.Gap.Services.Dev.AdServiceMock blablabla;
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
            // ARRANGE

            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = new ChiroGroep(),
                WerkJaar = 2012
            };
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var uniekeFunctie = new Functie
            {
                MaxAantal = 1,
                Groep = groepsWerkJaar.Groep,
                Niveau = Niveau.Groep
            };

            var lid1 = new Leiding { GroepsWerkJaar = groepsWerkJaar, Functie = new List<Functie> { uniekeFunctie } };
            var lid2 = new Leiding { GroepsWerkJaar = groepsWerkJaar };
            uniekeFunctie.Lid.Add(lid1);

            groepsWerkJaar.Lid.Add(lid1);
            groepsWerkJaar.Lid.Add(lid2);

            // ACT

            var functiesManager = Factory.Maak<FunctiesManager>();
            functiesManager.Toekennen(lid2, new List<Functie> { uniekeFunctie });

            // ASSERT

            var issues = functiesManager.AantallenControleren(groepsWerkJaar, new List<Functie> {uniekeFunctie});

            Assert.IsTrue(issues.Select(src=>src.ID).Contains(uniekeFunctie.ID));
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
            // ARRANGE

            var groepsWerkJaar = new GroepsWerkJaar
                                     {
                                         Groep = new ChiroGroep(),
                                         WerkJaar = 2012
                                     };
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);
            var lid = new Leiding {GroepsWerkJaar = groepsWerkJaar};

            var vervallenFunctie = new Functie
            {
                WerkJaarTot = groepsWerkJaar.WerkJaar - 1,
                MinAantal = 1,
                Groep = groepsWerkJaar.Groep,
                Niveau = Niveau.Groep
            };

            // ACT

            var functiesManager = Factory.Maak<FunctiesManager>();
            functiesManager.Toekennen(lid, new List<Functie>{vervallenFunctie});

            // ASSERT

            // Als er geen exception gethrowd worden, zal de test failen.
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
            // ARRANGE

            var groepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep(), WerkJaar = 2012};
            var vervallenFunctie = new Functie
                                       {
                                           WerkJaarTot = groepsWerkJaar.WerkJaar - 1,
                                           MinAantal = 1,
                                           Groep = groepsWerkJaar.Groep
                                       };
            // ACT

            var functiesManager = Factory.Maak<FunctiesManager>();
            var probleemIDs = functiesManager.AantallenControleren(groepsWerkJaar, new List<Functie> {vervallenFunctie}).Select(src => src.ID);

            // ASSERT

            Assert.IsFalse(probleemIDs.Contains(vervallenFunctie.ID));
        }

        /// <summary>
        /// Kijkt na of er een exception opgeworpen wordt als iemand zonder e-mailadres contactpersoon wil worden.
        /// </summary>
        [TestMethod]
        [ExpectedFoutNummer(typeof(FoutNummerException), FoutNummer.EMailVerplicht)]
        public void ContactZonderEmail()
        {
            // ARRANGE

            var groepsWerkJaar = new GroepsWerkJaar { Groep = new ChiroGroep(), WerkJaar = 2012 };
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var contactPersoonFunctie = new Functie
            {
                ID = (int)NationaleFunctie.ContactPersoon,
                MinAantal = 1,
                IsNationaal = true,
                Niveau = Niveau.LeidingInGroep,
            };
            var lid = new Leiding
            {
                GroepsWerkJaar = groepsWerkJaar,
                GelieerdePersoon = new GelieerdePersoon()
            };

            // ACT

            var functiesManager = Factory.Maak<FunctiesManager>();
            functiesManager.Toekennen(lid, new List<Functie> { contactPersoonFunctie });

            // ASSERT

            // Normaal gezien hebben we hier al lang een exception op ons dak.
            Assert.IsFalse(true);
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
		/// probeert een functie die zowel dit jaar als vorig jaar gebruikt is, 
		/// geforceerd te verwijderen.  We verwachten dat het 'werkJaar tot'  wordt
		/// ingevuld.
		/// </summary>
		[TestMethod]
		public void FunctieLangerInGebruikGeforceerdVerwijderenTest()
		{
            // ARRANGE

            // model
		    var groep = new ChiroGroep();

		    var vorigWerkJaar = new GroepsWerkJaar {WerkJaar = 2011, Groep = groep, ID = 2};
		    var ditWerkJaar = new GroepsWerkJaar {WerkJaar = 2012, Groep = groep, ID = 3};
            groep.GroepsWerkJaar.Add(vorigWerkJaar);
		    groep.GroepsWerkJaar.Add(ditWerkJaar);

		    var functie = new Functie {Groep = groep, ID = 1};
            groep.Functie.Add(functie);

		    var gelieerdePersoon = new GelieerdePersoon {Groep = groep};
            groep.GelieerdePersoon.Add(gelieerdePersoon);

		    var leidingToen = new Leiding {GelieerdePersoon = gelieerdePersoon, GroepsWerkJaar = vorigWerkJaar};
		    var leidingNu = new Leiding {GelieerdePersoon = gelieerdePersoon, GroepsWerkJaar = ditWerkJaar};
            vorigWerkJaar.Lid.Add(leidingToen);
            ditWerkJaar.Lid.Add(leidingNu);

            leidingToen.Functie.Add(functie);
            leidingNu.Functie.Add(functie);
            functie.Lid.Add(leidingToen);
            functie.Lid.Add(leidingNu);
            
            // ACT

            var mgr = Factory.Maak<FunctiesManager>();
            var result = mgr.Verwijderen(functie, true);

            // ASSERT

            // functie niet meer geldig
            Assert.IsTrue(groep.Functie.Contains(functie));
            Assert.AreEqual(result.WerkJaarTot, ditWerkJaar.WerkJaar - 1);

            // enkel het lid van dit werkJaar blijft over
            Assert.AreEqual(result.Lid.Count, 1);
		}

        /// <summary>
        /// Bekijkt AantallenControleren wel degelijk enkel de angeleverde functies?
        /// </summary>
        [TestMethod()]
        public void AantallenControlerenBeperkTest()
        {
            // ARRANGE

            var functie1 = new Functie {MaxAantal = 1};
            var functie2 = new Functie();

            var groepsWerkJaar = new GroepsWerkJaar();

            groepsWerkJaar.Lid.Add(new Leiding {Functie = new List<Functie> {functie1}});
            groepsWerkJaar.Lid.Add(new Leiding {Functie = new List<Functie> {functie1}});    // 2 personen met de functie

            // ACT

            var target = Factory.Maak<FunctiesManager>();
            var actual = target.AantallenControleren(groepsWerkJaar, new List<Functie>{functie2});
            // controleer enkel op functie2.

            // ASSERT

            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        /// Test op het controleren van maximum aantal leden met gegeven functie.
        ///</summary>
        [TestMethod()]
        public void AantallenControlerenBovengrensTest()
        {
            // ARRANGE

            var functie = new Functie { IsNationaal = true, MaxAantal = 1 };

            var groepsWerkJaar1 = new GroepsWerkJaar();
            var leiding1 = new Leiding {Functie = new List<Functie> {functie}};
            functie.Lid.Add(leiding1);
            groepsWerkJaar1.Lid.Add(leiding1);

            var groepsWerkJaar2 = new GroepsWerkJaar();
            var leiding2 = new Leiding { Functie = new List<Functie> { functie } };
            functie.Lid.Add(leiding2);
            groepsWerkJaar2.Lid.Add(leiding2);

            // ACT

            var target = Factory.Maak<FunctiesManager>();
            var actual = target.AantallenControleren(groepsWerkJaar1, new List<Functie> { functie });
            // controleer enkel op functie2.

            // ASSERT

            Assert.AreEqual(0, actual.Count);
        }
	}
}
