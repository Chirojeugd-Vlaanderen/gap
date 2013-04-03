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
using Cg2.Core.Domain;

namespace Cg2.Data
{
    public class Cg2ObjectContext: global::System.Data.Objects.ObjectContext
    {
        private global::System.Data.Objects.ObjectQuery<Groep> _groepen;
        private global::System.Data.Objects.ObjectQuery<Persoon> _personen;

        #region constructors
        //public Cg2ObjectContext() : base(@"metadata=F:\development\cg2\poc\EntityFramework\WebServices\bin\Cg2Domain.csdl|F:\development\cg2\poc\EntityFramework\WebServices\bin\Cg2Domain.ssdl|F:\development\cg2\poc\EntityFramework\WebServices\bin\Cg2Domain.msl;provider=System.Data.SqlClient;provider connection string='Data Source=DEVSERVER;Initial Catalog=ChiroGroep;Integrated Security=True;MultipleActiveResultSets=True'", "Cg2ObjectContext")  { } // (2de parameter verwijst naar Entity Container van csdl).

        public Cg2ObjectContext() : base("name=Cg2ObjectContext", "Cg2ObjectContext") { }
        public Cg2ObjectContext(string connectionString) : base(connectionString, "Cg2ObjectContext") { }
        public Cg2ObjectContext(global::System.Data.EntityClient.EntityConnection connection) : base(connection, "Cg2ObjectContext") { }
        #endregion

        public global::System.Data.Objects.ObjectQuery<Groep> Groepen
        {
            get
            {
                if (this._groepen == null)
                {
                    this._groepen = base.CreateQuery<Groep>("[Groep]");
                }
                return this._groepen;
            }
        }

        public global::System.Data.Objects.ObjectQuery<Persoon> Personen
        {
            get
            {
                if (this._personen == null)
                {
                    this._personen = base.CreateQuery<Persoon>("[Persoon]");
                }
                return this._personen;
            }
        }

    }
}
