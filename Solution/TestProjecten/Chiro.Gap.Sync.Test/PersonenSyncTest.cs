using System.Collections.Generic;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Test;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using NUnit.Framework;
using Moq;
using CommunicatieType = Chiro.Gap.Poco.Model.CommunicatieType;

namespace Chiro.Gap.Sync.Test
{
    /// <summary>
    ///This is a test class for PersonenSyncTest and is intended
    ///to contain all PersonenSyncTest Unit Tests
    ///</summary>
    [TestFixture]
    public class PersonenSyncTest: ChiroTest
    {
        /// <summary>
        /// Test of UpdatenOfMaken KipSync effectief aanroept.
        /// </summary>
        [Test]
        public void BewarenMetCommunicatieTest()
        {
            // ARRANGE

            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(src => src.PersoonUpdatenOfMaken(It.IsAny<PersoonDetails>())).Verifiable();
            Factory.InstantieRegistreren(kipSyncMock.Object);

            var gelieerdePersoon = new GelieerdePersoon
                                       {
                                           Persoon = new Poco.Model.Persoon { InSync = true },
                                           Communicatie =
                                               new List<CommunicatieVorm> 
                                               {
                                                   new CommunicatieVorm 
                                                   { 
                                                       CommunicatieType = new CommunicatieType { ID = 3 } 
                                                   }
                                               }
                                       };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);

            // ACT

            var target = Factory.Maak<PersonenSync>();
            target.UpdatenOfMaken(gelieerdePersoon);

            // ASSERT

            kipSyncMock.VerifyAll();
        }
    }
}
