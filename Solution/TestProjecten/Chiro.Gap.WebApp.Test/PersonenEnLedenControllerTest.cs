/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Chiro.Gap.WebApp.Models;
using System.Web.Mvc;

using Moq;

namespace Chiro.Gap.WebApp.Test
{
    /// <summary>
    /// Fake ledenservice.  Ik kan moq niet gebruiken, omdat moq niet overweg kan
    /// met de out-parameter van Inschrijven.
    /// </summary>
    internal class FakeLedenService: ILedenService
    {
        public static IEnumerable<InTeSchrijvenLid> DoorgekregenInschrijving { get; set; }

        #region irrelevante methods voor deze test

        public List<InTeSchrijvenLid> VoorstelTotInschrijvenGenereren(IList<int> gelieerdePersoonIDs, out string foutBerichten)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> Inschrijven(InTeSchrijvenLid[] lidInformatie, out string foutBerichten)
        {
            DoorgekregenInschrijving = lidInformatie;
            foutBerichten = String.Empty;
            return null;
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

        public List<LidOverzicht> Zoeken(LidFilter filter, bool metAdressen)
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
    [TestClass()]
    public class PersonenEnLedenControllerTest
    {
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Factory.ContainerInit();
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        internal virtual PersonenEnLedenController CreatePersonenEnLedenController()
        {
            // TODO Instantiate an appropriate concrete class
            PersonenEnLedenController target = null;
            return target;
        }

        ///<summary>
        ///Test of je met de webinterface leiding kunt maken zonder afdeling
        ///</summary>
        [TestMethod()]
        public void LedenMakenTest()
        {
            // Het inschrijven van leden en leiding via de webinterface is nogal vreemd
            // geïmplementeerd.  Je mag namelijk maar 1 afdeling(sjaar) kiezen, en dat kan ook
            // 'geen' zijn.  Dat gekozen item moet dan geconverteerd worden naar een lijst met
            // afdelingsjaarID's, die dan eventueel nul mag zijn.
            //
            // Als leden maken herwerkt wordt, dan vervalt natuurlijk deze test.

            const int GROEPID = 426;            // arbitrair
            const int WERKJAAR = 2011;          // werkJaar 2011-2012, om iets te doen
            const int GROEPSWERKJAARID = 2971;  // arbitrair

            // setup mocks
            // (dit is nogal hetzelfde in veel tests.  Best eens afsplitsen)

            var veelGebruiktMock = new Mock<IVeelGebruikt>();
            veelGebruiktMock.Setup(vg => vg.GroepsWerkJaarOphalen(GROEPID)).Returns(new GroepsWerkJaarDetail
                                                                                        {
                                                                                            GroepID = GROEPID,
                                                                                            WerkJaar = WERKJAAR,
                                                                                            WerkJaarID =
                                                                                                GROEPSWERKJAARID
                                                                                        });
            veelGebruiktMock.Setup(vg => vg.BivakStatusHuidigWerkjaarOphalen(GROEPID)).Returns(new BivakAangifteLijstInfo
                                                                                                   {
                                                                                                       AlgemeneStatus =
                                                                                                           BivakAangifteStatus.NogNietVanBelang
                                                                                                   });


            Factory.InstantieRegistreren(veelGebruiktMock.Object);
            Factory.InstantieRegistreren<ILedenService>(new FakeLedenService());

            // we doen het volgende:
            // We roepen LedenMaken aan met een model waarbij '0' geselecteerd is als enige afdeling voor
            // een leid(st)er, wat dus wil zeggen dat we een leid(st)er maken zonder afdeling.
            // We verifieren dat de ledenservice niet wordt aangeroepen met dat afdelingsjaar 0.

            // Ik maak een LedenController, omdat PersonenEnLedenController abstract is, en dus 
            // niet als dusdanig kan worden geïnstantieerd.

            var target = Factory.Maak<LedenController>();
            var model = new GeselecteerdePersonenEnLedenModel
                            {
                                PersoonEnLidInfos = new[]
                                                        {
                                                            new InschrijfbaarLid
                                                                {
                                                                    AfdelingsJaarIDs = new int[] {0},
                                                                    LeidingMaken = true,
                                                                    InTeSchrijven = true
                                                                }
                                                        }
                            };

            // act

            target.LedenMaken(model, GROEPID);

            // assert

            var probleem = from p in FakeLedenService.DoorgekregenInschrijving
                           where p.AfdelingsJaarIDs.Any(ajid => ajid == 0)
                           select p;

            Assert.IsNull(probleem.FirstOrDefault());

        }
    }
}
