using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        T Creeren(T entiteit);
        T Bewaren(T nieuweEntiteit, T oorspronkelijkeEntiteit);
        void Verwijderen(T entiteit);
        void Commit();
    }

}
