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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.Diagnostics.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontractje met groepsinformatie en e-mailadressen van contactpersonen
    /// </summary>
    [DataContract]
    public class GroepContactInfo
    {
        /// <summary>
        /// Naam van de groep
        /// </summary>
        [DataMember]
        public string Naam { get; set; }

        /// <summary>
        /// Stamnummer van de groep
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// Plaats (gemeente, parochie,...) van de groep
        /// </summary>
        [DataMember]
        public string Plaats { get; set; }

        /// <summary>
        /// Contactpersonen met e-mailadres
        /// </summary>
        [DataMember]
        public IEnumerable<MailContactInfo> Contacten { get; set; }
    }
}
