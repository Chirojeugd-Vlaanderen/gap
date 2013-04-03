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
    /// Deze interface bevat operaties van toepassing op groepen.
    /// </summary>
    public interface IGroepenManager
    {
        /// <summary>
        /// Ophalen van een groep uit de database
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <returns></returns>
        Groep Ophalen(int groepID);

        /// <summary>
        /// Persisteert een groep in de database
        /// </summary>
        /// <param name="g">Groep met te persisteren info</param>
        /// <param name="origineel">Origineel uit database, indien
        /// beschikbaar.  Anders null.</param>
        /// <returns>De bewaarde groep</returns>
        Groep Updaten(Groep g, Groep origineel);
    }
}
