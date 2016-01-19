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

using System.Collections.Generic;
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
    /// <summary>
    /// Faultcontract voor als er iets mis is met een object.  Dit contract bevat verschillende foutberichten, die
    /// betrekking kunnen hebben op property's van het object.
    /// </summary>
    [DataContract]
    public class OngeldigObjectFault : GapFault
    {
        // TODO (#1041): Dit wordt blijkbaar enkel gebruikt voor adressen.  Is heel die constructie dan wel
        // nodig? Misschien is een AdresFault wel even goed.

        /// <summary>
        /// Dictionary die voor elke fout een dictionary-entry (component, foutbericht) bevat.
        /// </summary>
        [DataMember]
        public Dictionary<string, FoutBericht> Berichten { get; set; }
    }
}
