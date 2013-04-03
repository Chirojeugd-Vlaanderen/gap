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
using Cg2.Orm;
using System.Diagnostics;
using System.Data.Objects;
using System.Data;
using Cg2.Orm.DataInterfaces;


namespace Cg2.Data.Ef
{
    public class LedenDao: Dao<Lid>, ILedenDao
    {
        /// <summary>
        /// Creeert een nieuw lid
        /// </summary>
        /// <param name="l">lidobject</param>
        /// <returns>bewaarde lidobject</returns>
        /// <remarks>Wijzigingen in GroepsWerkJaar of
        /// GelieerdePersoon worden niet meegenomen!</remarks>
        public override Lid Creeren(Lid l)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // Voor een nieuw lid moeten GelieerdePersoon en
                // GroepsWerkJaar gegeven zijn.

                Debug.Assert(l.GelieerdePersoon != null);
                Debug.Assert(l.GroepsWerkJaar != null);

                // We hebben deze situatie:
                //  - l is nieuw
                //  - l.GelieerdePersoon en l.GroepsWerkJaar zijn detached
                // Blijkbaar is het niet simpel om in deze situatie l aan de
                // context te koppelen.  Zie onder meer:
                //
                // http://www.codeproject.com/KB/architecture/attachobjectgraph.aspx?display=PrintAll&fid=1534536&df=90&mpp=25&noise=3&sort=Position&view=Quick&select=2935937
                // http://social.msdn.microsoft.com/Forums/en-US/adodotnetentityframework/thread/8524748e-85ba-48fb-b7d9-95433d6f3f04/
                //
                // Het kan wel via een omweg:

                // bewaar references

                GelieerdePersoon gp = l.GelieerdePersoon;
                GroepsWerkJaar gw = l.GroepsWerkJaar;

                // verwijder referenties van te bewaren lid

                l.GelieerdePersoon = null;
                l.GroepsWerkJaar = null;

                // koppel lid aan context

                db.AddToLid(l);

                // attach bewaarde referenties

                db.Attach(gp);
                db.Attach(gw);

                // koppel opnieuw aan lid

                l.GroepsWerkJaar = gw;
                l.GelieerdePersoon = gp;

                // Bewaren
                
                db.SaveChanges();
            }
            return l;
        }

        public IList<Lid> PaginaOphalen(int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            IList<Lid> lijst;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                var result = (
                    from l in db.Lid.Include("GelieerdePersoon.Persoon")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte);

                lijst = result.ToList<Lid>();
                aantalOpgehaald = lijst.Count;
            }

            return lijst;
        }
    }
}
