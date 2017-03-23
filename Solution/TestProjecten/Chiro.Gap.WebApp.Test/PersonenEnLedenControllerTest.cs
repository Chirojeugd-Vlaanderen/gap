/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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

using System;
using System.Collections.Generic;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.Test;
using Chiro.Gap.WebApp.Controllers;
using NUnit.Framework;
using Moq;

namespace Chiro.Gap.WebApp.Test
{
    /// <summary>
    /// Fake ledenservice.  Ik kan moq niet gebruiken, omdat moq niet overweg kan
    /// met de out-parameter van Inschrijven.
    /// </summary>
    internal class FakeLedenService: ILedenService
    {
        public static IEnumerable<InschrijvingsVerzoek> DoorgekregenInschrijving { get; set; }

        #region irrelevante methods voor deze test

        public List<InschrijvingsVoorstel> InschrijvingVoorstellen(IList<int> gelieerdePersoonIDs)
        {
            throw new NotImplementedException();
        }

        public List<InschrijvingsVoorstel> Inschrijven(IList<InschrijvingsVerzoek> inschrijfInfo)
        {
            DoorgekregenInschrijving = inschrijfInfo;
            return new List<InschrijvingsVoorstel>();
        }

        public void Uitschrijven(IList<int> gelieerdePersoonIDs, out string foutBerichten)
        {
            throw new NotImplementedException();
        }

        public void FunctiesVervangen(int lidID, IEnumerable<int> functieIDs)
        {
            throw new NotImplementedException();
        }

        public LidAfdelingInfo AfdelingenOphalen(int lidID)
        {
            throw new NotImplementedException();
        }

        public int AfdelingenVervangen(int lidID, IList<int> afdelingsJaarIDs)
        {
            throw new NotImplementedException();
        }

        public void AfdelingenVervangenBulk(IList<int> lidIDs, IList<int> afdelingsJaarIDs)
        {
            throw new NotImplementedException();
        }

        public int LoonVerliesVerzekeren(int lidID)
        {
            throw new NotImplementedException();
        }

        public PersoonLidInfo DetailsOphalen(int lidID)
        {
            throw new NotImplementedException();
        }

        public PersoonInfo PersoonOphalen(int lidID)
        {
            throw new NotImplementedException();
        }

        public LidInfo LidInfoOphalen(int lidID)
        {
            throw new NotImplementedException();
        }

        public List<LidOverzicht> LijstZoekenLidOverzicht(LidFilter filter, bool metAdressen)
        {
            throw new NotImplementedException();
        }

        public List<PersoonLidInfo> LijstZoekenPersoonLidInfo(LidFilter filter)
        {
            throw new NotImplementedException();
        }

        public List<PersoonLidInfo> ActieveLedenOphalen(int GroepId)
        {
            throw new NotImplementedException();
        }

        public int LidGeldToggle(int id)
        {
            throw new NotImplementedException();
        }

        public int TypeToggle(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    ///This is a test class for PersonenEnLedenControllerTest and is intended
    ///to contain all PersonenEnLedenControllerTest Unit Tests
    ///</summary>
    [TestFixture]
    public class PersonenEnLedenControllerTest: ChiroTest
    {
        internal virtual PersonenEnLedenController CreatePersonenEnLedenController()
        {
            // TODO Instantiate an appropriate concrete class
            PersonenEnLedenController target = null;
            return target;
        }


        ///<summary>
        /// Test of leden uitschrijven de functieproblemencache cleart
        ///</summary>
        [Test]
        public void FunctieProblemenClearenNaUitschrijven()
        {
            const int GROEPID = 426;            // arbitrair

            // setup mocks

            var veelGebruiktMock = new Mock<IVeelGebruikt>();
            veelGebruiktMock.Setup(src => src.LedenProblemenResetten(GROEPID));

            var ledenServiceMock = new Mock<ILedenService>();
            var channelProviderMock = new Mock<IChannelProvider>();
            channelProviderMock.Setup(mock => mock.GetChannel<ILedenService>()).Returns(ledenServiceMock.Object);

            Factory.InstantieRegistreren(veelGebruiktMock.Object);
            Factory.InstantieRegistreren(channelProviderMock.Object);

            // Ik maak een PersonenController, omdat PersonenEnLedenController abstract is, en dus 
            // niet als dusdanig kan worden geïnstantieerd.

            var target = Factory.Maak<PersonenController>();


            // act

            target.Uitschrijven(1, GROEPID);

            // assert

            veelGebruiktMock.Verify(src => src.LedenProblemenResetten(It.IsAny<int>()), Times.AtLeastOnce());

        }
    }
}
