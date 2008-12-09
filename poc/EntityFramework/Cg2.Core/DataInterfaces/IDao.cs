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
    /// <typeparam name="IdT">Type van het ID van de entiteit</typeparam>
    public interface IDao<T, IdT>
    {
        T Ophalen(IdT id);
        List<T> AllesOphalen();
        T Bewaren(T entiteit);
        void Verwijderen(T entiteit);
        void Commit();
    }

    /// <summary>
    /// Deze interface bepaalt hoe DAO-objecten geconstrueerd moeten worden.
    /// 
    /// Aangepast uit 'Pro LINQ Object Relational Mapping with C# 2008'.
    /// </summary>
    public interface IDaoFactory
    {
        IGroepenDao GroepenDaoGet();
        IChiroGroepenDao ChiroGroepenDaoGet();
        IPersonenDao PersonenDaoGet();
        ICommunicatieVormenDao CommunicatieVormenDaoGet();
    }

    #region Inline interface declarations
    public interface IPersonenDao : IDao<Persoon, int> { }
    public interface ICommunicatieVormenDao : IDao<CommunicatieVorm, int> { }
    public interface IGroepenDao : IDao<Groep, int> { }
    public interface IChiroGroepenDao : IDao<ChiroGroep, int> { }
    #endregion
}
