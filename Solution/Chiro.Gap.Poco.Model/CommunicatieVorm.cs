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
    public class CommunicatieVorm : BasisEntiteit, ICommunicatie
    {
        public string Nota { get; set; }
        public string Nummer { get; set; }

        /// <summary>
        /// De regular expressie waar de waarde voor dit communicatietype
        /// aan moet voldoen
        /// </summary>
        string ICommunicatie.CommunicatieTypeValidatie
        {
            get
            {
                return CommunicatieType.Validatie;
            }
            set
            {
                CommunicatieType.Validatie = value;
            }
        }

        public override int ID { get; set; }
        public bool IsGezinsgebonden { get; set; }
        public bool Voorkeur { get; set; }
        public override byte[] Versie { get; set; }

        public bool IsVerdacht { get; set; }
        public DateTime LaatsteControle { get; set; }

        public virtual GelieerdePersoon GelieerdePersoon { get; set; }
        public virtual CommunicatieType CommunicatieType { get; set; }
    }
    
}
