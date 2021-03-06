/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Test;
using NUnit.Framework;

namespace Chiro.Gap.Validatie.Test
{
    /// <summary>
    /// Testclass CommunicatieVormValideren
    /// </summary>
    [TestFixture]
    public class CommunicatieVormValideren: ChiroTest
    {
        [Test]
        public void TestOngeldigTelefoonnummerValideren()
        {
            // Arrange
            var cvValid = new CommunicatieVormValidator();
            var comm = new CommunicatieVorm
            {
                CommunicatieType = new CommunicatieType
                {
                    ID = (int) CommunicatieTypeEnum.TelefoonNummer,
                    Validatie = @"^0[0-9]{1,2}\-[0-9]{2,3} ?[0-9]{2} ?[0-9]{2}$|^04[0-9]{2}\-[0-9]{2,3} ?[0-9]{2} ?[0-9]{2}$|^[+][0-9]*$"
                },
                Nummer = "03/231.07.95"
            };

            // Act
            bool vlag = cvValid.Valideer(comm);

            // Assert
            Assert.IsFalse(vlag);
        }
    }
}
