using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using System.Data.Objects;
using Cg2.Data.Ef;
using Cg2.EfWrapper.Entity;

namespace Cg2.Data.Ef
{
    public class AfdelingsJaarDao: Dao<AfdelingsJaar>, IAfdelingsJaarDao
    {
        public AfdelingsJaarDao(): base()
        {
            connectedEntities = new System.Linq.Expressions.Expression<Func<AfdelingsJaar, object>>[3]
            {
                aj => aj.Afdeling.WithoutUpdate()
                    , aj => aj.GroepsWerkJaar.WithoutUpdate()
                        , aj => aj.OfficieleAfdeling.WithoutUpdate()
            };
        }

        /// <summary>
        /// Afdelingsjaar ophalen op basis van ID's van de
        /// afdeling en het groepswerkjaar.  Samen met afdelingsjaar
        /// wordt GroepsWerkJaar, OfficieleAfdeling en Afdeling teruggegeven.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
        /// <param name="afdelingID">ID van de afdeling</param>
        /// <returns>het gevraagde afdelingsjaar, of null indien niet
        /// gevonden.</returns>
        /// <remarks>Dit heeft enkel zin als de afdeling bepaald door
        /// AfdelingID een afdeling is van de groep bepaald door het
        /// gevraagde groepswerkjaar.</remarks>
        public AfdelingsJaar Ophalen(int groepsWerkJaarID, int afdelingID)
        {
            AfdelingsJaar resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.AfdelingsJaar.MergeOption = MergeOption.NoTracking;

                resultaat = (
                    from AfdelingsJaar aj 
                        in db.AfdelingsJaar
                        .Include("Afdeling")
                        .Include("GroepsWerkJaar")
                        .Include("OfficieleAfdeling")
                    where aj.GroepsWerkJaar.ID == groepsWerkJaarID
                    && aj.Afdeling.ID == afdelingID
                    select aj).FirstOrDefault();

                // Aangezien Eager Loading niet werkt, doen we het manueel :(

                if (resultaat != null)
                {
                    resultaat.GroepsWerkJaarReference.Load();
                    resultaat.AfdelingReference.Load();
                    resultaat.OfficieleAfdelingReference.Load();
                }
            }

            return resultaat;
        }
    }
}
