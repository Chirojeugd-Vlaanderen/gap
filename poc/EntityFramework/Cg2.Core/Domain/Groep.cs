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
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;

namespace Cg2.Core.Domain
{
    /// <summary>
    /// Algemene groepsklasse, parent voor zowel ChiroGroep als Satelliet.
    /// </summary>
    /// 
    [DataContract]
    [EdmEntityTypeAttribute
        (NamespaceName="Cg2.Core.Domain", Name="Groep")]
    // Als er ervende klasses zijn, moeten die hier als 'KnownType'
    // gemarkeerd worden? (als dat er niet staat, krijg je een exception
    // als een groep geserialiseerd moet worden.)
    [KnownType(typeof(ChiroGroep))]
    public class Groep: BasisEntiteit
    {
        #region Private members
        private string _code;
        private string _naam;
        private int? _oprichtingsJaar;
        private string _webSite;
        private byte[] _versie;
        #endregion

        #region Properties

        [EdmScalarPropertyAttribute (EntityKeyProperty = true, IsNullable = false)]
        public override int ID
        {
            get
            {
                return base.ID;
            }
            set
            {
                base.ID = value;
            }
        }

        [EdmScalarPropertyAttribute()]
        public override byte[] Versie
        {
            get { return base.Versie; }
            set { base.Versie = value; }
        }

        [DataMember]
        [EdmScalarPropertyAttribute()]
        public string Code
        {
            get { return _code; }
            set
            {
                this.PropertyChanging("Code");
                _code = value;
                this.PropertyChanged("Code");
            }
        }

        [DataMember]
        [EdmScalarPropertyAttribute (IsNullable=false)]
        public string Naam
        {
            get { return _naam; }
            set
            {
                this.PropertyChanging("Naam");
                _naam = value;
                this.PropertyChanged("Naam");
            }
        }

        [DataMember]
        [EdmScalarPropertyAttribute()]
        public int? OprichtingsJaar
        {
            get { return _oprichtingsJaar; }
            set
            {
                this.PropertyChanging("OprichtingsJaar");
                _oprichtingsJaar = value;
                this.PropertyChanged("OprichtingsJaar");
            }
        }

        [DataMember]
        [EdmScalarPropertyAttribute()]
        public string WebSite
        {
            get { return _webSite; }
            set
            {
                this.PropertyChanging("WebSite");
                _webSite = value;
                this.PropertyChanged("WebSite");
            }
        }
        #endregion
    }
}
