/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
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

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.Services.Test
{
    /// <summary>
    /// Unit tests voor de BaseService.
    /// 
    /// In praktijk: unit tests voor de mappings :-)
    /// </summary>
    [TestClass]
    public class BaseServiceTest
    {
        [ClassInitialize]
        public static void Init(TestContext ctx)
        {
            Factory.ContainerInit();
            // Een BaseService insantieren definieert de mappings.
            var dummy = Factory.Maak<BaseService>();
        }

        /// <summary>
        /// Test die probeert de leden uit huidig groepswerkjaar te mappen op een lijst van LidInfo.
        /// </summary>
        [TestMethod]
        public void MapLijstLeden()
        {
            var afdelingsJaar = new AfdelingsJaar { Afdeling = new Afdeling() };
            var huidigGwj = new GroepsWerkJaar { Lid = new List<Lid> { new Kind { AfdelingsJaar = afdelingsJaar }, new Kind { AfdelingsJaar = afdelingsJaar } } };
            var lidInfoLijst = Mapper.Map<IEnumerable<Lid>, IList<LidInfo>>(huidigGwj.Lid);

            Assert.IsTrue(lidInfoLijst.Count > 0);
        }

        /// <summary>
        /// Test voor de mapping van een groep naar GroepInfo
        /// </summary>
        [TestMethod]
        public void MapGroepGroepInfo()
        {
            var groep = new ChiroGroep { Code = "tst/0001" };
            GroepInfo gi = Mapper.Map<Groep, GroepInfo>(groep);

            Assert.IsTrue(gi.StamNummer != string.Empty);
        }

        /// <summary>
        /// Controleert mapping Functie -> FunctieDetail
        /// </summary>
        [TestMethod]
        public void MapFunctieInfo()
        {
            var functie = new Functie { Code = "BLA" };

            FunctieDetail fi = Mapper.Map<Functie, FunctieDetail>(functie);
            Assert.AreEqual(fi.Code, functie.Code);
        }

        /// <summary>
        /// Controleert of functies goed mee gemapt worden met lidinfo.
        /// </summary>
        [TestMethod]
        public void MapLidInfoFuncties()
        {
            var leiderJos = new Leiding { Functie = new List<Functie> { new Functie() } };
            LidInfo li = Mapper.Map<Lid, LidInfo>(leiderJos);
            Assert.IsTrue(li.Functies.Any());
        }

        /// <summary>
        /// Controleert of lid/leiding goed wordt gemapt van Deelnemer naar DeelnemerDetail.
        /// </summary>
        [TestMethod]
        public void MapDeelnemerUitstapDitJaar()
        {
            // ARRANGE

            var groep = new ChiroGroep();
            var vorigJaar = new GroepsWerkJaar { Groep = groep, WerkJaar = 2012 };
            var ditJaar = new GroepsWerkJaar { Groep = groep, WerkJaar = 2013 };
            var gelieerdePersoon = new GelieerdePersoon { Groep = groep, Persoon = new Persoon() };
            var lidVorigJaar = new Kind
            {
                GroepsWerkJaar = vorigJaar,
                GelieerdePersoon = gelieerdePersoon,
                AfdelingsJaar = new AfdelingsJaar { Afdeling = new Afdeling() }
            };
            var lidDitJaar = new Leiding { GroepsWerkJaar = ditJaar, GelieerdePersoon = gelieerdePersoon };
            gelieerdePersoon.Lid = new List<Lid> { lidVorigJaar, lidDitJaar };

            var uitstap = new Uitstap
            {
                DatumVan = new DateTime(ditJaar.WerkJaar + 1, 07, 1),
                DatumTot = new DateTime(ditJaar.WerkJaar + 1, 07, 10)
            };
            var deelnemer = new Deelnemer { Uitstap = uitstap, GelieerdePersoon = gelieerdePersoon, IsLogistieker = false };

            // ACT

            var result = Mapper.Map<Deelnemer, DeelnemerDetail>(deelnemer);

            // ASSERT

            Assert.AreEqual(DeelnemerType.Begeleiding, result.Type);
        }

        /// <summary>
        /// Controleert of lid/leiding goed wordt gemapt van Deelnemer naar DeelnemerDetail.
        /// </summary>
        [TestMethod]
        public void MapDeelnemerUitstapVorigJaar()
        {
            // ARRANGE

            var groep = new ChiroGroep();
            var vorigJaar = new GroepsWerkJaar { Groep = groep, WerkJaar = 2012 };
            var ditJaar = new GroepsWerkJaar { Groep = groep, WerkJaar = 2013 };
            var gelieerdePersoon = new GelieerdePersoon { Groep = groep, Persoon = new Persoon() };
            var lidVorigJaar = new Kind
            {
                GroepsWerkJaar = vorigJaar,
                GelieerdePersoon = gelieerdePersoon,
                AfdelingsJaar = new AfdelingsJaar { Afdeling = new Afdeling() }
            };
            var lidDitJaar = new Leiding { GroepsWerkJaar = ditJaar, GelieerdePersoon = gelieerdePersoon };
            gelieerdePersoon.Lid = new List<Lid> { lidDitJaar, lidVorigJaar };

            var uitstap = new Uitstap
            {
                DatumVan = new DateTime(vorigJaar.WerkJaar + 1, 07, 1),
                DatumTot = new DateTime(vorigJaar.WerkJaar + 1, 07, 10)
            };
            var deelnemer = new Deelnemer { Uitstap = uitstap, GelieerdePersoon = gelieerdePersoon, IsLogistieker = false };

            // ACT

            var result = Mapper.Map<Deelnemer, DeelnemerDetail>(deelnemer);

            // ASSERT

            Assert.AreEqual(DeelnemerType.Deelnemer, result.Type);
        }

        /// <summary>
        /// Controleert of lid/leiding goed wordt gemapt van Deelnemer naar DeelnemerDetail.
        /// </summary>
        [TestMethod]
        public void MapDeelnemerUitstapOudLid()
        {
            // ARRANGE

            var groep = new ChiroGroep();
            var vorigJaar = new GroepsWerkJaar { Groep = groep, WerkJaar = 2012 };
            var gelieerdePersoon = new GelieerdePersoon { Groep = groep, Persoon = new Persoon() };
            var lidVorigJaar = new Kind
            {
                GroepsWerkJaar = vorigJaar,
                GelieerdePersoon = gelieerdePersoon,
                AfdelingsJaar = new AfdelingsJaar { Afdeling = new Afdeling() }
            };
            gelieerdePersoon.Lid = new List<Lid> { lidVorigJaar };

            var uitstap = new Uitstap
            {
                DatumVan = new DateTime(vorigJaar.WerkJaar + 2, 07, 1),
                DatumTot = new DateTime(vorigJaar.WerkJaar + 2, 07, 10)
            };
            var deelnemer = new Deelnemer { Uitstap = uitstap, GelieerdePersoon = gelieerdePersoon, IsLogistieker = false };

            // ACT

            var result = Mapper.Map<Deelnemer, DeelnemerDetail>(deelnemer);

            // ASSERT

            Assert.AreEqual(DeelnemerType.Onbekend, result.Type);
            // Deelnemer voor bivak dit jaar, die dit jaar geen lid is, krijgt type 'onbekend'
        }

        /// <summary>
        /// Controleert of de afdeling van een deelnemer goed wordt gemapt.
        /// </summary>
        [TestMethod]
        public void MapDeelnemerAfdeling()
        {
            // ARRANGE

            var groep = new ChiroGroep();
            var vorigJaar = new GroepsWerkJaar { Groep = groep, WerkJaar = 2012 };
            var ditJaar = new GroepsWerkJaar { Groep = groep, WerkJaar = 2013 };
            var gelieerdePersoon = new GelieerdePersoon { Groep = groep, Persoon = new Persoon() };
            var lidVorigJaar = new Kind
            {
                GroepsWerkJaar = vorigJaar,
                GelieerdePersoon = gelieerdePersoon,
                AfdelingsJaar = new AfdelingsJaar { Afdeling = new Afdeling { ID = 1 } }
            };
            var lidDitJaar = new Kind
            {
                GroepsWerkJaar = ditJaar,
                GelieerdePersoon = gelieerdePersoon,
                AfdelingsJaar = new AfdelingsJaar { Afdeling = new Afdeling { ID = 2 } }
            };
            gelieerdePersoon.Lid = new List<Lid> { lidVorigJaar, lidDitJaar };

            var uitstap = new Uitstap
            {
                DatumVan = new DateTime(ditJaar.WerkJaar + 1, 07, 1),
                DatumTot = new DateTime(ditJaar.WerkJaar + 1, 07, 10)
            };
            var deelnemer = new Deelnemer { Uitstap = uitstap, GelieerdePersoon = gelieerdePersoon, IsLogistieker = false };

            // ACT

            var result = Mapper.Map<Deelnemer, DeelnemerDetail>(deelnemer);

            // ASSERT

            Assert.AreEqual(2, result.Afdelingen.First().ID);
        }
    }
}
