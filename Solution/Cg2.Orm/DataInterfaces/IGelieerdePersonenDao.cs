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
    public interface IGelieerdePersonenDao: IDao<GelieerdePersoon>
    {
        /// <summary>
        /// Haalt lijst op van GelieerdePersonen + persoonsinfo
        /// </summary>
        /// <param name="gelieerdePersonenIDs">ID's van op te halen
        /// GelieerdePersonen</param>
        /// <returns>lijst met GelieerdePersonen</returns>
        IList<GelieerdePersoon> LijstOphalen(IList<int> gelieerdePersonenIDs);

        /// <summary>
        /// Haalt de persoonsgegevens van alle gelieerde personen van een groep op.
        /// </summary>
        /// <param name="GroepID">ID van de groep</param>
        /// <returns>Lijst van gelieerde personen</returns>
        IList<GelieerdePersoon> AllenOphalen(int GroepID);

        /// <summary>
        /// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="pagina">paginanummer (1 of groter)</param>
        /// <param name="paginaGrootte">aantal records op een pagina</param>
        /// <param name="aantalTotaal">outputparameter die aangeeft hoeveel personen er in de volledige lijst zitten</param>
        /// <returns>Lijst met gelieerde personen</returns>
        IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal);

        /// <summary>
        /// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op, inclusief
        /// eventueel lidobject in het gegeven werkjaar.
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="pagina">paginanummer (1 of groter)</param>
        /// <param name="paginaGrootte">aantal records op een pagina</param>
        /// <param name="werkJaar">werkjaar waarvoor lidinfo op te halen</param>
        /// <param name="aantalTotaal">outputparameter die aangeeft hoeveel personen er in de volledige lijst zitten</param>
        /// <returns>Lijst met gelieerde personen</returns>
        IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, int werkJaar, out int aantalTotaal);

        /// <summary>
        /// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op, inclusief
        /// eventueel lidobject in het recentste werkjaar.
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="pagina">paginanummer (1 of groter)</param>
        /// <param name="paginaGrootte">aantal records op een pagina</param>
        /// <param name="aantalTotaal">outputparameter die aangeeft hoeveel personen er in de volledige lijst zitten</param>
        /// <returns>Lijst met gelieerde personen</returns>
        IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal);

        /// <summary>
        /// Haalt persoonsgegevens van een gelieerd persoon op, incl. adressen en communicatievormen
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van op te halen gelieerde persoon</param>
        /// <returns>Gelieerde persoon met persoonsgegevens, adressen en communicatievormen</returns>
        GelieerdePersoon DetailsOphalen(int gelieerdePersoonID);

        /// <summary>
        /// Laadt groepsgegevens in GelieerdePersoonsobject
        /// </summary>
        /// <param name="p">gelieerde persoon</param>
        /// <returns>referentie naar p, nadat groepsgegevens
        /// geladen zijn</returns>
        GelieerdePersoon GroepLaden(GelieerdePersoon p);

        /// <summary>
        /// Haalt alle gelieerde personen op die op een zelfde
        /// adres wonen als de gelieerde persoon met het gegeven ID.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gegeven gelieerde
        /// persoon.</param>
        /// <returns>Lijst met GelieerdePersonen (inc. persoonsinfo)</returns>
        /// <remarks>Als de persoon nergens woont, is hij toch zijn eigen
        /// huisgenoot.</remarks>
        IList<GelieerdePersoon> HuisGenotenOphalen(int gelieerdePersoonID);
    }
}
