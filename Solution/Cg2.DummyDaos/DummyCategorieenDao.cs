using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Dummies
{
    /// <summary>
    /// Dummy CategorieenDao, die niets doet
    /// </summary>
    class DummyCategorieenDao: ICategorieenDao
    {
        #region IDao<Categorie> Members

        public Cg2.Orm.Categorie Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Categorie Ophalen(int id, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Categorie, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Cg2.Orm.Categorie> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Categorie Bewaren(Cg2.Orm.Categorie nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Categorie Bewaren(Cg2.Orm.Categorie entiteit, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Categorie, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.Categorie> Bewaren(IList<Cg2.Orm.Categorie> es, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Categorie, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Cg2.Orm.Categorie, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
