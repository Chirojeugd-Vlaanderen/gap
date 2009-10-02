using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;
using System.Data.Objects;
using System.Linq.Expressions;

namespace Cg2.Data.Ef
{
    public class CategorieenDao: Dao<Categorie>, ICategorieenDao
    {
		  public CategorieenDao()
        {
            connectedEntities = new Expression<Func<Categorie, object>>[2] { 
                                        e => e.Groep, 
                                        e => e.GelieerdePersoon };
        }
    }
}
