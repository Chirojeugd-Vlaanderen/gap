using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm.DataInterfaces
{
    /// <summary>
    /// Met een GelieerdePersoon moet altijd het geassocieerde
    /// persoonsobject meekomen, anders heeft het weinig zin.
    /// </summary>
    public interface IPersonenDao: IDao<Persoon>
    {
        /// <summary>
        /// Haalt lijst op van Personen
        /// </summary>
        /// <param name="personenIDs">ID's van op te halen
        /// Personen</param>
        /// <returns>lijst met Personen</returns>
        IList<Persoon> LijstOphalen(IList<int> personenIDs);

        Persoon Bewaren(Persoon p);
    }
}
