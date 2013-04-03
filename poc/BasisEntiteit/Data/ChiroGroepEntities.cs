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
using Core.Domain;

namespace Data
{
    /// <summary>
    /// There are no comments for ChiroGroepEntities in the schema.
    /// </summary>
    public partial class ChiroGroepEntities : global::System.Data.Objects.ObjectContext
    {
        /// <summary>
        /// Initializes a new ChiroGroepEntities object using the connection string found in the 'ChiroGroepEntities' section of the application configuration file.
        /// </summary>
        public ChiroGroepEntities() :
            base("name=ChiroGroepEntities", "ChiroGroepEntities")
        {
            this.OnContextCreated();
        }
        /// <summary>
        /// Initialize a new ChiroGroepEntities object.
        /// </summary>
        public ChiroGroepEntities(string connectionString) :
            base(connectionString, "ChiroGroepEntities")
        {
            this.OnContextCreated();
        }
        /// <summary>
        /// Initialize a new ChiroGroepEntities object.
        /// </summary>
        public ChiroGroepEntities(global::System.Data.EntityClient.EntityConnection connection) :
            base(connection, "ChiroGroepEntities")
        {
            this.OnContextCreated();
        }
        partial void OnContextCreated();
        /// <summary>
        /// There are no comments for Groep in the schema.
        /// </summary>
        public global::System.Data.Objects.ObjectQuery<Groep> Groep
        {
            get
            {
                if ((this._Groep == null))
                {
                    this._Groep = base.CreateQuery<Groep>("[Groep]");
                }
                return this._Groep;
            }
        }
        private global::System.Data.Objects.ObjectQuery<Groep> _Groep;
        /// <summary>
        /// There are no comments for Groep in the schema.
        /// </summary>
        public void AddToGroep(Groep groep)
        {
            base.AddObject("Groep", groep);
        }
    }

}
