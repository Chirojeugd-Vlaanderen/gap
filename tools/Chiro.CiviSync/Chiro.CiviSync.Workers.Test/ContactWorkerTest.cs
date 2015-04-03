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
using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Services.Test;
using Chiro.Gap.UpdateApi.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Workers.Test
{
    [TestClass]
    public class ContactWorkerTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private Mock<IGapUpdateClient> _updateHelperMock;

        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);

        [TestInitialize]
        public void InitializeTest()
        {
            TestHelper.IocOpzetten(_vandaagZogezegd, out _civiApiMock, out _updateHelperMock);
        }

        /// <summary>
        /// Persoon met recentste lid mag niet crashen als de API geen persoon
        /// oplevert.
        /// </summary>
        [TestMethod]
        public void PersoonMetRecentsteLidOngeldigAd()
        {
            // ARRANGE

            // Als de API niets vindt, levert GetSingle een leeg object.
            _civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(new Contact());

            // ContactHelper vraagt de API-keys bij constructie. Dat is misschien niet
            // zo'n goed idee. Maar voorlopig doe ik het zo dus ook in deze test.

            var serviceHelper = Factory.Maak<ServiceHelper>();
            var contactHelper = new ContactWorker(serviceHelper);

            // ACT

            var result = contactHelper.PersoonMetRecentsteLid(2, 3);

            // ASSERT

            Assert.IsNull(result);
        }
    }
}
