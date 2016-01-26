/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
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

using System.Collections.Generic;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    /// <summary>
    /// Class voor een combinatie van gegevens die bepalen hoe iemand ingeschreven wordt
    /// </summary>
    public class LidVoorstel
    {
        /// <summary>
        /// De gelieerde persoon die lid moet worden.
        /// </summary>
        public GelieerdePersoon GelieerdePersoon { get; set; }

        /// <summary>
        /// Het groepswerkjaar waarin de gelieerde persoon lid wil worden.
        /// </summary>
        public GroepsWerkJaar GroepsWerkJaar { get; set; }

        /// <summary>
        /// In welk afdelingsjaren het lid moet worden ingeschreven.
        /// </summary>
        public IList<AfdelingsJaar> AfdelingsJaren { get; set; }

        /// <summary>
        /// True als er geen rekening moet worden gehouden met de inhoud van AfdelingsJaren
        /// </summary>
        public bool AfdelingsJarenIrrelevant
        {
            get { return AfdelingsJaren == null; }
            set { AfdelingsJaren = value ? null : AfdelingsJaren ?? new List<AfdelingsJaar>(); }
        }

        /// <summary>
        /// Of de gelieerde persoon moet worden ingeschreven als leiding
        /// </summary>
        public bool LeidingMaken;
    }
}