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
using System.ServiceModel;
using System.Web;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Updatet de afdelingen van een lid.
        /// </summary>
        /// <param name="persoon">
        /// Persoon waarvan de afdelingen geupdatet moeten worden
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van de groep waarin de persoon lid is
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar waarin de persoon lid is
        /// </param>
        /// <param name="afdelingen">
        /// Toe te kennen afdelingen.  Eventuele andere reeds toegekende functies worden verwijderd.
        /// </param>
        /// <remarks>
        /// Er is in Kipadmin maar plaats voor 2 afdelingen/lid
        /// </remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AfdelingenUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<AfdelingEnum> afdelingen)
        {
            throw new NotImplementedException();
        }
    }
}