using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Chiro.Cdf.Data
{
    /// <summary>
    /// Algemeen contract voor CRUD-operaties in de ORM-laag.  Implementeert een 'Repository'.
    /// 
    /// Aangepast uit 'Pro LINQ Object Relational Mapping with C# 2008'.
    /// </summary>
    /// <typeparam name="T">Entiteit die 'geDaot' moet worden</typeparam>
    public interface IDao<T>
    {
        T Ophalen(int id);
        T Ophalen(int id, params Expression<Func<T, object>>[] paths);
        IList<T> AllesOphalen();
        T Bewaren(T nieuweEntiteit);
        T Bewaren(T entiteit, params Expression<Func<T, object>>[] paths);
        IEnumerable<T> Bewaren(IEnumerable<T> es, params Expression<Func<T, object>>[] paths);
        Expression<Func<T, object>>[] getConnectedEntities();
    }

}
