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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Maakt een persoon met gekend ad-nummer lid, of updatet een bestaand lid
        /// </summary>
        /// <param name="adNummer">
        /// AD-nummer van het opgegeven lid
        /// </param>
        /// <param name="gedoe">
        /// De nodige info voor het lid.
        /// </param>
        public void LidBewaren(int adNummer, LidGedoe gedoe)
        {
            // TODO (#1767): Implementeren
            // Ik gooi even geen NotImplemented, zodat ik al wel wat kan testen.
        }
    }
}