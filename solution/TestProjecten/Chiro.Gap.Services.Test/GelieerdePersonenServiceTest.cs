// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>
// <summary>
//   Dit is een testclass voor Unit Tests van GelieerdePersonenServiceTest
// </summary>

using System;
using System.Data.Objects.DataClasses;
using System.Linq;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.SyncInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.ServiceModel;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.TestDbInfo;
using Chiro.Gap.WorkerInterfaces;

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

            var communicatieSyncMock = new Mock<ICommunicatieSync>();


            // verwacht dat CommunicatieSync.Toevoegen wordt aangeroepen.
            communicatieSyncMock.Setup(snc => snc.Toevoegen(It.IsAny<CommunicatieVorm>())).Verifiable();

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

            var testCommunicatieType = new Poco.Model.CommunicatieType { ID = TESTCTID, Validatie = ".*" };
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
                                               Persoon = new Poco.Model.Persoon(),
                                               Communicatie =
                                                   new EntityCollection<CommunicatieVorm>
                                                       {
                                                           testCommunicatieVorm
                                                       }
                                           };
            testCommunicatieVorm.GelieerdePersoon = testGelieerdePersoon;

            var communicatieSyncMock = new Mock<ICommunicatieSync>();

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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
            //// Arrange

            //const int TESTGPID = 1234;      // arbitrair ID van een gelieerde persoon
            //const int TESTCVID = 2345;      // en van een communicatievorm
            //const int TESTCTID = 3;         // en diens communicatietype

            //var testCommunicatieType = new Poco.Model.CommunicatieType { ID = TESTCTID, Validatie = ".*" };
            //var testCommunicatieVorm = new CommunicatieVorm
            //{
            //    ID = TESTCVID,
            //    CommunicatieType = testCommunicatieType,
            //    Nummer = "jos@linux.be",
            //    Voorkeur = true
            //};

            //// Koppel gauw testCommunicatieVorm aan testGelieerdePersoon

            //var testGelieerdePersoon = new GelieerdePersoon
            //{
            //    ID = TESTGPID,
            //    Persoon = new Poco.Model.Persoon(),
            //    Communicatie =
            //        new EntityCollection<CommunicatieVorm>
            //                                           {
            //                                               testCommunicatieVorm
            //                                           }
            //};
            //testCommunicatieVorm.GelieerdePersoon = testGelieerdePersoon;

            //var communicatieTypeDaoMock = new Mock<IDao<CommunicatieType>>();
            //var communicatieVormDaoMock = new Mock<ICommunicatieVormDao>();
            //var gelieerdePersonenDaoMock = new Mock<IGelieerdePersonenDao>();

            //var communicatieSyncMock = new Mock<ICommunicatieSync>();

            //// het communicatietype zal worden opgevraagd, maar is irrelevant voor deze test.
            //// zorg er wel voor dat alles valideert.
            //communicatieTypeDaoMock.Setup(dao => dao.Ophalen(TESTCTID)).Returns(testCommunicatieType);

            //// zorg ervoor dat de gelieerde persoon opgehaald wordt
            //gelieerdePersonenDaoMock.Setup(dao => dao.Ophalen(It.IsAny<IEnumerable<int>>(), It.IsAny<PersoonsExtras>()))
            //    .Returns(new[] { testGelieerdePersoon });

            //// in plaats van een communicatievorm te bewaren, zetten we iets in zijn opmerkingenveld.
            //// Op die manier kunnen we achteraf zien welke communicatievormen bewaard zijn.

            //// Dit gaat er nu vanuit dat de communicatievormen bewaard worden via CommunicatieVormDao.Bewaren;
            //// kan even goed iets anders zijn.  Unit test enkel ter illustratie

            //communicatieVormDaoMock.Setup(
            //    dao =>
            //    dao.Bewaren(It.IsAny<CommunicatieVorm>(), It.IsAny<Expression<Func<CommunicatieVorm, object>>[]>())).
            //    Returns((CommunicatieVorm cv, Expression<Func<CommunicatieVorm, object>>[] paths) =>
            //                {
            //                    cv.Nota = "bewaard";
            //                    return cv;
            //                });

            //Factory.InstantieRegistreren(communicatieTypeDaoMock.Object);
            //Factory.InstantieRegistreren(communicatieVormDaoMock.Object);
            //Factory.InstantieRegistreren(gelieerdePersonenDaoMock.Object);
            //Factory.InstantieRegistreren(communicatieSyncMock.Object);

            //var target = Factory.Maak<GelieerdePersonenService>();

            //var commDetail = new CommunicatieInfo
            //{
            //    CommunicatieTypeID = 3,    // e-mail
            //    ID = 0,                    // nieuwe communicatievorm
            //    Nummer = "johan@linux.be", // arbitrair nieuw e-mailadres
            //    Voorkeur = true
            //};

            //// Act
            //target.CommunicatieVormToevoegen(TESTGPID, commDetail);

            //// Assert

            //// De nieuwe communicatievorm zal wel bewaard zijn.  Maar de
            //// oorspronkelijke moet ook bewaard zijn, want die is zijn
            //// voorkeur verloren.

            //Assert.AreEqual("bewaard", testCommunicatieVorm.Nota);
        }


        // Tests die nagingen of gewijzigde entiteiten wel bewaard werden, moeten we niet meer doen, want voor
        // change tracking gebruiken we entity framework. We kunnen ervan uitgaan dat dat wel fatsoenlijk
        // werkt.


        /// <summary>
        /// Als een gelieerde persoon een account heeft zonder gebruikersrechten, moet deze informatie
        /// ook opgeleverd worden door AlleDetailsOphalen.
        ///</summary>
        [TestMethod()]
        public void AlleDetailsOphalenTest()
        {
            // arbitraire dingen

            const int someGid = 5;     
            const int someGpid = 3;    
            const string someUsername = "UserName";   
            DateTime someGeboorteDatum = new DateTime(1977,03,08);
            const int someWerkJaar = 2012;

            // arrange

            var gelieerdePersoon = new GelieerdePersoon
                                       {
                                           ID = someGpid,
                                           Persoon =
                                               new Persoon
                                                   {
                                                       GeboorteDatum = someGeboorteDatum,
                                                       Gav =
                                                           new[]
                                                               {
                                                                   new Gav
                                                                       {
                                                                           Login
                                                                               =
                                                                               someUsername
                                                                       }
                                                               }
                                                   },
                                           Groep = new ChiroGroep
                                                       {
                                                           ID = someGid,
                                                           GroepsWerkJaar =
                                                               new[] {new GroepsWerkJaar {WerkJaar = someWerkJaar}}
                                                       }
                                       };


            // IOC opzetten

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            var communicatieVormenManagerMock = new Mock<ICommunicatieVormenManager>();
            var gelieerdePersonenRepoMock = new Mock<IRepository<GelieerdePersoon>>();

            gelieerdePersonenRepoMock.Setup(mck => mck.Select()).Returns((new[] {gelieerdePersoon}).AsQueryable());

            repositoryProviderMock.Setup(mck => mck.RepositoryGet<GelieerdePersoon>())
                                  .Returns(gelieerdePersonenRepoMock.Object);

            Factory.InstantieRegistreren(repositoryProviderMock.Object);
            Factory.InstantieRegistreren(communicatieVormenManagerMock.Object);


            var target = Factory.Maak<GelieerdePersonenService>();

            // act

            int gelieerdePersoonID = someGpid;
            var actual = target.AlleDetailsOphalen(gelieerdePersoonID);

            // assert

            Assert.AreEqual(someUsername, actual.GebruikersInfo.GavLogin);
        }
    }
}
