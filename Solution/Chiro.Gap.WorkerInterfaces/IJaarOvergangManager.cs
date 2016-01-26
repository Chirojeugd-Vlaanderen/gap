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
﻿using System.Collections.Generic;
﻿using Chiro.Cdf.Poco;
﻿using Chiro.Gap.Poco.Model;
﻿using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IJaarOvergangManager
    {
        /// <summary>
        /// Maakt voor de groep met de opgegeven <paramref name="groepID"/> een nieuw werkJaar aan
        /// en maakt daarin de opgegeven afdelingen aan, met hun respectieve leeftijdsgrenzen (geboortejaren).
        /// </summary>
        /// <param name="teActiveren">
        /// De afdelingen die geactiveerd moeten worden, met ingestelde geboortejaren 
        /// </param>
        /// <param name="groepID">
        /// De ID van de groep die de jaarovergang uitvoert
        /// </param>
        /// <exception cref="GapException">
        /// Komt voor wanneer de jaarvergang te vroeg gebeurt.
        /// </exception>
        /// <exception cref="FoutNummerException">
        /// Komt voor als er een afdeling bij zit die niet gekend is in de groep, of als er een afdeling gekoppeld is
        /// aan een onbestaande nationale afdeling. Ook validatiefouten worden op deze manier doorgegeven.
        /// </exception>
        /// <remarks>Er worden geen leden gemaakt door deze method.</remarks>
        void JaarOvergangUitvoeren(IList<AfdelingsJaarDetail> teActiveren, Groep groep, IRepository<OfficieleAfdeling> officieleAfdelingenRepo);
    }
}