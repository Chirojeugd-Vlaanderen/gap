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
    public partial class AfdelingsJaar: BasisEntiteit, IPeriode
    {
        public AfdelingsJaar()
        {
            this.Leiding = new HashSet<Leiding>();
            this.Kind = new HashSet<Kind>();
        }
    
        public int GeboorteJaarTot { get; set; }
        public int GeboorteJaarVan { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
        private int GeslachtsInt { get; set; }
    
        public virtual GroepsWerkJaar GroepsWerkJaar { get; set; }
        public virtual Afdeling Afdeling { get; set; }
        public virtual OfficieleAfdeling OfficieleAfdeling { get; set; }
        public virtual ICollection<Leiding> Leiding { get; set; }
        public virtual ICollection<Kind> Kind { get; set; }

        /// <summary>
        /// Enumwaarde voor het 'geslacht' van de afdeling
        /// </summary>
        public GeslachtsType Geslacht
        {
            get { return (GeslachtsType)GeslachtsInt; }
            set { GeslachtsInt = (int)value; }
        }

        #region Implementeer IPeriode, om de PeriodeValidator te kunnen gebruiken

        DateTime? IPeriode.DatumVan
        {
            get { return new DateTime(GeboorteJaarVan, 1, 1); }
        }

        DateTime? IPeriode.DatumTot
        {
            get { return new DateTime(GeboorteJaarTot, 12, 31); }
        }

        #endregion
    }
    
}
