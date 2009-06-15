using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm.DataInterfaces
{
    public interface ILedenDao: IDao<Lid>
    {
        /// <summary>
        /// Haalt een ledenlijst op van een bepaald groepswerkjaar
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van groepswerkjaar in kwestie</param>
        /// <param name="pagina">gevraagde pagina</param>
        /// <param name="paginaGrootte">leden per pagina</param>
        /// <param name="aantalTotaal">geeft totaal aantal leden in groepswerkjaar terug</param>
        /// <returns>Een lijst van leden, inclusief info gelieerde personen en personen</returns>
        IList<Lid> PaginaOphalen(int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalTotaal);

        //void LidMaken(int gelieerdeID);
    }
}
