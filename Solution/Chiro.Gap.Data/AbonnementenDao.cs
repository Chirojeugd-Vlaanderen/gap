﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    public class AbonnementenDao: Dao<Abonnement, ChiroGroepEntities>, IAbonnementenDao
    {
        /// <summary>
        /// Haalt alle abonnementen op uit een gegeven groepswerkjaar, inclusief personen, voorkeursadressen, 
        /// groepswerkjaar en groep.
        /// </summary>
        /// <param name="gwjID">ID van het gegeven groepswerkjaar</param>
        /// <returns>
        /// Alle abonnementen op uit een gegeven groepswerkjaar, inclusief personen, voorkeursadressen, 
        /// groepswerkjaar en groep.
        /// </returns>
        public IEnumerable<Abonnement> OphalenUitGroepsWerkJaar(int gwjID)
        {
            Abonnement[] resultaat;

            using (var db = new ChiroGroepEntities())
            {
                resultaat = (
                                from a in
                                    db.Abonnement.Include(ab => ab.GroepsWerkJaar.Groep).Include(
                                        ab => ab.GelieerdePersoon.Persoon)
                                where a.GroepsWerkJaar.ID == gwjID
                                select a).ToArray();
                AdresHelper.VoorkeursAdresKoppelen(from a in resultaat select a.GelieerdePersoon);
            }
            return Utility.DetachObjectGraph<Abonnement>(resultaat);
        }
    }
}