/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkt gebruikersbeheer Copyright 2014, 2015 Chirojeugd-Vlaanderen vzw
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
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Test;
using Chiro.Gap.WorkerInterfaces;
using NUnit.Framework;
using Moq;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    ///This is a test class for AutorisatieManagerTest and is intended
    ///to contain all AutorisatieManagerTest Unit Tests
    ///</summary>
    [TestFixture]
    public class AutorisatieManagerTest: ChiroTest
    {
        /// <summary>
        ///Controleert of de GAV-check van een groep rekening houdt met de vervaldatum van gebruikersrechten.
        ///</summary>
        [Test]
        public void IsGavGroepVervallenTest()
        {
            // ARRANGE

            // testgroep; toegang voor deze persoon net vervallen.
            const int mijnAdNummer = 12345;

            Groep groep = new ChiroGroep
            {
                GebruikersRechtV2 = new[]
                {
                    new GebruikersRechtV2
                    {
                        Persoon = new Persoon {AdNummer = mijnAdNummer},
                        VervalDatum = DateTime.Today // net vervallen
                    }
                }
            };

            // Zet mock op voor het opleveren van gebruikersnaam
            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(src => src.AdNummerGet()).Returns(mijnAdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);


            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            var actual = target.IsGav(groep);


            // ASSERT

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Als je rechten hebt op iedereen van je groep, moet je ook rechten hebben op iedereen van je afdeling.
        /// </summary>
        [Test]
        public void GroepsRechtImpliceertAfdelingsRechtTest()
        {
            // ARRANGE

            // testgroep; toegang voor deze persoon net vervallen.
            const int mijnAdNummer = 12345;

            Groep groep = new ChiroGroep
            {
                GebruikersRechtV2 = new[]
                {
                    new GebruikersRechtV2
                    {
                        Persoon = new Persoon {AdNummer = mijnAdNummer},
                        VervalDatum = DateTime.Today.AddDays(1),
                        GroepsPermissies = Permissies.Bewerken
                    }
                }
            };

            // Zet mock op voor het opleveren van gebruikersnaam
            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(src => src.AdNummerGet()).Returns(mijnAdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);

            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            var actual = target.PermissiesOphalen(groep, SecurityAspect.PersonenInAfdeling);


            // ASSERT

            Assert.IsTrue(actual.HasFlag(Permissies.Bewerken));
        }

        /// <summary>
        /// Als je gebruikersrechten hebt, maar geen GAV-permissies, dan mag IsGav ook niet zeggen
        /// dat je GAV bent.
        /// </summary>
        [Test]
        public void RechtenMaarGeenGavGroepTest()
        {
            // ARRANGE

            var gebruikersRecht = new GebruikersRechtV2
            {
                Persoon = new Persoon { ID = 1, AdNummer = 2 },
                Groep = new ChiroGroep { ID = 3 },
                VervalDatum = DateTime.Now.AddDays(1)       // geldig tot morgen.
            };
            gebruikersRecht.Persoon.GebruikersRechtV2.Add(gebruikersRecht);
            gebruikersRecht.Groep.GebruikersRechtV2.Add(gebruikersRecht);

            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(mgr => mgr.AdNummerGet()).Returns(gebruikersRecht.Persoon.AdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);

            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            bool actual = target.IsGav(gebruikersRecht.Groep);

            // ASSERT

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Als je gebruikersrechten hebt, maar geen GAV-permissies, dan mag IsGav ook niet zeggen
        /// dat je GAV bent.
        /// </summary>
        [Test]
        public void RechtenMaarGeenGavGroepenTest()
        {
            // ARRANGE

            var gebruikersRecht = new GebruikersRechtV2
            {
                Persoon = new Persoon { ID = 1, AdNummer = 2 },
                Groep = new ChiroGroep { ID = 3 },
                VervalDatum = DateTime.Now.AddDays(1)       // geldig tot morgen.
            };
            gebruikersRecht.Persoon.GebruikersRechtV2.Add(gebruikersRecht);
            gebruikersRecht.Groep.GebruikersRechtV2.Add(gebruikersRecht);

            var gebruikersRecht2 = new GebruikersRechtV2
            {
                Persoon = new Persoon { ID = 1, AdNummer = 2 },
                Groep = new ChiroGroep { ID = 4 },
                VervalDatum = DateTime.Now.AddDays(1)       // geldig tot morgen.
            };
            gebruikersRecht2.Persoon.GebruikersRechtV2.Add(gebruikersRecht2);
            gebruikersRecht2.Groep.GebruikersRechtV2.Add(gebruikersRecht2);


            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(mgr => mgr.AdNummerGet()).Returns(gebruikersRecht.Persoon.AdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);

            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            bool actual = target.IsGav(new [] {gebruikersRecht.Groep, gebruikersRecht2.Groep});

            // ASSERT

            Assert.IsFalse(actual);
        }


        /// <summary>
        /// Als je gebruikersrechten hebt, maar geen GAV-permissies, dan mag IsGav ook niet zeggen
        /// dat je GAV bent.
        /// </summary>
        [Test]
        public void RechtenMaarGeenPermissiesPersonenTest()
        {
            // ARRANGE

            var gebruikersRecht = new GebruikersRechtV2
            {
                Persoon = new Persoon { ID = 1, AdNummer = 2 },
                Groep = new ChiroGroep { ID = 3 },
                VervalDatum = DateTime.Now.AddDays(1)       // geldig tot morgen.
            };
            gebruikersRecht.Persoon.GebruikersRechtV2.Add(gebruikersRecht);
            gebruikersRecht.Groep.GebruikersRechtV2.Add(gebruikersRecht);

            var gp1 = new GelieerdePersoon { ID = 4, Groep = gebruikersRecht.Groep, Persoon = new Persoon { ID = 6 } };
            var gp2 = new GelieerdePersoon { ID = 5, Groep = gebruikersRecht.Groep, Persoon = new Persoon { ID = 7 } };
            gebruikersRecht.Groep.GelieerdePersoon.Add(gp1);
            gebruikersRecht.Groep.GelieerdePersoon.Add(gp2);

            gp1.Persoon.GelieerdePersoon.Add(gp1);
            gp2.Persoon.GelieerdePersoon.Add(gp2);


            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(mgr => mgr.AdNummerGet()).Returns(gebruikersRecht.Persoon.AdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);

            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            bool actual = target.IsGav(new[] { gp1.Persoon, gp2.Persoon });

            // ASSERT

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Als je gebruikersrechten hebt, maar geen GAV-permissies, dan mag IsGav ook niet zeggen
        /// dat je GAV bent.
        /// </summary>
        [Test]
        public void RechtenMaarGeenPermissiesLedenTest()
        {
            // ARRANGE

            var gebruikersRecht = new GebruikersRechtV2
            {
                Persoon = new Persoon { ID = 1, AdNummer = 2 },
                Groep = new ChiroGroep { ID = 3 },
                VervalDatum = DateTime.Now.AddDays(1)       // geldig tot morgen.
            };
            gebruikersRecht.Persoon.GebruikersRechtV2.Add(gebruikersRecht);
            gebruikersRecht.Groep.GebruikersRechtV2.Add(gebruikersRecht);

            var l1 = new Kind 
            { 
                ID = 6, 
                GelieerdePersoon = new GelieerdePersoon { ID = 4, Groep = gebruikersRecht.Groep }, 
                GroepsWerkJaar = new GroepsWerkJaar { ID = 7, Groep = gebruikersRecht.Groep }
            };
            var l2 = new Kind
            {
                ID = 7,
                GelieerdePersoon = new GelieerdePersoon { ID = 5, Groep = gebruikersRecht.Groep },
                GroepsWerkJaar = l1.GroepsWerkJaar
            };
            l1.GroepsWerkJaar.Lid.Add(l1);
            l1.GroepsWerkJaar.Lid.Add(l2);
            gebruikersRecht.Groep.GroepsWerkJaar.Add(l1.GroepsWerkJaar);

            l1.GelieerdePersoon.Lid.Add(l1);
            l2.GelieerdePersoon.Lid.Add(l2);
            gebruikersRecht.Groep.GelieerdePersoon.Add(l1.GelieerdePersoon);
            gebruikersRecht.Groep.GelieerdePersoon.Add(l2.GelieerdePersoon);


            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(mgr => mgr.AdNummerGet()).Returns(gebruikersRecht.Persoon.AdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);

            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            bool actual = target.IsGav(new[] { l1, l2 });

            // ASSERT

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Als je gebruikersrechten hebt, maar geen GAV-permissies, dan mag IsGav ook niet zeggen
        /// dat je GAV bent.
        /// </summary>
        [Test]
        public void RechtenMaarGeenPermissiesGelieerdePersoonTest()
        {
            // ARRANGE

            var gebruikersRecht = new GebruikersRechtV2
            {
                Persoon = new Persoon { ID = 1, AdNummer = 2 },
                Groep = new ChiroGroep { ID = 3 },
                VervalDatum = DateTime.Now.AddDays(1)       // geldig tot morgen.
            };
            gebruikersRecht.Persoon.GebruikersRechtV2.Add(gebruikersRecht);
            gebruikersRecht.Groep.GebruikersRechtV2.Add(gebruikersRecht);

            var gp1 = new GelieerdePersoon { ID = 4, Groep = gebruikersRecht.Groep };
            var gp2 = new GelieerdePersoon { ID = 5, Groep = gebruikersRecht.Groep };
            gebruikersRecht.Groep.GelieerdePersoon.Add(gp1);
            gebruikersRecht.Groep.GelieerdePersoon.Add(gp2);


            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(mgr => mgr.AdNummerGet()).Returns(gebruikersRecht.Persoon.AdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);

            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            bool actual = target.IsGav(new [] {gp1, gp2});

            // ASSERT

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Als je gebruikersrechten hebt, maar geen GAV-permissies, dan mag IsGav ook niet zeggen
        /// dat je GAV bent.
        /// </summary>
        [Test]
        public void RechtenMaarGeenPermissiesPersoonsAdresTest()
        {
            // ARRANGE

            var gebruikersRecht = new GebruikersRechtV2
            {
                Persoon = new Persoon { ID = 1, AdNummer = 2 },
                Groep = new ChiroGroep { ID = 3 },
                VervalDatum = DateTime.Now.AddDays(1)       // geldig tot morgen.
            };
            gebruikersRecht.Persoon.GebruikersRechtV2.Add(gebruikersRecht);
            gebruikersRecht.Groep.GebruikersRechtV2.Add(gebruikersRecht);

            var adres = new BelgischAdres { ID = 6 };

            var gp1 = new GelieerdePersoon { ID = 4, Groep = gebruikersRecht.Groep, Persoon = new Persoon { ID = 7 } };
            var gp2 = new GelieerdePersoon { ID = 5, Groep = gebruikersRecht.Groep, Persoon = new Persoon { ID = 8 } };

            gp1.Persoon.GelieerdePersoon.Add(gp1);
            gp2.Persoon.GelieerdePersoon.Add(gp2);

            gebruikersRecht.Groep.GelieerdePersoon.Add(gp1);
            gebruikersRecht.Groep.GelieerdePersoon.Add(gp2);

            var pa1 = new PersoonsAdres { Persoon = gp1.Persoon, Adres = adres };
            var pa2 = new PersoonsAdres { Persoon = gp2.Persoon, Adres = adres };
            gp1.Persoon.PersoonsAdres.Add(pa1);
            gp2.Persoon.PersoonsAdres.Add(pa2);
            adres.PersoonsAdres.Add(pa1);
            adres.PersoonsAdres.Add(pa2);

            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(mgr => mgr.AdNummerGet()).Returns(gebruikersRecht.Persoon.AdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);

            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            bool actual = target.IsGav(new[] { pa1, pa2 });

            // ASSERT

            Assert.IsFalse(actual);
        }

        /// <summary>
        ///Controleert of de GAV-check van een gelieerde persoon rekening houdt met de vervaldatum van gebruikersrechten.
        ///</summary>
        [Test]
        public void IsGavGelieerdePersoonTest()
        {
            // ARRANGE

            // testgroep met gelieerde persoon; toegang met mijnLogin net vervallen.
            const string mijnLogin = "MijnLogin";

            var gp = new GelieerdePersoon { Persoon = new Persoon() };

            var groep = new ChiroGroep
                              {
                                  GebruikersRechtV2 = new[]
                                                        {
                                                            new GebruikersRechtV2
                                                                {
                                                                    Persoon = gp.Persoon,
                                                                    VervalDatum = DateTime.Today // net vervallen
                                                                }
                                                        },
                                  GelieerdePersoon = new[]
                                                         {
                                                            gp
                                                         }
                              };

            gp.Groep = groep;

            // Zet mock op voor het opleveren van gebruikersnaam
            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(src => src.GebruikersNaamGet()).Returns(mijnLogin);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);


            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            var actual = target.IsGav(groep);


            // ASSERT

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Ook als je niet gelieerd bent aan dezelfde groep als een andere gelieerde persoon, kun je permissies hebben
        /// op die gelieerde persoon.
        /// </summary>
        [Test]
        public void RechtenOpGelieerdePersoonAndereGroepTest()
        {
            // ARRANGE

            var ik = new Persoon { AdNummer = 1234 };
            var gp = new GelieerdePersoon { Persoon = new Persoon() };

            var groep = new ChiroGroep
            {
                GebruikersRechtV2 = new[]
                {
                    new GebruikersRechtV2
                    {
                        Persoon = ik,
                        VervalDatum = DateTime.Today.AddDays(1),
                        IedereenPermissies = Permissies.Lezen
                    }
                },
                GelieerdePersoon = new[]
                {
                    gp
                }
            };

            gp.Groep = groep;

            // Zet mock op voor het opleveren van gebruikersnaam
            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(src => src.AdNummerGet()).Returns(ik.AdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);

            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            var actual = target.PermissiesOphalen(gp);

            // ASSERT

            Assert.IsTrue(actual.HasFlag(Permissies.Lezen));
        }

        /// <summary>
        /// Ook als je niet gelieerd bent aan dezelfde groep als een lid, kun je permissies hebben
        /// op dat lid
        /// </summary>
        [Test]
        public void RechtenOpLidAndereGroepTest()
        {
            // ARRANGE

            var ik = new Persoon { AdNummer = 1234 };
            var gp = new GelieerdePersoon { Persoon = new Persoon() };

            var groep = new ChiroGroep
            {
                GebruikersRechtV2 = new[]
                {
                    new GebruikersRechtV2
                    {
                        Persoon = ik,
                        VervalDatum = DateTime.Today.AddDays(1),
                        IedereenPermissies = Permissies.Lezen
                    }
                },
                GelieerdePersoon = new[]
                {
                    gp
                }
            };

            gp.Groep = groep;

            var leiding = new Leiding
            {
                GelieerdePersoon = gp,
                GroepsWerkJaar = new GroepsWerkJaar {Groep = groep}
            };

            // Zet mock op voor het opleveren van gebruikersnaam
            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(src => src.AdNummerGet()).Returns(ik.AdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);

            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            var actual = target.PermissiesOphalen(leiding);

            // ASSERT

            Assert.IsTrue(actual.HasFlag(Permissies.Lezen));
        }

        /// <summary>
        /// Zelfs om te testen zijn we geen super-GAV
        /// </summary>
        [Test]
        public void IsSuperGavTest()
        {
            var target = Factory.Maak<AutorisatieManager>();

            Assert.IsFalse(target.IsSuperGav());
        }
    }
}
