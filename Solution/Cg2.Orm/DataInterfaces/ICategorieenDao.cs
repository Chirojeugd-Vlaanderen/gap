using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Orm.DataInterfaces
{
    public interface ICategorieenDao: IDao<Categorie>
    {
        /*/// <summary>
        /// Haalt de gekoppelde groep van een categorie op uit de database
        /// (als dat nog niet gebeurd zou zijn)
        /// </summary>
        /// <param name="categorie">Categorie met op te halen groep</param>
        /// <returns>Dezelfde categorie</returns>
        Categorie GroepLaden(Categorie categorie);*/

        IEnumerable<Categorie> OphalenVanGroep(int groepID);
    }
}
