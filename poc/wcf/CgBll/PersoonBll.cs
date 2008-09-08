using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CgDal;

namespace CgBll
{
    /// <summary>
    /// (test)business Class voor Persoon.
    /// </summary>
    public class PersoonBll
    {
        private ChiroGroepEntities context;

        public PersoonBll()
        {
            context = new ChiroGroepEntities();
        }

        /// <summary>
        /// Haal persoonsgegevens op op basis van ID
        /// </summary>
        /// <param name="persoonID">ID van op te halen persoon</param>
        /// <returns>Gedetacht entityobject voor gevraagde persoon</returns>
        public Persoon PersoonGet(int persoonID)
        {
            var q = from p in context.Persoon where p.PersoonID == persoonID select p;
            var result = q.First();
            context.Detach(result);
            return result;
        }
    }
}
