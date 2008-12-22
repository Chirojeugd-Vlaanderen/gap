using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cg2.Core.Domain;

namespace Cg2.Core.DataInterfaces
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
        T Bewaren(T entiteit);
        T Updaten(T nieuweEntiteit, T oorspronkelijkeEntiteit);
        void Verwijderen(T entiteit);
        void Commit();
    }

}
