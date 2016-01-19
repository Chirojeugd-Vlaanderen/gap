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
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public class ChiroGroep : Groep
    {
        public ChiroGroep()
        {
            this.Afdeling = new HashSet<Afdeling>();
        }
    
        public string Plaats { get; set; }
    
        public virtual KaderGroep KaderGroep { get; set; }
        public virtual ICollection<Afdeling> Afdeling { get; set; }

        /// <summary>
        /// Het niveau van een Chirogroep is altijd Niveau.Groep
        /// </summary>
        public override Niveau Niveau
        {
            get { return Niveau.Groep; }
        }
    }
    
}
