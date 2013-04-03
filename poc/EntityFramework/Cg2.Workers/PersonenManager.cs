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
using Cg2.Core.DataInterfaces;
using Cg2.Data.Ef;

namespace Cg2.Workers
{
    public class PersonenManager: IPersonenManager
    {
        public Persoon Updaten(Persoon p, Persoon origineel)
        {
            PersonenDao dao = new PersonenDao();
            return dao.Updaten(p, origineel);
        }

        public Persoon Ophalen(int persoonID)
        {
            PersonenDao dao = new PersonenDao();
            return dao.Ophalen(persoonID);
        }

        public Persoon Bewaren(Persoon p)
        {
            PersonenDao dao = new PersonenDao();
            return dao.Bewaren(p);
        }

        public void Verwijderen(Persoon p)
        {
            PersonenDao dao = new PersonenDao();
            dao.Verwijderen(p);
        }

        public Persoon OphalenMetCommunicatie(int persoonID)
        {
            PersonenDao dao = new PersonenDao();

            return dao.OphalenMetCommunicatie(persoonID);
        }


    }
}
