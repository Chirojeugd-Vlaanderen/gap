using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Cg2.Orm.DataInterfaces
{
    public interface ILedenDao: IDao<Lid>
    {
        /// <summary>
        /// Haalt een ledenlijst op van een bepaald groepswerkjaar
        /// Hier wordt geen paginering gebruikt
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van groepswerkjaar in kwestie</param>
        /// <returns>Een lijst van leden, inclusief info gelieerde personen en personen</returns>
        IList<Lid> PaginaOphalen(int groepsWerkJaarID);

        IList<Lid> PaginaOphalen(int groepsWerkJaarID, int afdelingsID);

        /// <summary>
        /// Zoekt lid op op basis van GelieerdePersoonID en GroepsWerkJaarID
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
        /// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
        /// <returns>Lidobject indien gevonden, anders null</returns>
        Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID);

        /// <summary>
        /// Haalt lid met gerelateerde entity's op, op basis van 
        /// GelieerdePersoonID en GroepsWerkJaarID
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
        /// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
        /// <param name="paths">lambda-expressies die de extra op te halen
        /// informatie definieren</param>
        /// <returns>Lidobject indien gevonden, anders null</returns>
        Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths);

        Lid OphalenMetDetails(int lidID);

        //void LidMaken(int gelieerdeID);
    }
}
