using System;
using System.Collections.Generic;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IGebruikersRechtenManager
    {
        /// <summary>
        /// Verlengt het gegeven <paramref name="gebruikersRecht"/> (indien mogelijk) tot het standaard aantal maanden
        /// na vandaag.
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te verlengen gebruikersrecht
        /// </param>
        void Verlengen(GebruikersRecht gebruikersRecht);

        /// <summary>
        /// Zoekt een account voor de gegeven <paramref name="gelieerdePersoon"/>, en maakt een account aan
        /// als die niet gevonden wordt. Als een nieuwe account wordt gemaakt, krijgt de gelieerde persoon
        /// een e-mail via zijn voorkeursmailadres.
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon waarvoor een account gezocht of gemaakt moet 
        /// worden. Er wordt verondersteld dat die al gekoppeld is aan zijn accounts. Wat mogelijk een
        /// beetje stom is. Soit.</param>
        /// <returns>Account voor de gelieerde persoon (klasse Gav zou beter hernoemd worden als account, 
        /// zie #1357)</returns>
        /// <remarks>De gemaakte account heeft geen rechten.</remarks>
        Gav AccountZoekenOfMaken(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Zoekt een account voor de gegeven <paramref name="gelieerdePersoon"/>. Als geen account wordt
        /// gevonden, en <paramref name="makenAlsNietGevonden"/> <c>true</c> is, wordt een nieuwe account 
        /// aangemaakt, en krijgt de gelieerde persoon een e-mail via zijn voorkeursmailadres.
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon waarvoor een account gezocht of gemaakt moet 
        /// worden. Er wordt verondersteld dat die al gekoppeld is aan zijn accounts. Wat mogelijk een
        /// beetje stom is. Soit.</param>
        /// <param name="makenAlsNietGevonden"> Indien <c>true</c> wordt een nieuwe account gemaakt als
        /// er geen is gevonden.</param>
        /// <returns>Account voor de gelieerde persoon (klasse Gav zou beter hernoemd worden als account, 
        /// zie #1357)</returns>
        /// <remarks>De gemaakte account heeft geen rechten.</remarks>
        Gav AccountZoekenOfMaken(GelieerdePersoon gelieerdePersoon, bool makenAlsNietGevonden);

        /// <summary>
        /// Pas de vervaldatum van het gegeven <paramref name="gebruikersRecht"/> aan, zodanig dat
        /// het niet meer geldig is.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te vervallen gebruikersrecht
        /// </param>
        void Intrekken(GebruikersRecht gebruikersRecht);

        /// <summary>
        /// Pas de vervaldatum van het de <paramref name="gebruikersRechten"/> aan, zodanig dat
        /// ze niet meer geldig zijn.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRechten">
        /// Te vervallen gebruikersrecht
        /// </param>
        void Intrekken(GebruikersRecht[] gebruikersRechten);

        /// <summary>
        /// Gegeven een <paramref name="groepID"/>, haal van de GAV's waarvan 
        /// de personen gekend zijn, de gelieerde personen op (die dus de
        /// koppeling bepalen tussen de persoon en de groep met gegeven
        /// <paramref name="groepID"/>).
        /// Voorlopig worden ook de communicatiemiddelen mee opgeleverd.
        /// </summary>
        /// <param name="groepID">
        /// ID van een groep
        /// </param>
        /// <returns>
        /// Beschikbare gelieerde personen waarvan we weten dat ze GAV zijn
        /// voor die groep
        /// </returns>
        /// <remarks>
        /// Vervallen GAV's worden niet opgeleverd
        /// </remarks>
        IEnumerable<GelieerdePersoon> GavGelieerdePersonenOphalen(int groepID);

        /// <summary>
        /// Geeft de gegeven <paramref name="gav"/> gebruikersrechten op de groep van <paramref name="notificatieOntvanger"/>,
        /// en stuurt <paramref name="notificatieOntvanger"/> een mailtje dat jij dat gebruikersrecht hebt gekregen.
        /// (Dit is uiteraard enkel zinvol wanneer je supergav-rechten hebt, en ook gewone gebruikersrechten wil krijgen).
        /// PERSISTEERT, want GAV-maken en mailtje sturen moet in 1 transactie
        /// </summary>
        /// <param name="gav">
        /// GAV die gebruikersrecht moet krijgen, met daaraan gekoppeld de groepen waarop hij/zij
        /// al gebruikersrecht heeft.
        /// </param>
        /// <param name="notificatieOntvanger">
        /// De gelieerde persoon die de groep bepaalt de <paramref name="gav"/> 
        /// rechten voor moet krijgen, en die een mailtje zal krijgen waarin staat dat hij/zij rechten hebt gekregen.  
        /// </param>
        /// <param name="vervalDatum">
        /// Vervaldatum van het nieuwe gebruikersrecht.
        /// </param>
        /// <param name="reden">
        /// Reden waarom het gebruikersrecht aangevraagd werd.  Wordt mee gemaild naar de
        /// <paramref name="notificatieOntvanger"/>
        /// </param>
        /// <returns>
        /// Het gecreëerde gebruikersrecht
        /// </returns>
        /// <remarks>
        /// Als de gebruiker al een gebruikersrecht heeft, wordt enkel de vervaldatum aangepast
        /// </remarks>
        GebruikersRecht ToekennenOfVerlengen(Gav gav,
                                                             GelieerdePersoon notificatieOntvanger,
                                                             DateTime vervalDatum,
                                                             string reden);

        /// <summary>
        /// Haalt de GAV op voor de gebruiker die momenteel is aangemeld.  Als er zo nog geen bestaat, wordt
        /// die aangemaakt.
        /// </summary>
        /// <returns>
        /// GAV, met eventueel gekoppelde groepen
        /// </returns>
        Gav MijnGavOphalen();

        /// <summary>
        /// Kent gebruikersrechten toe voor gegeven <paramref name="groep"/> aan gegeven <paramref name="account"/>.
        /// Standaard zijn deze rechten 14 maand geldig. Als de gebruikersrechten al bestonden, worden ze indien
        /// mogelijk verlengd.
        /// </summary>
        /// <param name="account">account die gebruikersrecht moet krijgen op <paramref name="groep"/></param>
        /// <param name="groep">groep waarvoor <paramref name="account"/> gebruikersrecht moet krijgen</param>
        /// <returns>Het gebruikersrecht</returns>
        /// <remarks>Persisteert niet.</remarks>
        GebruikersRecht ToekennenOfVerlengen(Gav account, Groep groep);
    }
}