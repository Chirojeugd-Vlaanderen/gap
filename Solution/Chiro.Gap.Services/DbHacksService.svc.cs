﻿/*
 * Copyright 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Service  met hacks voor dev- en testomgeving.
    /// </summary>
    /// <remarks>
    /// DEZE FILE MAG NIET AANWEZIG ZIJN IN DE CODE VOOR DE LIVE-OMGEVING!
    /// </remarks>
    public class DbHacksService : IDbHacksService
    {
        /// <summary>
        /// Deze method geeft de gebruiker met gegeven <paramref name="gebruikersNaam"/> GAV-rechten voor een
        /// willekeurige groep.
        /// </summary>
        /// <param name="gebruikersNaam"></param>
        /// <returns>Groep-ID van de groep waarop je rechten hebt gekregen.</returns>
        public int WillekeurigeGroepToekennen(string gebruikersNaam)
        {
            return TestHacks.TestHacks.TestGroepToevoegen(gebruikersNaam);
        }
    }
}