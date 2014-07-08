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
using System.Diagnostics;
using System.Linq;
using Chiro.Kip.Data;
using Groep = Chiro.Kip.ServiceContracts.DataContracts.Groep;

namespace Chiro.Kip.Services
{
    public partial class SyncPersoonService
    {
        /// <summary>
        /// Updatet de gegevens van groep <paramref name="g"/> in Kipadmin. Het stamnummer van <paramref name="g"/>
        /// bepaalt de groep waarover het gaat.
        /// </summary>
        /// <param name="g">Te updaten groep in Kipadmin</param>
        public void GroepUpdaten(Groep g)
        {
            using (var db = new kipadminEntities())
            {
                var groep = (from cg in db.Groep.OfType<ChiroGroep>()
                             where String.Compare(cg.STAMNR, g.Code, StringComparison.OrdinalIgnoreCase) == 0
                             select cg).FirstOrDefault();

                Debug.Assert(groep != null);
                groep.Naam = g.Naam;
                groep.STRAAT_NR = String.Format("{0} {1}", g.Adres.Straat, NummerEnBus(g.Adres));
                groep.POSTNR = KipPostNr(g.Adres);
                groep.GEMEENTE = g.Adres.WoonPlaats;

                db.SaveChanges();
                _log.BerichtLoggen(groep.GroepID, String.Format("Groep {0} - naam: {1}, adres {2}, {3} {4}", g.Code, g.Naam, groep.STRAAT_NR, groep.POSTNR, groep.GEMEENTE));
            }
        }

    }
}
