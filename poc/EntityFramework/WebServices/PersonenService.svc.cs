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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Core.Domain;
using Cg2.Workers;

namespace WebServices
{
    // NOTE: If you change the class name "PersonenService" here, you must also update the reference to "PersonenService" in Web.config.
    public class PersonenService : IPersonenService
    {
        #region IPersonenService Members

        public Persoon Ophalen(int persoonID)
        {
            // Het feit dat hier nog IPersonenManager als type
            // staat, is nog een overblijfsel van de IOC-achtige
            // implementatie uit de 'service factory'.

            IPersonenManager pm = new PersonenManager();

            var result = pm.Ophalen(persoonID);

            return result;
        }

        public Persoon OphalenMetCommunicatie(int persoonID)
        {
            IPersonenManager pm = new PersonenManager();
            return pm.OphalenMetCommunicatie(persoonID);
        }

        public int Bewaren(Persoon p)
        {
            PersonenManager pm = new PersonenManager();

            return pm.Bewaren(p).ID;
        }

        public void Verwijderen(Persoon p)
        {
            PersonenManager pm = new PersonenManager();
            pm.Verwijderen(p);
        }

        public string Hallo()
        {
            return "Hallo PersonenService.";
        }

        #endregion
    }
}
