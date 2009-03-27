using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm.DataInterfaces
{
    /// <summary>
    /// Data Access Object voor adressen.  
    /// 
    /// TODO: Straat en subgemeente zouden standaard
    /// mee opgehaald moeten worden.
    /// </summary>
    public interface IAdressenDao: IDao<Adres>
    {
        /// <summary>
        /// Haalt een adres op, samen met de gekoppelde personen
        /// </summary>
        /// <param name="adresID">ID op te halen adres</param>
        /// <param name="gelieerdAan">Als een lijst met groepID's gegeven,
        /// dan worden enkel personen gelieerd aan groepen met ID's uit
        /// de lijst meegeleverd.  Indien gelieerdAan null is, krijg
        /// je alle bewoners mee</param>
        /// <returns>Adresobject met gekoppelde personen</returns>
        Adres AdresMetBewonersOphalen(int adresID, IList<int> gelieerdAan);
    }
}
