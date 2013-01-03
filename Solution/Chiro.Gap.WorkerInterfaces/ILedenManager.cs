using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface ILedenManager
    {
        /// <summary>
        /// Doet een voorstel voor de inschrijving van de gegeven gelieerdepersoon <paramref name="gp"/> in groepswerkjaar 
        /// <paramref name="gwj"/>
        /// <para />
        /// Als de persoon in een afdeling past, krijgt hij die afdeling. Als er meerdere passen, wordt er een gekozen.
        /// Als de persoon niet in een afdeling past, en <paramref name="leidingIndienMogelijk"/> <c>true</c> is, 
        /// wordt hij leiding als hij oud genoeg is.
        /// Anders wordt een afdeling gekozen die het dichtst aanleunt bij de leeftijd van de persoon.
        /// Zijn er geen afdelingen, dan wordt een exception opgeworpen.
        /// </summary>
        /// <param name="gp">
        /// De persoon om in te schrijven, gekoppeld met groep en persoon
        /// </param>
        /// <param name="gwj">
        /// Het groepswerkjaar waarin moet worden ingeschreven, gekoppeld met afdelingsjaren
        /// </param>
        /// <param name="leidingIndienMogelijk">Als deze <c>true</c> is, dan stelt de method voor om een persoon
        /// leiding te maken als er geen geschikte afdeling is, en hij/zij oud genoeg is.</param>
        /// <returns>
        /// Voorstel tot inschrijving
        /// </returns>
        LidVoorstel InschrijvingVoorstellen(GelieerdePersoon gp, GroepsWerkJaar gwj, bool leidingIndienMogelijk);

        /// <summary>
        /// Wijzigt een bestaand lid, op basis van de gegevens in <paramref name="voorstellid"/>.
        /// (in praktijk wordt het lid verwijderd en terug aangemaakt.  Wat op zich zo geen ramp is,
        /// maar wel tot problemen kan leiden, omdat het ID daardoor verandert.)
        /// 
        /// Deze method persisteert.  Dat is belangrijk, want het kan zijn dat er entities
        /// verdwijnen.  (Bijv. als er nieuwe afdelingen gegeven zijn.)
        /// </summary>
        /// <param name="lid">
        /// Het lidobject van de inschrijving, met gekoppeld gelieerderpersoon, groepswerkjaar, afdelingsjaren
        /// </param>
        /// <param name="voorstellid">
        /// Bevat de afdelingen waar het nieuwe lidobject aan gekoppeld moet worden
        /// en heeft aan of de gelieerde persoon leiding is.
        /// </param>
        /// <returns>
        /// Het lidobject met de gegevens van de nieuwe inschrijving
        /// </returns>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker geen GAV-rechten heeft op het <paramref name="lid" />.
        /// </exception>
        Lid Wijzigen(Lid lid, LidVoorstel voorstellid);

        /// <summary>
        /// Schrijft een gelieerde persoon in, persisteert niet.  Er mag nog geen lidobject (ook geen inactief) voor de
        /// gelieerde persoon bestaan.
        /// </summary>
        /// <param name="gp">
        /// De persoon om in te schrijven, gekoppeld met groep en persoon
        /// </param>
        /// <param name="gwj">
        /// Het groepswerkjaar waarin moet worden ingeschreven, gekoppeld met afdelingsjaren
        /// </param>
        /// <param name="isJaarOvergang">
        /// Geeft aan of het over de automatische jaarovergang gaat; relevant voor de
        /// probeerperiode
        /// </param>
        /// <param name="voorstellid">
        /// Voorstel voor de eigenschappen van het in te schrijven lid.
        /// </param>
        /// <returns>
        /// Het aangemaakte lid object
        /// </returns>
        Lid NieuwInschrijven(GelieerdePersoon gp, GroepsWerkJaar gwj, bool isJaarOvergang, LidVoorstel voorstellid);

        /// <summary>
        /// Persisteert een lid met de gekoppelde entiteiten bepaald door <paramref name="extras"/>.
        /// </summary>
        /// <param name="lid">
        /// Het <paramref name="lid"/> dat bewaard moet worden
        /// </param>
        /// <param name="extras">
        /// De gekoppelde entiteiten
        /// </param>
        /// <param name="syncen">
        /// Als <c>true</c>, dan wordt het lid gesynct met Kipadmin.
        /// </param>
        /// <returns>
        /// Een kloon van het lid en de extra's, met eventuele nieuwe ID's ingevuld
        /// </returns>
        /// <remarks>
        /// De parameter <paramref name="syncen"/> heeft als doel een sync te vermijden als een
        /// irrelevante wijziging zoals 'lidgeld betaald' wordt bewaard.
        /// </remarks>
        Lid Bewaren(Lid lid, LidExtras extras, bool syncen);

        /// <summary>
        /// Haalt leden op, op basis van de <paramref name="lidIDs"/>
        /// </summary>
        /// <param name="lidIDs">
        /// ID gevraagde leden
        /// </param>
        /// <param name="lidExtras">
        /// Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden
        /// </param>
        /// <returns>
        /// Kinderen of leiding met gevraagde <paramref name="lidExtras"/>.
        /// </returns>
        /// <remarks>
        /// ID's van leden waarvoor de user geen GAV is, worden genegeerd
        /// </remarks>
        IEnumerable<Lid> Ophalen(IEnumerable<int> lidIDs, LidExtras lidExtras);

        /// <summary>
        /// Haalt lid op, op basis van zijn <paramref name="lidID"/>
        /// </summary>
        /// <param name="lidID">
        /// ID gevraagde lid
        /// </param>
        /// <param name="extras">
        /// Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden
        /// </param>
        /// <returns>
        /// Kind of Leiding met gevraagde <paramref name="extras"/>.
        /// </returns>
        Lid Ophalen(int lidID, LidExtras extras);

        /// <summary>
        /// Haalt lid en gekoppelde persoon op, op basis van <paramref name="lidID"/>
        /// </summary>
        /// <param name="lidID">
        /// ID op te halen lid
        /// </param>
        /// <returns>
        /// Lid, met daaraan gekoppeld gelieerde persoon en persoon.
        /// </returns>
        Lid Ophalen(int lidID);

        /// <summary>
        /// Haalt het lid op bepaald door <paramref name="gelieerdePersoonID"/> en
        /// <paramref name="groepsWerkJaarID"/>, inclusief de relevante details om het lid naar Kipadmin te krijgen:
        /// persoon, afdelingen, officiële afdelingen, functies, groepswerkjaar, groep
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// ID van de gelieerde persoon waarvoor het lidobject gevraagd is.
        /// </param>
        /// <param name="groepsWerkJaarID">
        /// ID van groepswerkjaar in hetwelke het lidobject gevraagd is
        /// </param>
        /// <returns>
        /// Het lid bepaald door <paramref name="gelieerdePersoonID"/> en
        /// <paramref name="groepsWerkJaarID"/>, inclusief de relevante details om het lid naar Kipadmin te krijgen
        /// </returns>
        Lid OphalenViaPersoon(int gelieerdePersoonID, int groepsWerkJaarID);

        /// <summary>
        /// Haalt leden op uit het groepswerkjaar met gegeven ID, inclusief persoonsgegevens,
        /// voorkeursadressen, functies en afdelingen.  (Geen communicatiemiddelen)
        /// </summary>
        /// <param name="gwjID">
        /// ID van het gevraagde groepswerkjaar
        /// </param>
        /// <param name="ookInactief">
        /// Geef hier <c>true</c> als ook de niet-actieve leden opgehaald
        /// moeten worden.
        /// </param>
        /// <returns>
        /// De lijst van leden
        /// </returns>
        IEnumerable<Lid> OphalenUitGroepsWerkJaar(int gwjID, bool ookInactief);

        /// <summary>
        /// Zoekt leden op, op basis van de gegeven <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">
        /// De niet-nulle properties van de filter
        /// bepalen waarop gezocht moet worden
        /// </param>
        /// <param name="extras">
        /// Bepaalt de mee op te halen gekoppelde entiteiten. 
        /// (Adressen ophalen vertraagt aanzienlijk.)
        /// </param>
        /// <returns>
        /// Lijst met info over gevonden leden
        /// </returns>
        /// <remarks>
        /// Er worden enkel actieve leden opgehaald
        /// </remarks>
        IEnumerable<Lid> Zoeken(LidFilter filter, LidExtras extras);

        /// <summary>
        /// Haalt alle leden op uit groepswerkjaar met gegeven <paramref name="groepsWerkJaarID"/> en gegeven
        /// <paramref name="nationaleFunctie"/>, met daaraan gekoppeld de gelieerde personen.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
        /// <param name="nationaleFunctie">een nationale functie</param>
        /// <returns>alle leden op uit groepswerkjaar met gegeven <paramref name="groepsWerkJaarID"/> en gegeven
        /// <paramref name="nationaleFunctie"/>, met daaraan gekoppeld de gelieerde personen.</returns>
        List<Lid> Ophalen(int groepsWerkJaarID, NationaleFunctie nationaleFunctie);
    }
}