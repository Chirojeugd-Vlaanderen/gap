/*
 * Copyright 2013 Ben Bridts.
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

using System.Linq;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WebApi.Models
{
    public class GroepModel
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Stamnummer { get; set; }

        public virtual IQueryable<PersoonModel> Personen { get; set; }
        public virtual IQueryable<AfdelingModel> Afdelingen { get; set; }

        public GroepModel(Groep groep)
        {
            Id = groep.ID;
            Naam = groep.Naam;
            Stamnummer = groep.Code;
        }
    }
}