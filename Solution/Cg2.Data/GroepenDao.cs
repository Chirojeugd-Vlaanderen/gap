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

        public GroepsWerkJaar OphalenNieuwsteGroepsWerkjaar(int groepID)
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
            // TODO Fix this constante
            int huidigwerkjaar = 2008;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
                return (
                    from t in db.Groep.Include("Afdeling.AfdelingsJaar.OfficieleAfdeling")
                    where t.ID == groepID //TODO&& t.GroepsWerkJaar. == huidigwerkjaar
                    select t).FirstOrDefault<Groep>();
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


        public void ToevoegenAfdeling(int groepID, string naam, string afkorting)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                var gp = (
                    from g in db.Groep
                    where g.ID == groepID
                    select g
                    ).FirstOrDefault<Groep>();

                Afdeling a = new Afdeling();

                a.AfdelingsNaam = naam;
                a.Afkorting = afkorting;
                a.Groep = gp;

                gp.Afdeling.Add(a);

                db.SaveChanges();
            }
        }

        public void ToevoegenAfdelingsJaar(Groep g, Afdeling a, OfficieleAfdeling oa, int geboortejaarbegin, int geboortejaareind)
        {
            GroepsWerkJaar huidigwerkjaar = null; //TODO

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.Attach(g);
                db.Attach(a);
                db.Attach(oa);

                IList<OfficieleAfdeling> oas = (
                    from t in db.OfficieleAfdeling
                    select t                    
                    ).ToList<OfficieleAfdeling>();

                if (geboortejaarbegin < System.DateTime.Today.Year - 20
                    || geboortejaarbegin > geboortejaareind
                    || geboortejaareind > System.DateTime.Today.Year - 5)
                {
                    throw new InvalidOperationException("Ongeldige geboortejaren voor het afdelingsjaar");
                }

                if(!g.Afdeling.Contains(a) || !oas.Contains(oa))
                {
                    throw new InvalidOperationException("Gebruik van ongeldige objecten");
                }

                //TODO check if no conflicts with existing afdelingsjaar

                AfdelingsJaar afdelingsjaar = new AfdelingsJaar();

                afdelingsjaar.OfficieleAfdeling = oa;
                afdelingsjaar.Afdeling = a;
                afdelingsjaar.GroepsWerkJaar = huidigwerkjaar;

                a.AfdelingsJaar.Add(afdelingsjaar);

                db.SaveChanges();
            }
        }

        public IList<OfficieleAfdeling> OphalenOfficieleAfdelingen()
        {
            //TODO
            throw new NotImplementedException();

        }
        
    }
}
