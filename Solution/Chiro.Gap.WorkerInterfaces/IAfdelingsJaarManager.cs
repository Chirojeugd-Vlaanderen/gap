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
﻿using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IAfdelingsJaarManager
    {
        /// <summary>
        /// Maakt een afdelingsjaar voor een groep en een afdeling, persisteert niet.
        /// </summary>
        /// <param name="a">
        /// Afdeling voor nieuw afdelingsjaar
        /// </param>
        /// <param name="oa">
        /// Te koppelen officiële afdeling
        /// </param>
        /// <param name="gwj">
        /// Groepswerkjaar (koppelt de afdeling aan een groep en een werkJaar)
        /// </param>
        /// <param name="geboorteJaarBegin">
        /// Geboortejaar van
        /// </param>
        /// <param name="geboorteJaarEind">
        /// Geboortejaar tot
        /// </param>
        /// <param name="geslacht">
        /// Bepaalt of de afdeling een jongensafdeling, meisjesafdeling of
        /// gemengde afdeling is.
        /// </param>
        /// <returns>
        /// Het aangemaakte afdelingsjaar
        /// </returns>
        AfdelingsJaar Aanmaken(
            Afdeling a,
            OfficieleAfdeling oa,
            GroepsWerkJaar gwj,
            int geboorteJaarBegin,
            int geboorteJaarEind,
            GeslachtsType geslacht);
    }
}
