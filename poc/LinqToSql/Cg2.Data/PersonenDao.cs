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
using System.Data.Linq;

namespace Cg2.Data.LTS
{
    public class PersonenDao: Dao<Persoon>, IPersonenDao
    {
        public Persoon OphalenMetCommunicatie(int id)
        {
            Persoon result;

            using (Cg2DataContext db = new Cg2DataContext())
            {
                DataLoadOptions dlo = new DataLoadOptions();
                dlo.LoadWith<Persoon>(p => p.Communicatie);
                db.LoadOptions = dlo;

                Table<Persoon> tabel = db.GetTable(typeof(Persoon))
                    as Table<Persoon>;


                result = (
                    from t in tabel
                    where t.ID == id
                    select t).FirstOrDefault<Persoon>();
            }
            return result;
        }
    }
}
