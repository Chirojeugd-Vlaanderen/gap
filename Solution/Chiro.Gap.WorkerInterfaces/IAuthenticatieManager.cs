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
namespace Chiro.Gap.WorkerInterfaces
{
    /// <summary>
    /// Interface voor klasse die gebruiker moet authenticeren.
    /// Deze interface zal gebruikt worden om bij het automatisch
    /// testen via mocking een user te emuleren.
    /// </summary>
    public interface IAuthenticatieManager
    {
        /// <summary>
        /// Opvragen gebruikersnaam huidige gebruiker
        /// </summary>
        /// <returns>
        /// De gebruikersnaam in een string,  de lege string
        /// indien geen gebruiker aangemeld.
        /// </returns>
        string GebruikersNaamGet();
    }
}