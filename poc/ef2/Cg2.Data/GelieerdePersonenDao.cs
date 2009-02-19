using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Data.Ef
{
    public class GelieerdePersonenDao: Dao<GelieerdePersoon>, IGelieerdePersonenDao
    {
        public IList<GelieerdePersoon> AllenOphalen(int groepID)
        {
            List<GelieerdePersoon> result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                result = (
                    from gp in db.GelieerdePersoon.Include("Persoon")
                    where gp.GroepID == groepID
                    select gp).ToList<GelieerdePersoon>();
            }
            return result;
        }

        public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            List<GelieerdePersoon> result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                result = (
                    from gp in db.GelieerdePersoon.Include("Persoon")
                    where gp.GroepID == groepID
                    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
                    select gp).Skip((pagina-1)*paginaGrootte).Take(paginaGrootte).ToList<GelieerdePersoon>();
            }
            aantalOpgehaald = result.Count;

            return result;
        }

    }
}
