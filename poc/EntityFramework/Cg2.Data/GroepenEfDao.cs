using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.DataInterfaces;
using Cg2.Core.Domain;

namespace Cg2.Data.Ef
{
    public class GroepenEfDao: AbstractEfDao<Groep, int>, IGroepenDao
    {
        /// <summary>
        /// Haalt een groep op, op basis van GroepID
        /// </summary>
        /// <param name="id">GroepID van gevraagde groep</param>
        /// <returns>gevonden groep, of null indien niet gevonden</returns>
        public override Groep Ophalen(int id)
        {
            // TODO: Waarom is dit zo ingewikkeld?

            Groep resultaat;
            using (Cg2ObjectContext db = new Cg2ObjectContext())
            {
                resultaat = (from g in db.Groepen
                             where g.ID == id
                             select g).FirstOrDefault();
            }
            return resultaat;
        }
    }
}
