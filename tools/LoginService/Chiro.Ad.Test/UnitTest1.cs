/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;

using Chiro.Ad.ServiceContracts;
using Chiro.Cdf.ServiceHelper;

namespace Chiro.Ad.Test
{
    // Testmogelijkheden zijn momenteel beperkt, omdat er nog geen IOC ondersteund wordt.

    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Een zeer domme test om te kijken of de WCF-service iets doet.
        /// Probeer een account aan te maken met een ongeldig adres, en verwacht een exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void ServiceAanroepTest()
        {
            var serviceHelper = new ServiceHelper(new ChannelFactoryChannelProvider());
            serviceHelper.CallService<IAdService, string>(client => client.GapLoginAanvragen(39198, "Johan", "Vervloet", "johan.vervloet@chiro"));
        }
    }
}
