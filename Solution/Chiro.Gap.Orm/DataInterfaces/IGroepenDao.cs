using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
    /// <summary>
    /// Interface voor de 'Groepenrepository'
    /// </summary>
    public interface IGroepenDao: IDao<Groep>
    {
        /// <summary>
        /// Bepaalt het recentste GroepsWerkJaar van een gegeven Groep.
        /// Voor een actieve groep is dit steeds het huidige 
        /// GroepsWerkJaar; er kan pas een GroepsWerkJaar gemaakt 
        /// worden als het nieuw werkjaar begonnen is.
        /// </summary>
        /// <param name="groepID">ID van Groep waarvoor werkjaar bepaald 
        /// moet worden</param>
        /// <returns>Het relevante GroepsWerkJaarobject</returns>
        GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID);

        Groep OphalenMetAfdelingen(int groepID);

        IList<OfficieleAfdeling> OphalenOfficieleAfdelingen();

        /// <summary>
        /// Haalt het groepswerkjaar op bij een gegeven GroepsWerkJaarID
        /// </summary>
        /// <param name="p">ID van het gevraagde GroepsWerkJaar</param>
        /// <returns>Gevraagde groepswerkjaar</returns>
        GroepsWerkJaar GroepsWerkJaarOphalen(int groepsWerkJaarID);

        /// <summary>
        /// Haalt het groepswerkjaar op bij een gegeven GroepsWerkJaarID
        /// samen met alle info over het AfdelingsJaar, de Afdeling, de gelinkte
        /// OfficieleAfdeling, de Kinderen en de Leiding, ...
        /// </summary>
        /// <param name="p">ID van het gevraagde GroepsWerkJaar</param>
        /// <returns>Gevraagde groepswerkjaar</returns>
        GroepsWerkJaar GroepsWerkJaarOphalenMetAfdelingInfo(int groepsWerkJaarID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groepID"></param>
        /// <returns></returns>
        Groep OphalenMetGroepsWerkJaren(int groepID);

    }
}
