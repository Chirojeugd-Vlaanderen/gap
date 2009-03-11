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

        // TODO: onderstaande misschien doen via GroepsWerkJaar ipv via
        // aparte persoon- en groepID?
        //
        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, int werkJaar, out int aantalOpgehaald)
        {
            int wj;
            IList<GelieerdePersoon> lijst;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // Als werkjaar = 0: neem huidig werkjaar

                if (werkJaar == 0)
                {
                    wj = (
                    from w in db.GroepsWerkJaar
                    where w.Groep.ID == groepID
                    orderby w.WerkJaar descending
                    select w).FirstOrDefault<GroepsWerkJaar>().WerkJaar;
                }
                else
                {
                    wj = werkJaar;
                }

                // Selecteer eerst gelieerde personen

                var result = (
                    from gp in db.GelieerdePersoon.Include("Persoon")
                    where gp.Groep.ID == groepID
                    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
                    select gp).Skip((pagina-1)*paginaGrootte).Take(paginaGrootte);

                // Dan een query die de lidinfo van het gevraagde werkjaar ophaalt,
                // zodat (hopelijk) enkel die aan de context geattacht geraken.

                // TODO: Kan dit echt niet op een properdere manier?

                IList<Lid> alleLeden = (
                    from l in db.Lid
                    where l.GroepsWerkJaar.WerkJaar == wj && l.GelieerdePersoon.Groep.ID == groepID
                    select l).ToList<Lid>();

                lijst = result.ToList<GelieerdePersoon>();

                aantalOpgehaald = lijst.Count;
            }

            return lijst;   // met wat change komt de relevante lidinfo mee.
        }

        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            return PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, 0, out aantalOpgehaald);
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
