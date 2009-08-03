using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using System.Diagnostics;
using System.Data.Objects;
using System.Data;
using Cg2.Orm.DataInterfaces;

using Cg2.EfWrapper.Entity;
using System.Linq.Expressions;

namespace Cg2.Data.Ef
{
    public class LedenDao: Dao<Lid>, ILedenDao
    {

        // pagineren gebeurt nu per werkjaar
        // pagina, paginaGrootte en aantalTotaal zijn niet meer nodig
        public IList<Lid> PaginaOphalen(int groepsWerkJaarID)
        {
            IList<Lid> lijst;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.Lid.MergeOption = MergeOption.NoTracking;

                var leden = (
                    from l in db.Lid.Include("GelieerdePersoon.Persoon")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList<Lid>();

                var kinderen = (
                    from l in db.Lid.OfType<Kind>().Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList<Kind>();

                var leiding = (
                    from l in db.Lid.OfType<Leiding>().Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList<Leiding>();

                lijst = new List<Lid>();
                foreach (Lid lid in kinderen)
                {
                    lijst.Add(lid);
                }
                foreach (Lid lid in leiding)
                {
                    lijst.Add(lid);
                }
                // normaal gezien hebben we nu iedereen: kinderen + leiding
                // toch nog even controleren of er geen leden zijn die noch kind noch leiding zijn
                foreach (Lid lid in leden)
                {
                    if (!(lid is Kind) && !(lid is Leiding))
                    {
                        lijst.Add(lid);
                    }
                }
            }

            return lijst;
        }

        private Expression<Func<Lid, object>>[] connectedEntities = { e => e.GroepsWerkJaar, e => e.GelieerdePersoon };

        public override Expression<Func<Lid, object>>[] getConnectedEntities()
        {
            return connectedEntities;
        }

        public Lid OphalenMetDetails(int lidID)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.Lid.MergeOption = MergeOption.NoTracking;

                Lid lid = (
                    from t in db.Lid.Include("GelieerdePersoon").Include("GroepsWerkJaar")
                    where t.ID == lidID
                    select t).FirstOrDefault<Lid>();

                if (lid is Kind)
                {
                    return (
                        from t in db.Lid.OfType<Kind>().Include("GelieerdePersoon").Include("GroepsWerkJaar").Include("AfdelingsJaar")
                        where t.ID == lidID
                        select t).FirstOrDefault<Kind>();
                }
                else if (lid is Leiding)
                {
                    return (
                        from t in db.Lid.OfType<Leiding>().Include("GelieerdePersoon").Include("GroepsWerkJaar").Include("AfdelingsJaar")
                        where t.ID == lidID
                        select t).FirstOrDefault<Leiding>();
                }
                return lid;
            }
        }


    }
}
