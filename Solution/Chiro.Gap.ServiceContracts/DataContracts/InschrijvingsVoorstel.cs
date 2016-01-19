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

using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Feedback op een inschrijvingsverzoek
    /// </summary>
    [DataContract]
    public class InschrijvingsVoorstel: InschrijvingsVerzoek
    {
        /// <summary>
        /// De volledige naam van de gelieerde persoon. Dan moeten we die straks niet meer opzoeken :)
        /// </summary>
        [DataMember]
        public string VolledigeNaam { get; set; }

        /// <summary>
        /// Als dit foutnummer niet null is, kan de gelieerde persoon niet ingeschreven worden.
        /// De reden kan hopelijk afgeleid worden van het foutnummer.
        /// </summary>
        [DataMember]
        public FoutNummer? FoutNummer { get; set; }
    }
}
