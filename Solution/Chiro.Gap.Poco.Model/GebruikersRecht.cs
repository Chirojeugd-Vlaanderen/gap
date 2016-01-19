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
using System;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public class GebruikersRecht: BasisEntiteit
    {
        public override int ID { get; set; }
        public DateTime? VervalDatum { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual Gav Gav { get; set; }
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
                return VervalDatum != null && ((DateTime) VervalDatum) < DateTime.Now.AddMonths(
                    Properties.Settings.Default.MaandenGebruikersRechtVerlengbaar);
            }
        }

        /// <summary>
        /// Rol van de gebruiker. Op dit moment ondersteunen we enkel GAV (zie #844)
        /// </summary>
        public Rol Rol
        {
            get { return VervalDatum == null || VervalDatum < DateTime.Now ? Rol.Geen : Rol.Gav; }
        }
    }
    
}
