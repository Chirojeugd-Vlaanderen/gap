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
﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Dummies;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers;
using Chiro.Gap.Validatie;
using Chiro.Gap.Services;
using Chiro.Adf.ServiceModel;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.Workers.Test
{
    // TODO: verplaatsen naar een ander project (ticket #160)
    // Deze Workers-tests moeten de workers testen, en niet de DAO. (refs #129)

    /// <summary>
    /// Testclass CommunicatieVormValideren
    /// </summary>
    [TestClass]
    public class CommunicatieVormValideren
    {
        PersonenManager _pMgr;
        GelieerdePersoon _jos;
        CommunicatieType _type;
        CommunicatieVorm _comm;
        CommunicatieVormValidator _cvValid;

        public CommunicatieVormValideren()
        {
        }

        [ClassInitialize()]
        public static void TestClassInitialiseren(TestContext testContext)
        {
            Factory.ContainerInit();
        }

        [ClassCleanup()]
        public static void TestClassOpruimen()
        {
            Factory.Dispose();
        }

        [TestInitialize()]
        public void TestInitialiseren()
        {
            _pMgr = Factory.Maak<PersonenManager>();
            _jos = DummyData.GelieerdeJos;
        }

        [TestCleanup()]
        public void TestOpruimen() { }


        [TestMethod]
        public void TestOngeldigTelefoonnummerValideren()
        {
            // Arrange
            _cvValid = new CommunicatieVormValidator();
            _comm = new CommunicatieVorm();
            _comm.CommunicatieType = ServiceHelper.CallService<IGelieerdePersonenService, CommunicatieType>(l => l.CommunicatieTypeOphalen(1));
            _comm.Nummer = "03/231.07.95";

            // Act
            bool vlag = _cvValid.Valideer(_comm);

            // Assert
            Assert.IsFalse(vlag);
        }
    }
}