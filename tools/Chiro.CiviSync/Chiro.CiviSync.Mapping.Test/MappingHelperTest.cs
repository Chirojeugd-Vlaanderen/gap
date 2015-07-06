/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using AutoMapper;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.CiviSync.Mapping.Test
{
    [TestClass]
    public class MappingHelperTest
    {
        /// <summary>
        /// Eens nakijken of GAP-Uitstap-ID wel goed wordt gemapt.
        /// </summary>
        [TestMethod]
        public void BivakNaarEventRequest()
        {
            // arrange

            MappingHelper.MappingsDefinieren();
            var bivak = new Bivak
            {
                DatumVan = new DateTime(2015, 07, 01),
                DatumTot = new DateTime(2015, 07, 11),
                Naam = "Op kamp bij de trappisten",
                Opmerkingen = "Dit wordt uiteraard een erg ingetogen bivak.",
                StamNummer = "MG /0102",
                UitstapID = 1,
                WerkJaar = 2015
            };

            // act

            var result = Mapper.Map<Bivak, EventRequest>(bivak);

            // assert

            Assert.AreEqual(result.GapUitstapId, bivak.UitstapID);
        }
    }
}
