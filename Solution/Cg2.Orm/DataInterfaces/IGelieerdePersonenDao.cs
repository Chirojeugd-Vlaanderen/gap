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
        /// <param name="aantalOpgehaald">outputparameter die aangeeft hoeveel personen meegegeven zijn</param>
        /// <returns>Lijst met gelieerde personen</returns>
        IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);

        /// <summary>
        /// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op, inclusief
        /// eventueel lidobject in het gegeven werkjaar.
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="pagina">paginanummer (1 of groter)</param>
        /// <param name="paginaGrootte">aantal records op een pagina</param>
        /// <param name="werkJaar">werkjaar waarvoor lidinfo op te halen</param>
        /// <param name="aantalOpgehaald">outputparameter die aangeeft hoeveel personen meegegeven zijn</param>
        /// <returns>Lijst met gelieerde personen</returns>
        IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, int werkJaar, out int aantalOpgehaald);

        /// <summary>
        /// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op, inclusief
        /// eventueel lidobject in het recentste werkjaar.
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="pagina">paginanummer (1 of groter)</param>
        /// <param name="paginaGrootte">aantal records op een pagina</param>
        /// <param name="aantalOpgehaald">outputparameter die aangeeft hoeveel personen meegegeven zijn</param>
        /// <returns>Lijst met gelieerde personen</returns>
        IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);

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
