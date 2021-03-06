/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
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

namespace Chiro.Ad.Domain
{
    /// <summary>
    /// Enum voor de verschillende domeinen in Active Directory waar mensen een login
    /// kunnen krijgen.
    /// </summary>
    public enum DomeinEnum
    {
        /// <summary>
        /// Domein voor interne gebruikers, typisch beroepskrachten, die op het
        /// netwerk van de Chiro moeten kunnen.
        /// </summary>
        Lokaal,

        /// <summary>
        /// Domein voor externe gebruikers, typisch vrijwilligers, die alleen op het extranet
        /// moeten kunnen.
        /// </summary>
        Wereld
    }
}