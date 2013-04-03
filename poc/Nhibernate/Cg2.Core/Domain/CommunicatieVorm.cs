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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.Core.Domain
{
    [DataContract]
    public enum CommunicatieType
    {
        [EnumMember]
        Telefoon = 1, 
        [EnumMember]
        Fax = 2, 
        [EnumMember]
        EMail = 3, 
        [EnumMember]
        WebSite = 4, 
        [EnumMember]
        Msn = 5, 
        [EnumMember]
        Jabber = 6
    }

    /// <summary>
    /// Een communicatievorm is gekoppeld aan precies 1 persoon.  Als er dus
    /// meerdere personen bijv. hetzelfde telefoonnummer hebben, dan komt die
    /// communicatievorm met dat telefoonnummer verschillende keren voor.
    /// </summary>
    /// 
    [DataContract]
    public class CommunicatieVorm: BasisEntiteit
    {
        #region private members
        private int _type;
        private string _nummer;
        private bool _isGezinsGebonden;
        private bool _voorkeur;
        private string _nota;
        private int _persoonID;
        #endregion

        #region properties

        // Voor LTS is het nodig dat alle property's expliciet gedefinieerd
        // zijn.  Vandaar dat ik ze override, en gewoon de base class
        // opnieuw aanroep.

        public override int ID
        {
            get { return base.ID; }
            set { base.ID = value; }
        }

        public override byte[] Versie
        {
            get { return base.Versie; }
            set { base.Versie = value; }
        }

        // Enums kunnen niet rechtstreeks gemapt worden.  Daarom gebruik ik
        // de property 'TypeInt' voor het Entity framework, die met ints werkt,
        // en de property 'Type' voor de applicatie; die werkt met 
        // 'CommunicatieTypes'.

        public int TypeInt
        {
            get { return _type; }
            set { _type = value; }
        }

        [DataMember]
        public CommunicatieType Type
        {
            get { return (CommunicatieType)this.TypeInt; }
            set { this.TypeInt = (int)value; }
        }

        [DataMember]
        public string Nummer
        {
            get { return _nummer; }
            set { _nummer = value; }
        }

        [DataMember]
        public bool IsGezinsGebonden
        {
            get { return _isGezinsGebonden; }
            set { _isGezinsGebonden = value; }
        }

        [DataMember]
        public bool Voorkeur
        {
            get { return _voorkeur; }
            set { _voorkeur = value; }
        }

        [DataMember]
        public string Nota
        {
            get { return _nota; }
            set { _nota = value; }
        }

        [DataMember]
        public int PersoonID
        {
            get { return _persoonID; }
            set { _persoonID = value; }
        }

        #endregion

    }
}
