/*
   Copyright 2016 Chirojeugd-Vlaanderen vzw

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
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Gap.UpdateApi.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    // Er zitten te veel zaken in dit soort van testklasses die in elke
    // testklasse zitten. Ik denk dat ik beter een soort van base clas maak,
    // waarvan ik dan iedere keer kan erven.

    [TestClass]
    public class WerkjaarAfsluitenTest
    {
        private readonly DateTime _vandaagZogezegd = new DateTime(2016, 8, 13);
        private readonly int _huidigWerkjaar = 2015;

        [ClassInitialize]
        public static void InitializeTestClass(TestContext c)
        {
            TestHelper.MappingsCreeren();
        }

        [TestMethod]
        public void RelatieTypeBijWerkjaarAfsluiten()
        {
            // ARRANGE 

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(_vandaagZogezegd, out factory, out civiApiMock, out updateHelperMock);

            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };

            // Mocking opzetten.
            // De ploeg bij het stamnummer zal opgezocht worden.
            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier)))
                .Returns(new ApiResultValues<Contact>(ploeg));
            // We hebben nu een API action in ChiroCivi om het werkjaar af te sluiten.
            civiApiMock.Setup(
                src => src.ChiroWerkjaarAfsluiten(It.IsAny<String>(), It.IsAny<string>(),
                    It.Is<ChiroWerkjaarRequest>(
                        r =>
                            r.StamNummer == ploeg.ExternalIdentifier && r.Werkjaar == _huidigWerkjaar)))
                .Returns(new ApiResultValues<Relationship>());

            // ACT
            var service = factory.Maak<SyncService>();
            service.GroepsWerkjaarAfsluiten(ploeg.ExternalIdentifier, _huidigWerkjaar);

            // ASSERT

            civiApiMock.Verify(src => src.ChiroWerkjaarAfsluiten(It.IsAny<String>(), It.IsAny<string>(),
                It.Is<ChiroWerkjaarRequest>(
                    r =>
                        r.StamNummer == ploeg.ExternalIdentifier && r.Werkjaar == _huidigWerkjaar)), Times.AtLeastOnce);
        }
    }
}
