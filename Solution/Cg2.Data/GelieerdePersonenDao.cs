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
using System.Linq.Expressions;

namespace Cg2.Data.Ef
{
    public class GelieerdePersonenDao : Dao<GelieerdePersoon>, IGelieerdePersonenDao
    {
        public GelieerdePersonenDao()
        {
            connectedEntities = new Expression<Func<GelieerdePersoon, object>>[2] { 
                                        e => e.Persoon, 
                                        e => e.Groep };
        }

        /// <summary>
        /// Herstelt de relevante entity keys voor een GelieerdePersoon.
        /// (Keys van GelieerdePersoon, Persoon en Groep worden zo nodig hersteld.)
        /// </summary>
        /// <param name="entiteit">Gelieerde Persoon met te herstellen entity keys</param>
        /// <param name="db">Objectcontext</param>
        public override void EntityKeysHerstellen(GelieerdePersoon entiteit, ChiroGroepEntities db)
        {
            // GelieerdePersoon zelf
            if (entiteit.ID != 0 && entiteit.EntityKey == null)
            {
                entiteit.EntityKey = db.CreateEntityKey(typeof(GelieerdePersoon).Name, entiteit);
            }

            // Gekoppelde persoon
            if (entiteit.Persoon.ID != 0 && entiteit.Persoon.EntityKey == null)
            {
                entiteit.Persoon.EntityKey = db.CreateEntityKey(typeof(Persoon).Name, entiteit.Persoon);
            }
            
            // Groep.  Haal die eventueel op als dat nog niet gebeurd zou zijn.
            if (entiteit.Groep == null)
            {
                entiteit = GroepLaden(entiteit);
            }
            else if (entiteit.Groep.ID != 0 && entiteit.EntityKey == null)
            {
                entiteit.Groep.EntityKey = db.CreateEntityKey(typeof(Groep).Name, entiteit.Groep);
            }

        }

        public IList<GelieerdePersoon> AllenOphalen(int groepID)
        {
            List<GelieerdePersoon> result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
                // direct gedetachte gelieerde personen ophalen

                result = (
                    from gp in db.GelieerdePersoon.Include("Persoon").Include("Groep")
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
                    from gp in db.GelieerdePersoon.Include("Persoon").Include("Groep")
                    where gp.Groep.ID == groepID
                    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
                    select gp).Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte);

                lijst = result.ToList<GelieerdePersoon>();

            }

            return lijst;
        }

        //todo na deze aanroep is result.Persoon toch nog == null!!?
        //Johan: probeer eens om MergeOption op MergeOption.NoTracking te zetten
        public override GelieerdePersoon Ophalen(int id)
        {
            GelieerdePersoon result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                result = (
                    from t in db.GelieerdePersoon.Include("Persoon").Include("Groep")
                    where t.ID == id
                    select t).FirstOrDefault<GelieerdePersoon>();
                db.Detach(result);
            }

            return result;
        }

        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            int wj;
            IList<GelieerdePersoon> lijst;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

                wj = (
                    from w in db.GroepsWerkJaar
                    where w.Groep.ID == groepID
                    orderby w.WerkJaar descending
                    select w).FirstOrDefault<GroepsWerkJaar>().WerkJaar;

                aantalTotaal = (
                    from gp in db.GelieerdePersoon
                    where gp.Groep.ID == groepID
                    select gp).Count();

                //TODO zoeken hoe je kan bepalen of hij alleen de leden includes als die aan 
                //bepaalde voorwaarden voldoen, maar wel alle gelieerdepersonen
                lijst = (
                    from gp in db.GelieerdePersoon.Include("Persoon").Include("Lid.GroepsWerkJaar").Include("Groep")
                    where gp.Groep.ID == groepID
                    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
                    select gp).Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte).ToList();

                // work around: alle leden verwijderen behalve het (eventuele) lid in het huidige werkjaar
                foreach (GelieerdePersoon gp in lijst)
                {
                    IList<Lid> verkeerdeLeden = (
                        from Lid l in gp.Lid
                        where l.GroepsWerkJaar.WerkJaar != wj
                        select l).ToList();

                    foreach (Lid verkeerdLid in verkeerdeLeden)
                    {
                        gp.Lid.Remove(verkeerdLid);
                    }
                }
                /*                foreach (GelieerdePersoon gp in lijst) {
                                    Lid huidigLid = gp.Lid.FirstOrDefault(lid => lid.GroepsWerkJaar.WerkJaar == wj);
                                    gp.Lid.Clear();
                                    if (huidigLid != null)
                                    {
                                        gp.Lid.Add(huidigLid);
                                    }
                                } */

                /* Dit hieronder werkt ook nog niet ...
                 * 
                 * var tmpLijst = (
                    from gp in db.GelieerdePersoon.Include("Persoon")
                    where gp.Groep.ID == groepID
                    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
                    select new { gp, gp.Persoon, huidigLid = gp.Lid.FirstOrDefault(lid => lid.GroepsWerkJaar.WerkJaar == wj) }
                    ).Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte).ToList();

                lijst = new List<GelieerdePersoon>();
                foreach (var tmp in tmpLijst)
                {
                    GelieerdePersoon gp = tmp.gp;
                    if (tmp.huidigLid != null)
                    {
                        gp.Lid.Add(tmp.huidigLid);
                    }
                    lijst.Add(gp);
                }*/
            }

            return lijst;   // met wat change komt de relevante lidinfo mee.
        }

        public IList<GelieerdePersoon> LijstOphalen(IList<int> gelieerdePersonenIDs)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

                return (
                    from gp in db.GelieerdePersoon.Include("Groep").Include("Persoon").Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.ID, gelieerdePersonenIDs))
                    select gp).ToList();
            }
        }

        public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
                return (
                    from gp in db.GelieerdePersoon.Include("Persoon").Include("Communicatie").Include("Persoon.PersoonsAdres.Adres.Straat").Include("Persoon.PersoonsAdres.Adres.Subgemeente").Include("Groep")
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

        /// <summary>
        /// Haalt alle Personen op die op een zelfde
        /// adres wonen als de gelieerde persoon met het gegeven ID.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gegeven gelieerde
        /// persoon.</param>
        /// <returns>Lijst met Personen (inc. persoonsinfo)</returns>
        /// <remarks>Als de persoon nergens woont, is hij toch zijn eigen
        /// huisgenoot.  Ik geef hier enkel Personen, geen GelieerdePersonen,
        /// omdat ik niet geinteresseerd ben in eventuele dubbels als ik 
        /// GAV ben van verschillende groepen.</remarks>
        public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            List<PersoonsAdres> paLijst;
            List<Persoon> resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.PersoonsAdres.MergeOption = MergeOption.NoTracking;


                // FIXME: enkel persoonsadressen van personen van groepen
                // waarvan je GAV bent

                var persoonsAdressen = (
                    from pa in db.PersoonsAdres.Include("Persoon")
                    where pa.Adres.PersoonsAdres.Any(
                    l => l.Persoon.GelieerdePersoon.Any(
                        gp => gp.ID == gelieerdePersoonID))
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
                         select pa.Persoon).Distinct().ToList();


            if (resultaat.Count == 0)
            {
                // Als de persoon toevallig geen adressen heeft, is het resultaat
                // leeg.  Dat willen we niet; ieder is zijn eigen huisgenoot,
                // ook al woont hij/zij nergens.  Ipv een leeg resultaat,
                // wordt dan gewoon de gevraagde persoon opgehaald.

                resultaat.Add(DetailsOphalen(gelieerdePersoonID).Persoon);

                // FIXME: Er wordt veel te veel info opgehaald.
            }

            return resultaat;
        }

        public IList<GelieerdePersoon> ZoekenOpNaam(int groepID, string zoekStringNaam)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
                return (
                    from gp in db.GelieerdePersoon.Include("Persoon")
                        .Include("Communicatie")
                        .Include("Persoon.PersoonsAdres.Adres.Straat")
                    where (gp.Persoon.VoorNaam + " " + gp.Persoon.Naam + " " + gp.Persoon.VoorNaam)
                        .Contains(zoekStringNaam)
                    select gp).ToList();
            }
        }

        public IList<GelieerdePersoon> OphalenUitCategorie(int categorieID)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.Categorie.MergeOption = MergeOption.NoTracking;

                var query
                    = from c in db.Categorie.Include("GelieerdePersoon.Persoon")
                      where c.ID == categorieID
                      select c;

                Categorie cat = query.FirstOrDefault();

                if (cat == null)
                {
                    // categorie niet gevonden
                    return new List<GelieerdePersoon>();
                }
                else
                {
                    return cat.GelieerdePersoon.ToList();
                }
            }
        }
    }
}
