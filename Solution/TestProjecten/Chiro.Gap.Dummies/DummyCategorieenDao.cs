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
    class DummyCategorieenDao: DummyDao<Categorie>, ICategorieenDao
    {
        public IEnumerable<Categorie> OphalenVanGroep(int groepID)
        {
            throw new NotImplementedException();
        }
    }
}
