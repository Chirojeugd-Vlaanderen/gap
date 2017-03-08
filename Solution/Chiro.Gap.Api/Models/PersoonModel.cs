/*
 * Copyright 2017 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using System;
using Chiro.Gap.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Chiro.Gap.Api.Models
{
    public class PersoonModel
    {
        public int AdNummer { get; set; }
        public int PersoonId { get; set; }
        public string Voornaam { get; set; }
        public string Familienaam { get; set; }
        public AdresModel[] Adressen { get; set; }
        public ContactinfoModel[] Telefoon { get; set; }
        public ContactinfoModel[] Email { get; set; }
        public DateTime? Geboortedatum { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GeslachtsType Geslacht { get; set; }
        public bool IsIngeschreven { get; set; }
        public bool IsLeiding { get; set; }
        public int Leeftijdscorrectie { get; set; }
        public bool LidgeldBetaald { get; set; }
        public DateTime? EindeInstapperiode { get; set; }
        public string[] Afdelingen { get; set; }
        public string[] Functies { get; set; }
        public string[] Categorieen { get; set; }
    }
}