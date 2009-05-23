using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;
using System.Data;
using System.Diagnostics;
using System.Data.Objects;

using Cg2.EfWrapper.Entity;
using Cg2.EfWrapper;

namespace Cg2.Data.Ef
{
    public class GelieerdePersonenDao: Dao<GelieerdePersoon>, IGelieerdePersonenDao
    {
        public IList<GelieerdePersoon> AllenOphalen(int groepID)
        {
            List<GelieerdePersoon> result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
                // direct gedetachte gelieerde personen ophalen

                result = (
                    from gp in db.GelieerdePersoon.Include("Persoon")
                    where gp.Groep.ID == groepID
                    select gp).ToList<GelieerdePersoon>();
            }
            return result;
        }

        public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            IList<GelieerdePersoon> lijst;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

                aantalTotaal = (
                    from gp in db.GelieerdePersoon
                    where gp.Groep.ID == groepID
                    select gp).Count();

                var result = (
                    from gp in db.GelieerdePersoon.Include("Persoon")
                    where gp.Groep.ID == groepID
                    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
                    select gp).Skip((pagina-1)*paginaGrootte).Take(paginaGrootte);

                lijst = result.ToList<GelieerdePersoon>();

            }
            
            return lijst;
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

        #region IGelieerdePersonenDao Members
        // TODO: onderstaande misschien doen via GroepsWerkJaar ipv via
        // aparte persoon- en groepID?
        //
        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, int werkJaar, out int aantalTotaal)
        {
            int wj;
            IList<GelieerdePersoon> lijst;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

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

                aantalTotaal = (
                    from gp in db.GelieerdePersoon
                    where gp.Groep.ID == groepID
                    select gp).Count();

               lijst = (
                    from gp in db.GelieerdePersoon.Include("Persoon").Include("Lid.GroepsWerkJaar")
                    where 
                        gp.Groep.ID == groepID //TODO zoeken hoe je kan bepalen of hij alleen de leden includes als die aan 
                        //bepaalde voorwaarden voldoen, maar wel alle gelieerdepersonen
                    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
                    select gp).Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte).ToList();
            }

            foreach (GelieerdePersoon gp in lijst)
            {
                Lid huidigLid = (
                    from Lid l in gp.Lid
                    where l.GroepsWerkJaar.WerkJaar == wj
                    select l).First();

                gp.Lid.Clear();
                gp.Lid.Add(huidigLid);
            }

            return lijst;   // met wat change komt de relevante lidinfo mee.
        }

        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            return PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, 0, out aantalTotaal);
        }

        public IList<GelieerdePersoon> LijstOphalen(IList<int> gelieerdePersonenIDs)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

                return (
                    from gp in db.GelieerdePersoon.Include("Persoon").Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.ID, gelieerdePersonenIDs))
                    select gp).ToList();
            }
        }

        public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
                return (
                    from gp in db.GelieerdePersoon.Include("Persoon").Include("Communicatie").Include("PersoonsAdres.Adres.Straat").Include("PersoonsAdres.Adres.Subgemeente")
                    where gp.ID == gelieerdePersoonID
                    select gp).FirstOrDefault();
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

                    g = (from gp in db.GelieerdePersoon
                         where gp.ID == p.ID
                         select gp.Groep).FirstOrDefault();
                }
                p.Groep = g;
                g.GelieerdePersoon.Add(p);  // nog niet zeker of dit gaat werken...
            }

            return p;
        }

        public IList<GelieerdePersoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            List<PersoonsAdres> paLijst;
            List<GelieerdePersoon> resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.PersoonsAdres.MergeOption = MergeOption.NoTracking;

                var persoonsAdressen = (
                    from pa in db.PersoonsAdres.Include("GelieerdePersoon.Persoon")
                    where pa.Adres.PersoonsAdres.Any(l => l.GelieerdePersoon.ID == gelieerdePersoonID)
                    select pa);

                // Het zou interessant zijn als ik hierboven al 
                // pa.GelieerdePersoon zou 'selecten'.  Maar gek genoeg wordt
                // dan GelieerdePersoon.Persoon niet meegenomen.
                // Als workaround selecteer ik zodadelijk uit
                // persoonsAdressen de GelieerdePersonen.

                paLijst = persoonsAdressen.ToList();

                // Als de persoon nergens woont, dan is deze lijst leeg.  In dat geval
                // halen we gewoon de gelieerde persoon zelf op.

            }

            // Om onderstaande Distinct te doen werken, moest ik
            // GelieerdePersoon voorzien van een custom Equals en
            // GetHashCode.
            resultaat = (from pa in paLijst
                         select pa.GelieerdePersoon).Distinct().ToList();


            if (resultaat.Count == 0)
            {
                // Als de persoon toevallig geen adressen heeft, is het resultaat
                // leeg.  Dat willen we niet; ieder is zijn eigen huisgenoot,
                // ook al woont hij/zij nergens.  Ipv een leeg resultaat,
                // wordt dan gewoon de gevraagde persoon opgehaald.

                resultaat.Add(DetailsOphalen(gelieerdePersoonID));

                // FIXME: Er wordt veel te veel info opgehaald.
            }

            return resultaat;
        }

        #endregion
    }
}
