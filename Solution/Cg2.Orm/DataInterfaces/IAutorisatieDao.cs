using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm.DataInterfaces
{
    public interface IAutorisatieDao: IDao<GebruikersRecht>
    {
        #region Alle records in GebruikersRecht doorzoeken
        /// <summary>
        /// Haalt rechten op die een gebruiker heeft op een groep.
        /// </summary>
        /// <param name="login">gebruikersnaam</param>
        /// <param name="groepID">groepid</param>
        /// <returns>null indien geen gebruikersrechten gevonden,
        /// anders een GebruikersRecht-object</returns>
        /// <remarks>Let op: de gebruikersrechten kunnen vervallen zijn!</remarks>
        GebruikersRecht RechtenMbtGroepGet(string login, int groepID);

        /// <summary>
        /// Haalt rechten op die een gebruiker heeft op een gelieerde persoon.
        /// (Dit komt erop neer dat de gelieerde persoon gelieerd is aan een
        /// groep waar de gebruiker GAV van is.)
        /// </summary>
        /// <param name="login">gebruikersnaam</param>
        /// <param name="groepID">ID val gelieerde persoon</param>
        /// <returns>null indien geen gebruikersrechten gevonden,
        /// anders een GebruikersRecht-object</returns>
        /// <remarks>Let op: de gebruikersrechten kunnen vervallen zijn!</remarks>
        GebruikersRecht RechtenMbtGelieerdePersoonGet(string login, int GelieerdePersoonID);
        #endregion

        #region Enkel de niet-vervallen gebruikersrechten

        /// <summary>
        /// Controleert of een gebruiker *nu* GAV is van een
        /// gegeven groep
        /// </summary>
        /// <param name="login">gebruikersnaam gebruiker</param>
        /// <param name="groepID">id groep</param>
        /// <returns>true indien de gebruiker nu GAV is</returns>
        bool IsGavGroep(string login, int groepID);

        /// <summary>
        /// Controleert of een gebruiker *nu* GAV is van een
        /// de groep van een gelieerde persoon
        /// </summary>
        /// <param name="login">gebruikersnaam gebruiker</param>
        /// <param name="groepID">id gelieerde persoon</param>
        /// <returns>true indien gebruiker nu gav is</returns>
        bool IsGavGelieerdePersoon(string login, int gelieerdePersoonID);

        /// <summary>
        /// Controleert of een gebruiker *nu* GAV is van een
        /// groep waaraan de gegeven persoon gelieerd is.
        /// </summary>
        /// <param name="login">gebruikersnaam</param>
        /// <param name="persoonID">ID van persoon</param>
        /// <returns>true indien gebruiker geahtoriseerd is
        /// (op dit moment) om persoonsgegevens te zien/wijzigen</returns>
        bool IsGavPersoon(string login, int persoonID);

        /// <summary>
        /// Controleert of een gebruiker *nu* GAV is van de groep
        /// horende bij gegeven GroepsWerkJaar
        /// </summary>
        /// <param name="login">gebruikersnaam</param>
        /// <param name="groepsWerkJaarID">ID van gevraagde GroepsWerkJaar</param>
        /// <returns>true indien de gebruiker GAV is</returns>
        bool IsGavGroepsWerkJaar(string login, int groepsWerkJaarID);

        /// <summary>
        /// Haalt lijst groepen op waaraan de GAV met gegeven
        /// login MOMENTEEL gekoppeld is
        /// </summary>
        /// <param name="login">Gebruikersnaam van de GAV</param>
        /// <returns>lijst met groepen</returns>
        IEnumerable<Groep> GekoppeldeGroepenGet(string login);

        /// <summary>
        /// Verwijdert uit een lijst met GelieerdePersonenID's de ID's
        /// waarvan een gegeven gebruiker geen GAV is
        /// </summary>
        /// <param name="gelieerdePersonenIDs">lijst met GelieerdePersonenID's</param>
        /// <param name="login">gebruiker</param>
        /// <returns>Een lijst met de ID's van GelieerdePersonen waar de gebruiker
        /// GAV over is. (hoeveel indirectie kan er in 1 zin?)</returns>
        IList<int> EnkelMijnGelieerdePersonen(IList<int> gelieerdePersonenIDs, string login);

        /// <summary>
        /// Verwijdert uit een lijst met PersonenID's de ID's
        /// waarvan een gegeven gebruiker geen GAV is
        /// </summary>
        /// <param name="personenIDs">lijst met PersonenID's</param>
        /// <param name="login">gebruiker</param>
        /// <returns>Een lijst met de ID's van Personen waar de gebruiker
        /// GAV over is. (hoeveel indirectie kan er in 1 zin?)</returns>
        IList<int> EnkelMijnPersonen(IList<int> personenIDs, string p);

        #endregion

    }
}
