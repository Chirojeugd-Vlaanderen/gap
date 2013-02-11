using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IAbonnementenManager
    {
        /// <summary>
        /// Creëert een abonnement voor de gelieerde persoon <paramref name="gp"/> op publicatie
        /// <paramref name="publicatie"/> in het groepswerkjaar <paramref name="groepsWerkJaar"/>.
        /// </summary>
        /// <param name="publicatie">
        /// Publicatie waarvoor abonnement aangevraagd wordt
        /// </param>
        /// <param name="gp">
        /// Gelieerde persoon die abonnement moet krijgen
        /// </param>
        /// <param name="groepsWerkJaar">
        /// Groepswerkjaar waarvoor abonnement aangevraagd wordt
        /// </param>
        /// <returns>
        /// Het aangevraagde abonnement
        /// </returns>
        /// <exception cref="BlokkerendeObjectenException{TEntiteit}">
        /// Komt voor als de <paramref name="gp"/> voor het opgegeven <paramref name="groepsWerkJaar"/> al een
        /// abonnement heeft op die <paramref name="publicatie"/>.
        /// </exception>
        /// <exception cref="FoutNummerException">
        /// Komt voor als de publicatie niet meer uitgegeven wordt en je dus geen abonnement meer kunt aanvragen,
        /// als de bestelperiode voorbij is, of als de <paramref name="gp"/> geen adres heeft waar we de publicatie 
        /// naar kunnen opsturen.
        /// </exception>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker geen GAV is voor de groep waar het <paramref name="groepsWerkJaar"/>
        /// aan gekoppeld is.
        /// </exception>
        Abonnement Abonneren(Publicatie publicatie, GelieerdePersoon gp, GroepsWerkJaar groepsWerkJaar);

        /// <summary>
        /// Geeft <c>true</c> als de <paramref name="gelieerdePersoon"/> een Dubbelpuntabonnement heeft voor
        /// het huidige werkjaar.
        /// </summary>
        /// <param name="gelieerdePersoon">een gelieerde persoon</param>
        /// <returns>
        /// <c>true</c> als de <paramref name="gelieerdePersoon"/> een Dubbelpuntabonnement heeft voor
        /// het huidige werkjaar.
        /// </returns>
        bool KrijgtDubbelpunt(GelieerdePersoon gelieerdePersoon);
    }
}