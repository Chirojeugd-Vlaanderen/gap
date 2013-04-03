using System;
using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IGelieerdePersonenManager
    {
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
        /// <param name="groep">
        /// groep waarin te zoeken
        /// </param>
        /// <returns>
        /// Lijstje met gelijkaardige personen
        /// </returns>
        List<GelieerdePersoon> ZoekGelijkaardig(Persoon persoon, Groep groep);

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
        void AdresToevoegen(IList<GelieerdePersoon> gelieerdePersonen,
                                            Adres adres,
                                            AdresTypeEnum adrestype,
                                            bool voorkeur);

        /// <summary>
        /// Koppelt de gelieerde personen met gegeven <paramref name="gelieerdePersoonIDs"/> los
        /// van de gegeven <paramref name="categorie"/>
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's van de los te koppelen gelieerde personen</param>
        /// <param name="categorie">Categorie waar de gelieerde personen losgekoppeld van moeten worden</param>
        void CategorieLoskoppelen(int[] gelieerdePersoonIDs, Categorie categorie);

        /// <summary>
        /// Voegt een <paramref name="nieuwePersoon"/> toe aan de gegegeven <paramref name="groep"/>. Als
        /// <paramref name="forceer"/> niet is gezet, wordt een exception opgegooid als er al een gelijkaardige
        /// persoon aan de groep gekoppeld is.
        /// </summary>
        /// <param name="nieuwePersoon">Nieuwe toe te voegen persoon</param>
        /// <param name="groep">Groep waaraan de persoon gelieerd/gekoppeld moet worden</param>
        /// <param name="chiroLeeftijd">Chiroleeftijd van de persoon</param>
        /// <param name="forceer">Als <c>false</c>, dan wordt een exception opgegooid als er al een gelijkaardige
        /// persoon aan de groep gekoppeld is.</param>
        /// <returns>De gelieerde persoon na het koppelen van <paramref name="nieuwePersoon"/> aan <paramref name="groep"/>.</returns>
        GelieerdePersoon Toevoegen(Persoon nieuwePersoon, Groep groep, int chiroLeeftijd, bool forceer);
    }
}