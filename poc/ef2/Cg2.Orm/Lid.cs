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

namespace Cg2.Orm
{
    /// <summary>
    /// stelt voor welke extra info er in een lid object opgeslagen is dat terug wordt gegeven.
    /// PERSOONSINFO
    /// VRIJEVELDEN
    /// AFDELINGSINFO
    /// FUNCTIES
    /// BIVAKINFO
    /// </summary>
    [DataContract]
    public enum LidInfo
    {
        [EnumMember]
        PERSOONSINFO,
        [EnumMember]
        VRIJEVELDEN,
        [EnumMember]
        AFDELINGSINFO,
        [EnumMember]
        FUNCTIES,
        [EnumMember]
        BIVAKINFO
    }

    public partial class Lid: IBasisEntiteit
    {
        public Lid()
        {
            BusinessKey = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return this.MyEquals(obj);
        }

        public override int GetHashCode()
        {
            return this.MyGetHashCode();
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }
    }
}
