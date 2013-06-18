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

namespace Chiro.Gap.Poco.Model
{
    public class GelieerdePersoon: BasisEntiteit
    {
        public GelieerdePersoon()
        {
            this.Communicatie = new HashSet<CommunicatieVorm>();
            this.Lid = new HashSet<Lid>();
            this.Categorie = new HashSet<Categorie>();
            this.Plaats = new HashSet<Plaats>();
            this.Deelnemer = new HashSet<Deelnemer>();
            this.Abonnement = new HashSet<Abonnement>();
        }
    
        public override int ID { get; set; }
        public int ChiroLeefTijd { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual Groep Groep { get; set; }
        public virtual ICollection<CommunicatieVorm> Communicatie { get; set; }
        public virtual Persoon Persoon { get; set; }
        public virtual ICollection<Lid> Lid { get; set; }
        public virtual ICollection<Categorie> Categorie { get; set; }
        public virtual PersoonsAdres PersoonsAdres { get; set; }
        public virtual ICollection<Plaats> Plaats { get; set; }
        public virtual ICollection<Deelnemer> Deelnemer { get; set; }
        public virtual ICollection<Abonnement> Abonnement { get; set; }

        /// <summary>
        /// Leeftijd van de persoon, rekening houdende met geboortedatum en chiroleeftijd
        /// </summary>
        public DateTime? GebDatumMetChiroLeefTijd
        {
            get
            {
                return Persoon.GeboorteDatum.HasValue ? Persoon.GeboorteDatum.Value.AddYears(-ChiroLeefTijd) : Persoon.GeboorteDatum;
            }
        }


    }
    
}
