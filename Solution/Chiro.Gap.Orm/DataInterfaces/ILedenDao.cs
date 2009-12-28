using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
    public interface ILedenDao: IDao<Lid>
    {
        IList<Lid> AllesOphalen(int groepsWerkJaarID);

        IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID);

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
