using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Orm.DataInterfaces
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

        /// <summary>
        /// Haalt alle personen op die op een zelfde
        /// adres wonen als de gelieerde persoon met het gegeven ID.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gegeven gelieerde
        /// persoon.</param>
        /// <returns>Lijst met GelieerdePersonen (inc. persoonsinfo)</returns>
        /// <remarks>Als de persoon nergens woont, is hij toch zijn eigen
        /// huisgenoot.</remarks>
        IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID);

        /// <summary>
        /// Haalt de persoon op die correspondeert met een gelieerde persoon.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde
        /// persoon.</param>
        /// <returns>Persoon inclusief adresinfo</returns>
        Persoon CorresponderendePersoonOphalen(int gelieerdePersoonID);
    }
}
