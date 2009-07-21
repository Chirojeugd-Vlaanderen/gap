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

        public IList<Lid> PaginaOphalen(int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            IList<Lid> lijst;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.Lid.MergeOption = MergeOption.NoTracking;

                var result = (
                    from l in db.Lid.Include("GelieerdePersoon.Persoon")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte);

                lijst = result.ToList<Lid>();
                aantalTotaal = lijst.Count;
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
