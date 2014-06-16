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
    /// Informatie over een gelieerde persoon die via e-mail te bereiken is
    /// </summary>
    [DataContract]
    public class MailContactInfo
    {
        /// <summary>
        /// GelieerdePersoonID van de gelieerde persoon
        /// </summary>
        [DataMember]
        public int GelieerdePersoonID { get; set; }

        /// <summary>
        /// Voornaam en familienaam van de gelieerde persoon
        /// </summary>
        [DataMember]
        public string VolledigeNaam { get; set; }

        /// <summary>
        /// E-mailadres van de gelieerde persoon
        /// </summary>
        [DataMember]
        public string EmailAdres { get; set; }

        /// <summary>
        /// Als (en slechts als) <c>true</c>, dan is de gelieerde persoon GAV van zijn groep.
        /// </summary>
        [DataMember]
        public bool IsGav { get; set; }

        /// <summary>
        /// Als (en slechts als) <c>true</c>, dan is de gelieerde persoon contactpersoon van zijn groep.
        /// </summary>
        [DataMember]
        public bool IsContact { get; set; }
    }
}
