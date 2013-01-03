using System.Collections.Generic;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IUitstappenManager
    {
        /// <summary>
        /// Koppelt een uitstap aan een groepswerkjaar.  Dit moet typisch
        /// enkel gebeuren bij een nieuwe uitstap.
        /// </summary>
        /// <param name="uitstap">
        /// Te koppelen uitstap
        /// </param>
        /// <param name="gwj">
        /// Te koppelen groepswerkjaar
        /// </param>
        /// <returns>
        /// <paramref name="uitstap"/>, gekoppeld aan <paramref name="gwj"/>.
        /// </returns>
        Uitstap Koppelen(Uitstap uitstap, GroepsWerkJaar gwj);

        /// <summary>
        /// Koppelt een plaats aan een uitstap
        /// </summary>
        /// <param name="uitstap">
        /// Te koppelen uitstap
        /// </param>
        /// <param name="plaats">
        /// Te koppelen plaats
        /// </param>
        /// <returns>
        /// Uitstap, met plaats gekoppeld.  Persisteert niet
        /// </returns>
        Uitstap Koppelen(Uitstap uitstap, Plaats plaats);

        /// <summary>
        /// Schrijft de gegeven <paramref name="gelieerdePersonen"/> in voor de gegeven
        /// <paramref name="uitstap"/>, al dan niet als <paramref name="logistiekDeelnemer"/>.
        /// </summary>
        /// <param name="uitstap">
        /// Uitstap waarvoor in te schrijven, gekoppeld aan groep
        /// </param>
        /// <param name="gelieerdePersonen">
        /// In te schrijven gelieerde personen, gekoppeld aan groep
        /// </param>
        /// <param name="logistiekDeelnemer">
        /// Als <c>true</c>, dan worden de 
        /// <paramref name="gelieerdePersonen"/> ingeschreven als logistiek deelnemer.
        /// </param>
        void Inschrijven(Uitstap uitstap,
                                         IEnumerable<GelieerdePersoon> gelieerdePersonen,
                                         bool logistiekDeelnemer);

        /// <summary>
        /// Stuurt alle bivakken van werkJaar <paramref name="werkJaar"/> opnieuw naar
        /// kipadmin.
        /// </summary>
        /// <param name="werkJaar">
        /// Het werkJaar waarvan de gegevens opnieuw gesynct moeten worden
        /// </param>
        void OpnieuwSyncen(int werkJaar);

        /// <summary>
        /// Gaat na welke gegevens er nog ontbreken in de geregistreerde bivakken om van een
        /// geldige bivakaangifte te kunnen spreken.
        /// </summary>
        /// <param name="groepID">
        /// De ID van de groep waar het over gaat.
        /// </param>
        /// <param name="groepsWerkJaar">
        /// Het werkJaar waarvoor de gegevens opgehaald moeten worden.
        /// </param>
        /// <returns>
        /// Een lijstje met opmerkingen/feedback voor de gebruiker, zodat die weet 
        /// of er nog iets extra's ingevuld moet worden.
        /// </returns>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker op dit moment geen GAV is voor de groep met de opgegeven <paramref name="groepID"/>,
        /// maar ook als de gebruiker geen GAV was/is in het opgegeven <paramref name="groepsWerkJaar"/>.
        /// </exception>
        BivakAangifteLijstInfo BivakStatusOphalen(int groepID, GroepsWerkJaar groepsWerkJaar);

        /// <summary>
        /// Bepaalt of het de tijd van het jaar is voor de bivakaangifte
        /// </summary>
        /// <param name="groepsWerkJaar">huidige groepswerkjaar</param>
        /// <returns><c>true</c> als de bivakaangifte voor <paramref name="groepsWerkJaar"/> moet worden doorgegeven, 
        /// anders <c>false</c></returns>
        bool BivakAangifteVanBelang(GroepsWerkJaar groepsWerkJaar);

        /// <summary>
        /// Bepaalt de status van de gegeven <paramref name="uitstap"/>
        /// </summary>
        /// <param name="uitstap">Uitstap, waarvan status bepaald moet worden</param>
        /// <returns>De status van de gegeven <paramref name="uitstap"/></returns>
        BivakAangifteStatus StatusBepalen(Uitstap uitstap);
    }
}