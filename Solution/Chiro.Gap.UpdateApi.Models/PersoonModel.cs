/*
 * Copyright 2014-2015 the GAP developers. See the NOTICE file at the 
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

namespace Chiro.Gap.UpdateApi.Models
{
    /// <summary>
    /// Dit is een erg beperkt model voor een persoon.
    /// 
    /// Op termijn kan de echte api gebruikt worden om updates van Chirocivi
    /// naar GAP te sturen.
    /// </summary>
    public class PersoonModel
    {
        /// <summary>
        /// Een persoon wordt altijd opgezocht op basis van zijn ID.
        /// </summary>
        public int PersoonId { get; set; }
        public int? AdNummer { get; set; }
    }
}