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
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
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
        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);

        /// <summary>
        /// Persoon met recentste lid mag niet crashen als de API geen persoon
        /// oplevert.
        /// </summary>
        [TestMethod]
        public void PersoonMetRecentsteLidOngeldigAd()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(_vandaagZogezegd, out factory, out civiApiMock, out updateHelperMock);

            civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(new ApiResultValues<Contact>());

            // ContactHelper vraagt de API-keys bij constructie. Dat is misschien niet
            // zo'n goed idee. Maar voorlopig doe ik het zo dus ook in deze test.

            var contactWorker = factory.Maak<ContactWorker>();

            // ACT

            var result = contactWorker.PersoonMetRecentsteLid(2, 3);

            // ASSERT

            Assert.IsNull(result);
        }

        /// <summary>
        /// Persoon met recentste membership mag niet crashen als de API geen persoon
        /// oplevert.
        /// </summary>
        [TestMethod]
        public void PersoonMetRecentsteMembershipOngeldigAd()
        {
            // ARRANGE
            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(_vandaagZogezegd, out factory, out civiApiMock, out updateHelperMock);

            civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(new ApiResultValues<Contact>());

            // ContactHelper vraagt de API-keys bij constructie. Dat is misschien niet
            // zo'n goed idee. Maar voorlopig doe ik het zo dus ook in deze test.
            var contactWorker = factory.Maak<ContactWorker>();

            // ACT
            var result = contactWorker.PersoonMetRecentsteMembership(2, MembershipType.Aansluiting);

            // ASSERT
            Assert.IsNull(result);
        }

        /// <summary>
        /// Workers worden per call opnieuw geinstantieerd. Als er een nieuwe ContactWorker
        /// wordt gemaakt, dan is het de bedoeling dat dezelfde cache blijft gebruiken.
        /// </summary>
        [TestMethod]
        public void AdNrCiviIdCacheTest()
        {
            // In principe is deze unit test niet helemaal juist, omdat het 2 dingen door elkaar
            // test: het aanroepen van de cache, en de werking van de cache. Ze zou dus beter
            // opgesplitst worden.

            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(_vandaagZogezegd, out factory, out civiApiMock, out updateHelperMock);

            // Vervang de gemockte cache (uit IocOpzetten) opnieuw door de echte cache.
            factory.InstantieRegistreren<ICiviCache>(new CiviCache());

            var myContact = new Contact
            {
                Id = 1,
                FirstName = "Johan",
                LastName = "Vervloet",
                ExternalIdentifier = "2"
            };

            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == myContact.ExternalIdentifier)))
                .Returns(new ApiResultValues<Contact>(myContact)).Verifiable();

            var contactWorker1 = factory.Maak<ContactWorker>(); 
            var contactWorker2 = factory.Maak<ContactWorker>();

            // ACT

            // vraag twee keer op, via aparte worker
            int? civiId1 = contactWorker1.ContactIdGet(myContact.ExternalIdentifier);
            int? civiId2 = contactWorker2.ContactIdGet(myContact.ExternalIdentifier);

            // ASSERT

            civiApiMock.Verify(src =>
                src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                    It.Is<ContactRequest>(r => r.ExternalIdentifier == myContact.ExternalIdentifier)), Times.Exactly(1));
        }
    }
}
