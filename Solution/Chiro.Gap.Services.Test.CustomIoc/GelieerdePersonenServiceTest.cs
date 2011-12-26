using System.Collections.Generic;

using Chiro.Cdf.Data;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Chiro.Gap.Workers;
using Chiro.Gap.ServiceContracts.DataContracts;

using Moq;

namespace Chiro.Gap.Services.Test.CustomIoc
{
    
    
    /// <summary>
    ///This is a test class for GelieerdePersonenServiceTest and is intended
    ///to contain all GelieerdePersonenServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GelieerdePersonenServiceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        /// run code before running first test
        /// </summary>
        [ClassInitialize()]
        static public void MyClassInitialize(TestContext context)
        {
            MappingHelper.MappingsDefinieren();
        }

        /// <summary>
        /// run code before running each test
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            // Restore IoC settings

            // (Ik had hier ook alle nodige mocks kunnen maken, en dan
            // een voor een registreren.  Maar ContainerInit herleest gewoon
            // de configuratiefile.)

            Factory.ContainerInit();
        }

        /// <summary>
        /// Kijkt na of CommunicatieVormToevoegen enkel de nieuwe communicatievorm
        /// naar Kipadmin synct, ipv alles te vervangen.  
        ///</summary>
        [TestMethod()]
        public void CommunicatieVormToevoegenTest()
        {
            // Arrange

            const int TESTGPID = 1234;

            var communicatieTypeDaoMock = new Mock<IDao<CommunicatieType>>();
            var gelieerdePersonenDaoMock = new Mock<IGelieerdePersonenDao>();

            var communicatieSyncMock = new Mock<ICommunicatieSync>();

            // het communicatietype zal worden opgevraagd, maar is irrelevant voor deze test.
            // zorg er wel voor dat alles valideert.
            communicatieTypeDaoMock.Setup(dao => dao.Ophalen(3)).Returns(new CommunicatieType {Validatie = ".*"});
            // idem voor gelieerde persoon
            gelieerdePersonenDaoMock.Setup(dao => dao.Ophalen(It.IsAny<IEnumerable<int>>(), It.IsAny<PersoonsExtras>()))
                .Returns(new[] {new GelieerdePersoon{Persoon = new Persoon()}});

            // verwacht dat CommunicatieSync.Toevoegen wordt aangeroepen.
            communicatieSyncMock.Setup(snc => snc.Toevoegen(It.IsAny<CommunicatieVorm>())).Verifiable();

            Factory.InstantieRegistreren(communicatieTypeDaoMock.Object);
            Factory.InstantieRegistreren(gelieerdePersonenDaoMock.Object);
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
    }
}
