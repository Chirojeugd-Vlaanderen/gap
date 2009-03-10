using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;
using System.Data;
using System.Diagnostics;
using System.Data.Objects;

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
            IList<GelieerdePersoon> lijst;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                var result = (
                    from gp in db.GelieerdePersoon.Include("Persoon")
                    where gp.Groep.ID == groepID
                    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
                    select gp).Skip((pagina-1)*paginaGrootte).Take(paginaGrootte);

                lijst = result.ToList<GelieerdePersoon>();
                aantalOpgehaald = lijst.Count;
            }
            
            return lijst;
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
                // creeer nieuwe gelieerde persoon

                return Creeren(p);
            }
            else
            {
                // update bestaande gelieerde persoon

                Debug.Assert(p.Persoon != null);    // gelieerde persoon is niet nuttig zonder persoon
                Debug.Assert(p.Persoon.ID != 0);    // een bestaande gelieerde persoon is nooit gekoppeld aan een nieuwe persoon

                using (ChiroGroepEntities db = new ChiroGroepEntities())
                {
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

        public GelieerdePersoon GroepLaden(GelieerdePersoon p)
        {
            Debug.Assert(p != null);

            if (p.Groep == null)
            {
                Groep g;

                Debug.Assert(p.ID != 0);
                // Voor een nieuwe gelieerde persoon (p.ID == 0) moet 
                // de groepsproperty altijd gezet zijn, anders kan hij
                // niet bewaard worden.  Aangezien g.Groep == null,
                // kan het dus niet om een nieuwe persoon gaan.

                using (ChiroGroepEntities db = new ChiroGroepEntities())
                {
                    db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

                    p.EntityKey = db.CreateEntityKey("GelieerdePersoon", p);
                    db.Attach(p);

                    p.GroepReference.Load();

                    g = p.Groep;

                    db.Detach(g);
                    db.Detach(p);
                }
                p.Groep = g;
            }

            return p;
        }

    }
}
