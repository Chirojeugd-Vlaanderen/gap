using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;
using System.Data.Objects;

namespace Cg2.Data.Ef
{
    public class CategorieenDao: Dao<Categorie>, ICategorieenDao
    {
        public Categorie GroepLaden(Categorie categorie)
        {
            if (categorie.Groep == null)
            {
                using (ChiroGroepEntities db = new ChiroGroepEntities())
                {
                    db.Categorie.MergeOption = MergeOption.NoTracking;

                    Groep g
                        = (from c in db.Categorie
                           where c.ID == categorie.ID
                           select c.Groep).FirstOrDefault();

                    categorie.Groep = g;
                    g.Categorie.Add(categorie);
                }
            }
            return categorie;
        }
    }
}
