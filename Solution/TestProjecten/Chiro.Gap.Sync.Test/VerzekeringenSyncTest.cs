using Chiro.Gap.Poco.Model;
using Chiro.Gap.Test;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using NUnit.Framework;
using Moq;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync.Test
{
    /// <summary>
    ///This is a test class for VerzekeringenSyncTest and is intended
    ///to contain all VerzekeringenSyncTest Unit Tests
    ///</summary>
    [TestFixture]
    public class VerzekeringenSyncTest: ChiroTest
    {
        /// <summary>
        /// Kijkt na of verzekeringen loonverlies ook gesynct worden voor personen zonder AD-nummer
        /// (dat AD-nummer is dan waarschijnlijk in aanvraag.)
        /// </summary>
        [Test]
        public void BewarenTest()
        {
            // ARRANGE

            var groepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep()};
            var gelieerdePersoon = new GelieerdePersoon
            {
                Groep = groepsWerkJaar.Groep,
                Persoon = new Persoon
                {
                    AdNummer = null,
                    InSync = true,
                }
            };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);
            groepsWerkJaar.Groep.GelieerdePersoon.Add(gelieerdePersoon);
                                                                                                          
            var persoonsVerzekering = new PersoonsVerzekering
                                          {
                                              Persoon = gelieerdePersoon.Persoon
                                          };

            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(
                src =>
                src.LoonVerliesVerzekerenAdOnbekend(It.IsAny<PersoonDetails>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>())).Verifiable();
            Factory.InstantieRegistreren(kipSyncMock.Object);

            // ACT
            var target = Factory.Maak<VerzekeringenSync>();
            target.Bewaren(persoonsVerzekering, groepsWerkJaar);

            // ASSERT

            kipSyncMock.VerifyAll();
        }
    }
}
