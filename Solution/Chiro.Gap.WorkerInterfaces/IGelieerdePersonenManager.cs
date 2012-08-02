using System;
using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IGelieerdePersonenManager
    {
        /// <summary>
        /// Haalt gelieerde persoon met gekoppelde persoon op.
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// ID van de gelieerde persoon.
        /// </param>
        /// <returns>
        /// Gelieerde persoon, met gekoppeld persoonsobject.
        /// </returns>
        /// <remarks>
        /// De groepsinfo wordt niet mee opgehaald, omdat we die in de
        /// meeste gevallen niet nodig zullen hebben.
        /// </remarks>
        GelieerdePersoon Ophalen(int gelieerdePersoonID);

        /// <summary>
        /// Haalt een gelieerde persoon op, met de gevraagde 'extra's'.
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// ID op te halen gelieerde persoon
        /// </param>
        /// <param name="extras">
        /// Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden
        /// </param>
        /// <returns>
        /// De gevraagde gelieerde persoon, met de gevraagde gekoppelde entiteiten.
        /// </returns>
        GelieerdePersoon Ophalen(int gelieerdePersoonID, PersoonsExtras extras);

        /// <summary>
        /// Haalt een lijst op van gelieerde personen.
        /// </summary>
        /// <param name="gelieerdePersonenIDs">
        /// ID's van de op te vragen
        /// gelieerde personen.
        /// </param>
        /// <returns>
        /// Lijst met gelieerde personen
        /// </returns>
        IList<GelieerdePersoon> Ophalen(IEnumerable<int> gelieerdePersonenIDs);

        /// <summary>
        /// Haalt gelieerde persoon op met persoonsgegevens, adressen en
        /// communicatievormen.
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// ID gevraagde gelieerde persoon
        /// </param>
        /// <returns>
        /// GelieerdePersoon met persoonsgegevens, adressen, categorieën, communicatievormen en eventuele lidgegevens uit het gegeven werkJaar.
        /// </returns>
        GelieerdePersoon DetailsOphalen(int gelieerdePersoonID);

        /// <summary>
        /// Bewaart een gelieerde persoon, inclusief persoonsgegevens en extra koppelingen bepaald
        /// door <paramref name="extras"/>.
        /// </summary>
        /// <param name="gelieerdePersoon">
        /// Te bewaren gelieerde persoon
        /// </param>
        /// <param name="extras">
        /// Bepaalt de mee te bewaren gekoppelde entiteiten
        /// </param>
        /// <returns>
        /// De bewaarde gelieerde persoon
        /// </returns>
        GelieerdePersoon Bewaren(GelieerdePersoon gelieerdePersoon, PersoonsExtras extras);

        /// <summary>
        /// Haal een lijst op met alle gelieerde personen van een groep
        /// </summary>
        /// <param name="groepID">
        /// GroepID van gevraagde groep
        /// </param>
        /// <param name="extras">
        /// Bepaalt gekoppelde entity's die mee moeten worden opgehaald
        /// </param>
        /// <param name="sortering">
        /// Geeft aan hoe de pagina gesorteerd moet worden
        /// </param>
        /// <returns>
        /// Lijst met alle gelieerde personen
        /// </returns>
        /// <remarks>
        /// Opgelet! Dit kan een zware query zijn!
        /// </remarks>
        IList<GelieerdePersoon> AllenOphalen(int groepID, PersoonsExtras extras, PersoonSorteringsEnum sortering);

        /// <summary>
        /// Haal een lijst op van de eerste letters van de achternamen van gelieerde personen van een groep
        /// </summary>
        /// <param name="groepID">
        /// GroepID van gevraagde groep
        /// </param>
        /// <returns>
        /// Lijst met de eerste letter gegroepeerd van de achternamen
        /// </returns>
        IList<String> EersteLetterNamenOphalen(int groepID);

        /// <summary>
        /// Haal een lijst op van de eerste letters van de achternamen van gelieerde personen van
        /// de categorie met ID <paramref name="categorieID"/>
        /// </summary>
        /// <param name="categorieID">
        ///   ID van de Categorie waaruit we de letters willen halen
        /// </param>
        /// <returns>
        /// Lijst met de eerste letter gegroepeerd van de achternamen
        /// </returns>
        IList<string> EersteLetterNamenOphalenCategorie(int categorieID);

        /// <summary>
        /// Haalt een pagina op met gelieerde personen van een groep.
        /// </summary>
        /// <param name="groepID">
        /// GroepID gevraagde groep
        /// </param>
        /// <param name="pagina">
        /// Paginanummer (&gt;=1)
        /// </param>
        /// <param name="paginaGrootte">
        /// Aantal personen per pagina
        /// </param>
        /// <param name="sortering">
        /// Geeft aan hoe de pagina gesorteerd moet worden
        /// </param>
        /// <param name="extras">
        /// Bepaalt de mee op te halen gekoppelde objecten
        /// </param>
        /// <param name="aantalTotaal">
        /// Outputparameter voor totaal aantal
        /// personen in de groep
        /// </param>
        /// <returns>
        /// Lijst met GelieerdePersonen
        /// </returns>
        IList<GelieerdePersoon> PaginaOphalen(
            int groepID,
            int pagina,
            int paginaGrootte,
            PersoonSorteringsEnum sortering,
            PersoonsExtras extras,
            out int aantalTotaal);

        /// <summary>
        /// Haalt de gelieerde personen van een groep op, die een familienaam hebben
        /// beginnend met de letter <paramref name="letter"/>.
        /// </summary>
        /// <param name="groepID">
        /// GroepID gevraagde groep
        /// </param>
        /// <param name="letter">
        /// Eerste letter van de achternamen van de personen die we willen bekijken
        /// </param>
        /// <param name="sortering">
        /// Geeft aan hoe de pagina gesorteerd moet worden
        /// </param>
        /// <param name="extras">
        /// Bepaalt de mee op te halen gekoppelde objecten
        /// </param>
        /// <param name="aantalTotaal">
        /// Outputparameter voor totaal aantal
        /// personen in de groep
        /// </param>
        /// <returns>
        /// Lijst met GelieerdePersonen
        /// </returns>
        IList<GelieerdePersoon> Ophalen(
            int groepID,
            string letter,
            PersoonSorteringsEnum sortering,
            PersoonsExtras extras,
            out int aantalTotaal);

        /// <summary>
        /// Haalt een pagina op met gelieerde personen van een groep die tot de categorie behoren,
        /// inclusief eventuele lidobjecten voor deze groep
        /// </summary>
        /// <param name="categorieID">
        /// ID gevraagde categorie
        /// </param>
        /// <param name="pagina">
        /// Paginanummer (minstens 1)
        /// </param>
        /// <param name="paginaGrootte">
        /// Aantal personen per pagina
        /// </param>
        /// <param name="sortering">
        /// Geeft aan hoe de pagina gesorteerd moet worden
        /// </param>
        /// <param name="extras">
        /// Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden
        /// </param>
        /// <param name="aantalTotaal">
        /// Outputparameter voor totaal aantal
        /// personen in de groep
        /// </param>
        /// <returns>
        /// Lijst met GelieerdePersonen
        /// </returns>
        IList<GelieerdePersoon> PaginaOphalenUitCategorie(int categorieID,
                                                                          string letter,
                                                                          PersoonSorteringsEnum sortering,
                                                                          PersoonsExtras extras,
                                                                          out int aantalTotaal);

        /// <summary>
        /// Koppelt het relevante groepsobject aan de gegeven
        /// gelieerde persoon.
        /// </summary>
        /// <param name="gp">
        /// Gelieerde persoon
        /// </param>
        /// <returns>
        /// Diezelfde gelieerde persoon, met zijn of haar groep 
        /// eraan gekoppeld.
        /// </returns>
        GelieerdePersoon GroepLaden(GelieerdePersoon gp);

        /// <summary>
        /// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
        /// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
        /// (inclusief communicatie en adressen)
        /// </summary>
        /// <param name="groepID">
        /// GroepID dat bepaalt in welke gelieerde personen gezocht mag worden
        /// </param>
        /// <param name="naam">
        /// Te zoeken naam (ongeveer)
        /// </param>
        /// <param name="voornaam">
        /// Te zoeken voornaam (ongeveer)
        /// </param>
        /// <returns>
        /// Lijst met gevonden matches
        /// </returns>
        IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam);

        /// <summary>
        /// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
        /// en voornaam gelijk aan <paramref name="naam"/> en <paramref name="voornaam"/>.
        /// (enkel persoonsinfo)
        /// </summary>
        /// <param name="groepID">
        /// GroepID dat bepaalt in welke gelieerde personen gezocht mag worden
        /// </param>
        /// <param name="naam">
        /// Te zoeken naam
        /// </param>
        /// <param name="voornaam">
        /// Te zoeken voornaam
        /// </param>
        /// <returns>
        /// Rij gevonden matches
        /// </returns>
        IEnumerable<GelieerdePersoon> ZoekenOpNaam(int groepID, string naam, string voornaam);

        /// <summary>
        /// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> waarbij
        /// naam of voornaam ongeveer begint met <paramref name="teZoeken"/>
        /// </summary>
        /// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
        /// <param name="teZoeken">Te zoeken voor- of achternaam (ongeveer)</param>
        /// <returns>Lijst met gevonden matches</returns>
        IList<GelieerdePersoon> ZoekenOpNaamVoornaamBegin(int groepID, string teZoeken);

        /// <summary>
        /// Maak een GelieerdePersoon voor gegeven persoon en groep
        /// </summary>
        /// <param name="persoon">
        /// Te liëren persoon
        /// </param>
        /// <param name="groep">
        /// Groep waaraan te liëren
        /// </param>
        /// <param name="chiroLeeftijd">
        /// Chiroleeftijd gelieerde persoon
        /// </param>
        /// <returns>
        /// Een nieuwe GelieerdePersoon
        /// </returns>
        GelieerdePersoon Koppelen(Persoon persoon, Groep groep, int chiroLeeftijd);

        /// <summary>
        /// Maakt GelieerdePersoon, gekoppelde Persoon, Adressen en Communicatie allemaal
        /// te verwijderen.  Persisteert!
        /// </summary>
        /// <param name="gp">
        /// Te verwijderen gelieerde persoon
        /// </param>
        void VolledigVerwijderen(GelieerdePersoon gp);

        /// <summary>
        /// TODO (#190): Documenteren
        /// </summary>
        /// <param name="personenLijst">
        /// </param>
        void Bewaren(IList<GelieerdePersoon> personenLijst);

        /// <summary>
        /// Gaat na of een gelieerde persoon dit werkJaar ingeschreven is als lid
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// De ID van de gelieerde persoon in kwestie
        /// </param>
        /// <returns>
        /// <c>True</c> als de gelieerde persoon dit werkJaar ingeschreven is als lid,
        /// <c>false</c> in het andere geval
        /// </returns>
        bool IsLid(int gelieerdePersoonID);

        /// <summary>
        /// Koppelt een gelieerde persoon aan een categorie, en persisteert dan de aanpassingen
        /// </summary>
        /// <param name="gelieerdePersonen">
        /// Te koppelen gelieerde persoon
        /// </param>
        /// <param name="categorie">
        /// Te koppelen categorie
        /// </param>
        void CategorieKoppelen(IList<GelieerdePersoon> gelieerdePersonen, Categorie categorie);

        /// <summary>
        /// Verwijdert de gelieerde personen uit de categorie, en persisteert
        /// </summary>
        /// <remarks>
        /// De methode is reentrant, als er bepaalde personen niet gelinkt zijn aan de categorie, 
        /// gebeurt er niets met die personen, ook geen error.
        /// </remarks>
        /// <param name="gelieerdePersonenIDs">
        /// Gelieerde persoon IDs
        /// </param>
        /// <param name="categorie">
        /// Te verwijderen categorie MET gelinkte gelieerdepersonen 
        /// </param>
        /// <returns>
        /// Een kloon van de categorie, waaruit de gevraagde personen verwijderd zijn
        /// </returns>
        Categorie CategorieLoskoppelen(IList<int> gelieerdePersonenIDs, Categorie categorie);

        /// <summary>
        /// Zoekt naar gelieerde personen die gelijkaardig zijn aan een gegeven
        /// <paramref name="persoon"/>.
        /// </summary>
        /// <param name="persoon">
        /// Persoon waarmee vergeleken moet worden
        /// </param>
        /// <param name="groepID">
        /// ID van groep waarin te zoeken
        /// </param>
        /// <returns>
        /// Lijstje met gelijkaardige personen
        /// </returns>
        IList<GelieerdePersoon> ZoekGelijkaardig(Persoon persoon, int groepID);

        /// <summary>
        /// Haalt een rij gelieerde personen op, eventueel met extra info
        /// </summary>
        /// <param name="gelieerdePersoonIDs">
        /// ID's op te halen gelieerde personen
        /// </param>
        /// <param name="extras">
        /// Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden
        /// </param>
        /// <returns>
        /// De gevraagde rij gelieerde personen.  De personen komen sowieso mee.
        /// </returns>
        IEnumerable<GelieerdePersoon> Ophalen(IEnumerable<int> gelieerdePersoonIDs, PersoonsExtras extras);

        /// <summary>
        /// Maakt het persoonsAdres <paramref name="voorkeur"/> het voorkeursadres van de gelieerde persoon
        /// <paramref name="gp"/>
        /// </summary>
        /// <param name="gp">
        /// Gelieerde persoon die een nieuw voorkeursadres moet krijgen
        /// </param>
        /// <param name="voorkeur">
        /// Persoonsadres dat voorkeursadres moet worden van <paramref name="gp"/>.
        /// </param>
        void VoorkeurInstellen(GelieerdePersoon gp, PersoonsAdres voorkeur);

        /// <summary>
        /// Koppelt het gegeven Adres via nieuwe PersoonsAdresObjecten
        /// aan de Personen gekoppeld aan de gelieerde personen <paramref name="gelieerdePersonen"/>.  
        /// Persisteert niet.
        /// </summary>
        /// <param name="gelieerdePersonen">
        /// Gelieerde  die er een adres bij krijgen, met daaraan gekoppeld hun huidige
        /// adressen, en de gelieerde personen waarop de gebruiker GAV-rechten heeft.
        /// </param>
        /// <param name="adres">
        /// Toe te voegen adres
        /// </param>
        /// <param name="adrestype">
        /// Het adrestype (thuis, kot, enz.)
        /// </param>
        /// <param name="voorkeur">
        /// Indien true, wordt het nieuwe adres voorkeursadres van de gegeven gelieerde personen
        /// </param>
        void AdresToevoegen(IEnumerable<GelieerdePersoon> gelieerdePersonen,
                                            Adres adres,
                                            AdresTypeEnum adrestype,
                                            bool voorkeur);

        /// <summary>
        /// Haalt gelieerde personen op die een adres gemeen hebben met de 
        /// GelieerdePersoon met gegeven ID
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// ID van GelieerdePersoon waarvan huisgenoten
        /// gevraagd zijn
        /// </param>
        /// <returns>
        /// Lijstje met gelieerde personen
        /// </returns>
        /// <remarks>
        /// Enkel de huisgenoten die gelieerd zijn aan de groep van de originele gelieerde persoon worden
        /// mee opgehaald, ongeacht of er misschien nog huisgenoten zijn in een andere groep waar de gebruiker ook
        /// GAV-rechten op heeft.
        /// </remarks>
        IList<GelieerdePersoon> HuisGenotenOphalenZelfdeGroep(int gelieerdePersoonID);

        /// <summary>
        /// Verwijder persoonsadressen, en persisteer.  Als ergens een voorkeuradres wegvalt, dan wordt een willekeurig
        /// ander adres voorkeuradres van de gelieerde persoon.
        /// </summary>
        /// <param name="persoonsAdressen">
        /// Te verwijderen persoonsadressen
        /// </param>
        /// <remarks>
        /// Deze method staat wat vreemd onder GelieerdePersonenManager, maar past wel voorkeursadressen
        /// van gelieerde personen aan.
        /// </remarks>
        void AdressenVerwijderen(IEnumerable<PersoonsAdres> persoonsAdressen);
    }
}