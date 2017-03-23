using System.Collections.Generic;
using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Test;
using NUnit.Framework;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    ///This is a test class for AdressenManagerTest and is intended
    ///to contain all AdressenManagerTest Unit Tests
    ///</summary>
    [TestFixture]
    public class AdressenManagerTest: ChiroTest
    {
        /// <summary>
        /// Controleert of ZoekenOfMaken wel degelijk rekening houdt
        /// met busnummers.
        ///</summary>
        [Test]
        public void ZoekenOfMakenBusTest1()
        {
            // ARRANGE

            var adres = new BelgischAdres
                        {
                            ID = 1,
                            StraatNaam = new StraatNaam {Naam = "Kipdorp", PostNummer = 2000},
                            HuisNr = 30,
                            WoonPlaats = new WoonPlaats {Naam = "Antwerpen", PostNummer = 2000}
                        };

            // ACT

            var target = Factory.Maak<AdressenManager>();
            var adresInfo = new AdresInfo
                            {
                                StraatNaamNaam = adres.StraatNaam.Naam,
                                HuisNr = adres.HuisNr,
                                Bus = "B", // iets anders.
                                PostNr = adres.StraatNaam.PostNummer,
                                WoonPlaatsNaam = adres.WoonPlaats.Naam
                            };

            var adressen = new List<Adres> {adres}.AsQueryable();
            var straatNamen = new List<StraatNaam> {adres.StraatNaam}.AsQueryable();
            var woonPlaatsen = new List<WoonPlaats> {adres.WoonPlaats}.AsQueryable();
            var landen = new List<Land>().AsQueryable();

            var actual = target.ZoekenOfMaken(adresInfo, adressen, straatNamen, woonPlaatsen, landen);

            // ASSERT

            Assert.AreNotEqual(adres, actual);
        }
        /// <summary>
        /// Controleert of ZoekenOfMaken voor de busnummers null en leeg gelijkschakelt
        ///</summary>
        [Test]
        public void ZoekenOfMakenBusTest2()
        {
            // ARRANGE

            var adres = new BelgischAdres
            {
                ID = 1,
                StraatNaam = new StraatNaam { Naam = "Kipdorp", PostNummer = 2000 },
                HuisNr = 30,
                Bus = null,
                WoonPlaats = new WoonPlaats { Naam = "Antwerpen", PostNummer = 2000 }
            };

            // ACT

            var target = Factory.Maak<AdressenManager>();
            var adresInfo = new AdresInfo
            {
                StraatNaamNaam = adres.StraatNaam.Naam,
                HuisNr = adres.HuisNr,
                Bus = "",
                PostNr = adres.StraatNaam.PostNummer,
                WoonPlaatsNaam = adres.WoonPlaats.Naam
            };

            var adressen = new List<Adres> { adres }.AsQueryable();
            var straatNamen = new List<StraatNaam> { adres.StraatNaam }.AsQueryable();
            var woonPlaatsen = new List<WoonPlaats> { adres.WoonPlaats }.AsQueryable();
            var landen = new List<Land>().AsQueryable();

            var actual = target.ZoekenOfMaken(adresInfo, adressen, straatNamen, woonPlaatsen, landen);

            // ASSERT

            Assert.AreEqual(adres, actual);
        }
    }
}
