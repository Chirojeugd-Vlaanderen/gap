using System.Collections.Generic;
using System.Linq;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Test;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using NUnit.Framework;
using Moq;
using Adres = Chiro.Kip.ServiceContracts.DataContracts.Adres;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync.Test
{
    
    
    /// <summary>
    ///This is a test class for AdressenSyncTest and is intended
    ///to contain all AdressenSyncTest Unit Tests
    ///</summary>
    [TestFixture]
    public class AdressenSyncTest: ChiroTest
    {
        /// <summary>
        ///A test for StandaardAdressenBewaren
        ///</summary>
        [Test]
        public void StandaardAdressenBewarenTest()
        {
            // ARRANGE

            var adres = new BuitenLandsAdres {Land = new Land()};

            var persoonsAdres1 = new PersoonsAdres {Adres = adres, Persoon = new Persoon{InSync = true}};
            var persoonsAdres2 = new PersoonsAdres { Adres = adres, Persoon = new Persoon{InSync = true}};

            adres.PersoonsAdres.Add(persoonsAdres1);
            adres.PersoonsAdres.Add(persoonsAdres2);

            IEnumerable<Bewoner> teSyncenBewoners = null;

            // we mocken kipsync; registreer te syncen bewoners
            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(
                src =>
                    src.StandaardAdresBewaren(It.IsAny<Adres>(),
                        It.IsAny<IEnumerable<Bewoner>>()))
                .Callback<Adres, IEnumerable<Bewoner>>(
                    (a, b) => teSyncenBewoners = b);

            Factory.InstantieRegistreren(kipSyncMock.Object);
            
            // ACT

            var target = Factory.Maak<AdressenSync>(); 
            target.StandaardAdressenBewaren(new List<PersoonsAdres>{persoonsAdres1});

            // ASSERT
            
            Assert.IsNotNull(teSyncenBewoners);
            Assert.AreEqual(1, teSyncenBewoners.Count());
        }
    }
}
