using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using System.Data.Objects;

namespace Chiro.Gap.Data.Ef
{
    public class GroepsWerkJaarDao: Dao<GroepsWerkJaar>, IGroepsWerkJaarDao
    {
        /// <summary>
        /// Groepswerkjaar ophalen op basis van ID.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van gevraagd 
        /// groepswerkjaar</param>
        /// <returns>Groepswerkjaar en gekoppelde groep</returns>
        public override GroepsWerkJaar Ophalen(int groepsWerkJaarID)
        {
            GroepsWerkJaar resultaat;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GroepsWerkJaar.MergeOption = MergeOption.NoTracking;

                resultaat = (
                    from GroepsWerkJaar gwj
                    in db.GroepsWerkJaar.Include("Groep")
                    where gwj.ID == groepsWerkJaarID
                    select gwj).FirstOrDefault();

                if (resultaat != null)
                {
                    // Ook hier doet Eager Loading het niet...
                    resultaat.GroepReference.Load();
                }
            }

            return resultaat;
        }
    }
}
