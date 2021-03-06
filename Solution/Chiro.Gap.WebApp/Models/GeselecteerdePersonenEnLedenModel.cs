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

using System.Collections.Generic;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Wordt voor de jaarovergang gebruikt; lijst met leden die al dan niet over te zetten
    /// zijn naar het nieuwe werkJaar
    /// </summary>
    public class GeselecteerdePersonenEnLedenModel : MasterViewModel
    {
        /// <summary>
        /// Een lijst met alle nodige persoons en leden informatie.
        /// </summary>
        public InschrijfbaarLid[] PersoonEnLidInfos { get; set; }

        /// <summary>
        /// Lijst met de actieve afdelingen dit werkJaar
        /// </summary>
        public IEnumerable<ActieveAfdelingInfo> BeschikbareAfdelingen { get; set; }
    }
}