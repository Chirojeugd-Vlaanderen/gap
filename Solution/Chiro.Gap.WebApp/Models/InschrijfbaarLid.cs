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

using System;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Gegevens relevant voor een inschrijving, aangevuld met een boolean die aangeeft
    /// of de gelieerde persoon wel of niet ingeschreven moet worden
    /// </summary>
    [Serializable]
    public class InschrijfbaarLid : InschrijvingsVoorstel
    {
        /// <summary>
        /// Enkel true als de persoon wel degelijk ingeschreven moet worden.
        /// </summary>
        public bool InTeSchrijven { get; set; }

        /// <summary>
        /// Eventuele foutboodschap bij dit in te schrijven lid.
        /// </summary>
        public string FoutBoodschap { get; set; }
    }
}