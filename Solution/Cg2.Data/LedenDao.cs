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


namespace Cg2.Data.Ef
{
    public class LedenDao: Dao<Lid>, ILedenDao
    {
        /// <summary>
        /// Creeert een nieuw lid
        /// </summary>
        /// <param name="l">lidobject</param>
        /// <returns>bewaarde lidobject</returns>
        /// <remarks>Wijzigingen in GroepsWerkJaar of
        /// GelieerdePersoon worden niet meegenomen!</remarks>
        public override Lid Creeren(Lid l)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.AttachObjectGraph(l, lid => lid.GroepsWerkJaar, lid => lid.GelieerdePersoon);
                db.SaveChanges();
            }
            return l;
        }

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

        /// <summary>
        /// ook om te maken en te deleten
        /// </summary>
        /// <param name="persoon"></param>
        public void LidBewaren(Lid lid)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
