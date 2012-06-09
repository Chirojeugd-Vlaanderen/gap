// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    /// <summary>
    /// Gegevenstoegangsobject voor gelieerde personen
    /// </summary>
    public class GelieerdePersonenDao : Dao<GelieerdePersoon, ChiroGroepEntities>, IGelieerdePersonenDao
    {
        /// <summary>
        /// Instantieert een gegevenstoegangsobject voor gelieerde personen
        /// </summary>
        public GelieerdePersonenDao()
        {
            ConnectedEntities = new Expression<Func<GelieerdePersoon, object>>[] 
			{ 
				e => e.Persoon, 
				e => e.Groep.WithoutUpdate()
			};
        }

        /// <summary>
        /// Haalt alle gelieerde personen van een groep op, inclusief de gerelateerde entity's gegeven
        /// in <paramref name="extras"/>
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan we de gelieerde personen willen opvragen</param>
        /// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
        /// <param name="extras">Bepaalt op te halen gerelateerde entity's</param>
        /// <returns>De gevraagde lijst gelieerde personen</returns>
        public IList<GelieerdePersoon> AllenOphalen(
            int groepID,
            PersoonSorteringsEnum sortering,
            PersoonsExtras extras)
        {
            IList<GelieerdePersoon> result;

            using (var db = new ChiroGroepEntities())
            {
                // Eerst alles 'gewoon' ophalen'.  Op die manier komen straatnamen en woonplaatsen
                // niet dubbel over voor alle adressen en standaardadres.

                var query = (from gp in db.GelieerdePersoon
                             where gp.Groep.ID == groepID
                             select gp) as ObjectQuery<GelieerdePersoon>;

                var paths = ExtrasNaarLambdas(extras);
                result = Sorteren(IncludesToepassen(query, paths), sortering).ToList();

                if ((extras & PersoonsExtras.Adressen) == PersoonsExtras.Adressen)
                {
                    AdresHelper.AlleAdressenKoppelen(result);
                }
                else if ((extras & PersoonsExtras.VoorkeurAdres) == PersoonsExtras.VoorkeurAdres)
                {
                    AdresHelper.VoorkeursAdresKoppelen(result);
                }

                if ((extras & PersoonsExtras.LedenDitWerkJaar) == PersoonsExtras.LedenDitWerkJaar)
                {
                    HuidigeLedenKoppelen(db, result);
                }
            }

            // Dan detachen

            result = Utility.DetachObjectGraph(result);

            return result;
        }

        /// <summary>
        /// Haalt een gelieerde persoon op op basis van <paramref name="persoonID"/> en <paramref name="groepID"/>
        /// </summary>
        /// <param name="persoonID">ID van de *persoon* waarvoor de gelieerde persoon opgehaald moet worden</param>
        /// <param name="groepID">ID van de groep waaraan de gelieerde persoon gelieerd moet zijn</param>
        /// <param name="metVoorkeurAdres">Indien <c>true</c>, worden ook het voorkeursadres mee opgehaald.</param>
        /// <param name="paths">Bepaalt op te halen gekoppelde entiteiten</param>
        /// <returns>De gevraagde gelieerde persoon</returns>
        /// <remarks>Het ophalen van adressen kan niet beschreven worden met de lambda-
        /// expressies in <paramref name="paths"/>, o.w.v. de verschillen tussen Belgische en buitenlandse
        /// adressen.</remarks>
        public GelieerdePersoon Ophalen(int persoonID, int groepID, bool metVoorkeurAdres, params Expression<Func<GelieerdePersoon, object>>[] paths)
        {
            GelieerdePersoon result;

            using (var db = new ChiroGroepEntities())
            {
                var query = (from gp in (metVoorkeurAdres ? db.GelieerdePersoon.Include(gp => gp.PersoonsAdres.Adres) : db.GelieerdePersoon)
                             where gp.Persoon.ID == persoonID && gp.Groep.ID == groepID
                             select gp) as ObjectQuery<GelieerdePersoon>;
                result = IncludesToepassen(query, paths).FirstOrDefault();

                if (metVoorkeurAdres && result.PersoonsAdres != null)
                {
                    // Zoek naar Belgisch adres met gegeven AdresID.  Als er een gevonden is,
                    // instantieert FirstOrDefault dit, en koppelt Entity Framework de
                    // adresgegevens magischerwijze aan resultaat.

                    (from adr in db.Adres.OfType<BelgischAdres>().Include(ba => ba.StraatNaam).Include(ba => ba.WoonPlaats)
                     where adr.ID == result.PersoonsAdres.Adres.ID
                     select adr).FirstOrDefault();

                    // Instantieer Buitenlands adres.  Ook hier instantiëren om te koppelen
                    // aan result.

                    (from adr in db.Adres.OfType<BuitenLandsAdres>().Include(ba => ba.Land)
                     where adr.ID == result.PersoonsAdres.Adres.ID
                     select adr).FirstOrDefault();
                }
            }

            return Utility.DetachObjectGraph(result);
        }

        /// <summary>
        /// Haalt een gelieerde persoon op op basis van <paramref name="persoonID"/> en <paramref name="groepID"/>
        /// </summary>
        /// <param name="persoonID">ID van de *persoon* waarvoor de gelieerde persoon opgehaald moet worden</param>
        /// <param name="groepID">ID van de groep waaraan de gelieerde persoon gelieerd moet zijn</param>
        /// <param name="paths">Bepaalt op te halen gekoppelde entiteiten</param>
        /// <returns>De gevraagde gelieerde persoon</returns>
        /// <remarks>Adressen worden niet opgehaald, want adressen kunnen niet beschreven worden met de lambda-
        /// expressies in <paramref name="paths"/>.</remarks>
        public GelieerdePersoon Ophalen(int persoonID, int groepID, params Expression<Func<GelieerdePersoon, object>>[] paths)
        {
            return Ophalen(persoonID, groepID, false, paths);
        }

        /// <summary>
        /// Haalt een aantal gelieerde personen op, samen met de gekoppelde entiteiten bepaald door
        /// <paramref name="extras"/>
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's op te halen gelieerde personen</param>
        /// <param name="extras">Bepaalt de extra op te halen entiteiten</param>
        /// <returns>De gevraagde gelieerde personen.</returns>
        public IEnumerable<GelieerdePersoon> Ophalen(IEnumerable<int> gelieerdePersoonIDs, PersoonsExtras extras)
        {
            IEnumerable<GelieerdePersoon> resultaat;

            using (var db = new ChiroGroepEntities())
            {
                resultaat =
                    db.GelieerdePersoon.Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.ID, gelieerdePersoonIDs));

                resultaat = IncludesToepassen(resultaat as ObjectQuery<GelieerdePersoon>, ExtrasNaarLambdas(extras)).ToArray();

                if ((extras & PersoonsExtras.Adressen) == PersoonsExtras.Adressen)
                {
                    AdresHelper.AlleAdressenKoppelen(resultaat);
                }
                else if ((extras & PersoonsExtras.VoorkeurAdres) == PersoonsExtras.VoorkeurAdres)
                {
                    AdresHelper.VoorkeursAdresKoppelen(resultaat);
                }

                if ((extras & PersoonsExtras.LedenDitWerkJaar) == PersoonsExtras.LedenDitWerkJaar)
                {
                    // Als niet alle lidobjecten gevraagd zijn, maar wel de lidobjecten van
                    // het huidige werkjaar, dan moeten we die expliciet koppelen

                    HuidigeLedenKoppelen(db, resultaat);
                }

                if ((extras & PersoonsExtras.AbonnementenDitWerkjaar) == PersoonsExtras.AbonnementenDitWerkjaar)
                {
                    HuidigeAbonnementenKoppelen(db, resultaat);
                }
            }

            return Utility.DetachObjectGraph(resultaat);
        }

        /// <summary>
        /// Haal een lijst op van de eerste letters van de achternamen van gelieerde personen van een groep
        /// </summary>
        /// <param name="groepID">
        /// GroepID van gevraagde groep
        /// </param>
        /// <returns>
        /// Lijst met de eerste letter gegroepeerd van de achternamen
        /// </returns>
        public IList<String> EersteLetterNamenOphalen(int groepID)
        {
            IList<String> lijst;

            using (var db = new ChiroGroepEntities())
            {
                lijst = (from gp in db.GelieerdePersoon
                         where gp.Groep.ID == groepID
                         let letter = gp.Persoon.Naam.Substring(0, 1)
                         orderby letter
                         select letter).Distinct().ToList();
            }

            return lijst;
        }

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
        public IList<string> EersteLetterNamenOphalenCategorie(int categorieID)
        {
            IList<String> lijst;

            using (var db = new ChiroGroepEntities())
            {
                lijst = (from gp in db.GelieerdePersoon
                         where gp.Categorie.Any(cat => cat.ID == categorieID)
                         let letter = gp.Persoon.Naam.Substring(0, 1)
                         orderby letter
                         select letter).Distinct().ToList();
            }

            return lijst;
        }

        /// <summary>
        /// Instantieert de abonnementen van de gegeven gelieerde personen voor het huidige werkjaar
        /// </summary>
        /// <param name="db">De Objectcontext</param>
        /// <param name="gelieerdePersonen">Gelieerde personen waaraan abonnementen gekoppeld moeten worden</param>
        /// <returns>Dezelfde gelieerde personen, maar met abonnementen gekoppeld</returns>
        /// <remarks>Dit werkt enkel als alle gelieerde personen aan dezelfde groep gekoppeld zijn</remarks>
        private IEnumerable<GelieerdePersoon> HuidigeAbonnementenKoppelen(ChiroGroepEntities db, IEnumerable<GelieerdePersoon> gelieerdePersonen)
        {
            var groepIDs = (from gp in gelieerdePersonen select gp.Groep.ID).Distinct();
            Debug.Assert(groepIDs.Count() == 1);

            int groepID = groepIDs.First();

            var gelieerdePersoonIDs = (from gp in gelieerdePersonen select gp.ID).ToArray();

            int groepsWerkJaarID = (from w in db.GroepsWerkJaar
                                    where w.Groep.ID == groepID
                                    orderby w.WerkJaar descending
                                    select w.ID).FirstOrDefault();

            // Selecteer en instantieer de abonnementen met uit groepswerkjaar met ID groepsWerkJaarID
            // en gelieerdePersoon uit GelieerdePersoonIDs.  Omdat de objectcontext nog actief
            // is, en de gelieerde personen daaraan gekoppeld zijn, zullen de geïnstantieerde
            // abonnementen aan de goede gelieerde personen gekoppeld worden.

            var abonnementen = db.Abonnement.Include(ab => ab.GelieerdePersoon).Include(ab => ab.Publicatie).Where(Utility.BuildContainsExpression<Abonnement, int>(
                ab => ab.GelieerdePersoon.ID,
                gelieerdePersoonIDs)).Where(ab => ab.GroepsWerkJaar.ID == groepsWerkJaarID).ToArray();

            return gelieerdePersonen;
        }

        /// <summary>
        /// Haalt een gelieerde persoon op, samen met de gekoppelde entiteiten bepaald door
        /// <paramref name="extras"/>
        /// </summary>
        /// <param name="gelieerdePersoonID">ID op te halen gelieerde persoon</param>
        /// <param name="extras">Bepaalt de extra op te halen entiteiten</param>
        /// <returns>De gevraagde gelieerde persoon.</returns>
        public GelieerdePersoon Ophalen(int gelieerdePersoonID, PersoonsExtras extras)
        {
            return Ophalen(new[] { gelieerdePersoonID }, extras).FirstOrDefault();
        }

        /// <summary>
        /// Sorteert een 'queryable' van gelieerde personen. Eerst volgens de gegeven ordening, dan steeds op naam.
        /// <para />
        /// De sortering is vrij complex om met meerdere opties rekening te houden.
        /// <para />
        /// Steeds wordt eerst gesorteerd op lege velden/gevulde velden, de lege komen laatst.
        /// Dan wordt gesorteerd op "sortering"
        ///		Naam => Naam, Voornaam
        ///		Categorie => Naam van de categorie die eerst in de lijst staat #TODO dit is mss niet optimaal
        ///		Leeftijd => Op leeftijd, jongste eerst
        /// Dan worden overblijvende gelijke records op naam en voornaam gesorteerd
        /// </summary>
        /// <param name="lijst">De te sorteren 'queryable'</param>
        /// <param name="sortering">Hoe te sorteren</param>
        /// <returns>Een nieuwe queryable, die de resultaten op de gewenste manier sorteert</returns>
        private static IQueryable<GelieerdePersoon> Sorteren(IQueryable<GelieerdePersoon> lijst, PersoonSorteringsEnum sortering)
        {
            switch (sortering)
            {
                case PersoonSorteringsEnum.Naam:
                    return lijst
                        .OrderBy(gp => gp.Persoon.Naam).ThenBy(gp => gp.Persoon.VoorNaam);
                case PersoonSorteringsEnum.Leeftijd:
                    return lijst
                        .OrderBy(gp => gp.Persoon.GeboorteDatum == null)
                        .ThenByDescending(gp => gp.Persoon.GeboorteDatum)
                        .ThenBy(gp => gp.Persoon.Naam).ThenBy(gp => gp.Persoon.VoorNaam);
                case PersoonSorteringsEnum.Categorie:
                    return lijst
                        .OrderBy(gp => gp.Categorie.FirstOrDefault() == null)
                        .ThenBy(gp => (gp.Categorie.FirstOrDefault() == null ? null : gp.Categorie.First().Naam))
                        .ThenBy(gp => gp.Persoon.Naam).ThenBy(gp => gp.Persoon.VoorNaam);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Haal een pagina op met gelieerde personen van een groep.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan gelieerde personen op te halen zijn</param>
        /// <param name="pagina">Gevraagde pagina</param>
        /// <param name="paginaGrootte">Aantal personen per pagina</param>
        /// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
        /// <param name="extras">Bepaalt de mee op te halen gekoppelde entiteiten</param>
        /// <param name="aantalTotaal">Out-parameter die weergeeft hoeveel gelieerde personen er in totaal 
        /// zijn. </param>
        /// <returns>De gevraagde lijst gelieerde personen</returns>
        public IList<GelieerdePersoon> PaginaOphalen(
            int groepID,
            int pagina,
            int paginaGrootte,
            PersoonSorteringsEnum sortering,
            PersoonsExtras extras,
            out int aantalTotaal)
        {
            IList<GelieerdePersoon> lijst;

            using (var db = new ChiroGroepEntities())
            {
                // Haal de gelieerde personen op van de gevraagde groep

                var gpQuery = (from gp in db.GelieerdePersoon
                               where gp.Groep.ID == groepID
                               select gp) as ObjectQuery<GelieerdePersoon>;

                Debug.Assert(gpQuery != null);

                // simpele includes toepassen, sorteren en tellen

                var paths = ExtrasNaarLambdas(extras);
                lijst = Sorteren(
                    IncludesToepassen(gpQuery, paths),
                    sortering).PaginaSelecteren(pagina, paginaGrootte).ToList();

                aantalTotaal = gpQuery.Count();

                // De moeilijkere gekoppelde entiteiten:

                if ((extras & PersoonsExtras.Adressen) == PersoonsExtras.Adressen)
                {
                    AdresHelper.AlleAdressenKoppelen(lijst);
                }
                else if ((extras & PersoonsExtras.VoorkeurAdres) == PersoonsExtras.VoorkeurAdres)
                {
                    AdresHelper.VoorkeursAdresKoppelen(lijst);
                }

                if ((extras & PersoonsExtras.LedenDitWerkJaar) == PersoonsExtras.LedenDitWerkJaar)
                {
                    HuidigeLedenKoppelen(db, lijst);
                }
            }

            Utility.DetachObjectGraph(lijst);
            return lijst;
        }


        /// <summary>
        /// Haalt alle gelieerde personen op waarvan de familienaam begint met de letter <paramref name="letter"/>.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan gelieerde personen op te halen zijn</param>
        /// <param name="letter">Eerste letter van de achternamen van de personen die we willen bekijken</param>
        /// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
        /// <param name="extras">Bepaalt de mee op te halen gekoppelde entiteiten</param>
        /// <param name="aantalTotaal">Out-parameter die weergeeft hoeveel gelieerde personen er in totaal 
        /// zijn. </param>
        /// <returns>De gevraagde lijst gelieerde personen</returns>
        public IList<GelieerdePersoon> Ophalen(
            int groepID,
            string letter,
            PersoonSorteringsEnum sortering,
            PersoonsExtras extras,
            out int aantalTotaal)
        {
            IList<GelieerdePersoon> lijst;

            using (var db = new ChiroGroepEntities())
            {
                // Misschien een slechte oplossing voor alleen maar tellen hoeveel er in totaal zijn??
                var gpQueryForCount = (from gp in db.GelieerdePersoon
                               where gp.Groep.ID == groepID
                               select gp) as ObjectQuery<GelieerdePersoon>;

                // Haal de gelieerde personen op van de gevraagde groep
                var gpQuery = (from gp in db.GelieerdePersoon
                               where gp.Groep.ID == groepID &&
                               gp.Persoon.Naam.Substring(0, 1) == letter
                               select gp) as ObjectQuery<GelieerdePersoon>;

                Debug.Assert(gpQuery != null);

                // simpele includes toepassen, sorteren en tellen

                var paths = ExtrasNaarLambdas(extras);
                lijst = Sorteren(
                    IncludesToepassen(gpQuery, paths),
                    sortering).ToList();

                aantalTotaal = gpQueryForCount.Count();

                // De moeilijkere gekoppelde entiteiten:

                if ((extras & PersoonsExtras.Adressen) == PersoonsExtras.Adressen)
                {
                    AdresHelper.AlleAdressenKoppelen(lijst);
                }
                else if ((extras & PersoonsExtras.VoorkeurAdres) == PersoonsExtras.VoorkeurAdres)
                {
                    AdresHelper.VoorkeursAdresKoppelen(lijst);
                }

                if ((extras & PersoonsExtras.LedenDitWerkJaar) == PersoonsExtras.LedenDitWerkJaar)
                {
                    HuidigeLedenKoppelen(db, lijst);
                }
            }

            Utility.DetachObjectGraph(lijst);
            return lijst;
        }

        /// <summary>
        /// Haalt alle gelieerde personen op uit categorie met ID <paramref name="categorieID"/> 
        /// wiens familienaam begint met de letter <paramref name="letter"/>.
        /// </summary>
        /// <param name="categorieID">ID van de categorie waarvan gelieerde personen op te halen zijn</param>
        /// <param name="letter">Eerste letter van de achternamen van de personen die we willen bekijken</param>
        /// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
        /// <param name="extras">Bepaalt de mee op te halen gekoppelde entiteiten</param>
        /// <param name="aantalTotaal">Out-parameter die weergeeft hoeveel gelieerde personen er in totaal 
        /// zijn. </param>
        /// <returns>De gevraagde lijst gelieerde personen</returns>
        public IList<GelieerdePersoon> OphalenUitCategorie(int categorieID, string letter, PersoonSorteringsEnum sortering, out int aantalTotaal, PersoonsExtras extras)
        {
            IList<GelieerdePersoon> lijst;

            using (var db = new ChiroGroepEntities())
            {
                // Misschien een slechte oplossing voor alleen maar tellen hoeveel er in totaal zijn in de categorie??
                var queryForCount = from gp in db.GelieerdePersoon.Include(gp => gp.Categorie)
                            where gp.Categorie.Any(cat => cat.ID == categorieID)
                            select gp;

                // Haal alle personen in de gevraagde categorie op
                var query = from gp in db.GelieerdePersoon.Include(gp => gp.Categorie)
                            where gp.Categorie.Any(cat => cat.ID == categorieID) &&
                            gp.Persoon.Naam.Substring(0, 1) == letter
                            select gp;

                var paths = ExtrasNaarLambdas(extras);

                var queryMetExtras = (letter != "A-Z") ? IncludesToepassen(query as ObjectQuery<GelieerdePersoon>, paths) : IncludesToepassen(queryForCount as ObjectQuery<GelieerdePersoon>, paths);

                // Sorteer ze en bepaal totaal aantal personen
                lijst = Sorteren(queryMetExtras, sortering).ToList();

                aantalTotaal = queryForCount.Count();

                if ((extras & PersoonsExtras.Adressen) == PersoonsExtras.Adressen)
                {
                    AdresHelper.AlleAdressenKoppelen(lijst);
                }
                else if ((extras & PersoonsExtras.VoorkeurAdres) == PersoonsExtras.VoorkeurAdres)
                {
                    AdresHelper.VoorkeursAdresKoppelen(lijst);
                }

                // als enkel de lidobjecten van dit werkjaar opgevraagd worden, dan moeten we dat nog
                // arrangeren:

                if ((extras & PersoonsExtras.LedenDitWerkJaar) == PersoonsExtras.LedenDitWerkJaar)
                {
                    HuidigeLedenKoppelen(db, lijst);
                }
            }
            Utility.DetachObjectGraph(lijst);

            return lijst;   // lijst met personen + lidinfo
        }

        /// <summary>
        /// Haalt de gelieerde personen op die bij de gegeven ID's horen
        /// </summary>
        /// <param name="gelieerdePersonenIDs">Een lijst van ID's van gelieerde personen</param>
        /// <returns>De gelieerde personen die bij de <paramref name="gelieerdePersonenIDs"/> horen</returns>
        public override IList<GelieerdePersoon> Ophalen(IEnumerable<int> gelieerdePersonenIDs)
        {
            using (var db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

                return (
                    from gp in db.GelieerdePersoon.Include("Groep").Include("Persoon").Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.ID, gelieerdePersonenIDs))
                    select gp).ToList();
            }
        }

        /// <summary>
        /// Haalt een gelieerde persoon op, inclusief
        ///  - persoon
        ///  - communicatievormen
        ///  - (alle!) adressen
        ///  - groepen
        ///  - categorieen
        ///  - lidobjecten in het huidige werkjaar
        ///  - afdelingen en functies van die lidobjecen
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gevraagde gelieerde persoon</param>
        /// <returns>Gelieerde persoon met alle bovenvernoemde details</returns>
        public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
        {
            GelieerdePersoon gelpers;

            using (var db = new ChiroGroepEntities())
            {
                gelpers = (
                            from gp in db.GelieerdePersoon
                                .Include(gp => gp.Persoon)
                                .Include(gp => gp.Communicatie.First().CommunicatieType)
                                .Include(gp => gp.PersoonsAdres)
                                .Include(gp => gp.Persoon.PersoonsAdres.First().Adres)
                                .Include(gp => gp.Groep)
                                .Include(gp => gp.Categorie)
                                .Include(gp => gp.Persoon.PersoonsVerzekering.First().VerzekeringsType)
                            where gp.ID == gelieerdePersoonID
                            select gp).FirstOrDefault();

                AdresHelper.AlleAdressenKoppelen(gelpers);
                HuidigeAbonnementenKoppelen(db, new[] { gelpers });

                var gwj = (from g in db.GroepsWerkJaar
                           where g.Groep.ID == gelpers.Groep.ID
                           orderby g.WerkJaar descending
                           select g).FirstOrDefault();

                if (gwj != null)
                {
                    // Lidgegevens ophalen.  Koppelen aan persoon en groepswerkjaar.  Ook afdelingen
                    // en functies ophalen.

                    Lid lid = (from l in db.Lid
                                .Include(l => l.GelieerdePersoon)
                               where l.GroepsWerkJaar.ID == gwj.ID && l.GelieerdePersoon.ID == gelpers.ID
                               select l).FirstOrDefault();

                    if (lid != null)
                    {
                        int lidID = lid.ID;

                        if (lid is Kind)
                        {
                            // Haal opnieuw op met afdelingen
                            lid = (
                                from t in db.Lid.OfType<Kind>()
                                    .Include("GelieerdePersoon.Persoon")
                                    .Include("GroepsWerkJaar")
                                    .Include("AfdelingsJaar.Afdeling")
                                    .Include(knd => knd.Functie)
                                where t.ID == lidID
                                select t).FirstOrDefault();
                        }
                        if (lid is Leiding)
                        {
                            lid = (
                                from t in db.Lid.OfType<Leiding>()
                                    .Include("GelieerdePersoon.Persoon")
                                    .Include("GroepsWerkJaar")
                                    .Include("AfdelingsJaar.Afdeling")
                                    .Include(leid => leid.Functie)
                                where t.ID == lidID
                                select t).FirstOrDefault();
                        }
                    }
                }
            }

            return Utility.DetachObjectGraph(gelpers);
        }

        /// <summary>
        /// Plakt de groep aan de gelieerde persoon
        /// </summary>
        /// <param name="p">De gelieerde persoon in kwestie</param>
        /// <returns>De gelieerde persoon met zijn/haar groep</returns>
        public GelieerdePersoon GroepLaden(GelieerdePersoon p)
        {
            Debug.Assert(p != null);

            if (p.Groep == null)
            {
                Groep g;

                Debug.Assert(p.ID != 0);
                // Voor een nieuwe gelieerde persoon (p.ID == 0) moet 
                // de groepsproperty altijd gezet zijn, anders kan hij
                // niet bewaard worden.  Aangezien g.Groep == null,
                // kan het dus niet om een nieuwe persoon gaan.

                using (var db = new ChiroGroepEntities())
                {
                    db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

                    g = (from gp in db.GelieerdePersoon
                         where gp.ID == p.ID
                         select gp.Groep).FirstOrDefault();
                }
                p.Groep = g;
                g.GelieerdePersoon.Add(p);  // nog niet zeker of dit gaat werken...
            }

            return p;
        }

        /// <summary>
        /// Een lijstje ophalen van communicatietypes
        /// </summary>
        /// <returns>
        /// Een IEnumerable van communicatietypes
        /// </returns>
        public IEnumerable<CommunicatieType> CommunicatieTypesOphalen()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Haalt een lijst op van gelieerde personen (met persoons-, adres- en communicatiegegevens)
        /// die gekoppeld zijn aan de gegeven groep en bij wie de <paramref name="zoekStringNaam"/>
        /// voorkomt in hun naam
        /// </summary>
        /// <param name="groepID">ID van de groep waar de gelieerde persoon aan gekoppeld moet zijn</param>
        /// <param name="zoekStringNaam">De zoekterm</param>
        /// <returns>Een lijst van gelieerde personen die aan de voorwaarden voldoen</returns>
        public IList<GelieerdePersoon> ZoekenOpNaam(int groepID, string zoekStringNaam)
        {
            IList<GelieerdePersoon> result;

            using (var db = new ChiroGroepEntities())
            {
                result = (
                    from gp in db.GelieerdePersoon
                        .Include(gp => gp.Persoon)
                        .Include(gp => gp.Communicatie)
                        .Include(gp => gp.PersoonsAdres)
                        .Include(gp => gp.Persoon.PersoonsAdres.First().Adres)
                    where (gp.Persoon.VoorNaam + " " + gp.Persoon.Naam + " " + gp.Persoon.VoorNaam).Contains(zoekStringNaam)
                    && gp.Groep.ID == groepID
                    select gp).ToList();

                AdresHelper.AlleAdressenKoppelen(result);
            }

            return Utility.DetachObjectGraph(result);
        }

        /// <summary>
        /// Gelieerde personen opzoeken op (exacte) naam en voornaam.
        /// Gelieerde persoon, persoonsgegevens, adressen en communicatie
        /// worden opgehaald.
        /// </summary>
        /// <param name="groepID">ID van groep</param>
        /// <param name="naam">Exacte naam om op te zoeken</param>
        /// <param name="voornaam">Exacte voornaam om op te zoeken</param>
        /// <returns>Lijst met gevonden gelieerde personen</returns>
        public IEnumerable<GelieerdePersoon> ZoekenOpNaam(int groepID, string naam, string voornaam)
        {
            IEnumerable<GelieerdePersoon> resultaat;

            using (var db = new ChiroGroepEntities())
            {
                resultaat = (
                    from gp in db.GelieerdePersoon
                        .Include(gp => gp.Persoon)
                        .Include(gp => gp.Communicatie)
                        .Include(gp => gp.PersoonsAdres)
                        .Include(gp => gp.Persoon.PersoonsAdres.First().Adres)
                    where gp.Groep.ID == groepID && String.Compare(gp.Persoon.Naam, naam, true) == 0
                    && String.Compare(gp.Persoon.VoorNaam, voornaam, true) == 0
                    select gp).ToList();

                // Instantieer zowel Belgische als buitenlandse adressen.

                AdresHelper.AlleAdressenKoppelen(resultaat);
            }
            return Utility.DetachObjectGraph(resultaat);
        }

        /// <summary>
        /// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
        /// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
        /// (inclusief communicatie en adressen)
        /// </summary>
        /// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
        /// <param name="naam">Te zoeken naam (ongeveer)</param>
        /// <param name="voornaam">Te zoeken voornaam (ongeveer)</param>
        /// <returns>Lijst met gevonden matches</returns>
        /// <remarks>Includeert bij wijze van standaard de persoonsinfo</remarks>
        public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam)
        {
            return ZoekenOpNaamOngeveer(groepID, naam, voornaam, gp => gp.Persoon);
        }

        /// <summary>
        /// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
        /// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
        /// (inclusief communicatie en adressen)
        /// </summary>
        /// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
        /// <param name="naam">Te zoeken naam (ongeveer)</param>
        /// <param name="voornaam">Te zoeken voornaam (ongeveer)</param>
        /// <param name="paths">Expressies die aangeven welke dependencies mee opgehaald moeten worden</param>
        /// <returns>Lijst met gevonden matches</returns>
        public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam, params Expression<Func<GelieerdePersoon, object>>[] paths)
        {
            using (var db = new ChiroGroepEntities())
            {
                // Ik denk dat sql server user defined functions niet aangeroepen kunnen worden via LINQ to entities,
                // vandaar dat de query in Entity SQL opgesteld wordt.

                // !Herinner van bij de scouts dat die soundex best bij in de tabel terecht komt,
                // en dat daarop een index moet komen te liggen!

                const string ESQL_QUERY = "SELECT VALUE gp FROM ChiroGroepEntities.GelieerdePersoon AS gp " +
                                         "WHERE gp.Groep.ID = @groepid " +
                                         "AND ChiroGroepModel.Store.ufnSoundEx(gp.Persoon.Naam)=ChiroGroepModel.Store.ufnSoundEx(@naam) " +
                                         "AND ChiroGroepModel.Store.ufnSoundEx(gp.Persoon.Voornaam)=ChiroGroepModel.Store.ufnSoundEx(@voornaam)";

                // Query casten naar ObjectQuery, om zodadelijk de includes te kunnen toepassen.

                var query = db.CreateQuery<GelieerdePersoon>(ESQL_QUERY);

                query.Parameters.Add(new ObjectParameter("groepid", groepID));
                query.Parameters.Add(new ObjectParameter("voornaam", voornaam));
                query.Parameters.Add(new ObjectParameter("naam", naam));

                return IncludesToepassen(query, paths).ToList();
            }
        }

        /// <summary>
        /// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> waarvan de naam 
        /// of voornaam begint met <paramref name="teZoeken"/>
        /// </summary>
        /// <param name="groepID">
        /// GroepID dat bepaalt in welke gelieerde personen gezocht mag worden
        /// </param>
        /// <param name="teZoeken">
        /// Patroon te vinden in voornaam of naam
        /// </param>
        /// <returns>
        /// Lijst met gevonden matches
        /// </returns>
        public IList<GelieerdePersoon> ZoekenOpNaamVoornaamBegin(int groepID, string teZoeken)
        {
            IList<GelieerdePersoon> personen;

            using (var db = new ChiroGroepEntities())
            {
                personen = (
                    from gp in db.GelieerdePersoon
                        .Include(gp => gp.Persoon)
                    where gp.Groep.ID == groepID && (gp.Persoon.Naam.StartsWith(teZoeken) || gp.Persoon.VoorNaam.StartsWith(teZoeken))
                    select gp).ToList();
            }
            return personen;
        }

        /*public IList<GelieerdePersoon> OphalenUitCategorie(int categorieID)
        {
            using (var db = new ChiroGroepEntities())
            {
            db.Categorie.MergeOption = MergeOption.NoTracking;

            var query
                = from c in db.Categorie.Include("GelieerdePersoon.Persoon")
                  where c.ID == categorieID
                  select c;

            Categorie cat = query.FirstOrDefault();

            if (cat == null)
            {
                // categorie niet gevonden
                return new List<GelieerdePersoon>();
            }
            else
            {
                return cat.GelieerdePersoon.ToList();
            }
            }
        }*/

        /// <summary>
        /// Haalt een lijst op met de communicatietypes
        /// </summary>
        /// <returns>Een lijst met communicatietypes</returns>
        public IEnumerable<CommunicatieType> OphalenCommunicatieTypes()
        {
            using (var db = new ChiroGroepEntities())
            {
                db.CommunicatieType.MergeOption = MergeOption.NoTracking;
                return (
                    from gp in db.CommunicatieType
                    select gp).ToList();
            }
        }

        /// <summary>
        /// Haalt alle gelieerde personen op (incl persoonsinfo) die op een zelfde
        /// adres wonen en gelieerd zijn aan dezelfde groep als de gelieerde persoon met het gegeven ID.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gegeven gelieerde
        /// persoon.</param>
        /// <returns>Lijst met GelieerdePersonen (inc. persoonsinfo)</returns>
        /// <remarks>Als de persoon nergens woont, is hij toch zijn eigen
        /// huisgenoot.  Enkel huisgenoten uit dezelfde groep als de gelieerde persoon worden opgeleverd.</remarks>
        public IList<GelieerdePersoon> HuisGenotenOphalenZelfdeGroep(int gelieerdePersoonID)
        {
            using (var db = new ChiroGroepEntities())
            {
                // Zoek eerst de geleerde persoon zelf.

                var zelf = (from gp in db.GelieerdePersoon.Include(gp => gp.Persoon).Include(gp => gp.Groep)
                            where gp.ID == gelieerdePersoonID
                            select gp).FirstOrDefault();

                if (zelf == null)
                {
                    return new List<GelieerdePersoon>();
                }
                else
                {
                    var query = (from gp in db.GelieerdePersoon.Include(gp => gp.Persoon)
                                 where gp.Groep.ID == zelf.Groep.ID &&
                                   gp.Persoon.PersoonsAdres.Any(
                                       pa => pa.Adres.PersoonsAdres.Any(pa2 => pa2.Persoon.ID == zelf.Persoon.ID))
                                 select gp);

                    if (query.FirstOrDefault() == null)
                    {
                        return new List<GelieerdePersoon> { zelf };
                    }
                    else
                    {
                        return query.ToList();
                    }
                }
            }
        }

        /// <summary>
        /// Bewaart een gelieerde persoon samen met eventueel gekoppelde entiteiten
        /// </summary>
        /// <param name="gelieerdePersoon">Te bewaren gelieerde persoon</param>
        /// <param name="extras">Bepaalt de gekoppelde entiteiten</param>
        /// <returns>De bewaarde gelieerde persoon</returns>
        public GelieerdePersoon Bewaren(GelieerdePersoon gelieerdePersoon, PersoonsExtras extras)
        {
            return Bewaren(gelieerdePersoon, ExtrasNaarLambdas(extras));
        }

        /// <summary>
        /// Gegeven een <paramref name="groepID"/>, haal van de (ex-)GAV's waarvan 
        /// de personen gekend zijn, de gelieerde personen op (die dus de
        /// koppeling bepalen tussen de persoon en de groep met gegeven
        /// <paramref name="groepID"/>).
        /// Voorlopig worden ook de communicatiemiddelen mee opgeleverd.
        /// </summary>
        /// <param name="groepID">ID van een groep</param>
        /// <returns>Beschikbare gelieerde personen waarvan we weten dat ze GAV zijn
        /// voor die groep</returns>
        /// <remarks>Vervallen GAV's worden ook meegeleverd</remarks>
        public IEnumerable<GelieerdePersoon> OphalenOpBasisVanGavs(int groepID)
        {
            IEnumerable<GelieerdePersoon> resultaat;

            using (var db = new ChiroGroepEntities())
            {
                // selecteer de relevanten gebruikersrechten, en instantieer met 'ToArray'.

                var gebruikersRechten =
                    (from gr in db.GebruikersRecht.Include(gr => gr.Gav.Persoon).Include(gr => gr.Groep)
                     where gr.Groep.ID == groepID
                     select gr).ToArray();

                // In theorie kan er aan een Gav maar 1 persoon gekoppeld zijn.  In praktijk heb ik dat verkeerd
                // gedaan in de database, en zijn er mogelijk meer.  Dat verklaart de 'SelectMany' op
                // Gav.Persoon.  (Zie #1158)

                var persoonIDs = gebruikersRechten.SelectMany(gr => gr.Gav.Persoon).Select(p => p.ID);

                // Selecteer de relevante gelieerde personen, en instantieer met 'ToArray'.  De objectcontext
                // zal ervoor zorgen dat de juiste gelieerde persoon aan het juiste gebruikersrecht wordt
                // gekoppeld.

                var gelieerdePersonen =
                    (from gp in
                         db.GelieerdePersoon.Include(gp => gp.Persoon).Include(
                             gp => gp.Communicatie.First().CommunicatieType).Where(
                                 Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.Persoon.ID, persoonIDs))
                     where gp.Groep.ID == groepID
                     select gp).ToArray();

                resultaat = gelieerdePersonen;
            }
            return Utility.DetachObjectGraph(resultaat);
        }

        /// <summary>
        /// Koppelt de lidobjecten van dit werkjaar aan de gegeven
        /// <paramref name="gelieerdePersonen"/>.
        /// </summary>
        /// <param name="db">Objectcontext waaraan de <paramref name="gelieerdePersonen"/> gekoppeld
        /// moeten zijn.</param>
        /// <param name="gelieerdePersonen">De gelieerde personen waaraan we eventuele lidobjecten
        /// willen koppelen</param>
        /// <returns>Opnieuw de <paramref name="gelieerdePersonen"/>, maar zij die lid zijn in het
        /// recentste groepswerkjaar, hebben hun lidobject gekoppeld.</returns>
        /// <remarks>We gaan ervan uit dat alle <paramref name="gelieerdePersonen"/> aan dezelfde
        /// groep gekoppeld zijn.</remarks>
        public static IEnumerable<GelieerdePersoon> HuidigeLedenKoppelen(
            ChiroGroepEntities db,
            IEnumerable<GelieerdePersoon> gelieerdePersonen)
        {
            if (gelieerdePersonen.FirstOrDefault() != null)
            {
                // Enkel als de lijst daadwerkelijk personen bevat, doen we er iets mee.

                var groepIDs = (from gp in gelieerdePersonen select gp.Groep.ID).Distinct();
                Debug.Assert(groepIDs.Count() == 1);

                int groepID = groepIDs.First();

                var gelieerdePersoonIDs = (from gp in gelieerdePersonen select gp.ID).ToArray();

                int groepsWerkJaarID = (from w in db.GroepsWerkJaar
                                        where w.Groep.ID == groepID
                                        orderby w.WerkJaar descending
                                        select w.ID).FirstOrDefault();

                // Selecteer en instantieer de leden met uit groepswerkjaar met ID groepsWerkJaarID
                // en gelieerdePersoon uit GelieerdePersoonIDs.  Omdat de objectcontext nog actief
                // is, en de gelieerde personen daaraan gekoppeld zijn, zullen de geïnstantieerde
                // leden aan de goede gelieerde personen gekoppeld worden.

                var leden = db.Lid.Include(ld => ld.GelieerdePersoon).Where(Utility.BuildContainsExpression<Lid, int>(
                    l => l.GelieerdePersoon.ID,
                    gelieerdePersoonIDs)).Where(ld => ld.GroepsWerkJaar.ID == groepsWerkJaarID).ToArray();
            }

            return gelieerdePersonen;
        }

        /// <summary>
        /// Converteert de PersoonsExtras <paramref name="extras"/> naar lambda-expressies die mee naar 
        /// de data access moeten om de extra's daadwerkelijk op te halen.
        /// </summary>
        /// <param name="extras">Te converteren PersoonsExtra's</param>
        /// <returns>Lambda-expressies geschikt voor onze DAO's</returns>
        /// <remarks>
        /// Het gekoppeld persoonsobject wordt *altijd* mee opgehaald.  (Dit is min of meer
        /// historisch gegroeid)
        /// </remarks>
        private static Expression<Func<GelieerdePersoon, object>>[] ExtrasNaarLambdas(PersoonsExtras extras)
        {
            var paths = new List<Expression<Func<GelieerdePersoon, object>>> { gp => gp.Persoon };

            if ((extras & PersoonsExtras.VoorkeurAdres) != 0)
            {
                // standaardadres (bovenstaande conditie is ook true voor alle adressen)
                paths.Add(gp => gp.PersoonsAdres.Adres);
            }

            if ((extras & PersoonsExtras.Adressen) != 0)
            {
                // alle adressen
                paths.Add(gp => gp.Persoon.PersoonsAdres.First().Adres);
            }

            if ((extras & (PersoonsExtras.Groep | PersoonsExtras.LedenDitWerkJaar)) != 0)
            {
                // Als de leden van dit werkjaar opgevraagd worden, dan kunnen we
                // dat niet via een lambda-expressie uitdrukken.  Maar dan hebben
                // we straks de groep wel nodig, dus halen we die meteen ook maar op.

                paths.Add(gp => gp.Groep.WithoutUpdate());
            }

            if ((extras & PersoonsExtras.Communicatie) != 0)
            {
                paths.Add(gp => gp.Communicatie.First().CommunicatieType.WithoutUpdate());
            }

            if ((extras & PersoonsExtras.Categorieen) != 0)
            {
                paths.Add(gp => gp.Categorie.First().WithoutUpdate());
            }

            if ((extras & PersoonsExtras.GroepsWerkJaren) != 0)
            {
                paths.Add(gp => gp.Lid.First().GroepsWerkJaar.WithoutUpdate());
            }
            else if ((extras & PersoonsExtras.AlleLeden) == PersoonsExtras.AlleLeden)
            {
                paths.Add(gp => gp.Lid);
            }

            if ((extras & PersoonsExtras.Communicatie) != 0)
            {
                paths.Add(gp => gp.Communicatie.First().CommunicatieType.WithoutUpdate());
            }

            if ((extras & PersoonsExtras.Categorieen) != 0)
            {
                paths.Add(gp => gp.Categorie.First().WithoutUpdate());
            }

            if ((extras & PersoonsExtras.GroepsWerkJaren) != 0)
            {
                paths.Add(gp => gp.Lid.First().GroepsWerkJaar.WithoutUpdate());
            }
            else if ((extras & PersoonsExtras.AlleLeden) == PersoonsExtras.AlleLeden)
            {
                paths.Add(gp => gp.Lid);
            }

            if ((extras & PersoonsExtras.Uitstappen) == PersoonsExtras.Uitstappen)
            {
                paths.Add(gp => gp.Deelnemer.First().Uitstap.GroepsWerkJaar.Groep);
            }

            if ((extras & PersoonsExtras.GebruikersRechten) == PersoonsExtras.GebruikersRechten)
            {
                paths.Add(gp => gp.Persoon.Gav.FirstOrDefault().GebruikersRecht.FirstOrDefault().Groep);
            }

            return paths.ToArray();
        }
    }
}
