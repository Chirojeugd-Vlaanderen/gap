using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Data.Ef
{
    public class AuthorisatieDao: Dao<GebruikersRecht>, IAuthorisatieDao
    {
        public GebruikersRecht RechtenMbtGroepGet(string login, int groepID)
        {
            GebruikersRecht resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
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
                // LET OP: Hier mag geen rekening gehouden worden
                // met de vervaldatum; we willen ook rechten kunnen
                // opvragen die vervallen zijn.
                //
                // Naar de vervaldatum kijken moet gebeuren in de
                // businesslaag.

                var query
                    = from r in db.GebruikersRecht
                      from gp in db.GelieerdePersoon
                      where r.Groep.ID == gp.Groep.ID && r.Gav.Login == login && gp.ID == gelieerdePersoonID 
                      select r;

                resultaat = query.FirstOrDefault();
            }

            return resultaat;
        }

        public IList<int> GekoppeldeGroepenGet(string login)
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
                     select r.Groep.ID).ToList<int>();
            }
        }
    }
}
