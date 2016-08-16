/*
 * Copyright 2008-2013, 2016 the GAP developers. See the NOTICE file at the 
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

using ﻿System;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.SyncInterfaces
{
    public interface IGroepenSync
    {
        /// <summary>
        /// Updatet de gegevens van groep <paramref name="g"/> in Kipadmin. Het stamnummer van <paramref name="g"/>
        /// bepaalt de groep waarover het gaat.
        /// </summary>
        /// <param name="g">Te updaten groep in Kipadmin</param>
        void Bewaren(Groep g);

        /// <summary>
        /// Sluit het huidige werkjaar van de gegeven <param name="groep"> af in Civi. Dat komt erop neer dat
        /// alle lidrelaties worden beeindigd.</param>
        /// </summary>
        /// <param name="groep">Groep waarvan het werkjaar afgesloten moet worden.</param>
        void WerkjaarAfsluiten(Groep groep);

        /// <summary>
        /// Draait de jaarovergang naar het gegeven <paramref name="groepsWerkJaar"/> terug in civi.
        /// Lidrelaties worden hersteld naar de toestand op de dag van de jaarovergang.
        /// </summary>
        /// <param name="groepsWerkJaar">Terug te draaien groepswerkjaar</param>
        void WerkjaarTerugDraaien(GroepsWerkJaar groepsWerkJaar);
    }
}
