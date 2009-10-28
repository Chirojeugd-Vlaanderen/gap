using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Chiro.Gap.Orm.DataInterfaces
{
    /// <summary>
    /// Algemeen contract voor CRUD-operaties in de ORM-laag.
    /// 
    /// Aangepast uit 'Pro LINQ Object Relational Mapping with C# 2008'.
    /// </summary>
    /// <typeparam name="T">Entiteit die 'geDaot' moet worden</typeparam>
    public interface IDao<T>
    {
        T Ophalen(int id);
        T Ophalen(int id, params Expression<Func<T, object>>[] paths);
        List<T> AllesOphalen();
        T Bewaren(T nieuweEntiteit);
        T Bewaren(T entiteit, params Expression<Func<T, object>>[] paths);
        IList<T> Bewaren(IList<T> es, params Expression<Func<T, object>>[] paths);
        Expression<Func<T, object>>[] getConnectedEntities();
    }

}
