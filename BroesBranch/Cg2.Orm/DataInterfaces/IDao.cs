using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Cg2.Orm.DataInterfaces
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
        List<T> AllesOphalen();
        T Bewaren(T nieuweEntiteit);
        Expression<Func<T, object>>[] getConnectedEntities();
    }

}
