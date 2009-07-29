using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;
using System.Data.Objects;
using Cg2.EfWrapper.Entity;
using System.Linq.Expressions;

namespace Cg2.Data.Ef
{
    public class GroepenDao: Dao<Groep>, IGroepenDao
    {
        /*
        TODO creeren(groep) ?
        */

        public GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID)
        {
            GroepsWerkJaar result;
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                result = (
                    from wj in db.GroepsWerkJaar
                    where wj.Groep.ID == groepID
                    orderby wj.WerkJaar descending
                    select wj).FirstOrDefault<GroepsWerkJaar>();

                db.Detach(result);
            }
            return result;
        }

        public Groep OphalenMetAdressen(int groepID)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
                return (
                    from t in db.Groep.Include("Adres")
                    where t.ID == groepID
                    select t).FirstOrDefault<Groep>();
            }
        }

        public Groep OphalenMetCategorieen(int groepID)
        {
            //TODO
            throw new NotImplementedException();
        }

        public Groep OphalenMetFuncties(int groepID)
        {
            //TODO
            throw new NotImplementedException();
        }

        //ophalen van groep, afdeling, afdelingsjaar en officiele afdelingen voor huidig werkjaar
        public Groep OphalenMetAfdelingen(int groepID)
        {
            int huidigwerkjaar = RecentsteGroepsWerkJaarGet(groepID).WerkJaar;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

                var groep = (
                    from t in db.Groep.Include("Afdeling")
                    where t.ID == groepID
                    select t);

                var afdelingsjaren = (
                    from x in db.AfdelingsJaar.Include("Afdeling").Include("GroepsWerkJaar").Include("OfficieleAfdeling")
                    where x.GroepsWerkJaar.WerkJaar == huidigwerkjaar
                    select x);

                try
                {
                    var result = (
                    from x in groep.First().Afdeling
                    join y in afdelingsjaren
                    on x equals y.Afdeling
                    into volledige
                    select groep
                    ).First().First();
                    return db.DetachObjectGraph(result);
                    
                }
                catch (System.InvalidOperationException)
                {
                    return groep.First();
                }
                
            }
        }

        public Groep OphalenMetVrijeVelden(int groepID)
        {
            //TODO
            throw new NotImplementedException();
        }

        public void BewarenMetAdressen(Groep g)
        {
            //TODO
            throw new NotImplementedException();
        }

        public void BewarenMetCategorieen(Groep g)
        {
            //TODO
            throw new NotImplementedException();
        }

        public void BewarenMetFuncties(Groep g)
        {
            //TODO
            throw new NotImplementedException();
        }

        public void BewarenMetAfdelingen(Groep g)
        {
            Expression<Func<Groep, object>>[] e = 
            {
                d => d.Afdeling.First(),
                d => d.Afdeling.First().AfdelingsJaar.First(),
                d => d.Afdeling.First().AfdelingsJaar.First().OfficieleAfdeling.WithoutUpdate()
            };
            Bewaren(g, e);
        }

        public void BewarenMetVrijeVelden(Groep g)
        {
            //TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Creeert een nieuwe afdeling.
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="naam">naam van de afdeling</param>
        /// <param name="afkorting">afkorting voor de afdeling</param>
        /// <returns>Een relevant afdelingsobject</returns>
        public Afdeling AfdelingCreeren(int groepID, string naam, string afkorting)
        {
            Afdeling a = new Afdeling();

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                var gp = (
                    from g in db.Groep
                    where g.ID == groepID
                    select g
                    ).FirstOrDefault<Groep>();

                a.AfdelingsNaam = naam;
                a.Afkorting = afkorting;
                a.Groep = gp;

                gp.Afdeling.Add(a);

                db.SaveChanges();

                db.Detach(a);
            }

            return a;
        }

        /// <summary>
        /// Creeert een nieuw afdelingsjaar.
        /// </summary>
        /// <param name="g">Groep voor afdelingsjaar</param>
        /// <param name="a">Afdeling voor afdelingsjaar</param>
        /// <param name="oa">Officiele afdeling voor afdelingsjaar</param>
        /// <param name="geboorteJaarVan">begingeboortejaar voor afdeling</param>
        /// <param name="geboorteJaarTot">eindgeboortejaar voor afdeling</param>
        /// <returns>Het nieuwe afdelingsjaar</returns>
        public AfdelingsJaar AfdelingsJaarCreeren(Groep g, Afdeling a, OfficieleAfdeling oa, int geboorteJaarVan, int geboorteJaarTot)
        {
            AfdelingsJaar afdelingsJaar = new AfdelingsJaar();
            GroepsWerkJaar huidigWerkJaar = RecentsteGroepsWerkJaarGet(g.ID);

            // Omdat we met een combinatie van geattachte en nieuwe objecten
            // zitten, geeft het het minste problemen als de we relaties tussen
            // de objecten gedetachet leggen, en daarna AttachObjectGraph
            // aanroepen.

            // In theorie zouden de eerstvolgende zaken in Business kunnen gebeuren,
            // en zou daarna het resulterende AfdelingsJaar
            // doorgespeeld moeten worden aan deze functie, die het dan enkel
            // nog moet persisteren.

            // TODO: verifieren of de afdeling bij de groep hoort door
            // de groep van de afdeling op te halen, ipv alle afdelingen
            // van de groep.

            // Groep g heeft niet altijd de afdelingen mee
            Groep groepMetAfdelingen = OphalenMetAfdelingen(g.ID);

            // TODO: deze test hoort thuis in business, niet in DAL:

            if (!groepMetAfdelingen.Afdeling.Contains(a))
            {
                throw new InvalidOperationException("Afdeling " + a.AfdelingsNaam + " is geen afdeling van Groep " + g.Naam);
            }

            // TODO: test of de officiele afdeling bestaat, heb
            // ik voorlopig even weggelaten.  Als de afdeling niet
            // bestaat, zal er bij het bewaren toch een exception
            // optreden, aangezien het niet de bedoeling is dat
            // een officiele afdeling bijgemaakt wordt.

            //TODO check if no conflicts with existing afdelingsjaar
            //TODO: bovenstaande TODO moet ook in business layer gebeuren

            afdelingsJaar.OfficieleAfdeling = oa;
            afdelingsJaar.Afdeling = a;
            afdelingsJaar.GroepsWerkJaar = huidigWerkJaar;
            afdelingsJaar.GeboorteJaarVan = geboorteJaarVan;
            afdelingsJaar.GeboorteJaarTot = geboorteJaarTot;

            a.AfdelingsJaar.Add(afdelingsJaar);
            oa.AfdelingsJaar.Add(afdelingsJaar);
            huidigWerkJaar.AfdelingsJaar.Add(afdelingsJaar);

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                AfdelingsJaar geattachtAfdJr = db.AttachObjectGraph(afdelingsJaar
                    , aj=>aj.OfficieleAfdeling.WithoutUpdate()
                    , aj=>aj.Afdeling.WithoutUpdate()
                    , aj=>aj.GroepsWerkJaar.WithoutUpdate());

                db.SaveChanges();

                // SaveChanges geeft geattachtAfdJr een ID.  Neem dit
                // id over in het gedetachte afdelingsJaar.

                afdelingsJaar.ID = geattachtAfdJr.ID;
            }

            return afdelingsJaar;
        }

        public IList<OfficieleAfdeling> OphalenOfficieleAfdelingen()
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                IList<OfficieleAfdeling> result = (
                    from d in db.OfficieleAfdeling
                    select d
                ).ToList();
                return db.DetachObjectGraph(result);
            }
        }

        public IList<Afdeling> OphalenEigenAfdelingen(int groep)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                IList<Afdeling> result = (
                    from d in db.Afdeling
                    where d.Groep.ID == groep
                    select d
                ).ToList();
                return db.DetachObjectGraph(result);
            }
        }

    }
}
