using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.DataInterfaces;
using Cg2.Core.Domain;

namespace Cg2.Data.Ef
{
    public class ChiroGroepenEfDao: AbstractEfDao<ChiroGroep, int>, IChiroGroepenDao
    {
        /// <summary>
        /// Haalt een ChiroHroep op, op basis van GroepID
        /// </summary>
        /// <param name="id">GroepID van gevraagde ChiroGroep</param>
        /// <returns>gevonden chirogroep, of null indien geen chirogroep 
        /// gevonden</returns>
        public override ChiroGroep Ophalen(int id)
        {
            ChiroGroep resultaat;
            using (Cg2ObjectContext db = new Cg2ObjectContext())
            {
                resultaat = (from g in db.Groepen.OfType<ChiroGroep>()
                             where g.ID == id
                             select g).FirstOrDefault();
            }
            return resultaat;
        }
    }
}
