using System;
using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.WorkerInterfaces;
using GebruikersRecht = Chiro.Gap.ServiceContracts.DataContracts.GebruikersRecht;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Interface voor de service voor gebruikersrechtenbeheer.
    /// </summary>
    public class GebruikersService : IGebruikersService
    {
        private readonly IGelieerdePersonenManager _gelieerdePersonenManager;
        private readonly IGebruikersRechtenManager _gebruikersRechtenManager;
        private readonly IGroepenManager _groepenManager;

        /// <summary>
        /// Constructor, zorgt voor dependency injection
        /// </summary>
        /// <param name="gelieerdePersonenManager">businesslogica voor gelieerde personen</param>
        /// <param name="gebruikersRechtenManager">businesslogica voor gebruikersrechten</param>
        /// <param name="groepenManager">businesslogica voor groepen</param>
        public GebruikersService(
            IGelieerdePersonenManager gelieerdePersonenManager,
            IGebruikersRechtenManager gebruikersRechtenManager,
            IGroepenManager groepenManager)
        {
            _gelieerdePersonenManager = gelieerdePersonenManager;
            _gebruikersRechtenManager = gebruikersRechtenManager;
            _groepenManager = groepenManager;
        }

        /// <summary>
        /// Als de persoon met gegeven <paramref name="gelieerdePersoonID"/> nog geen account heeft, wordt er een
        /// account voor gemaakt. Aan die account worden dan de meegegeven <paramref name="gebruikersRechten"/>
        /// gekoppeld.  Gebruikersrechten zijn standaard 14 maanden geldig.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon die rechten moet krijgen</param>
        /// <param name="gebruikersRechten">Rechten die de account moet krijgen. Mag leeg zijn. Bestaande 
        /// gebruikersrechten worden zo mogelijk verlengd als ze in <paramref name="gebruikersRechten"/> 
        /// voorkomen, eventuele bestaande rechten niet in <paramref name="gebruikersRechten"/> blijven
        /// onaangeroerd.
        /// </param>
        public void RechtenToekennen(int gelieerdePersoonID, GebruikersRecht[] gebruikersRechten)
        {
            var groepIDs = gebruikersRechten == null ? null : gebruikersRechten.Select(gr => gr.GroepID).ToArray();

            // Haal gelieerde persoon op met eventuele bestaande gebruikersrechten en communicatie

            var gelieerdePersoon = _gelieerdePersonenManager.Ophalen(gelieerdePersoonID, PersoonsExtras.GebruikersRechten | PersoonsExtras.Groep | PersoonsExtras.Communicatie);
            var account = _gebruikersRechtenManager.AccountZoekenOfMaken(gelieerdePersoon);

            // Als we geen rechten moeten toekennen, zijn we klaar.

            if (groepIDs == null) return;

            // Momenteel ondersteunen we enkel nog GAV-rollen

            var nietOndersteund = (from gr in gebruikersRechten
                                   where gr.Rol != Rol.Gav
                                   select gr).FirstOrDefault();
            if (nietOndersteund != null)
            {
                throw new NotSupportedException(String.Format(Properties.Resources.RolNietOndersteund, nietOndersteund.Rol));
            }

            // Tamelijk nifty hier: We gaan op zoek naar de groepen waarvoor er nog geen rechten aan account gekoppeld is,
            // we koppelen die groepen daan aan de account, en bewaren die koppelingen in 'toegekendeRechten'.

            var toegekendeRechten = (from id in groepIDs
                                     let bestaandRecht = (from gr in account.GebruikersRecht
                                                          where gr.Groep.ID == id
                                                          select gr).FirstOrDefault()
                                     where bestaandRecht == null    // voor de groepID's waarvoor de account nog geen rechten heeft
                                     select                         // selecteren we de groepen
                                         gelieerdePersoon.Groep.ID == id    // als het toevallig de groep van de gelieerde persoon is...
                                             ? gelieerdePersoon.Groep       // ... dan hebben we hem al
                                             : _groepenManager.Ophalen(id)  // ... en anders komt hij uit de database
                                         into groep
                                         // voor het lijstje groepen waarvoor we nog geen rechten hebben, kennen we er toe.
                                         // Het resultaat selecteren we in toegekendeRechten.
                                         select _gebruikersRechtenManager.ToekennenOfVerlengen(account, groep)).ToArray();

            // Veel gedoe voor in een service method. Zie #1250.

            _gebruikersRechtenManager.Bewaren(toegekendeRechten);
        }

        /// <summary>
        /// Geeft de account met gegeven <paramref name="gebruikersNaam"/> de gegeven
        /// <paramref name="gebruikersRechten"/>.  Gebruikersrechten zijn standaard 14 maanden geldig.
        /// De gegeven accout moet bestaan; we moeten vermijden dat eender welke user zomaar accounts
        /// kan maken voor chiro.wereld.
        /// </summary>
        /// <param name="gebruikersNaam">gebruikersnaam van de account die rechten moet krijgen</param>
        /// <param name="gebruikersRechten">Rechten die de account moet krijgen. 
        /// Bestaande gebruikersrechten worden zo mogelijk verlengd als ze in 
        /// <paramref name="gebruikersRechten"/> voorkomen, eventuele bestaande rechten niet in 
        /// <paramref name="gebruikersRechten"/> blijven onaangeroerd.
        /// </param>
        public void RechtenToekennenGebruiker(string gebruikersNaam, GebruikersRecht[] gebruikersRechten)
        {
            // Als we geen rechten moeten toekennen, wat komen we hier dan eigenlijk doen?
            if (gebruikersRechten == null || !gebruikersRechten.Any()) return;

            var groepIDs = gebruikersRechten.Select(gr => gr.GroepID).ToArray();

            // Haal gelieerde persoon op met eventuele bestaande gebruikersrechten en communicatie
            Gav account = _gebruikersRechtenManager.AccountOphalen(gebruikersNaam);
            // We gaan ervan uit dat de groepen mee opgehaald zijn.

            // Momenteel ondersteunen we enkel nog GAV-rollen
            var nietOndersteund = (from gr in gebruikersRechten
                                   where gr.Rol != Rol.Gav
                                   select gr).FirstOrDefault();
            if (nietOndersteund != null)
            {
                throw new NotSupportedException(String.Format(Properties.Resources.RolNietOndersteund, nietOndersteund.Rol));
            }

            // Nifty linq query:
            // Vertrek van de groepID's.
            // Als voor een gegeven ID de groep al gekoppeld is aan de account, selecteer dan die gekoppelde groep.
            // In het andere geval wordt die groep opgehaald.
            // Met die gevonden groep en de gegeven account wordt dan _gebruikersRechtenManager.ToekennenOfVerlengen
            // aangeroepen.  Dat resulteert in een nieuwe of aangepaste rij gebruikersrechten, die dan gepersisteerd
            // wordt.

            _gebruikersRechtenManager.Bewaren(groepIDs.Select(id => (from gr in account.GebruikersRecht
                                                                     where gr.Groep.ID == id
                                                                     select gr.Groep).FirstOrDefault() ??
                                                                    _groepenManager.Ophalen(id)).Select(
                                                                        gevondenGroep =>
                                                                        _gebruikersRechtenManager.ToekennenOfVerlengen(
                                                                            account, gevondenGroep)).ToArray());

            // Veel gedoe voor in een service method. Zie #1250.
        }

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="gelieerdePersoonID"/> af voor de groepen met gegeven <paramref name="groepIDs"/>
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIDs">ID's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        public void RechtenAfnemen(int gelieerdePersoonID, int[] groepIDs)
        {
            // We gaan niet belachelijk doen...

            if (groepIDs == null) return;

            // Haal gelieerde persoon op met eventuele bestaande gebruikersrechten en communicatie

            var gelieerdePersoon = _gelieerdePersonenManager.Ophalen(gelieerdePersoonID, PersoonsExtras.GebruikersRechten | PersoonsExtras.Groep);
            var account = _gebruikersRechtenManager.AccountZoekenOfMaken(gelieerdePersoon, false);

            // Af te nemen rechten selecteren, intrekken en persisteren

            var teVerwijderen = (from gr in account.GebruikersRecht
                                 where groepIDs.Contains(gr.Groep.ID)
                                 select gr).ToArray();

            _gebruikersRechtenManager.Intrekken(teVerwijderen);

            _gebruikersRechtenManager.Bewaren(teVerwijderen);
        }

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="gebruikersNaam"/> af voor de groepen met gegeven <paramref name="groepIDs"/>
        /// </summary>
        /// <param name="gebruikersNaam">gebruikersnaam van gelieerde persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIDs">ID's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        public void RechtenAfnemenGebruiker(string gebruikersNaam, int[] groepIDs)
        {
            // We gaan niet belachelijk doen...

            if (groepIDs == null) return;

            // Haal account op met zijn gebruikersrechten.

            var account = _gebruikersRechtenManager.AccountOphalen(gebruikersNaam);

            // Af te nemen rechten selecteren, intrekken en persisteren

            var teVerwijderen = (from gr in account.GebruikersRecht
                                 where groepIDs.Contains(gr.Groep.ID)
                                 select gr).ToArray();

            _gebruikersRechtenManager.Intrekken(teVerwijderen);

            _gebruikersRechtenManager.Bewaren(teVerwijderen);
        }
    }
}