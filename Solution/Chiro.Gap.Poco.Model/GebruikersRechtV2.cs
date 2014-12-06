/*
 * Copyright 2014 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Poco.Model
{
    /// <summary>
    /// Gebruikersrecht koppelt een persoon aan een groep.
    /// </summary>
    public class GebruikersRechtV2: BasisEntiteit
    {
        public override int ID { get; set; }
        public DateTime? VervalDatum { get; set; }
        public override byte[] Versie { get; set; }

        // virtual for lazy loading

        public virtual Persoon Persoon { get; set; }
        public virtual Groep Groep { get; set; }
        internal int PermissiesInt { get; set; }

        /// <summary>
        /// Geeft weer of de vervaldatum verlengbaar is. Dit is eigenlijk business, dus dat
        /// staat hier verkeerd.
        /// </summary>
        /// <returns></returns>
        public bool IsVerlengbaar
        {
            get
            {
                return VervalDatum != null && ((DateTime)VervalDatum) < DateTime.Now.AddMonths(
                    Properties.Settings.Default.MaandenGebruikersRechtVerlengbaar);
            }
        }

        public Permissies Permissies
        {
            get
            {
                return (Permissies)PermissiesInt;
            }
            set
            {
                PermissiesInt = (int)value;
            }
        }
    }
}
