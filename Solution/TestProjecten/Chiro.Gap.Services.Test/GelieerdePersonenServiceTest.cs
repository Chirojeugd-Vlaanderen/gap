// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>
// <summary>
//   Dit is een testclass voor Unit Tests van GelieerdePersonenServiceTest
// </summary>

using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.Mappers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.ServiceModel;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.TestDbInfo;

namespace Chiro.Gap.Services.Test
{
    /// <summary>
    /// Dit is een testclass voor Unit Tests van GelieerdePersonenServiceTest
    /// </summary>
    [TestClass]
    public class GelieerdePersonenServiceTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// Run code before running first test
        /// </summary>
        [ClassInitialize]
        static public void MyClassInitialize(TestContext context)
        {
            MappingHelper.MappingsDefinieren();
        }

        /// <summary>
        /// Run code before running each test
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            // Restore IoC settings

            // (Ik had hier ook alle nodige mocks kunnen maken, en dan
            // een voor een registreren.  Maar ContainerInit herleest gewoon
            // de configuratiefile.)

            Factory.ContainerInit();
        }

        /// <summary>
        ///A test for CommunicatieVormToevoegen
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FaultException<FoutNummerFault>))]
        public void CommunicatieVormToevoegenTestOngeldig()
        {
            var target = Factory.Maak<GelieerdePersonenService>();

            var gelieerdePersoonID = TestInfo.GELIEERDE_PERSOON_ID;

            var commInfo = new CommunicatieDetail()
            {
                CommunicatieTypeID = 1,
                Nummer = TestInfo.ONGELDIG_TELEFOON_NR
            };

            target.CommunicatieVormToevoegen(gelieerdePersoonID, commInfo);
            Assert.IsTrue(false);
        }

        ///<summary>
        ///Toevoegen van een geldig telefoonnr
        /// </summary>
        [TestMethod]
        public void CommunicatieVormToevoegenTestGeldig()
        {
            var target = Factory.Maak<GelieerdePersonenService>();

            var gelieerdePersoonID = TestInfo.GELIEERDE_PERSOON_ID;

            var commInfo = new CommunicatieDetail()
            {
                CommunicatieTypeID = 1,
                Voorkeur = true,
                Nummer = TestInfo.GELDIG_TELEFOON_NR
            };

            target.CommunicatieVormToevoegen(gelieerdePersoonID, commInfo);
            Assert.IsTrue(true);	// al blij als er geen exception optreedt
        }

        /// <summary>
        /// Kijkt na of CommunicatieVormToevoegen enkel de nieuwe communicatievorm
        /// naar Kipadmin synct, ipv alle communicatie in kipadmin te vervangen  
        /// </summary>
        [TestMethod]
        public void CommunicatieVormToevoegenTest()
        {
            // Arrange

            const int TESTGPID = 1234;

            var communicatieVormDaoMock = new Mock<ICommunicatieVormDao>();
            var communicatieTypeDaoMock = new Mock<IDao<CommunicatieType>>();
            var gelieerdePersonenDaoMock = new Mock<IGelieerdePersonenDao>();

            var communicatieSyncMock = new Mock<ICommunicatieSync>();

            // het communicatietype zal worden opgevraagd, maar is irrelevant voor deze test.
            // zorg er wel voor dat alles valideert.
            communicatieTypeDaoMock.Setup(dao => dao.Ophalen(3)).Returns(new CommunicatieType { Validatie = ".*" });

            // idem voor gelieerde persoon
            gelieerdePersonenDaoMock.Setup(dao => dao.Ophalen(It.IsAny<IEnumerable<int>>(), It.IsAny<PersoonsExtras>()))
                .Returns(new[] { new GelieerdePersoon { Persoon = new Persoon { AdInAanvraag = true } } });

            // de communicatievorm zal ook worden bewaard
            communicatieVormDaoMock.Setup(
                dao =>
                dao.Bewaren(It.IsAny<CommunicatieVorm>(), It.IsAny<Expression<Func<CommunicatieVorm, object>>[]>())).
                Returns((CommunicatieVorm cv, Expression<Func<CommunicatieVorm, object>>[] paths) => cv);

            // verwacht dat CommunicatieSync.Toevoegen wordt aangeroepen.
            communicatieSyncMock.Setup(snc => snc.Toevoegen(It.IsAny<CommunicatieVorm>())).Verifiable();

            Factory.InstantieRegistreren(communicatieTypeDaoMock.Object);
            Factory.InstantieRegistreren(gelieerdePersonenDaoMock.Object);
            Factory.InstantieRegistreren(communicatieVormDaoMock.Object);
            Factory.InstantieRegistreren(communicatieSyncMock.Object);

            var target = Factory.Maak<GelieerdePersonenService>();

            int gelieerdePersoonID = TESTGPID;     // arbitrair en irrelevant
            var commDetail = new CommunicatieInfo
                                 {
                                     CommunicatieTypeID = 3,    // e-mail
                                     ID = 4,    // verschillend van nul, in een poging de communicatiemanager te misleiden.
                                     Nummer = "johan@linux.be"  // arbitrair
                                 };

            // Act
            target.CommunicatieVormToevoegen(gelieerdePersoonID, commDetail);

            // Assert
            communicatieSyncMock.Verify(snc => snc.Toevoegen(It.IsAny<CommunicatieVorm>()));

            // Als de verify geen exception opleverde, is het gelukt.
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Controleren of 'communicatievormaanpassen' wel degelijk een communicatievorm aanpast.
        /// </summary>
        [TestMethod]
        public void CommunicatieVormAanpassenTest()
        {
            // Arrange

            const int TESTGPID = 1234;      // arbitrair ID van een gelieerde persoon
            const int TESTCVID = 2345;      // en van een communicatievorm
            const int TESTCTID = 3;         // en diens communicatietype

            var testCommunicatieType = new CommunicatieType { ID = TESTCTID, Validatie = ".*" };
            var testCommunicatieVorm = new CommunicatieVorm
                                           {
                                               ID = TESTCVID,
                                               CommunicatieType = testCommunicatieType,
                                               Nummer = "jos@linux.be"
                                           };

            // Koppel gauw testCommunicatieVorm aan testGelieerdePersoon

            var testGelieerdePersoon = new GelieerdePersoon
                                           {
                                               ID = TESTGPID,
                                               Persoon = new Persoon(),
                                               Communicatie =
                                                   new EntityCollection<CommunicatieVorm>
                                                       {
                                                           testCommunicatieVorm
                                                       }
                                           };
            testCommunicatieVorm.GelieerdePersoon = testGelieerdePersoon;

            var communicatieTypeDaoMock = new Mock<IDao<CommunicatieType>>();
            var communicatieVormDaoMock = new Mock<ICommunicatieVormDao>();

            var communicatieSyncMock = new Mock<ICommunicatieSync>();

            // het communicatietype zal worden opgevraagd, maar is irrelevant voor deze test.
            // zorg er wel voor dat alles valideert.
            communicatieTypeDaoMock.Setup(dao => dao.Ophalen(TESTCTID)).Returns(testCommunicatieType);
            // idem voor communicatievorm

            // Toevallig weet ik dat IDao.Ophalen opgeroepen wordt met 4 lambda-expressies om de
            // communicatievorm op te halen.  Dus die moet gemockt worden.
            communicatieVormDaoMock.Setup(
                dao =>
                dao.Ophalen(TESTCVID,
                            It.IsAny<Expression<Func<CommunicatieVorm, object>>[]>()))
                .Returns(testCommunicatieVorm);

            Factory.InstantieRegistreren(communicatieTypeDaoMock.Object);
            Factory.InstantieRegistreren(communicatieVormDaoMock.Object);
            Factory.InstantieRegistreren(communicatieSyncMock.Object);

            var target = Factory.Maak<GelieerdePersonenService>();

            var commDetail = new CommunicatieInfo
            {
                CommunicatieTypeID = 3,    // e-mail
                ID = TESTCVID,    // ID van de bestaande communicatievorm
                Nummer = "johan@linux.be"  // arbitrair nieuw e-mailadres
            };

            // Act
            target.CommunicatieVormAanpassen(commDetail);

            // Assert

            Assert.AreEqual(testCommunicatieVorm.Nummer, "johan@linux.be");
        }

        /// <summary>
        /// Bij  het toevoegen van een communicatievorm die voorkeur moet zijn, moeten
        /// bestaande communicatievormen van hetzelfde type hun voorkeur verliezen. 
        /// </summary>
        [TestMethod]
        public void VoorkeursCommunicatieVormToevoegenTest()
        {
            // Arrange

            const int TESTGPID = 1234;      // arbitrair ID van een gelieerde persoon
            const int TESTCVID = 2345;      // en van een communicatievorm
            const int TESTCTID = 3;         // en diens communicatietype

            var testCommunicatieType = new CommunicatieType { ID = TESTCTID, Validatie = ".*" };
            var testCommunicatieVorm = new CommunicatieVorm
            {
                ID = TESTCVID,
                CommunicatieType = testCommunicatieType,
                Nummer = "jos@linux.be",
                Voorkeur = true
            };

            // Koppel gauw testCommunicatieVorm aan testGelieerdePersoon

            var testGelieerdePersoon = new GelieerdePersoon
            {
                ID = TESTGPID,
                Persoon = new Persoon(),
                Communicatie =
                    new EntityCollection<CommunicatieVorm>
                                                       {
                                                           testCommunicatieVorm
                                                       }
            };
            testCommunicatieVorm.GelieerdePersoon = testGelieerdePersoon;

            var communicatieTypeDaoMock = new Mock<IDao<CommunicatieType>>();
            var communicatieVormDaoMock = new Mock<ICommunicatieVormDao>();
            var gelieerdePersonenDaoMock = new Mock<IGelieerdePersonenDao>();

            var communicatieSyncMock = new Mock<ICommunicatieSync>();

            // het communicatietype zal worden opgevraagd, maar is irrelevant voor deze test.
            // zorg er wel voor dat alles valideert.
            communicatieTypeDaoMock.Setup(dao => dao.Ophalen(TESTCTID)).Returns(testCommunicatieType);

            // zorg ervoor dat de gelieerde persoon opgehaald wordt
            gelieerdePersonenDaoMock.Setup(dao => dao.Ophalen(It.IsAny<IEnumerable<int>>(), It.IsAny<PersoonsExtras>()))
                .Returns(new[] { testGelieerdePersoon });

            // in plaats van een communicatievorm te bewaren, zetten we iets in zijn opmerkingenveld.
            // Op die manier kunnen we achteraf zien welke communicatievormen bewaard zijn.

            // Dit gaat er nu vanuit dat de communicatievormen bewaard worden via CommunicatieVormDao.Bewaren;
            // kan even goed iets anders zijn.  Unit test enkel ter illustratie

            communicatieVormDaoMock.Setup(
                dao =>
                dao.Bewaren(It.IsAny<CommunicatieVorm>(), It.IsAny<Expression<Func<CommunicatieVorm, object>>[]>())).
                Returns((CommunicatieVorm cv, Expression<Func<CommunicatieVorm, object>>[] paths) =>
                            {
                                cv.Nota = "bewaard";
                                return cv;
                            });

            Factory.InstantieRegistreren(communicatieTypeDaoMock.Object);
            Factory.InstantieRegistreren(communicatieVormDaoMock.Object);
            Factory.InstantieRegistreren(gelieerdePersonenDaoMock.Object);
            Factory.InstantieRegistreren(communicatieSyncMock.Object);

            var target = Factory.Maak<GelieerdePersonenService>();

            var commDetail = new CommunicatieInfo
            {
                CommunicatieTypeID = 3,    // e-mail
                ID = 0,                    // nieuwe communicatievorm
                Nummer = "johan@linux.be", // arbitrair nieuw e-mailadres
                Voorkeur = true
            };

            // Act
            target.CommunicatieVormToevoegen(TESTGPID, commDetail);

            // Assert

            // De nieuwe communicatievorm zal wel bewaard zijn.  Maar de
            // oorspronkelijke moet ook bewaard zijn, want die is zijn
            // voorkeur verloren.

            Assert.AreEqual("bewaard", testCommunicatieVorm.Nota);
        }


        /// <summary>
        /// Als een communicatievorm voorkeur wordt gemaakt voor zijn type, dan moet
        /// de huidige voorkeurscommunicatie opnieuw gepersisteerd worden, want die
        /// verliest zijn voorkeur.
        /// </summary>
        [TestMethod]
        public void CommunicatieVormVoorkeurMakenTest()
        {
            // Arrange

            const int TESTGPID = 1234;      // arbitrair ID van een gelieerde persoon
            const int TESTCVID = 2345;      // en van een communicatievorm
            const int TESTCVID2 = 2346;     // en van een andere communicatievorm
            const int TESTCTID = 3;         // en hun communicatietype

            var testCommunicatieType = new CommunicatieType { ID = TESTCTID, Validatie = ".*" };
            var testCommunicatieVorm = new CommunicatieVorm
            {
                ID = TESTCVID,
                CommunicatieType = testCommunicatieType,
                Nummer = "jos@linux.be",
                Voorkeur = true
            };
            var testCommunicatieVorm2 = new CommunicatieVorm
                                            {
                                                ID = TESTCVID2,
                                                CommunicatieType = testCommunicatieType,
                                                Nummer = "johan@linux.be",
                                                Voorkeur = false
                                            };

            // Koppel communicatievormen handmatig aan testGelieerdePersoon

            var testGelieerdePersoon = new GelieerdePersoon
            {
                ID = TESTGPID,
                Persoon = new Persoon(),
                Communicatie =
                    new EntityCollection<CommunicatieVorm>
                                                       {
                                                           testCommunicatieVorm,
                                                           testCommunicatieVorm2
                                                       }
            };
            testCommunicatieVorm.GelieerdePersoon = testGelieerdePersoon;
            testCommunicatieVorm2.GelieerdePersoon = testGelieerdePersoon;

            var communicatieTypeDaoMock = new Mock<IDao<CommunicatieType>>();
            var communicatieVormDaoMock = new Mock<ICommunicatieVormDao>();
            var gelieerdePersonenDaoMock = new Mock<IGelieerdePersonenDao>();

            var communicatieSyncMock = new Mock<ICommunicatieSync>();

            // het communicatietype zal worden opgevraagd, maar is irrelevant voor deze test.
            // zorg er wel voor dat alles valideert.
            communicatieTypeDaoMock.Setup(dao => dao.Ophalen(TESTCTID)).Returns(testCommunicatieType);

            // Als communicatievorm met ID TESTCVID2 opgevraagd wordt, geef dan testCommunicatieVorm2.

            communicatieVormDaoMock.Setup(
                dao => dao.Ophalen(TESTCVID2, It.IsAny<Expression<Func<CommunicatieVorm, object>>[]>())).Returns(
                    testCommunicatieVorm2);

            // zorg ervoor dat de gelieerde persoon opgehaald wordt
            gelieerdePersonenDaoMock.Setup(dao => dao.Ophalen(It.IsAny<IEnumerable<int>>(), It.IsAny<PersoonsExtras>()))
                .Returns(new[] { testGelieerdePersoon });

            // in plaats van een communicatievorm te bewaren, zetten we iets in zijn opmerkingenveld.
            // Op die manier kunnen we achteraf zien welke communicatievormen bewaard zijn.

            // Dit gaat er nu vanuit dat de communicatievormen bewaard worden via CommunicatieVormDao.Bewaren;
            // kan even goed iets anders zijn.  Unit test enkel ter illustratie

            communicatieVormDaoMock.Setup(
                dao =>
                dao.Bewaren(It.IsAny<CommunicatieVorm>(), It.IsAny<Expression<Func<CommunicatieVorm, object>>[]>())).
                Returns((CommunicatieVorm cv, Expression<Func<CommunicatieVorm, object>>[] paths) =>
                {
                    cv.Nota = "bewaard";
                    return cv;
                });

            Factory.InstantieRegistreren(communicatieTypeDaoMock.Object);
            Factory.InstantieRegistreren(communicatieVormDaoMock.Object);
            Factory.InstantieRegistreren(gelieerdePersonenDaoMock.Object);
            Factory.InstantieRegistreren(communicatieSyncMock.Object);

            var target = Factory.Maak<GelieerdePersonenService>();

            var info = new CommunicatieInfo
            {
                CommunicatieTypeID = TESTCTID,    // ID testcommunicatietype
                ID = TESTCVID2,                   // ID niet-voorkeurscommunicatie
                Nummer = "johan@linux.be",        // arbitrair nieuw e-mailadres
                Voorkeur = true                   // nu wel voorkeur
            };

            // Act
            target.CommunicatieVormAanpassen(info);

            // Assert

            // Wijzigingen voor communicatievorm2 zullen wel bewaard zijn.  Maar 
            // communicatievorm1 moet ook bewaard zijn, want die is zijn
            // voorkeur verloren.

            Assert.AreEqual("bewaard", testCommunicatieVorm.Nota);
        }
    }
}
