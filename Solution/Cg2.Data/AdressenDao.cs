using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using System.Diagnostics;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Data.Ef
{
    public class AdressenDao: Dao<Adres>, IAdressenDao
    {
        public Adres AdresMetBewonersOphalen(int adresID, IList<int> gelieerdAan)
        {
            Adres resultaat = null;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // query opbouwen

                var query
                    = db.PersoonsAdres.Include("Adres.Straat")
                    .Include("Adres.Subgemeente")
                    .Include("GelieerdePersoon.Persoon")
                    .Where(pera => pera.AdresID == adresID);

                if (gelieerdAan != null)
                {
                    query = query
                        .Where(Utility.BuildContainsExpression<PersoonsAdres, int>(pera => pera.GelieerdePersoon.Groep.ID, gelieerdAan));
                }

                query = query.Select(pera => pera);

                // genereer lijst met alle persoonsadressen.

                IList<PersoonsAdres> persoonsAdressen = query.ToList();


                // deze zijn allemaal gekoppeld aan hetzelfde adres
                // (met gegeven adresid).  Het eerste adres is dus al
                // ok.  (Ik ga er voor het gemak even van uit dat
                // er altijd een adres met een gekoppelde persoon
                // gevonden wordt.)

                Debug.Assert(persoonsAdressen.Count > 0);

                resultaat = persoonsAdressen[0].Adres;
            }

            return resultaat;
        }
    }
}
