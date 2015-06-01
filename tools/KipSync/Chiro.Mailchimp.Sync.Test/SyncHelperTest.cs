/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Mailchimp.Sync.Test
{
    /// <summary>
    /// Zeggen dat dit unit tests zijn, is er wat over. Maar tests zijn het wel.
    /// </summary>
    [TestClass]
    public class SyncHelperTest
    {
        [TestMethod]
        public void AbonnementSyncenTest()
        {
            var target = new SyncHelper();
            var info = new AbonnementInfo
            {
                EmailAdres = "helpdesk@chiro.be",
                AbonnementType = 1,
                Naam = "Help",
                VoorNaam = "Desk"
            };
            target.AbonnementSyncen(info);
        }
    }
}
