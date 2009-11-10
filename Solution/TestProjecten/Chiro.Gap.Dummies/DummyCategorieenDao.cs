using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Dummy CategorieenDao, die niets doet
    /// </summary>
    class DummyCategorieenDao: ICategorieenDao
    {
        #region IDao<Categorie> Members

        public Categorie Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Categorie Ophalen(int id, params System.Linq.Expressions.Expression<Func<Categorie, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Categorie> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Categorie Bewaren(Categorie nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Categorie Bewaren(Categorie entiteit, params System.Linq.Expressions.Expression<Func<Categorie, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Categorie> Bewaren(IEnumerable<Categorie> es, params System.Linq.Expressions.Expression<Func<Categorie, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Categorie, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Categorie> OphalenVanGroep(int groepID)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
