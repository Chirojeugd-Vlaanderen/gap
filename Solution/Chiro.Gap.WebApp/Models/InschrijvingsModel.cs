/*
 * Copyright 2014 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using System.Collections.Generic;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Informatie nodig voor een inschrijving van meerdere personen.
    /// </summary>
    public class InschrijvingsModel: MasterViewModel
    {
        /// <summary>
        /// GelieerdePersoonID's van de in te schrijven personen.
        /// </summary>
        public List<int> GelieerdePersoonIDs { get; set; }

        /// <summary>
        /// Inschrijvingsgegevens voor de in te schrijven personen.
        /// </summary>
        public List<InschrijfbaarLid> Inschrijvingen { get; set; }

        /// <summary>
        /// Afdeling(sjar)en beschikbaar voor het werkjaar van de inschrijving.
        /// </summary>
        public List<ActieveAfdelingInfo> BeschikbareAfdelingen { get; set; }
    }
}