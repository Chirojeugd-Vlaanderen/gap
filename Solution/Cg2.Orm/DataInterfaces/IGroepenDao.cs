using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm.DataInterfaces
{
    /// <summary>
    /// Interface voor de 'Groepenrepository'
    /// </summary>
    public interface IGroepenDao: IDao<Groep>
    {
        /// <summary>
        /// Bepaalt het huidige GroepsWerkJaar van een gegeven Groep.
        /// Dit moet steeds het 'laatst' toegevoegde zijn; er wordt pas
        /// een nieuw GroepsWerkJaar gemaakt als het nieuw werkjaar
        /// begint.
        /// </summary>
        /// <param name="groepID">ID van Groep waarvoor werkjaar bepaald 
        /// moet worden</param>
        /// <returns>Het relevante GroepsWerkJaarobject</returns>
        GroepsWerkJaar HuidigWerkJaarGet(int groepID);
    }
}
