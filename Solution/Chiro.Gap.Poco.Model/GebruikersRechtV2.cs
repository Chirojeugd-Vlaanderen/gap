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

using System;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model.Properties;

namespace Chiro.Gap.Poco.Model
{
    /// <summary>
    /// Gebruikersrecht koppelt een persoon aan een groep.
    /// </summary>
    public class GebruikersRechtV2 : BasisEntiteit
    {
        public override int ID { get; set; }
        public DateTime? VervalDatum { get; set; }
        public override byte[] Versie { get; set; }

        internal int PersoonsPermissiesInt { get; set; }
        internal int GroepsPermissiesInt { get; set; }
        internal int AfdelingsPermissiesInt { get; set; }
        internal int IedereenPermissiesInt { get; set; }

        public Permissies PersoonsPermissies {
            get { return (Permissies) PersoonsPermissiesInt; }
            set { PersoonsPermissiesInt = (int)value; }
        }
        public Permissies GroepsPermissies
        {
            get { return (Permissies)GroepsPermissiesInt; }
            set { GroepsPermissiesInt = (int)value; }
        }
        public Permissies AfdelingsPermissies
        {
            get { return (Permissies)AfdelingsPermissiesInt; }
            set { AfdelingsPermissiesInt = (int)value; }
        }
        public Permissies IedereenPermissies
        {
            get { return (Permissies)IedereenPermissiesInt; }
            set { IedereenPermissiesInt = (int)value; }
        }

        public bool SuperGav { get; set; }

    // virtual for lazy loading

        public virtual Persoon Persoon { get; set; }
        public virtual Groep Groep { get; set; }

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
                    Settings.Default.MaandenGebruikersRechtVerlengbaar);
            }
        }
    }
}
