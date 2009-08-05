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

        /*Groep OphalenMetAdressen(int groepID);

        Groep OphalenMetCategorieen(int groepID);

        Groep OphalenMetFuncties(int groepID);

        Groep OphalenMetAfdelingen(int groepID);

        Groep OphalenMetVrijeVelden(int groepID);

        void BewarenMetAdressen(Groep g);

        void BewarenMetCategorieen(Groep g);

        void BewarenMetFuncties(Groep g);

        void BewarenMetAfdelingen(Groep g);

        void BewarenMetVrijeVelden(Groep g);*/

        IList<OfficieleAfdeling> OphalenOfficieleAfdelingen();

        //IList<Afdeling> OphalenEigenAfdelingen(int groep);
    }
}
