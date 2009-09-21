using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;
using System.Data.Objects;
using Cg2.EfWrapper;

namespace Cg2.Data.Ef
{
    public class AutorisatieDao : Dao<GebruikersRecht>, IAutorisatieDao
    {
        #region IAuthorisatieDao Members

        public GebruikersRecht RechtenMbtGroepGet(string login, int groepID)
        {
            GebruikersRecht resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GebruikersRecht.MergeOption = MergeOption.NoTracking;

                // LET OP: Hier mag geen rekening gehouden worden
                // met de vervaldatum; we willen ook rechten kunnen
                // opvragen die vervallen zijn.
                //
                // Naar de vervaldatum kijken moet gebeuren in de
                // businesslaag.

                var query
                    = from r in db.GebruikersRecht
                      where r.Groep.ID == groepID && r.Gav.Login == login
                      select r;

                resultaat = query.FirstOrDefault();
            }

            return resultaat;
        }

        public GebruikersRecht RechtenMbtGelieerdePersoonGet(string login, int gelieerdePersoonID)
        {
            GebruikersRecht resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GebruikersRecht.MergeOption = MergeOption.NoTracking;

                // LET OP: Hier mag geen rekening gehouden worden
                // met de vervaldatum; we willen ook rechten kunnen
                // opvragen die vervallen zijn.
                //
                // Naar de vervaldatum kijken moet gebeuren in de
                // businesslaag.

                var query
                    = from GebruikersRecht r in db.GebruikersRecht
                      where r.Gav.Login == login && r.Groep.GelieerdePersoon.Any(gp => gp.ID == gelieerdePersoonID)
                      select r;

                resultaat = query.FirstOrDefault();
            }

            return resultaat;
        }

        public bool IsGavGroep(string login, int groepID)
        {
            bool resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                var query
                    = from r in db.GebruikersRecht
                      where r.Groep.ID == groepID && r.Gav.Login == login
                      && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
                      select r;

                resultaat = query.Count() > 0;
            }

            return resultaat;
        }

        public bool IsGavGelieerdePersoon(string login, int gelieerdePersoonID)
        {
            bool resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // TODO: Check vervaldatum GAV!
                var query
                    = from GebruikersRecht r in db.GebruikersRecht
                      where r.Gav.Login == login && r.Groep.GelieerdePersoon.Any(gp => gp.ID == gelieerdePersoonID)
                      select r;

                resultaat = query.Count() > 0;
            }

            return resultaat;
        }


        public IEnumerable<Groep> GekoppeldeGroepenGet(string login)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // Hier kan het geen kwaad om wel rekening te houden
                // met de vervaldatum; dit is een specifieke query.
                //
                // In RechtenMbtGelieerdePersoonGet en RechtenMbtGroepGet
                // mogen we dat niet, omdat dit 'generieke' vragen zijn;
                // daar willen we ook vervallen rechten kunnen opvragen.

                return
                    (from r in db.GebruikersRecht
                     where r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
                     select r.Groep).ToList();
            }
        }

        public IList<int> EnkelMijnGelieerdePersonen(IList<int> gelieerdePersonenIDs, string login)
        {
            List<int> resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // TODO: Check vervaldatum

                var query
                    = db.GelieerdePersoon
                    .Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.ID, gelieerdePersonenIDs))
                    .Where(gp => gp.Groep.GebruikersRecht.Any(r => r.Gav.Login == login))
                    .Select(gp => gp.ID);

                resultaat = query.ToList<int>();
            }

            return resultaat;
        }


        public IList<int> EnkelMijnPersonen(IList<int> personenIDs, string login)
        {
            List<int> resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // TODO: Check vervaldatum!

                var query
                    = db.GelieerdePersoon
                    .Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.Persoon.ID, personenIDs))
                    .Where(gp => gp.Groep.GebruikersRecht.Any(r => r.Gav.Login == login))
                    .Select(gp => gp.Persoon.ID);

                resultaat = query.ToList<int>();
            }

            return resultaat;
        }

        public bool IsGavPersoon(string login, int persoonID)
        {
            bool resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // TODO: Check vervaldatum GAV!
                var query
                    = from GebruikersRecht r in db.GebruikersRecht
                      where r.Gav.Login == login && r.Groep.GelieerdePersoon.Any(gp => gp.Persoon.ID == persoonID)
                      select r;

                resultaat = query.Count() > 0;
            }

            return resultaat;
        }

        public bool IsGavGroepsWerkJaar(string login, int groepsWerkJaarID)
        {
            bool resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                var query
                    = from GebruikersRecht r in db.GebruikersRecht
                      where r.Gav.Login == login && r.Groep.GroepsWerkJaar.Any(gwj => gwj.ID == groepsWerkJaarID)
                      && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
                      select r;

                resultaat = query.Count() > 0;
            }
            return resultaat;
        }

        #endregion
    }
}
