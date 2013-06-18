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

using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WebApi.Models
{
    public class ContactgegevenModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Waarde { get; set; }

        public virtual PersoonModel Persoon { get; set; }

        public ContactgegevenModel(CommunicatieVorm communicatieVorm)
        {
            Id = communicatieVorm.ID;
            Type = communicatieVorm.CommunicatieType.Omschrijving;
            Waarde = communicatieVorm.Nummer;
        }
    }
}