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
using System.Text;

namespace Cg2.Core.Domain
{
    /// <summary>
    /// Deze interface biedt de operaties aan die met een persoon kunnen
    /// gebeuren.
    /// </summary>
    public interface IPersonenManager
    {
        /// <summary>
        /// Updatet een persoon
        /// </summary>
        /// <param name="p">Te updaten persoon</param>
        /// <param name="origineel">Originele persoon, als die beschikbaar is.
        /// Anders null.</param>
        /// <returns>De bewaarde persoon</returns>
        Persoon Updaten(Persoon p, Persoon origineel);

        /// <summary>
        /// Haalt persoonsinfo op uit database
        /// </summary>
        /// <param name="persoonID">ID van de persoon met de gevraagde info.
        /// </param>
        /// <returns>Gevonden persoonsobject, null als niet gevonden</returns>
        Persoon Ophalen(int persoonID);

        /// <summary>
        /// Haalt persoonsinfo op uit database, incl. communicatievormen
        /// </summary>
        /// <param name="persoonID">ID op te halen persoon</param>
        /// <returns>Gevraagde persoonsobject, inclusief communicatievormen
        /// in member Communicatie.  Null indien de persoon niet gevonden.
        /// </returns>
        Persoon OphalenMetCommunicatie(int persoonID);
    }
}
