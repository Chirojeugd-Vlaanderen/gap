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
using System.ComponentModel.DataAnnotations;
﻿using System.Diagnostics;

namespace Chiro.Cdf.Poco
{
    public abstract class BasisEntiteit
    {
        /// <summary>
        /// De bedoeling is dat Versie een timestamp (row version) is, voor concurrency control
        /// </summary>
        [Timestamp]
        public abstract byte[] Versie { get; set; }

        /// <summary>
        /// ID is de primary key
        /// </summary>
        public abstract int ID { get; set; }

        /// <summary>
        /// Versie als string, om gemakkelijk te kunnen gebruiken
        /// met MVC model binding in forms
        /// </summary>
        public string VersieString
        {
            get { return Versie == null ? String.Empty : Convert.ToBase64String(Versie); }
            set { Versie = Convert.FromBase64String(value ?? String.Empty); }
        }

        /// <summary>
        /// Experimentele equals die objecten als gelijk beschouwt als hun ID's niet nul en gelijk zijn.
        /// </summary>
        /// <param name="obj">Te vergelijken entiteit</param>
        /// <returns><c>True</c> als this en <paramref name="obj"/> hetzelfde niet-nulle ID 
        /// hebben</returns>
        /// <remarks>Als zowel this als <paramref name="obj"/> ID 0 hebben, wordt
        /// Object.Equas aangeroepen</remarks>
        public override bool Equals(object obj)
        {
            if (obj is BasisEntiteit)
            {
                var f = obj as BasisEntiteit;

                if (f.ID != 0 && f.ID == ID)
                {
                    return true;
                }
                if (f.ID == 0 && ID == 0)
                {
                    return ReferenceEquals(this, obj);
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Er moet gelden: a equals b => a.gethashcode == b.gethashcode
        /// </summary>
        /// <returns>De hashcode</returns>
        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return ID;
        }
    }
}
