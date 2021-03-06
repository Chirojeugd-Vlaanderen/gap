/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Verfijnen gebruikersrechten Copyright 2015 Chirojeugd-Vlaanderen vzw
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
using System.ServiceModel;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.Test;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers;
using NUnit.Framework;
using Moq;
namespace Chiro.Gap.Services.Test
{

    /// <summary>
    /// Dit is een testclass voor Unit Tests van LedenServiceTest,
    /// to contain all LedenServiceTest Unit Tests
    /// </summary>
    [TestFixture]
    public class LedenServiceTest: ChiroTest
    {
        [SetUp]
        public void MyTestInitialize()
        {
            PermissionHelper.FixPermissions();
        }

        /// <summary>
        /// Kijkt na of opgehaalde functies goed gemapt worden.
        /// </summary>
        [Test]
        public void OphalenTest()
        {
            // Dit is eigenlijk niet zo'n interessante unit test.
            // Zou wel nuttig zijn als 'testscenario', met database, webinterface, en alles erin

            // Arrange 

            // model
            var contactPersoon = new Functie {ID = (int) NationaleFunctie.ContactPersoon};
            var mijnFunctie = new Functie {ID = 1234};

            var lid = new Leiding
                          {
                              ID = 1,
                              GelieerdePersoon = new GelieerdePersoon { Persoon = new Persoon(), Groep = new ChiroGroep() },
                              Functie =
                                  new List<Functie> { contactPersoon, mijnFunctie }
                          };

            // (mocking opzetten)
            var dummyRepositoryProvider = new Mock<IRepositoryProvider>();
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Lid>())
                                   .Returns(new DummyRepo<Lid>(new List<Lid> {lid}));
            Factory.InstantieRegistreren(dummyRepositoryProvider.Object);


            // Act

            var ledenService = Factory.Maak<LedenService>();
            var actual = ledenService.DetailsOphalen(lid.ID);

            // Assert
            var ids = (from f in actual.LidInfo.Functies select f.ID).ToList();
            Assert.IsTrue(ids.Contains((int) NationaleFunctie.ContactPersoon));
            Assert.IsTrue(ids.Contains(mijnFunctie.ID));
        }

        ///<summary>
        /// A test for FunctiesVervangen
        /// </summary>
        [Test]
        public void FunctiesVervangenTest()
        {
            #region arrange

            // Hier gebruiken we onze private container, want als we met de gedeelde container aan de autorisatie
            // foefelen, heeft dat effect op de andere tests.
            using (var container = ContainerCreate())
            {

                // testdata genereren

                var ik = new Persoon {AdNummer = 12345};
                var gwj = new GroepsWerkJaar();
                var groep = new ChiroGroep
                {
                    ID = 5,
                    GroepsWerkJaar = new List<GroepsWerkJaar> {gwj}
                };
                gwj.Groep = groep;

                var gebruikersrRecht = new GebruikersRechtV2
                {
                    Groep = groep,
                    Persoon = ik,
                    VervalDatum = DateTime.Now.AddDays(1),
                    IedereenPermissies = Permissies.Bewerken,
                    GroepsPermissies = Permissies.Lezen
                };
                ik.GebruikersRechtV2.Add(gebruikersrRecht);
                groep.GebruikersRechtV2.Add(gebruikersrRecht);

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
                    Functie = new List<Functie> {contactPersoon, redactie},
                    GelieerdePersoon = new GelieerdePersoon {Groep = groep, Persoon = new Persoon()}
                };

                groep.GelieerdePersoon.Add(leiding.GelieerdePersoon);
                groep.GelieerdePersoon.Add(new GelieerdePersoon {Groep = groep, Persoon = ik});

                // repositories maken die de testdata opleveren

                var ledenRepo = new DummyRepo<Lid>(new[] {leiding});
                var functieRepo = new DummyRepo<Functie>(new[] {contactPersoon, finVer, vb, redactie});

                // repositoryprovider opzetten
                var repoProviderMock = new Mock<IRepositoryProvider>();
                repoProviderMock.Setup(src => src.RepositoryGet<Lid>()).Returns(ledenRepo);
                repoProviderMock.Setup(src => src.RepositoryGet<Functie>()).Returns(functieRepo);
                container.InstantieRegistreren(repoProviderMock.Object);

                // om #1413 te detecteren, moeten we de echte authorisatiemanager
                // gebruiken. Dat wil dan weer zeggen dat we met de authenticatiemanager
                // moeten foefelen, en gebruikersrechten moeten simuleren.

                var authenticatieMgrMock = new Mock<IAuthenticatieManager>();
                authenticatieMgrMock.Setup(src => src.GebruikersNaamGet()).Returns("testGebruiker");
                authenticatieMgrMock.Setup(src => src.AdNummerGet()).Returns(12345);

                container.InstantieRegistreren<IAutorisatieManager>(new AutorisatieManager(authenticatieMgrMock.Object));

                // Hier maken we uiteindelijk de ledenservice

                var ledenService = container.Maak<LedenService>();

                #endregion

                #region act

                var leidingsFuncties = leiding.Functie;

                IEnumerable<int> functieIDs = new int[]
                {
                    finVer.ID,
                    vb.ID,
                    redactie.ID
                };

                ledenService.FunctiesVervangen(leiding.ID, functieIDs);

                #endregion

                #region Assert

                Assert.AreEqual(leiding.Functie.Count(), 3);
                Assert.IsTrue(leiding.Functie.Contains(finVer));
                Assert.IsTrue(leiding.Functie.Contains(vb));
                Assert.IsTrue(leiding.Functie.Contains(redactie));

                // om problemen te vermijden met entity framework, mag je bestaande collecties niet zomaar vervangen;
                // je moet entiteiten toevoegen aan/verwijderen uit bestaande collecties.
                Assert.AreEqual(leiding.Functie, leidingsFuncties);

                #endregion
            }
        }

        /// <summary>
        /// Test voor lid maken, met andere afdeling dan de  voorgestelde.
        /// </summary>
        [Test]
        public void TestLidMakenBuitenVoorstel()
        {
            #region Arrange

            var mijnAfdeling = new AfdelingsJaar
                                   {
                                       ID = 1,
                                       GeboorteJaarVan = 1996,
                                       GeboorteJaarTot = 1997,
                                       Geslacht = GeslachtsType.Gemengd
                                   };
            var jongereAfdeling = new AfdelingsJaar
                                      {
                                          ID = 2,
                                          GeboorteJaarVan = 1998,
                                          GeboorteJaarTot = 1999,
                                          Geslacht = GeslachtsType.Gemengd
                                      };

            var groepsWerkJaar = new GroepsWerkJaar
                                     {
                                         WerkJaar = 2013,
                                         AfdelingsJaar =
                                             new List<AfdelingsJaar>
                                                 {
                                                     mijnAfdeling,
                                                     jongereAfdeling
                                                 }
                                     };
            mijnAfdeling.GroepsWerkJaar = groepsWerkJaar;
            jongereAfdeling.GroepsWerkJaar = groepsWerkJaar;

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 3,
                Persoon = new Persoon
                {
                    GeboorteDatum = new DateTime(1997, 3, 8),
                    Geslacht = GeslachtsType.Vrouw
                },
                Groep = new ChiroGroep
                {
                    GroepsWerkJaar = new List<GroepsWerkJaar>
                    {
                        groepsWerkJaar
                    }
                },
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        }
                    }

            };
            groepsWerkJaar.Groep = gelieerdePersoon.Groep;

            // dependency injection voor data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<AfdelingsJaar>())
                                  .Returns(
                                      new DummyRepo<AfdelingsJaar>(new List<AfdelingsJaar>
                                                                       {
                                                                           mijnAfdeling,
                                                                           jongereAfdeling
                                                                       }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);
            #endregion

            #region act
            var ledenService = Factory.Maak<LedenService>();
            ledenService.Inschrijven(new [] {new InschrijvingsVerzoek
                                                 {
                                                     AfdelingsJaarIDs = new [] {jongereAfdeling.ID},
                                                     AfdelingsJaarIrrelevant = false,
                                                     GelieerdePersoonID = gelieerdePersoon.ID,
                                                     LeidingMaken = false,
                                                 }});
            #endregion

            #region Assert
            Assert.AreEqual(gelieerdePersoon.Lid.First().AfdelingsJaarIDs.First(), jongereAfdeling.ID);
            #endregion
        }

        /// <summary>
        /// Controleert of de return value van een inschrijvingsvoorstel de leden sorteert op 
        /// geboortedatum met Chiroleeftijd.  Dat is op dit moment belangrijk omdat de ledenlijst
        /// bij de jaarovergang anders door elkaar staat; we kunnen die aan de UI-kant nog niet
        /// helemaal sorteren.  Zie ticket #1391.  (Als er iets is aangepast waardoor deze test
        /// wel moet failen, moet ticket #1391 mogelijk opnieuw geopend worden.)
        /// </summary>
        [Test]
        public void VoorstelTotInschrijvenGenererenSorteringTest()
        {
            // ARRANGE
            // Gauw wat handgemaakte dummydata:

            var mijnAfdeling = new AfdelingsJaar
                                   {
                                       ID = 1,
                                       GeboorteJaarVan = 1996,
                                       GeboorteJaarTot = 1998,
                                       Geslacht = GeslachtsType.Gemengd,
                                       OfficieleAfdeling = new OfficieleAfdeling {ID = (int) NationaleAfdeling.Ketis}
                                   };

            var groepsWerkJaar = new GroepsWerkJaar
                                     {
                                         WerkJaar = 2013,
                                         AfdelingsJaar =
                                             new List<AfdelingsJaar>
                                                 {
                                                     mijnAfdeling,
                                                 }
                                     };

            // Afdelingsjaar aan groepswerkjaar koppelen, anders loopt validatie straks mis.
            mijnAfdeling.GroepsWerkJaar = groepsWerkJaar;

            var oudeGelieerdePersoon = new GelieerdePersoon
            {
                ID = 3,
                Persoon = new Persoon
                {
                    GeboorteDatum = new DateTime(1997, 3, 8),
                    Geslacht = GeslachtsType.Vrouw
                },
                Groep = new ChiroGroep
                {
                    GroepsWerkJaar = new List<GroepsWerkJaar>
                    {
                        groepsWerkJaar
                    }
                },
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        }
                    }

            };
            groepsWerkJaar.Groep = oudeGelieerdePersoon.Groep;
            var jongeGelieerdePersoon = new GelieerdePersoon
            {
                ID = 4,
                Persoon = new Persoon
                {
                    GeboorteDatum = new DateTime(1998, 3, 8),
                    Geslacht = GeslachtsType.Vrouw
                },
                Groep = groepsWerkJaar.Groep,
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        }
                    }
            };
         
            // We mocken een en ander:

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(
                                      new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon>
                                                                          {
                                                                              oudeGelieerdePersoon,
                                                                              jongeGelieerdePersoon
                                                                          }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<LedenService>();
            var actual = target.InschrijvingVoorstellen(new[]{oudeGelieerdePersoon.ID, jongeGelieerdePersoon.ID});

            // We verwachten nu dat de personen opgeleverd worden van jong naar oud.  Dus
            // eerst persoon 2, dan persoon 1.

            Assert.IsTrue(actual.First().GelieerdePersoonID == jongeGelieerdePersoon.ID);
            Assert.IsTrue(actual.Last().GelieerdePersoonID == oudeGelieerdePersoon.ID);
        }


        ///<summary>
        ///Controleert of het vervangen van functies gesynct wordt naar Kipadmin.
        ///</summary>
        [Test]
        public void FunctiesVervangenSyncTest()
        {
            #region arrange

            // testdata genereren

            var gwj = new GroepsWerkJaar();
            var groep = new ChiroGroep
            {
                GroepsWerkJaar = new List<GroepsWerkJaar> { gwj }
            };
            gwj.Groep = groep;

            // De functie hieronder speelt eigenlijk geen rol, maar kies niet 'contactpersoon', want dan
            // doet de backend lastig over het ontbreken van een e-mailadres.

            var financieelVerantwoordelijke = new Functie
            {
                ID = (int)NationaleFunctie.FinancieelVerantwoordelijke,
                IsNationaal = true,
                Niveau = Niveau.Alles,
                Naam = "Financieel Verantwoordelijke",
                Type = LidType.Leiding
            };

            var leiding = new Leiding
            {
                ID = 100,
                GroepsWerkJaar = gwj,
                Functie = new List<Functie>(),
                GelieerdePersoon = new GelieerdePersoon { Groep = groep }
            };

            // repositories maken die de testdata opleveren
            var ledenRepo = new DummyRepo<Lid>(new[] { leiding });
            var functieRepo = new DummyRepo<Functie>(new[] { financieelVerantwoordelijke });

            // repositoryprovider opzetten
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Lid>()).Returns(ledenRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<Functie>()).Returns(functieRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            // sync mocken.
            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(src => src.FunctiesUpdaten(leiding)).Verifiable();
            Factory.InstantieRegistreren(ledenSyncMock.Object);

            // Hier maken we uiteindelijk de ledenservice

            var ledenService = Factory.Maak<LedenService>();
            #endregion

            #region act
            ledenService.FunctiesVervangen(leiding.ID, new [] { financieelVerantwoordelijke.ID });
            #endregion

            #region Assert
            ledenSyncMock.Verify();
            #endregion
        }

        ///<summary>
        /// Controleert of de uitschrijving van kadermedewerkers wordt gesynct naar kipadmin.
        /// (Voor kadermedewerkers is er geen probeerperiode, want kaderinschrijvingen zijn gratis)
        ///</summary>
        [Test]
        public void UitschrijvenKaderSyncTest()
        {
            // arrange

            // testsituatie opbouwen
            var groepsWerkJaar = new GroepsWerkJaar {Groep = new KaderGroep {NiveauInt = (int) Niveau.Gewest}};
            groepsWerkJaar.Groep.GroepsWerkJaar = new List<GroepsWerkJaar> { groepsWerkJaar };

            var gelieerdePersoon = new GelieerdePersoon {ID = 1, Groep = groepsWerkJaar.Groep};
            groepsWerkJaar.Groep.GelieerdePersoon = new List<GelieerdePersoon> { gelieerdePersoon };          

            var medewerker = new Leiding
                                 {
                                     EindeInstapPeriode = DateTime.Today,
                                     // probeerperiode kadermedewerker is irrelevant
                                     GroepsWerkJaar = groepsWerkJaar,
                                     GelieerdePersoon = gelieerdePersoon
                                 };
            gelieerdePersoon.Lid = new List<Lid> {medewerker};
            


            // data access opzetten
            var dummyLeidingRepo = new DummyRepo<Leiding>(new List<Leiding>{medewerker});
            var dummyGpRepo = new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {medewerker.GelieerdePersoon});
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Leiding>()).Returns(dummyLeidingRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>()).Returns(dummyGpRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            // synchronisatie mocken
            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(snc => snc.Verwijderen(It.IsAny<Lid>())).Verifiable();   // verwacht dat ledensync een lid moet bewaren
            Factory.InstantieRegistreren(ledenSyncMock.Object);

            var target = Factory.Maak<LedenService>();

            // ACT

            string foutbericht;
            target.Uitschrijven(new[] {medewerker.GelieerdePersoon.ID}, out foutbericht);

            // ASSERT: controleer of de ledensync is aangeroepen

            ledenSyncMock.VerifyAll();
        }

        /// <summary>
        ///A test for Uitschrijven
        ///</summary>
        [Test]
        public void UitschrijvenTest()
        {
            // ARRANGE

            // testsituatie opbouwen
            var groepsWerkJaar = new GroepsWerkJaar { Groep = new KaderGroep { NiveauInt = (int)Niveau.Gewest } };
            groepsWerkJaar.Groep.GroepsWerkJaar = new List<GroepsWerkJaar> { groepsWerkJaar };

            var gelieerdePersoon1 = new GelieerdePersoon { ID = 1, Groep = groepsWerkJaar.Groep };
            var gelieerdePersoon2 = new GelieerdePersoon { ID = 2, Groep = groepsWerkJaar.Groep };
            groepsWerkJaar.Groep.GelieerdePersoon = new List<GelieerdePersoon> { gelieerdePersoon1, gelieerdePersoon2 };

            var medewerker1 = new Leiding
            {
                EindeInstapPeriode = DateTime.Today,
                // probeerperiode kadermedewerker is irrelevant
                GroepsWerkJaar = groepsWerkJaar,
                GelieerdePersoon = gelieerdePersoon1
            };
            var medewerker2 = new Leiding
            {
                EindeInstapPeriode = DateTime.Today,
                // probeerperiode kadermedewerker is irrelevant
                GroepsWerkJaar = groepsWerkJaar,
                GelieerdePersoon = gelieerdePersoon2
            };
            gelieerdePersoon1.Lid = new List<Lid> { medewerker1 };
            gelieerdePersoon2.Lid = new List<Lid> { medewerker2 };

            // data access opzetten
            var dummyLeidingRepo = new DummyRepo<Leiding>(new List<Leiding> { medewerker1, medewerker2 });
            var dummyGpRepo = new DummyRepo<GelieerdePersoon>(groepsWerkJaar.Groep.GelieerdePersoon.ToList());
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Leiding>()).Returns(dummyLeidingRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>()).Returns(dummyGpRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            var target = Factory.Maak<LedenService>();

            // ACT

            string foutBerichten = string.Empty; 
            target.Uitschrijven(new [] {gelieerdePersoon1.ID}, out foutBerichten);

            // ASSERT

            Assert.IsNotNull(medewerker1.UitschrijfDatum);  // medewerker 1 uitgeschreven
            Assert.IsNull(medewerker2.UitschrijfDatum);     // medewerker 2 niet uitgeschreven
        }

        ///<summary>
        /// Test of leiding waarvan de probeerperiode voor bij is, daadwerkelijk NIET gesynct
        /// wordt met Kipadmin.
        ///</summary>
        [Test]
        public void UitschrijvenLeidingSyncTest()
        {
            // arrange

            // testsituatie opbouwen
            var groepsWerkJaar = new GroepsWerkJaar { Groep = new ChiroGroep () };
            groepsWerkJaar.Groep.GroepsWerkJaar = new List<GroepsWerkJaar> { groepsWerkJaar };

            var gelieerdePersoon = new GelieerdePersoon { ID = 1, Groep = groepsWerkJaar.Groep };
            groepsWerkJaar.Groep.GelieerdePersoon = new List<GelieerdePersoon> { gelieerdePersoon };

            var leiding = new Leiding
            {
                EindeInstapPeriode = DateTime.Today, // (defaults naar 0:00 uur; instapperiode voorbij)
                GroepsWerkJaar = groepsWerkJaar,
                GelieerdePersoon = gelieerdePersoon
            };
            gelieerdePersoon.Lid = new List<Lid> { leiding };



            // data access opzetten
            var dummyLeidingRepo = new DummyRepo<Leiding>(new List<Leiding> { leiding });
            var dummyGpRepo = new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { leiding.GelieerdePersoon });
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Leiding>()).Returns(dummyLeidingRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>()).Returns(dummyGpRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            // synchronisatie mocken
            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(snc => snc.Verwijderen(It.IsAny<Lid>())).Verifiable();   // verwacht dat ledensync geen lid moet verwijderen...
            ledenSyncMock.Setup(snc => snc.Bewaren(It.Is<Lid>(ld => ld.UitschrijfDatum != null))).Verifiable();       // maar wel bewaren.
            Factory.InstantieRegistreren(ledenSyncMock.Object);

            var target = Factory.Maak<LedenService>();

            // ACT

            string foutbericht;
            target.Uitschrijven(new[] { leiding.GelieerdePersoon.ID }, out foutbericht);

            // ASSERT: controleer dat ledensync.Verwijderen NIET werd aangeroepen,
            // maar ledensync.Bewaren wel.

            ledenSyncMock.Verify(src=>src.Verwijderen(leiding), Times.Never());
            ledenSyncMock.Verify(src => src.Bewaren(leiding), Times.AtLeastOnce);
        }

        ///<summary>
        ///Controleert of zoeken op leden geen uitgeschreven leden oplevert.
        ///</summary>
        [Test]
        public void ZoekenIngeschrevenTest()
        {
            // ARRANGE

            // testdata: een werkjaar met 1 lid, dat vandaag werd uitgeschreven.
            var kind = new Kind
                           {
                               UitschrijfDatum = DateTime.Today,
                               GelieerdePersoon = new GelieerdePersoon {Persoon = new Persoon()},
                               AfdelingsJaar = new AfdelingsJaar {Afdeling = new Afdeling()}
                           };
            var gwj = new GroepsWerkJaar {ID = 1, Lid = new[] {kind}};
            kind.GroepsWerkJaar = gwj;

            // dependency injection opzetten
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>()).Returns(new DummyRepo<Lid>(gwj.Lid.ToList()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>())
                                  .Returns(new DummyRepo<GroepsWerkJaar>(new List<GroepsWerkJaar> {gwj}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Kind>())
                                  .Returns(new DummyRepo<Kind>(new List<Kind> {kind}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Leiding>())
                                  .Returns(new DummyRepo<Leiding>(new List<Leiding> ()));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // service construeren
            var target = Factory.Maak<LedenService>();

            // ACT

            var filter = new LidFilter { GroepsWerkJaarID = gwj.ID };
            var actual = target.LijstZoekenLidOverzicht(filter, false);

            // ASSERT

            Assert.IsNull(actual.FirstOrDefault());
        }

        /// <summary>
        ///Controleert of een uitgeschreven lid opnieuw ingeschreven kan worden.
        ///</summary>
        [Test]
        public void HerinschrijvenTest()
        {
            // ARRANGE

            // testdata
            var groepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep()};
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var leider = new Leiding {UitschrijfDatum = DateTime.Today, GroepsWerkJaar = groepsWerkJaar};
            var gp = new GelieerdePersoon  // gelieerde persoon, uitgeschreven als leiding
                         {
                             ID = 1,
                             Persoon = new Persoon {Naam = "Bosmans", VoorNaam = "Jos"},
                             Lid = new List<Lid> {leider},
                             Groep = groepsWerkJaar.Groep
                         };

            // dependency injection
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gp}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<AfdelingsJaar>())
                                  .Returns(new DummyRepo<AfdelingsJaar>(new List<AfdelingsJaar>()));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            var target = Factory.Maak<LedenService>();

            // ACT
            var lidInformatie = new[]
                                    {
                                        new InschrijvingsVerzoek
                                            {
                                                AfdelingsJaarIrrelevant = true,
                                                GelieerdePersoonID = gp.ID,
                                                LeidingMaken = true,
                                            }
                                    };
            target.Inschrijven(lidInformatie);

            // ASSERT
            Assert.IsNull(leider.UitschrijfDatum);
        }

        /// <summary>
        /// Controleert of TypeToggle een kind omzet naar een leid(st)er
        /// </summary>
        [Test]
        public void TypeToggleTest()
        {
            // ARRANGE
            // We maken een eenvoudig model.

            var groep = new ChiroGroep
            {
                GroepsWerkJaar = new List<GroepsWerkJaar>(),
                Afdeling = new List<Afdeling>(),
                GelieerdePersoon = new List<GelieerdePersoon>(),
            };

            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = groep,
                Lid = new List<Lid>(),
                AfdelingsJaar = new List<AfdelingsJaar>(),
                WerkJaar = 2012
            };
            groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var afdeling = new Afdeling { Naam = "Dingskes", ChiroGroep = groep };
            groep.Afdeling.Add(afdeling);

            var afdelingsJaar = new AfdelingsJaar
            {
                Afdeling = afdeling,
                GeboorteJaarVan = 1995,
                GeboorteJaarTot = 1996
            };
            groepsWerkJaar.AfdelingsJaar.Add(afdelingsJaar);

            var gelieerdePersoon = new GelieerdePersoon
            {
                Groep = groep,
                ID = 2,
                Persoon =
                    new Persoon
                    {
                        Geslacht = GeslachtsType.Vrouw,
                        GeboorteDatum = new DateTime(1996, 7, 3)
                    },
                ChiroLeefTijd = 0,
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        },
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.Email}
                        }
                    }
            };
            groep.GelieerdePersoon.Add(gelieerdePersoon);

            var lid = new Kind { ID = 1, AfdelingsJaar = afdelingsJaar, GelieerdePersoon = gelieerdePersoon, GroepsWerkJaar = groepsWerkJaar };
            groepsWerkJaar.Lid.Add(lid);

            // inversion of control

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(groepsWerkJaar.Lid.ToList()));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<LedenService>();
            target.TypeToggle(lid.ID);

            // ASSERT

            var gevondenLeden = (from l in groepsWerkJaar.Lid
                                 where l.GelieerdePersoon.ID == gelieerdePersoon.ID
                                 select l).ToList();

            Assert.AreEqual(gevondenLeden.Count, 1);
            Assert.IsInstanceOf<Leiding>(gevondenLeden.First());
        }

        /// <summary>
        /// Als een functie van toepassing is op leden en leiding,
        /// mag 'typetoggle' die behouden. Anders niet.
        ///</summary>
        [Test]
        public void TypeToggleTestFunctieBewaren()
        {
            // ARRANGE
            // We maken een eenvoudig model.

            var groep = new ChiroGroep
            {
                GroepsWerkJaar = new List<GroepsWerkJaar>(),
                Afdeling = new List<Afdeling>(),
                GelieerdePersoon = new List<GelieerdePersoon>(),
            };

            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = groep,
                Lid = new List<Lid>(),
                AfdelingsJaar = new List<AfdelingsJaar>(),
                WerkJaar = 2012
            };
            groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var afdeling = new Afdeling { Naam = "Dingskes", ChiroGroep = groep };
            groep.Afdeling.Add(afdeling);

            var afdelingsJaar = new AfdelingsJaar
            {
                Afdeling = afdeling,
                GeboorteJaarVan = 1995,
                GeboorteJaarTot = 1996
            };
            groepsWerkJaar.AfdelingsJaar.Add(afdelingsJaar);

            var gelieerdePersoon = new GelieerdePersoon
            {
                Groep = groep,
                ID = 2,
                Persoon =
                    new Persoon
                    {
                        Geslacht = GeslachtsType.Vrouw,
                        GeboorteDatum = new DateTime(1996, 7, 3)
                    },
                ChiroLeefTijd = 0,
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        },
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.Email}
                        }
                    }
            };
            groep.GelieerdePersoon.Add(gelieerdePersoon);

            var lid = new Kind { ID = 1, AfdelingsJaar = afdelingsJaar, GelieerdePersoon = gelieerdePersoon, GroepsWerkJaar = groepsWerkJaar };
            groepsWerkJaar.Lid.Add(lid);

            var algemeneFunctie = new Functie {Niveau = Niveau.Groep, ID = 3};    // functie voor leden EN leiding
            var kindFunctie = new Functie {Niveau = Niveau.LidInGroep, ID = 4};      // functie enkel voor (kind)leden

            lid.Functie.Add(algemeneFunctie);
            lid.Functie.Add(kindFunctie);

            // inversion of control

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(groepsWerkJaar.Lid.ToList()));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<LedenService>();
            target.TypeToggle(lid.ID);

            // ASSERT

            var gevondenLeden = (from l in groepsWerkJaar.Lid
                                 where l.GelieerdePersoon.ID == gelieerdePersoon.ID
                                 select l).ToList();

            Assert.AreEqual(gevondenLeden.Count, 1);

            var nieuwLid = gevondenLeden.First();

            Assert.AreEqual(nieuwLid.Functie.Count,1);
            Assert.AreEqual(nieuwLid.Functie.First().ID, algemeneFunctie.ID);
        }

        /// <summary>
        ///Flauwe test voor AfdelingenVervangenBulk
        ///</summary>
        [Test]
        public void AfdelingenVervangenBulkTest()
        {
            // ARRANGE

            // We stellen een model op:
            // iemand die leiding is in afdelingsjaar1
            var groep = new ChiroGroep();
            var groepsWerkJaar = new GroepsWerkJaar {Groep = groep};

            var afdelingsJaar1 = new AfdelingsJaar {ID = 1, GroepsWerkJaar = groepsWerkJaar};
            var afdelingsJaar2 = new AfdelingsJaar {ID = 2, GroepsWerkJaar = groepsWerkJaar};
            groepsWerkJaar.AfdelingsJaar.Add(afdelingsJaar1);
            groepsWerkJaar.AfdelingsJaar.Add(afdelingsJaar2);

            var leiding = new Leiding
                              {
                                  ID = 3,
                                  GelieerdePersoon = new GelieerdePersoon {Groep = groep},
                                  AfdelingsJaar = new List<AfdelingsJaar> {afdelingsJaar1},
                                  GroepsWerkJaar = groepsWerkJaar
                              };
            groepsWerkJaar.Lid.Add(leiding);
            afdelingsJaar1.Leiding.Add(leiding);

            // Dependency injection voor data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<AfdelingsJaar>())
                                  .Returns(
                                      new DummyRepo<AfdelingsJaar>(new List<AfdelingsJaar>
                                                                       {
                                                                           afdelingsJaar1,
                                                                           afdelingsJaar2
                                                                       }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> {leiding}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<LedenService>();
            target.AfdelingenVervangenBulk(new [] {leiding.ID}, new [] {afdelingsJaar2.ID});

            // ASSERT
            Assert.IsFalse(afdelingsJaar1.Leiding.Contains(leiding));
            Assert.IsTrue(afdelingsJaar2.Leiding.Contains(leiding));
        }

        /// <summary>
        ///Een test voor het zoeken naar leden zonder adressen
        ///</summary>
        [Test]
        public void ZoekenLedenZonderAdressenTest()
        {
            // ARRANGE

            // testmodelletje
            var lid = new Leiding
                          {
                              ID = 1,
                              GelieerdePersoon = new GelieerdePersoon
                                                     {
                                                         PersoonsAdres = new PersoonsAdres {Adres = new BelgischAdres()},
                                                         Persoon = new Persoon()
                                                     }
                          };

            // dependency injection
            
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> {lid}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>())
                      .Returns(new DummyRepo<GroepsWerkJaar>(new List<GroepsWerkJaar>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Kind>())
                                  .Returns(new DummyRepo<Kind>(new List<Kind> ()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Leiding>())
                                  .Returns(new DummyRepo<Leiding>(new List<Leiding> {lid}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<LedenService>();
            var actual = target.LijstZoekenLidOverzicht(new LidFilter {HeeftVoorkeurAdres = false}, false);

            // ASSERT
            Assert.IsFalse(actual.Select(ld => ld.LidID == lid.ID).Any());
            // alle leden hebben een voorkeursadres. Ik verwacht er dus geen te vinden zonder.

        }

        ///<summary>
        /// Kijkt na of leden zonder telefoonnummer maar met e-mail toch gevonden worden als leden zonder
        /// telefoonnummer
        ///</summary>
        [Test]
        public void ZoekenZonderTelefoonNr()
        {
            // ARRANGE

            // testmodelletje
            var lid = new Leiding
                          {
                              ID = 1,
                              GelieerdePersoon = new GelieerdePersoon
                                                     {
                                                         Persoon = new Persoon(),
                                                         Communicatie =
                                                             new List<CommunicatieVorm>
                                                                 {
                                                                     new CommunicatieVorm
                                                                         {
                                                                             CommunicatieType
                                                                                 =
                                                                                 new CommunicatieType
                                                                                     {
                                                                                         ID
                                                                                             =
                                                                                             (
                                                                                             int
                                                                                             )
                                                                                             CommunicatieTypeEnum
                                                                                                 .Email
                                                                                     }
                                                                         }
                                                                 }
                                                     }
                          };

            // dependency injection

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> { lid }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>())
                                  .Returns(new DummyRepo<GroepsWerkJaar>(new List<GroepsWerkJaar>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Kind>())
                                  .Returns(new DummyRepo<Kind>(new List<Kind>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Leiding>())
                                  .Returns(new DummyRepo<Leiding>(new List<Leiding> { lid }));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<LedenService>();
            var actual = target.LijstZoekenLidOverzicht(new LidFilter { HeeftTelefoonNummer = false }, false);

            // ASSERT
            Assert.IsTrue(actual.Select(ld => ld.LidID == lid.ID).Any());
            // het lid heeft geen telefoonnummer, dus we verwachten een fout te vinden.
        }

        ///<summary>
        /// Kijkt na of leden zonder e-mail maar mettelefoonnummer toch gevonden worden als leden zonder
        /// telefoonnummer
        ///</summary>
        [Test]
        public void ZoekenZonderEmail()
        {
            // ARRANGE

            // testmodelletje
            var lid = new Leiding
            {
                ID = 1,
                GelieerdePersoon = new GelieerdePersoon
                {
                    Persoon = new Persoon(),
                    Communicatie =
                        new List<CommunicatieVorm>
                                                                 {
                                                                     new CommunicatieVorm
                                                                         {
                                                                             CommunicatieType
                                                                                 =
                                                                                 new CommunicatieType
                                                                                     {
                                                                                         ID
                                                                                             =
                                                                                             (
                                                                                             int
                                                                                             )
                                                                                             CommunicatieTypeEnum
                                                                                                 .TelefoonNummer
                                                                                     }
                                                                         }
                                                                 }
                }
            };

            // dependency injection

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> { lid }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>())
                      .Returns(new DummyRepo<GroepsWerkJaar>(new List<GroepsWerkJaar>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Kind>())
                                  .Returns(new DummyRepo<Kind>(new List<Kind>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Leiding>())
                                  .Returns(new DummyRepo<Leiding>(new List<Leiding> { lid }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<LedenService>();
            var actual = target.LijstZoekenLidOverzicht(new LidFilter { HeeftEmailAdres = false }, false);

            // ASSERT
            Assert.IsTrue(actual.Select(ld => ld.LidID == lid.ID).Any());
            // het lid heeft geen telefoonnummer, dus we verwachten een fout te vinden.
        }

        /// <summary>
        /// Kijkt na of ingeschreven personen gesynct worden naar kipadmin.
        /// </summary>
        [Test]
        public void InschrijvenSyncTest()
        {
            // ARRANGE

            // model

            var groepsWerkJaar = new GroepsWerkJaar
                                     {
                                         WerkJaar = 2013
                                     };
            var groep = new KaderGroep()
                            {
                                GroepsWerkJaar =
                                    new List<GroepsWerkJaar>
                                        {
                                            groepsWerkJaar
                                        }
                            };
            groepsWerkJaar.Groep = groep;

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Groep = groep,
                Persoon =
                    new Persoon
                    {
                        Geslacht = GeslachtsType.Vrouw,
                        GeboorteDatum = new DateTime(1980, 8, 8)
                    },
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        },
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.Email}
                        }
                    }
            };

            var verzoek = new InschrijvingsVerzoek
                                 {
                                     AfdelingsJaarIrrelevant = true,
                                     GelieerdePersoonID = gelieerdePersoon.ID,
                                     LeidingMaken = true,
                                 };

            // dependency injection

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<AfdelingsJaar>())
                                              .Returns(new DummyRepo<AfdelingsJaar>(new List<AfdelingsJaar>()));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(snc => snc.Bewaren(It.IsAny<IList<Lid>>())).Verifiable();   // verwacht dat ledensync een lid moet bewaren
            Factory.InstantieRegistreren(ledenSyncMock.Object);

            // ACT
            var ledenService = Factory.Maak<LedenService>();
            ledenService.Inschrijven(new[] {verzoek});

            var lid = groepsWerkJaar.Lid.First();

            // ASSERT

            ledenSyncMock.Verify(src => src.Bewaren(It.IsAny<IList<Lid>>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Kijkt na of na het inschrijven van een persoon, het AD-nummer gemarkeerd wordt
        /// als zijnde 'in aanvraag'.
        /// </summary>
        [Test]
        public void InschrijvenAdNrAanvraagTest()
        {
            // ARRANGE

            // model

            var groepsWerkJaar = new GroepsWerkJaar
            {
                WerkJaar = 2013
            };
            var groep = new KaderGroep()
            {
                GroepsWerkJaar =
                    new List<GroepsWerkJaar>
                                        {
                                            groepsWerkJaar
                                        }
            };
            groepsWerkJaar.Groep = groep;

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Groep = groep,
                Persoon =
                    new Persoon
                    {
                        Geslacht = GeslachtsType.Vrouw,
                        GeboorteDatum = new DateTime(1980, 8, 8)
                    },
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        },
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.Email}
                        }
                    }
            };

            var verzoek = new InschrijvingsVerzoek
            {
                AfdelingsJaarIrrelevant = true,
                GelieerdePersoonID = gelieerdePersoon.ID,
                LeidingMaken = true,
            };

            // dependency injection
            var ledenSyncMock = new Mock<ILedenSync>();
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoon }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<AfdelingsJaar>())
                                  .Returns(new DummyRepo<AfdelingsJaar>(new List<AfdelingsJaar>()));

            Factory.InstantieRegistreren(ledenSyncMock.Object);
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var ledenService = Factory.Maak<LedenService>();
            ledenService.Inschrijven(new[] { verzoek });

            // ASSERT

            Assert.IsTrue(gelieerdePersoon.Persoon.InSync);
        }


        /// <summary>
        /// Controleert of AfdelingenVervangenBulk wel synchroniseert met Kipadmin
        /// </summary>
        [Test]
        public void AfdelingenVervangenBulkTestSync()
        {
            // ARRANGE

            var oudAfdelingsJaar = new AfdelingsJaar {ID = 1};
            var nieuwAfdelingsJaar = new AfdelingsJaar {ID = 2};
            var groepsWerkJaar = new GroepsWerkJaar
                                     {
                                         AfdelingsJaar =
                                             new List<AfdelingsJaar>
                                                 {
                                                     oudAfdelingsJaar,
                                                     nieuwAfdelingsJaar
                                                 },
                                         Groep = new ChiroGroep()
                                     };
            oudAfdelingsJaar.GroepsWerkJaar = groepsWerkJaar;
            nieuwAfdelingsJaar.GroepsWerkJaar = groepsWerkJaar;

            var lid = new Kind {ID = 3, AfdelingsJaar = oudAfdelingsJaar, GroepsWerkJaar = groepsWerkJaar};

            // Dependency injection sync

            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(src => src.AfdelingenUpdaten(It.IsAny<Lid>())).Verifiable();
            Factory.InstantieRegistreren(ledenSyncMock.Object);
            
            // Dependency injection data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> {lid}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<LedenService>();
            target.AfdelingenVervangenBulk(new[] {lid.ID}, new[] {nieuwAfdelingsJaar.ID});

            // ASSERT

            ledenSyncMock.Verify(src=>src.AfdelingenUpdaten(It.IsAny<Lid>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Kijkt na of LoonVerliesVerzekeren synct met Kipadmin
        /// </summary>
        [Test]
        public void LoonVerliesVerzekerenSyncTest()
        {
            // ARRANGE

            var lid = new Leiding
                          {
                              ID = 1,
                              GroepsWerkJaar =
                                  new GroepsWerkJaar {WerkJaar = DateTime.Now.Year, Groep = new ChiroGroep()},
                              GelieerdePersoon = new GelieerdePersoon {Persoon = new Persoon()}
                          };

            var verzekering = new VerzekeringsType {ID = (int) Verzekering.LoonVerlies};

            // mock voor synchronisatie registreren

            var verzekeringenSyncMock = new Mock<IVerzekeringenSync>();
            verzekeringenSyncMock.Setup(src => src.Bewaren(It.IsAny<PersoonsVerzekering>(), It.IsAny<GroepsWerkJaar>())).Verifiable();
            Factory.InstantieRegistreren(verzekeringenSyncMock.Object);

            // mock voor data-access registreren

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> {lid}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<VerzekeringsType>())
                                  .Returns(new DummyRepo<VerzekeringsType>(new List<VerzekeringsType> {verzekering}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<LedenService>();
            target.LoonVerliesVerzekeren(lid.ID);

            // ASSERT

            verzekeringenSyncMock.Verify(src => src.Bewaren(It.IsAny<PersoonsVerzekering>(), It.IsAny<GroepsWerkJaar>()), Times.Once());
        }

        /// <summary>
        /// Controleert of het togglen kind/leiding synct naar Kipadmin
        /// </summary>
        [Test]
        public void TypeToggleSyncTest()
        {
            // ARRANGE
            // We maken een eenvoudig model.

            var groep = new ChiroGroep
            {
                GroepsWerkJaar = new List<GroepsWerkJaar>(),
                Afdeling = new List<Afdeling>(),
                GelieerdePersoon = new List<GelieerdePersoon>(),
            };

            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = groep,
                Lid = new List<Lid>(),
                AfdelingsJaar = new List<AfdelingsJaar>(),
                WerkJaar = 2012
            };
            groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var afdeling = new Afdeling { Naam = "Dingskes", ChiroGroep = groep };
            groep.Afdeling.Add(afdeling);

            var afdelingsJaar = new AfdelingsJaar
            {
                Afdeling = afdeling,
                GeboorteJaarVan = 1995,
                GeboorteJaarTot = 1996
            };
            groepsWerkJaar.AfdelingsJaar.Add(afdelingsJaar);

            var gelieerdePersoon = new GelieerdePersoon
            {
                Groep = groep,
                ID = 2,
                Persoon =
                    new Persoon
                    {
                        Geslacht = GeslachtsType.Vrouw,
                        GeboorteDatum = new DateTime(1996, 7, 3)
                    },
                ChiroLeefTijd = 0,
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        },
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.Email}
                        }
                    }
            };
            groep.GelieerdePersoon.Add(gelieerdePersoon);

            var lid = new Kind { ID = 1, AfdelingsJaar = afdelingsJaar, GelieerdePersoon = gelieerdePersoon, GroepsWerkJaar = groepsWerkJaar };
            groepsWerkJaar.Lid.Add(lid);

            // mocking van de ledensync

            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(src => src.TypeUpdaten(It.IsAny<Lid>())).Verifiable();
            ledenSyncMock.Setup(src => src.AfdelingenUpdaten(It.IsAny<Lid>())).Verifiable();
            Factory.InstantieRegistreren(ledenSyncMock.Object);

            // mocking van de data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(groepsWerkJaar.Lid.ToList()));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<LedenService>();
            target.TypeToggle(lid.ID);

            // ASSERT

            ledenSyncMock.Verify(src => src.TypeUpdaten(It.IsAny<Lid>()), Times.AtLeastOnce());
            ledenSyncMock.Verify(src => src.AfdelingenUpdaten(It.IsAny<Lid>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Kijkt na of leden zoeken rekening houdt met gebruikersrechten
        /// </summary>
        [Test]
        public void ZoekenAutorisatieGroepTest()
        {
            using (var container = ContainerCreate())
            {
                // ARRANGE

                var groep = new ChiroGroep {ID = 1};

                // we gaan de 'gewone' autorisatiemanager gebruiken voor autorisatie. Dan
                // hebben we meer controle over waar we toegang toe hebben, en waar niet.

                var repositoryProviderMock = new Mock<IRepositoryProvider>();
                repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                    .Returns(new DummyRepo<Groep>(new List<Groep> {groep}));
                repositoryProviderMock.Setup(src => src.RepositoryGet<Kind>())
                    .Returns(new DummyRepo<Kind>(new List<Kind>()));
                repositoryProviderMock.Setup(src => src.RepositoryGet<Leiding>())
                    .Returns(new DummyRepo<Leiding>(new List<Leiding>()));


                container.InstantieRegistreren(repositoryProviderMock.Object);
                var auMgr = container.Maak<AutorisatieManager>();
                container.InstantieRegistreren<IAutorisatieManager>(auMgr);
                var target = container.Maak<LedenService>();

                // ASSERT
                Assert.Throws<FaultException<FoutNummerFault>>(
                    () => target.LijstZoekenLidOverzicht(new LidFilter {GroepID = groep.ID}, false));

            }
        }

        /// <summary>
        /// Kijkt na of leden zoeken rekening houdt met gebruikersrechten
        /// </summary>
        [Test]
        public void ZoekenAutorisatieGroepsWerkjaarTest()
        {
            using (var container = ContainerCreate())
            {
                // ARRANGE

                var groepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep {ID = 1}};

                // we gaan de 'gewone' autorisatiemanager gebruiken voor autorisatie. Dan
                // hebben we meer controle over waar we toegang toe hebben, en waar niet.

                var repositoryProviderMock = new Mock<IRepositoryProvider>();
                repositoryProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>())
                    .Returns(new DummyRepo<GroepsWerkJaar>(new List<GroepsWerkJaar> {groepsWerkJaar}));
                repositoryProviderMock.Setup(src => src.RepositoryGet<Kind>())
                    .Returns(new DummyRepo<Kind>(new List<Kind>()));
                repositoryProviderMock.Setup(src => src.RepositoryGet<Leiding>())
                    .Returns(new DummyRepo<Leiding>(new List<Leiding>()));


                container.InstantieRegistreren(repositoryProviderMock.Object);
                var auMgr = container.Maak<AutorisatieManager>();
                container.InstantieRegistreren<IAutorisatieManager>(auMgr);
                var target = container.Maak<LedenService>();

                // ASSERT

                Assert.Throws<FaultException<FoutNummerFault>>(
                    () => target.LijstZoekenLidOverzicht(new LidFilter {GroepsWerkJaarID = groepsWerkJaar.ID}, false));
            }
        }

        /// <summary>
        /// Probeer iemand die uitgeschreven is als leiding terug in te schrijven als lid
        /// </summary>
        [Test]
        public void LeidingHerinschrijvenAlsLidTest()
        {
            // ARRANGE

            // testdata
            var groepsWerkJaar = new GroepsWerkJaar { Groep = new ChiroGroep(), ID = 4, WerkJaar = 2012 };
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var afdelingsJaar1 = new AfdelingsJaar {ID = 2, GroepsWerkJaar = groepsWerkJaar, OfficieleAfdeling = new OfficieleAfdeling()};
            var afdelingsJaar2 = new AfdelingsJaar {ID = 3, GroepsWerkJaar = groepsWerkJaar, OfficieleAfdeling = new OfficieleAfdeling()};
            groepsWerkJaar.AfdelingsJaar.Add(afdelingsJaar1);
            groepsWerkJaar.AfdelingsJaar.Add(afdelingsJaar2);

            Lid lid = new Leiding
                          {
                              UitschrijfDatum = DateTime.Today,
                              GroepsWerkJaar = groepsWerkJaar,
                              AfdelingsJaar = new List<AfdelingsJaar> {afdelingsJaar1}
                          };
            groepsWerkJaar.Lid.Add(lid);

            var gp = new GelieerdePersoon // gelieerde persoon, uitgeschreven als leiding
            {
                ID = 1,
                Persoon =
                    new Persoon
                    {
                        Naam = "Bosmans",
                        VoorNaam = "Jos",
                        Geslacht = GeslachtsType.Man,
                        GeboorteDatum = new DateTime(1997, 3, 8)
                    },
                Lid = new List<Lid> {lid},
                Groep = groepsWerkJaar.Groep,
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        }
                    }
            };
            
            lid.GelieerdePersoon = gp;

            // dependency injection
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gp }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> {lid}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<AfdelingsJaar>())
                                  .Returns(new DummyRepo<AfdelingsJaar>(new List<AfdelingsJaar>{afdelingsJaar1, afdelingsJaar2}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            var target = Factory.Maak<LedenService>();

            // ACT
            var lidInformatie = new[]
                                    {
                                        new InschrijvingsVerzoek
                                            {
                                                AfdelingsJaarIrrelevant = false,
                                                GelieerdePersoonID = gp.ID,
                                                LeidingMaken = false,
                                                AfdelingsJaarIDs = new [] {afdelingsJaar2.ID}
                                            }
                                    };
            target.Inschrijven(lidInformatie);

            // ASSERT

            var leden = (from l in groepsWerkJaar.Lid
                         where l.GelieerdePersoon.ID == gp.ID
                         select l).ToList();
            Assert.IsTrue(leden.Count() == 1);

            var nieuwLid = leden.FirstOrDefault() as Kind;
            Assert.IsNotNull(nieuwLid);

            Assert.AreEqual(nieuwLid.AfdelingsJaar,  afdelingsJaar2);
        }

        /// <summary>
        /// Test op inschrijving voorstellen voor leiding (illustreert #1593)
        /// </summary>
        [Test]
        public void VoorstelTotInschrijvenGenererenLeidingTest()
        {
            // ARRANGE
            // Gauw wat handgemaakte dummydata:

            var mijnAfdeling = new AfdelingsJaar
            {
                ID = 1,
                GeboorteJaarVan = 1996,
                GeboorteJaarTot = 1998,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling { ID = (int)NationaleAfdeling.Ketis }
            };

            var groepsWerkJaar = new GroepsWerkJaar
            {
                WerkJaar = 2013,
                AfdelingsJaar =
                    new List<AfdelingsJaar>
                                                 {
                                                     mijnAfdeling,
                                                 }
            };

            // gelieerde persoon te oud voor een afdeling
            var oudeGelieerdePersoon = new GelieerdePersoon
            {
                ID = 3,
                Persoon = new Persoon
                {
                    GeboorteDatum = new DateTime(1992, 3, 8),
                    Geslacht = GeslachtsType.Vrouw
                },
                Groep = new ChiroGroep
                {
                    GroepsWerkJaar = new List<GroepsWerkJaar>
                    {
                        groepsWerkJaar
                    }
                },
                PersoonsAdres = new PersoonsAdres()
            };
            groepsWerkJaar.Groep = oudeGelieerdePersoon.Groep;

            // We mocken een en ander:

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(
                                      new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon>
                                                                          {
                                                                              oudeGelieerdePersoon,
                                                                          }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<LedenService>();
            var actual = target.InschrijvingVoorstellen(new[] { oudeGelieerdePersoon.ID });

            // We verwachten nu dat de personen opgeleverd worden van jong naar oud.  Dus
            // eerst persoon 2, dan persoon 1.

            Assert.IsTrue(actual.First().LeidingMaken);
        }

        /// <summary>
        /// Schrijf iemand uit als leiding, en opnieuw in als lid.
        /// Verwacht dat een lid gesynct wordt, en geen leiding. 
        /// (Bug nog te rapporteren. Stash voor fix staat klaar)
        ///</summary>
        [Test]
        public void InschrijvenOudLeidingAlsLidSyncTest()
        {
            // ARRANGE
            // We maken een eenvoudig model.

            var groep = new ChiroGroep
            {
                GroepsWerkJaar = new List<GroepsWerkJaar>(),
                Afdeling = new List<Afdeling>(),
                GelieerdePersoon = new List<GelieerdePersoon>(),
            };

            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = groep,
                Lid = new List<Lid>(),
                AfdelingsJaar = new List<AfdelingsJaar>(),
                WerkJaar = 2012
            };
            groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var afdeling = new Afdeling {Naam = "Dingskes", ChiroGroep = groep};
            groep.Afdeling.Add(afdeling);

            var afdelingsJaar = new AfdelingsJaar
                                    {
                                        Afdeling = afdeling,
                                        GeboorteJaarVan = 1995,
                                        GeboorteJaarTot = 1996,
                                        ID = 3,
                                        GroepsWerkJaar = groepsWerkJaar,
                                        OfficieleAfdeling = new OfficieleAfdeling()
                                    };


            groepsWerkJaar.AfdelingsJaar.Add(afdelingsJaar);

            var gelieerdePersoon = new GelieerdePersoon
            {
                Groep = groep,
                ID = 2,
                Persoon =
                    new Persoon
                    {
                        Geslacht = GeslachtsType.Vrouw,
                        GeboorteDatum = new DateTime(1996, 7, 3)
                    },
                ChiroLeefTijd = 0,
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        }
                    }
            };
            groep.GelieerdePersoon.Add(gelieerdePersoon);

            var lid = new Leiding
                          {
                              ID = 1,
                              GelieerdePersoon = gelieerdePersoon,
                              GroepsWerkJaar = groepsWerkJaar,
                              UitschrijfDatum = DateTime.Now.AddDays(-1) // uitgeschreven
                          };
            gelieerdePersoon.Lid.Add(lid);
            groepsWerkJaar.Lid.Add(lid);

            // mocking van de ledensync

            // als bewaren(List) wordt aangeroepen, registreren we of List een Kind bevat of een Leid(st)er.
            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(src => src.Bewaren(It.Is<IList<Lid>>(lst => lst.Any(el => el is Kind)))).Verifiable();
            ledenSyncMock.Setup(src => src.Bewaren(It.Is<IList<Lid>>(lst => lst.Any(el => el is Leiding)))).Verifiable();
            

            Factory.InstantieRegistreren(ledenSyncMock.Object);

            // mocking van de data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<AfdelingsJaar>())
                                  .Returns(new DummyRepo<AfdelingsJaar>(new List<AfdelingsJaar> {afdelingsJaar}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> {lid}));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<LedenService>();
            var lidInformatie = new[]
                                    {
                                        new InschrijvingsVerzoek
                                            {
                                                AfdelingsJaarIrrelevant = false,
                                                AfdelingsJaarIDs = new [] {afdelingsJaar.ID},
                                                GelieerdePersoonID = gelieerdePersoon.ID,
                                                LeidingMaken = false,
                                            }
                                    };
            target.Inschrijven(lidInformatie);

            // ASSERT

            ledenSyncMock.Verify(src => src.Bewaren(It.Is<IList<Lid>>(lst => lst.Any(el => el is Leiding))), Times.Never());
            ledenSyncMock.Verify(src => src.Bewaren(It.Is<IList<Lid>>(lst => lst.Any(el => el is Kind))), Times.Once());
        }

        /// <summary>
        /// Test op een foutnummer als je probeert iemand in te schrijven bij een inactieve groep.
        /// </summary>
        [Test]
        public void InschrijvenGestoptTest()
        {
            // ARRANGE

            // model
            var groep = new KaderGroep()
            {
                StopDatum = DateTime.Now.AddMonths(-1)
            };

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Groep = groep,
                Persoon =
                    new Persoon
                    {
                        Geslacht = GeslachtsType.Vrouw,
                        GeboorteDatum = new DateTime(1980, 8, 8)
                    }
            };

            var verzoek = new InschrijvingsVerzoek
            {
                AfdelingsJaarIrrelevant = true,
                GelieerdePersoonID = gelieerdePersoon.ID,
                LeidingMaken = true,
            };

            // dependency injection
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoon }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var ledenService = Factory.Maak<LedenService>();
            var result = ledenService.Inschrijven(new[] { verzoek });

            // ASSERT

            Assert.IsTrue(result.First().FoutNummer == FoutNummer.GroepInactief);

        }

        /// <summary>
        /// Gestopte groepen kunnen geen leden uitschrijven.
        /// </summary>
        [Test]
        public void UitschrijvenGestoptTest()
        {
            // ARRANGE

            // testsituatie opbouwen
            var groepsWerkJaar = new GroepsWerkJaar
                                     {
                                         Groep =
                                             new KaderGroep
                                                 {
                                                     NiveauInt = (int) Niveau.Gewest,
                                                     StopDatum = DateTime.Now.AddMonths(-1)
                                                 }
                                     };
            groepsWerkJaar.Groep.GroepsWerkJaar = new List<GroepsWerkJaar> { groepsWerkJaar };

            var gelieerdePersoon1 = new GelieerdePersoon { ID = 1, Groep = groepsWerkJaar.Groep };
            groepsWerkJaar.Groep.GelieerdePersoon = new List<GelieerdePersoon> { gelieerdePersoon1 };

            var medewerker1 = new Leiding
            {
                EindeInstapPeriode = DateTime.Today,
                // probeerperiode kadermedewerker is irrelevant
                GroepsWerkJaar = groepsWerkJaar,
                GelieerdePersoon = gelieerdePersoon1
            };

            gelieerdePersoon1.Lid = new List<Lid> { medewerker1 };

            // data access opzetten
            var dummyGpRepo = new DummyRepo<GelieerdePersoon>(groepsWerkJaar.Groep.GelieerdePersoon.ToList());
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>()).Returns(dummyGpRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            var target = Factory.Maak<LedenService>();

            // ASSERT

            string foutBerichten = string.Empty;
            var ex =
                Assert.Throws<FaultException<FoutNummerFault>>(
                    () => target.Uitschrijven(new[] {gelieerdePersoon1.ID}, out foutBerichten));
            Assert.AreEqual(FoutNummer.GroepInactief, ex.Detail.FoutNummer);
        }

        /// <summary>
        /// Gestopte groepen mogen niet verzekeren voor loonverlies.
        /// </summary>
        /// <remarks>
        /// Loonverlies is een uitbreiding op de 'gewone' Chiroverzekering. Dus in principe kun je je daarvoor
        /// maar verzekeren als je lid bent. Leden van een gestopte groep zijn per definitie oude leden 
        /// (van vroeger). Dus kun je geen uitbreiding op de verzekering nemen.
        /// </remarks>
        [Test]
        public void LoonVerliesVerzekerenGestoptTest()
        {
            // ARRANGE

            var lid = new Leiding
                          {
                              ID = 1,
                              GroepsWerkJaar =
                                  new GroepsWerkJaar
                                      {
                                          WerkJaar = DateTime.Now.Year,
                                          Groep = new ChiroGroep {StopDatum = DateTime.Now.AddMonths(-1)}
                                      },
                              GelieerdePersoon = new GelieerdePersoon {Persoon = new Persoon()}
                          };

            var verzekering = new VerzekeringsType { ID = (int)Verzekering.LoonVerlies };

            // mock voor data-access registreren

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> { lid }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);
            var target = Factory.Maak<LedenService>();

            // ASSERT

            var ex = Assert.Throws<FaultException<FoutNummerFault>>(() => target.LoonVerliesVerzekeren(lid.ID));
            Assert.AreEqual(FoutNummer.GroepInactief, ex.Detail.FoutNummer);
        }

        /// <summary>
        /// Een inschrijvingsvoorstel voor een kadergroep moet altijd suggereren om als leiding in te schrijven.
        /// </summary>
        [Test]
        public void VoorstelTotInschrijvenGenererenKaderTest()
        {
            // ARRANGE
            // Gauw wat handgemaakte dummydata:

            var groepsWerkJaar = new GroepsWerkJaar
            {
                WerkJaar = 2013,
            };

            var gelieerdePersoon = new GelieerdePersoon
                                   {
                                       ID = 3,
                                       Persoon = new Persoon
                                                 {
                                                     GeboorteDatum = new DateTime(1997, 3, 8),  // extreem jong voor kaderploeg
                                                     Geslacht = GeslachtsType.Vrouw
                                                 },
                                       Groep = new KaderGroep
                                               {
                                                   GroepsWerkJaar = new List<GroepsWerkJaar>
                                                                    {
                                                                        groepsWerkJaar
                                                                    },
                                                   NiveauInt = (int)Niveau.Gewest
                                               },
                                       PersoonsAdres = new PersoonsAdres()
                                   };

            groepsWerkJaar.Groep = gelieerdePersoon.Groep;

            // We mocken een en ander:

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(
                                      new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon>
                                                                          {
                                                                              gelieerdePersoon,
                                                                          }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<LedenService>();
            var actual = target.InschrijvingVoorstellen(new[] { gelieerdePersoon.ID });

            // We verwachten dat de opgeleverde gelieerde persoon ingeschreven wordt als lid.

            Assert.IsTrue(actual.First().LeidingMaken);
        }

        /// <summary>
        /// Ook kaderleden moeten aan de minimum(leidings)leeftijd voldoen
        /// </summary>
        public void VoorstelTotInschrijvenGenererenKaderTeJongTest()
        {
            // ARRANGE
            // Gauw wat handgemaakte dummydata:

            var groepsWerkJaar = new GroepsWerkJaar
            {
                WerkJaar = 2013,
            };

            var gelieerdePersoon = new GelieerdePersoon
                                   {
                                       ID = 3,
                                       Persoon = new Persoon
                                                 {
                                                     GeboorteDatum = new DateTime(2007, 3, 8), // te jong voor leiding
                                                     Geslacht = GeslachtsType.Vrouw
                                                 },
                                       Groep = new KaderGroep
                                               {
                                                   GroepsWerkJaar = new List<GroepsWerkJaar>
                                                                    {
                                                                        groepsWerkJaar
                                                                    },
                                                   NiveauInt = (int) Niveau.Gewest
                                               }
                                   };

            groepsWerkJaar.Groep = gelieerdePersoon.Groep;

            // We mocken een en ander:

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(
                                      new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon>
                                                                          {
                                                                              gelieerdePersoon,
                                                                          }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);
            var target = Factory.Maak<LedenService>();

            // ASSERT

            var ex =
                Assert.Throws<FaultException<FoutNummerFault>>(() => target.InschrijvingVoorstellen(new[]
                    {gelieerdePersoon.ID}));
            Assert.Equals(FoutNummer.LidTeJong, ex.Detail.FoutNummer);
        }

        /// <summary>
        ///Als je probeert een kindlid aan te sluiten bij een kaderploeg, verwachten we een foutmelding.
        ///</summary>
        [Test]
        public void InschrijvenKindBijKaderTest()
        {
            // ARRANGE

            // model
            var groep = new KaderGroep()
                        {
                            NiveauInt = (int) Niveau.Gewest
                        };

            var groepswerkjaar = new GroepsWerkJaar {WerkJaar = 2013, Groep = groep};
            groep.GroepsWerkJaar.Add(groepswerkjaar);

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Groep = groep,
                Persoon =
                    new Persoon
                    {
                        Geslacht = GeslachtsType.Vrouw,
                        GeboorteDatum = new DateTime(1997, 8, 8)
                    },
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        }
                    }
            };

            var verzoek = new InschrijvingsVerzoek
            {
                AfdelingsJaarIrrelevant = true,
                GelieerdePersoonID = gelieerdePersoon.ID,
                LeidingMaken = false,
            };

            // dependency injection
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<AfdelingsJaar>())
                .Returns(new DummyRepo<AfdelingsJaar>(new List<AfdelingsJaar>()));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var ledenService = Factory.Maak<LedenService>();
            var feedback = ledenService.Inschrijven(new[] { verzoek });

            // ASSERT
            Assert.IsTrue(feedback.Any(info => info.FoutNummer == FoutNummer.LidTypeVerkeerd));
        }
        
        /// <summary>
        ///A test for Zoeken op functie
        ///</summary>
        [Test]
        public void ZoekenOpFunctieTest()
        {
            // ARRANGE

            // testmodelletje

            var functie1 = new Functie {ID = 2};
            var functie2 = new Functie {ID = 3};

            var lid = new Leiding
                          {
                              ID = 1,
                              GelieerdePersoon = new GelieerdePersoon
                                                     {
                                                         Persoon = new Persoon()
                                                     },
                              Functie = new List<Functie> {functie1}
                          };

            var lid2 = new Leiding
                           {
                               ID = 4,
                               GelieerdePersoon = new GelieerdePersoon
                                                      {
                                                          Persoon = new Persoon()
                                                      },
                               Functie = new List<Functie> {functie2}
                           };


            // dependency injection

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> { lid, lid2 }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>())
                      .Returns(new DummyRepo<GroepsWerkJaar>(new List<GroepsWerkJaar>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Kind>())
                                  .Returns(new DummyRepo<Kind>(new List<Kind>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Leiding>())
                                  .Returns(new DummyRepo<Leiding>(new List<Leiding> { lid, lid2 }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Functie>())
                                  .Returns(new DummyRepo<Functie>(new List<Functie> {functie1, functie2}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<LedenService>();
            var actual = target.LijstZoekenLidOverzicht(new LidFilter { FunctieID = functie1.ID }, false);

            // ASSERT
            Assert.AreEqual(1, actual.Count);
        }        
    }
}
