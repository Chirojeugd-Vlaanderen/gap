using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;
using System.Data;
using System.Diagnostics;

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
                    where gp.Groep.ID == groepID
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
                    where gp.Groep.ID == groepID
                    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
                    select gp).Skip((pagina-1)*paginaGrootte).Take(paginaGrootte).ToList<GelieerdePersoon>();
            }
            aantalOpgehaald = result.Count;

            return result;
        }

        public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                return (
                    from gp in db.GelieerdePersoon.Include("Persoon").Include("Communicatie").Include("PersoonsAdres.Adres.Straat").Include("PersoonsAdres.Adres.Subgemeente")
                    where gp.ID == gelieerdePersoonID
                    select gp).FirstOrDefault();
            }

        }

        public override GelieerdePersoon Bewaren(GelieerdePersoon p)
        {
            if (p.ID == 0)
            {
                return Creeren(p);
            }
            else
            {
                using (ChiroGroepEntities db = new ChiroGroepEntities())
                {
                    Debug.Assert(p.Persoon != null);    // gelieerde persoon is niet nuttig zonder persoon
                    Debug.Assert(p.Persoon.ID != 0);    // een bestaande gelieerde persoon is nooit gekoppeld aan een nieuwe persoon

                    p.EntityKey = db.CreateEntityKey("GelieerdePersoon", p);
                    p.Persoon.EntityKey = db.CreateEntityKey("Persoon", p.Persoon);

                    db.Attach(p);
                    db.Attach(p.Persoon);

                    SetAllModified(p.EntityKey, db);
                    SetAllModified(p.Persoon.EntityKey, db);

                    db.SaveChanges();
                }
                return p;
            }
        }

    }
}
