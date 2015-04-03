/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;

namespace Chiro.CiviSync.Logic
{
    /// <summary>
    /// Een helper om de datum van vandaag op te vragen. Door deze te gebruiken i.p.v. DateTime.Now(), kunnen we
    /// die datum vervangen bij het testen.
    /// </summary>
    public class DatumProvider: IDatumProvider
    {
        /// <summary>
        /// Levert de datum van vandaag op (zonder tijdscomponent)
        /// </summary>
        /// <returns>De datum van vandaag.</returns>
        public DateTime Vandaag()
        {
            return DateTime.Now.Date;
        }
    }
}
