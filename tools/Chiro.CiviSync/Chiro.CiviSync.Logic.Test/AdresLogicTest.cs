/*
   Copyright 2017 Chirojeugd-Vlaanderen vzw

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


using Chiro.Kip.ServiceContracts.DataContracts;
using NUnit.Framework;

namespace Chiro.CiviSync.Logic.Test
{
    /// <summary>
    /// Tests voor AdresLogic.
    /// </summary>
    [TestFixture]
    public class AdresLogicTest
    {
        [Test]
        public void BuitenlandseProvincieMappen()
        {
            var adres = new Adres
            {
                Straat = "Vosselmanstraat",
                HuisNr = 299,
                PostNr = "7311 CL",
                WoonPlaats = "Apeldoorn",
                Land = "Nederland"
            };

            int? result = AdresLogic.ProvincieId(adres);

            Assert.IsNull(result);
        }
    }
}