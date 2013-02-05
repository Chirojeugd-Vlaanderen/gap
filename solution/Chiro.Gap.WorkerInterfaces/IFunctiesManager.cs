using System.Collections.Generic;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IFunctiesManager
    {
        /// <summary>
        /// Haalt alle functies op die mogelijk toegekend kunnen worden aan een lid uit het groepswerkjaar
        /// bepaald door <paramref name="groepsWerkJaarID"/> en van het type <paramref name="lidType"/>.
        /// </summary>
        /// <param name="groepsWerkJaarID">
        /// ID van het groepswerkjaar waarvoor de relevante functies gevraagd
        /// zijn.
        /// </param>
        /// <param name="lidType">
        /// <c>LidType.Kind</c> of <c>LidType.Leiding</c>
        /// </param>
        /// <returns>
        /// Lijst met functies die mogelijk toegekend kunnen worden aan een lid uit het groepswerkjaar
        /// bepaald door <paramref name="groepsWerkJaarID"/> en van het type <paramref name="lidType"/>.
        /// </returns>
        IList<Functie> OphalenRelevant(int groepsWerkJaarID, LidType lidType);

        /// <summary>
        /// Kent de meegegeven <paramref name="functies"/> toe aan het gegeven <paramref name="lid"/>.
        /// Als het lid al andere functies had, blijven die behouden.  Persisteert niet.
        /// </summary>
        /// <param name="lid">
        /// Lid dat de functies moet krijgen, gekoppeld aan zijn groep
        /// </param>
        /// <param name="functies">
        /// Rij toe te kennen functies
        /// </param>
        /// <remarks>
        /// Er wordt verondersteld dat er heel wat geladen is!
        /// - lid.groepswerkjaar.groep
        /// - lid.functie
        /// - voor elke functie:
        ///  - functie.lid (voor leden van dezelfde groep)
        ///  - functie.groep
        /// </remarks>
        void Toekennen(Lid lid, IEnumerable<Functie> functies);

        /// <summary>
        /// Vervangt de functies van het lid <paramref name="lid"/> door de functies in 
        /// <paramref name="functies"/>.  Persisteert.
        /// </summary>
        /// <param name="lid">
        /// Lid waarvan de functies vervangen moeten worden
        /// </param>
        /// <param name="functies">
        /// Nieuwe lijst functies
        /// </param>
        /// <returns>
        /// Het <paramref name="lid"/> met daaraan gekoppeld de nieuwe functies
        /// </returns>
        /// <remarks>
        /// Aan <paramref name="lid"/>moeten de huidige functies gekoppeld zijn
        /// </remarks>
        Lid Vervangen(Lid lid, IEnumerable<Functie> functies);

        /// <summary>
        /// Verwijdert een functie (PERSISTEERT!)
        /// </summary>
        /// <param name="functie">
        /// Te verwijderen functie, 
        ///  inclusief groep, leden en groepswerkjaar leden
        /// </param>
        /// <param name="forceren">
        /// Indien <c>true</c> wordt de functie ook verwijderd als er
        ///  dit werkJaar personen met de gegeven functie zijn.  Anders krijg je een exception.
        /// </param>
        /// <returns>
        /// <c>Null</c> als de functie effectief verwijderd is, anders het functie-object met
        /// aangepast 'werkjaartot'.
        /// </returns>
        /// <remarks>
        /// Als de functie geen leden meer bevat na verwijdering van die van het huidige werkJaar,
        /// dan wordt ze verwijderd.  Zo niet, wordt er een stopdatum op geplakt.
        /// </remarks>
        Functie Verwijderen(Functie functie, bool forceren);

        /// <summary>
        /// Kijkt na voor een gegeven <paramref name="groepsWerkJaar"/> of de maximum- en
        /// minimumaantallen van de functies (eigen en nationaal bepaald) niet overschreden zijn.
        /// </summary>
        /// <param name="groepsWerkJaar">
        /// Te controleren werkJaar
        /// </param>
        /// <returns>
        /// Een lijst met tellingsgegevens voor de functies waar de aantallen niet kloppen.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Deze functie is zich niet bewust van de aanwezigheid van een database, en verwacht
        /// dat groepsWerkJaar.Groep.Functie en groepsWerkJaar.Lid[i].Functie geladen zijn.
        /// </para>
        /// </remarks>
        IEnumerable<Telling> AantallenControleren(GroepsWerkJaar groepsWerkJaar);

        /// <summary>
        /// Kijkt na voor een gegeven <paramref name="groepsWerkJaar"/> of de maximum- en
        /// minimumaantallen van de functies <paramref name="functies"/> niet overschreden zijn.
        /// </summary>
        /// <param name="groepsWerkJaar">
        /// Te controleren werkJaar
        /// </param>
        /// <param name="functies">
        /// Functies waarop te controleren
        /// </param>
        /// <returns>
        /// Een lijst met tellingsgegevens voor de functies waar de aantallen niet kloppen.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Deze functie is zich niet bewust van de aanwezigheid van een database, en verwacht
        /// dat groepsWerkJaar.Lid[i].Functie geladen is.
        /// </para>
        /// <para>
        /// Functies in <paramref name="functies"/> waar geen groep aan gekoppeld is, worden als
        /// nationaal bepaalde functies beschouwd.
        /// </para>
        /// <para>
        /// Functies die niet geldig zijn in het gevraagde groepswerkjaar, worden genegeerd
        /// </para>
        /// </remarks>
        IEnumerable<Telling> AantallenControleren(
            GroepsWerkJaar groepsWerkJaar,
            IEnumerable<Functie> functies);
    }

    /// <summary>
    /// Struct die gebruikt wordt om van een functie max aantal leden, min aantal leden en totaal aantal
    /// leden te stockeren.
    /// </summary>
    public struct Telling
    {
        public int ID;
        public int Aantal;
        public int? Max;
        public int Min;
    }
}