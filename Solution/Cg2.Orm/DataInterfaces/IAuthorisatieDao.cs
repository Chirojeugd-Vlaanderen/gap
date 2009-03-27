using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm.DataInterfaces
{
    public interface IAuthorisatieDao: IDao<GebruikersRecht>
    {
        /// <summary>
        /// Haalt lijst groepID's op waaraan de GAV met gegeven
        /// login MOMENTEEL gekoppeld is
        /// </summary>
        /// <param name="login">Gebruikersnaam van de GAV</param>
        /// <returns>lijst met groepID's</returns>
        IList<int> GekoppeldeGroepenGet(string login);

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
    }
}
