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
using System.Data.Linq.Mapping;
using System.Configuration;
using Cg2.Core.Domain;

namespace Cg2.Data.LTS
{
    public sealed class Cg2DataContext: System.Data.Linq.DataContext
    {
        static XmlMappingSource map 
            = XmlMappingSource.FromXml(System.IO.File.ReadAllText(@"f:\development\cg2\poc\LinqToSql\Cg2.Data\ChiroGroep.map"));

        static string connectionString
            = ConfigurationManager.ConnectionStrings["ChiroGroep"].ToString();

        public Cg2DataContext() : base(connectionString, map) { }
        public Cg2DataContext(string connection) : base(connection, map) { }
    }
}
