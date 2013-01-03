using System.Collections.Generic;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IAbonnementenManager
    {
        /// <summary>
        /// Haalt een publicatie op, gegeven zijn <paramref name="publicatieID"/>
        /// </summary>
        /// <param name="publicatieID">
        /// Bepaalt op te halen publicatie
        /// </param>
        /// <returns>
        /// De gevraagde publicatie
        /// </returns>
        Publicatie PublicatieOphalen(PublicatieID publicatieID);

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
        /// Persisteert een abonnement
        /// </summary>
        /// <param name="abonnement">
        /// Te persisteren abonnement
        /// </param>
        void Bewaren(Abonnement abonnement);

        /// <summary>
        /// Haalt alle abonnementen op uit een gegeven groepswerkjaar, inclusief personen, voorkeursadressen, 
        /// groepswerkjaar en groep.
        /// </summary>
        /// <param name="gwjID">
        /// ID van het gegeven groepswerkjaar
        /// </param>
        /// <returns>
        /// Alle abonnementen op uit een gegeven groepswerkjaar, inclusief personen, voorkeursadressen, 
        /// groepswerkjaar en groep.
        /// </returns>
        IEnumerable<Abonnement> OphalenUitGroepsWerkJaar(int gwjID);
    }
}