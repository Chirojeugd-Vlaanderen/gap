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

using System.Collections.Generic;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public class Functie: BasisEntiteit
    {
        public Functie()
        {
            this.Lid = new HashSet<Lid>();
        }
    
        public string Naam { get; set; }
        public string Code { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
        public int? MaxAantal { get; set; }
        public int MinAantal { get; set; }
        public int? WerkJaarVan { get; set; }
        public int? WerkJaarTot { get; set; }
        public bool IsNationaal { get; set; }
        public int LidTypeInt { internal get; set; }
        public int NiveauInt { get; set; }
    
        public virtual Groep Groep { get; set; }
        public virtual ICollection<Lid> Lid { get; set; }

        /// <summary>
        /// Controleert of de functie de nationale functie de nationale functie <paramref name="natFun"/> is.
        /// </summary>
        /// <param name="natFun">Een nationale functie</param>
        /// <returns><c>True</c> als de functie de nationale functie is, anders <c>false</c></returns>
        public bool Is(NationaleFunctie natFun)
        {
            return ID == ((int) natFun);
        }

        /// <summary>
        /// Koppeling tussen enum Niveau en databaseveld NiveauInt
        /// </summary>
        public Niveau Niveau
        {
            get { return (Niveau)NiveauInt; }
            set { NiveauInt = (int)value; }
        }

        /// <summary>
        /// Koppeling tussen enum LidType en databaseveld LidTypeInt
        /// </summary>
        public LidType Type
        {
            get { return (LidType)LidTypeInt; }
            set { LidTypeInt = (int)value; }
        }
    }
    
}
