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
        GroepsWerkJaar OphalenNieuwsteGroepsWerkjaar(int groepID);

        Groep OphalenMetAdressen(int groepID);

        Groep OphalenMetCategorieen(int groepID);

        Groep OphalenMetFuncties(int groepID);

        Groep OphalenMetAfdelingen(int groepID);

        Groep OphalenMetVrijeVelden(int groepID);

        void BewarenMetAdressen(Groep g);

        void BewarenMetCategorieen(Groep g);

        void BewarenMetFuncties(Groep g);

        void BewarenMetAfdelingen(Groep g);

        void BewarenMetVrijeVelden(Groep g);

        void ToevoegenAfdeling(int groepID, string naam, string afkorting);

        void ToevoegenAfdelingsJaar(Groep g, Afdeling aj, OfficieleAfdeling oa, int geboortejaarbegin, int geboortejaareind);

        IList<OfficieleAfdeling> OphalenOfficieleAfdelingen();
    }
}
