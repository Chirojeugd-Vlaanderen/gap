using System;
using System.Diagnostics;
using System.Linq;
using Chiro.Kip.Data;
using Groep = Chiro.Kip.ServiceContracts.DataContracts.Groep;

namespace Chiro.Kip.Services
{
    public partial class SyncPersoonService
    {
        /// <summary>
        /// Updatet de gegevens van groep <paramref name="g"/> in Kipadmin. Het stamnummer van <paramref name="g"/>
        /// bepaalt de groep waarover het gaat.
        /// </summary>
        /// <param name="g">Te updaten groep in Kipadmin</param>
        public void GroepUpdaten(Groep g)
        {
            using (var db = new kipadminEntities())
            {
                var groep = (from cg in db.Groep.OfType<ChiroGroep>()
                             where String.Compare(cg.STAMNR, g.Code, StringComparison.OrdinalIgnoreCase) == 0
                             select cg).FirstOrDefault();

                Debug.Assert(groep != null);
                groep.Naam = g.Naam;
                db.SaveChanges();
                _log.BerichtLoggen(groep.GroepID, String.Format("Groep {0} hernoemd: {1}", g.Code, g.Naam));
            }
        }

    }
}
