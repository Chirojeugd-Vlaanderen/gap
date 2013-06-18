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
using System;
using System.Collections.Generic;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public abstract class Groep: BasisEntiteit
    {
        protected Groep()
        {
            GelieerdePersoon = new HashSet<GelieerdePersoon>();
            GroepsWerkJaar = new HashSet<GroepsWerkJaar>();
            GebruikersRecht = new HashSet<GebruikersRecht>();
            Categorie = new HashSet<Categorie>();
            Functie = new HashSet<Functie>();
            BivakPlaats = new HashSet<Plaats>();
        }
    
        public string Naam { get; set; }
        public string Code { get; set; }
        public int? OprichtingsJaar { get; set; }
        public DateTime? StopDatum { get; set; }
        public string WebSite { get; set; }
        public byte[] Logo { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual ICollection<GelieerdePersoon> GelieerdePersoon { get; set; }
        public virtual ICollection<GroepsWerkJaar> GroepsWerkJaar { get; set; }
        public virtual ICollection<GebruikersRecht> GebruikersRecht { get; set; }
        public virtual ICollection<Categorie> Categorie { get; set; }
        public virtual ICollection<Functie> Functie { get; set; }
        public virtual ICollection<Plaats> BivakPlaats { get; set; }

        public abstract Niveau Niveau { get; }


    }
    
}
