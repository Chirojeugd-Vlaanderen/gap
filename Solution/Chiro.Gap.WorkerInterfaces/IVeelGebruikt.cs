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
using System.Collections.Generic;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    /// <summary>
    /// Levert veel gebruikte basislijstje op, eventueel via een cache
    /// </summary>
    public interface IVeelGebruikt
    {
        /// <summary>
        /// Haalt het groepID van de groep met gegeven stamnummer op uit de cache.
        /// </summary>
        /// <param name="code">
        /// Stamnummer van groep waarvan groepID te bepalen is
        /// </param>
        /// <returns>
        /// GroepID van de groep met stamnummer <paramref name="code"/>.
        /// </returns>
        int CodeNaarGroepID(string code);

        /// <summary>
        /// Haalt het beginjaar van het huidig werkjaar van de gegeven <paramref name="groep"/> op.
        /// (Bijv. 2012 voor 2012-2013)
        /// </summary>
        /// <param name="groep">groep waarvan het werkjaar opgezocht moet worden</param>
        /// <returns>
        /// het beginjaar van het huidig werkjaar van de <paramref name="groep"/>.
        /// (Bijv. 2012 voor 2012-2013)
        /// </returns>
        int WerkJaarOphalen(Groep groep);
    }
}