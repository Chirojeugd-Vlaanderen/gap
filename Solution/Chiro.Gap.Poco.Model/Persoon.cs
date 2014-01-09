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
    public class Persoon: BasisEntiteit
    {
        private bool _inSync = false;

        public Persoon()
        {
            this.GelieerdePersoon = new HashSet<GelieerdePersoon>();
            this.PersoonsAdres = new HashSet<PersoonsAdres>();
            this.PersoonsVerzekering = new HashSet<PersoonsVerzekering>();
            this.Gav = new HashSet<Gav>();
        }
    
        public int? AdNummer { get; set; }
        public string Naam { get; set; }
        public string VoorNaam { get; set; }
        public DateTime? GeboorteDatum { get; set; }
        internal int GeslachtsInt { get; set; }
        public DateTime? SterfDatum { get; set; }
        public override int ID { get; set; }
        public int? CiviID { get; set; }
        public override byte[] Versie { get; set; }
        public bool InSync
        {
            get 
            { 
                // TODO (#1757): Dit moet op termijn gewoon een automatic property
                // worden, zoals al de rest ({get;set;}). Maar dat kan pas als
                // de database is bijgewerkt.

                return _inSync || AdNummer.HasValue || CiviID.HasValue; 
            }
            set { _inSync = value; }
        }
        public string SeNaam { get; set; }          // Resultaat SOUNDEX op naam (computed)
        public string SeVoornaam { get; set; }      // Resultaat SOUNDEX op naam (computed)
    
        public virtual ICollection<GelieerdePersoon> GelieerdePersoon { get; set; }
        public virtual ICollection<PersoonsAdres> PersoonsAdres { get; set; }
        public virtual ICollection<PersoonsVerzekering> PersoonsVerzekering { get; set; }
        public virtual ICollection<Gav> Gav { get; set; }

        /// <summary>
        /// Een enumwaarde voor het geslacht van de persoon
        /// </summary>
        public GeslachtsType Geslacht
        {
            get { return (GeslachtsType) GeslachtsInt; }
            set { GeslachtsInt = (int) value; }
        }

        /// <summary>
        /// Concatenatie van voornaam en naam
        /// </summary>
        public string VolledigeNaam
        {
            get { return String.Format("{0} {1}", VoorNaam, Naam); }
        }

    }
    
}
