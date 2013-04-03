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

namespace HelloNhibernate
{
    public enum CommunicatieType
    {
        Telefoon = 1, 
        Fax = 2, 
        EMail = 3, 
        WebSite = 4, 
        Msn = 5, 
        Jabber = 6
    }

    /// <summary>
    /// Een communicatievorm is gekoppeld aan precies 1 persoon.  Als er dus
    /// meerdere personen bijv. hetzelfde telefoonnummer hebben, dan komt die
    /// communicatievorm met dat telefoonnummer verschillende keren voor.
    /// </summary>
    /// 
    public class CommunicatieVorm
    {
        #region private members
        private int _id;
        private byte[] _versie;
        private int _type;
        private string _nummer;
        private bool _isGezinsGebonden;
        private bool _voorkeur;
        private string _nota;
        private Persoon _persoon;
        #endregion

        #region properties

        // Voor LTS is het nodig dat alle property's expliciet gedefinieerd
        // zijn.  Vandaar dat ik ze override, en gewoon de base class
        // opnieuw aanroep.

        public int ID
        {
            get { return _id; }
        }

        public byte[] Versie
        {
            get { return _versie; }
            set { _versie = value; }
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

        public CommunicatieType Type
        {
            get { return (CommunicatieType)this.TypeInt; }
            set { this.TypeInt = (int)value; }
        }

        public string Nummer
        {
            get { return _nummer; }
            set { _nummer = value; }
        }

        public bool IsGezinsGebonden
        {
            get { return _isGezinsGebonden; }
            set { _isGezinsGebonden = value; }
        }

        public bool Voorkeur
        {
            get { return _voorkeur; }
            set { _voorkeur = value; }
        }

        public string Nota
        {
            get { return _nota; }
            set { _nota = value; }
        }

        public Persoon Persoon
        {
            get { return _persoon; }
            set { _persoon = value; }
        }

        #endregion

    }
}
