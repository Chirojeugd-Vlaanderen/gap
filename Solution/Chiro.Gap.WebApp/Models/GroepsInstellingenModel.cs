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

using System.Collections.Generic;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor overzicht van algemene groepsinfo
    /// </summary>
    public class GroepsInstellingenModel : MasterViewModel
    {
        public GroepsInstellingenModel()
        {
            Detail = new GroepDetail();
            NieuweCategorie = new CategorieInfo();
            NieuweFunctie = new FunctieDetail();
            Types = new List<LidType>();
            NonActieveAfdelingen = new List<AfdelingInfo>();
            Mededelingen = new List<Mededeling>();
        }

        public GroepDetail Detail { get; set; }
        public GroepInfo Info { get { return Detail; } }
        public CategorieInfo NieuweCategorie { get; set; }
        public FunctieDetail NieuweFunctie { get; set; }
        
        public IEnumerable<LidType> Types { get; set; }
       
        public List<AfdelingInfo> NonActieveAfdelingen { get; set; }
    }
}
