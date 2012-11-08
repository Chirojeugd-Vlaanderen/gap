using System;
using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IGelieerdePersonenManager
    {
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