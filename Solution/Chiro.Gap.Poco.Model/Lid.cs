/*
 * Copyright 2008-2013, 2016 the GAP developers. See the NOTICE file at the 
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public class Lid: BasisEntiteit
    {
        public Lid()
        {
            this.Functie = new HashSet<Functie>();
        }
    
        public bool? LidgeldBetaald { get; set; }
        public bool NonActief { get; set; }
        public bool Verwijderd { get; set; }
        public short VolgendWerkjaarInt { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
        public DateTime? EindeInstapPeriode { get; set; }
        public DateTime? UitschrijfDatum { get; set; }
        public bool IsAangesloten { get; set; }
    
        public virtual GroepsWerkJaar GroepsWerkJaar { get; set; }
        public virtual GelieerdePersoon GelieerdePersoon { get; set; }
        public virtual ICollection<Functie> Functie { get; set; }

        /// <summary>
        /// Geeft een LidType weer, wat Kind of Leiding kan zijn.
        /// </summary>
        public LidType Type
        {
            get { return (this is Kind ? LidType.Kind : LidType.Leiding); }
        }

        /// <summary>
        /// Geeft een lijst terug van alle afdelingen waaraan het lid gegeven gekoppeld is.
        /// (komt van past bij het mappen)
        /// </summary>
        /// <returns>
        /// Lijst met afdelingIDs
        /// </returns>
        /// <remarks>
        /// Een kind is precies aan 1 afdeling gekoppeld
        /// </remarks>
        public IList<int> AfdelingIds
        {
            // Misschien is dit beter een extension method in de mappinghelper.
            // Dit is alleszins te veel logica voor in een entity.
            get
            {
                if (this is Kind)
                {
                    var k = this as Kind;
                    Debug.Assert(k != null);
                    return new List<int>{k.AfdelingsJaar.Afdeling.ID};
                }
                if (this is Leiding)
                {
                    var l = this as Leiding;
                    Debug.Assert(l != null);

                    return (from aj in l.AfdelingsJaar select aj.Afdeling.ID).ToList();
                }
                throw new NotSupportedException("Lid moet kind of leiding zijn.");
            }
        }

        /// <summary>
        /// Geeft een lijst terug van ID's van de afdelingsjaren waaraan het lid gekoppeld is.
        /// (komt van past bij het mappen)
        /// </summary>
        /// <returns>
        /// ID's van de afdelingsjaren waaraan het lid gekopeld is.
        /// </returns>
        /// <remarks>
        /// Een kind is hoogstens aan 1 afdelingsjaar gekoppeld
        /// </remarks>
        public IList<int> AfdelingsJaarIDs
        {
            get
            {
                if (this is Kind)
                {
                    var k = this as Kind;
                    Debug.Assert(k != null);
                    return new List<int> { k.AfdelingsJaar.ID };
                }
                if (this is Leiding)
                {
                    var l = this as Leiding;
                    Debug.Assert(l != null);

                    return (from aj in l.AfdelingsJaar select aj.ID).ToList();
                }
                throw new NotSupportedException("Lid moet kind of leiding zijn.");
            }
        }

        /// <summary>
        /// Bepaalt het niveau van het lid.  Hiervoor moet wel de groep beschikbaar zijn.
        /// </summary>
        public Niveau Niveau
        {
            get
            {
                var resultaat = GroepsWerkJaar.Groep.Niveau;
                if ((resultaat & Niveau.Groep) != 0)
                {
                    if (!(this is Leiding))
                    {
                        resultaat &= ~Niveau.LeidingInGroep;
                    }
                    if (!(this is Kind))
                    {
                        resultaat &= ~Niveau.LidInGroep;
                    }
                }
                return resultaat;
            }
        }
    }   
}