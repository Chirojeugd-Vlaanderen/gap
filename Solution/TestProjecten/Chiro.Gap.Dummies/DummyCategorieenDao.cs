using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Dummy CategorieenDao, die niets doet
    /// </summary>
    class DummyCategorieenDao: ICategorieenDao
    {
        #region IDao<Categorie> Members

        public Chiro.Gap.Orm.Categorie Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Categorie Ophalen(int id, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Categorie, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Chiro.Gap.Orm.Categorie> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Categorie Bewaren(Chiro.Gap.Orm.Categorie nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Categorie Bewaren(Chiro.Gap.Orm.Categorie entiteit, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Categorie, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.Categorie> Bewaren(IList<Chiro.Gap.Orm.Categorie> es, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Categorie, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Categorie, object>>[] getConnectedEntities()
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
