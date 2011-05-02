// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
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
    /// Gegevenstoegangsobject voor leden
    /// </summary>
    public class LedenDao : Dao<Lid, ChiroGroepEntities>, ILedenDao
    {
        /// <summary>
        /// Instantieert een gegevenstoegangsobject voor leden
        /// </summary>
        public LedenDao()
        {
            ConnectedEntities = new Expression<Func<Lid, object>>[] 
			{ 
				e => e.GroepsWerkJaar.WithoutUpdate(), 
				e => e.GelieerdePersoon.Persoon.WithoutUpdate()
			};
        }

        /// <summary>
        /// Zoekt lid op op basis van GelieerdePersoonID en GroepsWerkJaarID
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
        /// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
        /// <returns>Lidobject indien gevonden, anders null</returns>
        public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID)
        {
            return Ophalen(gelieerdePersoonID, groepsWerkJaarID, GetConnectedEntities());
        }

        /// <summary>
        /// Haalt een gemengde lijst leden (leden en leiding) op, samen met
        /// de gekoppelde entiteiten bepaald door <paramref name="extras"/>
        /// </summary>
        /// <param name="lidIDs">LidIDs op te halen leden</param>
        /// <param name="extras">bepaalt op te halen gekoppelde entiteiten</param>
        /// <returns>De gevraagde lijst leden</returns>
        public IEnumerable<Lid> Ophalen(IEnumerable<int> lidIDs, LidExtras extras)
        {
            IEnumerable<Lid> resultaat;
            using (var db = new ChiroGroepEntities())
            {
                var query = db.Lid.Where(Utility.BuildContainsExpression<Lid, int>(ld => ld.ID, lidIDs)) as ObjectQuery<Lid>;

                // Voer query uit, en pas eenvoudige includes toe

                resultaat = IncludesToepassen(query, ExtrasNaarLambdas<Lid>(extras & ~LidExtras.Afdelingen).ToArray()).ToArray();

                // Als de afdelingen gevraagd zijn, doen we dit via een louche truuk:

                if ((extras & LidExtras.Afdelingen) == LidExtras.Afdelingen)
                {
                    AfdelingenKoppelen(db, resultaat);
                }

                // Idem voor adressen

                if ((extras & LidExtras.Adressen) == LidExtras.Adressen)
                {
                    GelieerdePersonenDao.AlleAdressenKoppelen(db, (from ld in resultaat select ld.GelieerdePersoon));
                }
                else if ((extras & LidExtras.VoorkeurAdres) == LidExtras.VoorkeurAdres)
                {
                    GelieerdePersonenDao.VoorkeursAdresKoppelen(db, (from ld in resultaat select ld.GelieerdePersoon));
                }
            }
            return Utility.DetachObjectGraph(resultaat);
        }

        /// <summary>
        /// Haalt lid (kind of leiding) op, samen met
        /// de gekoppelde entiteiten bepaald door <paramref name="extras"/>
        /// </summary>
        /// <param name="lidID">LidID op te halen lid</param>
        /// <param name="extras">bepaalt op te halen gekoppelde entiteiten</param>
        /// <returns>De gevraagde lijst leden</returns>
        public Lid Ophalen(int lidID, LidExtras extras)
        {
            return Ophalen(new int[] { lidID }, extras).FirstOrDefault();
        }

        /// <summary>
        /// Koppelt de afdelingen aan een lijst <paramref name="leden"/>, die geattacht zijn aan 
        /// de objectcontext <paramref name="db"/>.
        /// </summary>
        /// <param name="db">te gebruiken objectcontext</param>
        /// <param name="leden">leden waarvan de afdelingen gekoppeld moeten worden</param>
        /// <returns>Dezelfde ledenlijst, maar nu met gekoppelde afdelingen.</returns>
        public static IEnumerable<Lid> AfdelingenKoppelen(ChiroGroepEntities db, IEnumerable<Lid> leden)
        {
            foreach (Lid l in leden)
            {
                if (l is Kind)
                {
                    (l as Kind).AfdelingsJaarReference.Load();
                }
                else if (l is Leiding)
                {
                    (l as Leiding).AfdelingsJaar.Load();
                }
            }
            return leden;
        }

        /// <summary>
        /// Haalt lid met gerelateerde entity's op, op basis van 
        /// GelieerdePersoonID en GroepsWerkJaarID
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
        /// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
        /// <param name="paths">Lambda-expressies die de extra op te halen
        /// informatie definieren</param>
        /// <returns>Lidobject indien gevonden, anders null</returns>
        public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
        {
            using (var db = new ChiroGroepEntities())
            {
                var query = (from l in db.Lid
                             where l.GelieerdePersoon.ID == gelieerdePersoonID
                             && l.GroepsWerkJaar.ID == groepsWerkJaarID
                             select l) as ObjectQuery<Lid>;
                return (IncludesToepassen(query, paths).FirstOrDefault());
            }
        }

        /// <summary>
        /// Sorteert een lijst van leiding. Eerst volgens de gegeven ordening, dan steeds op naam.
        /// <para />
        /// De sortering is vrij complex om met meerdere opties rekening te houden.
        /// <para />
        /// Steeds wordt eerst gesorteerd op lege velden/gevulde velden, de lege komen laatst.
        /// Dan wordt gesorteerd op "sortering"
        ///		Naam => Naam+Voornaam
        ///		Afdeling => Naam van de afdeling van het afdelingsjaar dat eerst in de lijst staat #TODO dit is mss niet optimaal
        ///		Leeftijd => Op leeftijd, jongste eerst
        /// Dan worden overblijvende gelijke records op naam+voornaam gesorteerd
        /// </summary>
        /// <param name="lijst">De te sorteren lijst</param>
        /// <param name="sortering">Hoe te sorteren</param>
        /// <returns>De gesorteerde lijst!!! In place sorteren lijkt niet mogelijk!!!</returns>
        private static List<Leiding> SorteerLijst(IEnumerable<Leiding> lijst, LidEigenschap sortering)
        {
            IEnumerable<Leiding> lijst2;
            switch (sortering)
            {
                case LidEigenschap.Naam:
                    lijst2 = lijst.OrderBy(gp => String.Format(
                                    "{0} {1}",
                                    gp.GelieerdePersoon.Persoon.Naam,
                                    gp.GelieerdePersoon.Persoon.VoorNaam));
                    break;
                case LidEigenschap.Leeftijd:
                    lijst2 = lijst
                        .OrderBy(gp => gp.GelieerdePersoon.GebDatumMetChiroLeefTijd == null)
                        .ThenByDescending(gp => gp.GelieerdePersoon.GebDatumMetChiroLeefTijd)
                        .ThenBy(gp => String.Format(
                            "{0} {1}",
                            gp.GelieerdePersoon.Persoon.Naam,
                            gp.GelieerdePersoon.Persoon.VoorNaam));
                    break;
                case LidEigenschap.Afdeling:
                    lijst2 = lijst
                        .OrderBy(gp => gp.AfdelingsJaar.FirstOrDefault() == null)
                        .ThenBy(
                            gp => (gp.AfdelingsJaar.FirstOrDefault() == null ? null : gp.AfdelingsJaar.First().Afdeling.Naam))
                        .ThenBy(gp => String.Format(
                            "{0} {1}",
                            gp.GelieerdePersoon.Persoon.Naam,
                            gp.GelieerdePersoon.Persoon.VoorNaam));
                    break;
                default: // Stom dat C# niet kan detecteren dat alle cases gecontroleerd zijn?
                    lijst2 = new List<Leiding>();
                    break;
            }
            return lijst2.ToList();
        }

        /// <summary>
        /// Sorteert een lijst van kinderen. Eerst volgens de gegeven ordening, dan steeds op naam.
        /// <para />
        /// De sortering is vrij complex om met meerdere opties rekening te houden.
        /// <para />
        /// Steeds wordt eerst gesorteerd op lege velden/gevulde velden, de lege komen laatst.
        /// Dan wordt gesorteerd op "sortering"
        ///		Naam => Naam+Voornaam
        ///		Afdeling => Naam van de afdeling van het afdelingsjaar dat eerst in de lijst staat #TODO dit is mss niet optimaal
        ///		Leeftijd => Op leeftijd, jongste eerst
        /// Dan worden overblijvende gelijke records op naam+voornaam gesorteerd
        /// </summary>
        /// <param name="lijst">De te sorteren lijst</param>
        /// <param name="sortering">Hoe te sorteren</param>
        /// <returns>De gesorteerde lijst!!! In place sorteren lijkt niet mogelijk!!!</returns>
        private static List<Kind> SorteerLijst(IEnumerable<Kind> lijst, LidEigenschap sortering)
        {
            IEnumerable<Kind> lijst2;
            switch (sortering)
            {
                case LidEigenschap.Naam:
                    lijst2 = lijst.OrderBy(gp => String.Format(
                                    "{0} {1}",
                                    gp.GelieerdePersoon.Persoon.Naam,
                                    gp.GelieerdePersoon.Persoon.VoorNaam));
                    break;
                case LidEigenschap.Leeftijd:
                    lijst2 = lijst
                        .OrderBy(gp => gp.GelieerdePersoon.GebDatumMetChiroLeefTijd == null)
                        .ThenByDescending(gp => gp.GelieerdePersoon.GebDatumMetChiroLeefTijd)
                        .ThenBy(gp => String.Format(
                            "{0} {1}",
                            gp.GelieerdePersoon.Persoon.Naam,
                            gp.GelieerdePersoon.Persoon.VoorNaam));
                    break;
                case LidEigenschap.Afdeling:
                    lijst2 = lijst
                        .OrderBy(gp => gp.AfdelingsJaar == null)
                        .ThenBy(gp => (gp.AfdelingsJaar.Afdeling.Naam))
                        .ThenBy(gp => String.Format(
                            "{0} {1}",
                            gp.GelieerdePersoon.Persoon.Naam,
                            gp.GelieerdePersoon.Persoon.VoorNaam));
                    break;
                default: // Stom dat C# niet kan detecteren dat alle cases gecontroleerd zijn?
                    lijst2 = lijst;
                    break;
            }
            return lijst2.ToList();
        }

        /// <summary>
        /// Maak een lijst van de gegeven ingeschreven leden (kinderen en leiding),
        /// gesorteerd volgens de opgegeven parameter
        /// </summary>
        /// <param name="kinderen">De lijst van kinderen</param>
        /// <param name="leiding">De lijst van leiders en/of leidsters</param>
        /// <param name="sortering">De parameter waarop de samengestelde lijst gesorteerd moet worden</param>
        /// <returns>De samengestelde en daarna gesorteerde lijst</returns>
        private static List<Lid> MaakLedenLijst(List<Kind> kinderen, List<Leiding> leiding, LidEigenschap sortering)
        {
            kinderen = SorteerLijst(kinderen, sortering);
            leiding = SorteerLijst(leiding, sortering);

            var lijst = new List<Lid>();
            lijst.AddRange(leiding.Cast<Lid>());
            lijst.AddRange(kinderen.Cast<Lid>());

            return lijst.OrderBy(e => e.NonActief).ToList();
        }

        /// <summary>
        /// Een lijst ophalen van alle leden voor het opgegeven groepswerkjaar op.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
        /// <param name="sortering">Parameter waarop de gegevens gesorteerd moeten worden</param>
        /// <returns>Een lijst alle leden voor het opgegeven groepswerkjaar</returns>
        public IList<Lid> AllesOphalen(int groepsWerkJaarID, LidEigenschap sortering)
        {
            using (var db = new ChiroGroepEntities())
            {
                db.Lid.MergeOption = MergeOption.NoTracking;

                var kinderen = (
                    from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList();

                var leiding = (
                    from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList();

                return MaakLedenLijst(kinderen, leiding, sortering);
            }
        }

        /// <summary>
        /// Een lijst ophalen van alle NIET-UITGESCHREVEN leden voor het opgegeven groepswerkjaar op.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
        /// <param name="sortering">Parameter waarop de gegevens gesorteerd moeten worden</param>
        /// <returns>Een lijst alle leden voor het opgegeven groepswerkjaar</returns>
        public IList<Lid> ActieveLedenOphalen(int groepsWerkJaarID, LidEigenschap sortering)
        {
            using (var db = new ChiroGroepEntities())
            {
                db.Lid.MergeOption = MergeOption.NoTracking;

                var kinderen = (
                    from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID && !l.NonActief
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList();

                var leiding = (
                    from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID && !l.NonActief
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList();

                return MaakLedenLijst(kinderen, leiding, sortering);
            }
        }

        /// <summary>
        /// Haalt een pagina op van de gevraagde gegevens:
        /// leden van een bepaalde groep in een gegeven werkjaar en die niet uitgeschreven zijn
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het aan een groep gekoppelde werkjaar</param>
        /// <param name="sortering">Parameter waarop de gegevens gesorteerd zijn</param>
        /// <returns>De leden die de groep in dat werkjaar heeft/had</returns>
        /// <remarks>
        /// Pagineren gebeurt per werkjaar.
        /// De parameters pagina, paginaGrootte en aantalTotaal zijn hier niet nodig
        /// omdat alle leden van dat werkjaar samen getoond worden.
        /// </remarks>
        public IList<Lid> PaginaOphalen(int groepsWerkJaarID, LidEigenschap sortering)
        {
            using (var db = new ChiroGroepEntities())
            {
                db.Lid.MergeOption = MergeOption.NoTracking;

                var kinderen = (
                    from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID && !l.NonActief
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList();

                var leiding = (
                    from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList();

                return MaakLedenLijst(kinderen, leiding, sortering);
            }
        }

        /// <summary>
        /// Haalt een pagina op van de gevraagde gegevens:
        /// leden van een bepaalde groep in een gegeven werkjaar, die in de gegeven afdeling zitten en die niet uitgeschreven zijn
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het aan een groep gekoppelde werkjaar</param>
        /// <param name="afdelingsID">ID van de afdeling waar de leden in moeten zitten</param>
        /// <param name="sortering">Parameter waarop de gegevens gesorteerd zijn</param>
        /// <returns>De leden die de groep in dat werkjaar heeft/had en die in de gegeven afdeling zitten/zaten</returns>
        /// <remarks>
        /// Pagineren gebeurt per werkjaar.
        /// </remarks>
        public IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, LidEigenschap sortering)
        {
            using (var db = new ChiroGroepEntities())
            {
                db.Lid.MergeOption = MergeOption.NoTracking;

                var kinderen = (
                    from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                        &&
                      l.AfdelingsJaar.Afdeling.ID == afdelingsID
                       &&
                       !l.NonActief
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList();

                var leiding = (
                    from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                        &&
                      l.AfdelingsJaar.Any(x => x.Afdeling.ID == afdelingsID)
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList();

                return MaakLedenLijst(kinderen, leiding, sortering);
            }
        }

        /// <summary>
        /// Haalt een pagina op van de gevraagde gegevens:
        /// leden van een bepaalde groep in een gegeven werkjaar, die een bepaalde functie hebben/hadden en die niet uitgeschreven zijn
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het aan een groep gekoppelde werkjaar</param>
        /// <param name="functieID">ID van de functie die de leden moeten hebben</param>
        /// <param name="sortering">Parameter waarop de gegevens gesorteerd zijn</param>
        /// <returns>De leden met de gegeven functie die de groep in dat werkjaar heeft/had</returns>
        /// <remarks>
        /// Pagineren gebeurt per werkjaar.
        /// Haalt GEEN afdeling mee op (nakijken of dit ook effectief niet nodig is?)
        /// </remarks>
        public IList<Lid> PaginaOphalenVolgensFunctie(int groepsWerkJaarID, int functieID, LidEigenschap sortering)
        {
            using (var db = new ChiroGroepEntities())
            {
                db.Lid.MergeOption = MergeOption.NoTracking;

                var kinderen = (
                    from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                        &&
                      l.Functie.Any(e => e.ID == functieID)
                       &&
                       !l.NonActief
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList();

                var leiding = (
                    from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
                    where l.GroepsWerkJaar.ID == groepsWerkJaarID
                        &&
                      l.Functie.Any(e => e.ID == functieID)
                    orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
                    select l).ToList();

                return MaakLedenLijst(kinderen, leiding, sortering);
            }
        }

        /// <summary>
        /// Lid met afdelingsjaren, afdelingen en gelieerdepersoon
        /// </summary>
        /// <param name="lidID">ID van het lid waarvan we gegevens willen opvragen</param>
        /// <returns>Een lid met afdelingsjaren, afdelingen en gelieerdepersoon</returns>
        public Lid OphalenMetDetails(int lidID)
        {
            using (var db = new ChiroGroepEntities())
            {
                db.Lid.MergeOption = MergeOption.NoTracking;

                var lid = (
            from t in db.Lid.Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling").Include("Functie")
            where t.ID == lidID
            select t).FirstOrDefault();

                if (lid is Kind)
                {
                    return (
                        from t in db.Lid.OfType<Kind>().Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling").Include("AfdelingsJaar.Afdeling").Include("Functie")
                        where t.ID == lidID
                        select t).FirstOrDefault();
                }
                if (lid is Leiding)
                {
                    return (
                        from t in db.Lid.OfType<Leiding>().Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling").Include("AfdelingsJaar.Afdeling").Include("Functie")
                        where t.ID == lidID
                        select t).FirstOrDefault();
                }
                return lid;
            }
        }

        /// <summary>
        /// Haalt de leden op die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
        /// de functie bepaald door <paramref name="functieID"/> hebben
        /// </summary>
        /// <param name="functieID">ID van een functie</param>
        /// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
        /// <param name="paths">Bepaalt de mee op te halen gekoppelde entiteiten</param>
        /// <returns>Lijst leden die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
        /// de functie bepaald door <paramref name="functieID"/> hebben.</returns>
        public IList<Lid> OphalenUitFunctie(int functieID, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
        {
            IList<Lid> result;
            using (var db = new ChiroGroepEntities())
            {
                var query = (from lid in db.Lid
                             where lid.Functie.Any(fnc => fnc.ID == functieID) &&
                            lid.GroepsWerkJaar.ID == groepsWerkJaarID
                             select lid) as ObjectQuery<Lid>;
                result = (IncludesToepassen(query, paths)).ToList();
            }

            return Utility.DetachObjectGraph(result);
        }

        /// <summary>
        /// Haalt de leden op die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
        /// de gepredefinieerde functie met type <paramref name="f"/> hebben.
        /// </summary>
        /// <param name="f">Type gepredefinieerde functie</param>
        /// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
        /// <param name="paths">Bepaalt de mee op te halen gekoppelde entiteiten</param>
        /// <returns>Lijst leden die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
        /// de gepredefinieerde functie met type <paramref name="f"/> hebben.</returns>
        public IList<Lid> OphalenUitFunctie(NationaleFunctie f, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
        {
            return OphalenUitFunctie((int)f, groepsWerkJaarID, paths);
        }

        /// <summary>
        /// Geeft <c>true</c> indien het lid met <paramref name="lidID"/> leiding is, anders <c>false</c>
        /// </summary>
        /// <param name="lidID">ID van lid waarvoor na te gaan of het al dan niet leiding is</param>
        /// <returns><c>true</c> indien het lid met <paramref name="lidID"/> leiding is, anders <c>false</c></returns>
        public bool IsLeiding(int lidID)
        {
            using (var db = new ChiroGroepEntities())
            {
                Lid l = (from ld in db.Lid
                         where ld.ID == lidID
                         select ld).FirstOrDefault();
                return (l is Leiding);
            }
        }

        /// <summary>
        /// Haalt het lid op bepaald door <paramref name="gelieerdePersoonID"/> en
        /// <paramref name="groepsWerkJaarID"/>, inclusief persoon, afdelingen, functies, groepswerkjaar
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon waarvoor het lidobject gevraagd is.</param>
        /// <param name="groepsWerkJaarID">ID van groepswerkjaar in hetwelke het lidobject gevraagd is</param>
        /// <returns>
        /// Het lid bepaald door <paramref name="gelieerdePersoonID"/> en
        /// <paramref name="groepsWerkJaarID"/>, inclusief persoon, afdelingen, functies, groepswerkjaar
        /// </returns>
        public Lid OphalenViaPersoon(int gelieerdePersoonID, int groepsWerkJaarID)
        {
            using (var db = new ChiroGroepEntities())
            {
                db.Lid.MergeOption = MergeOption.NoTracking;

                var lid = (
                    from t in db.Lid
                    where t.GelieerdePersoon.ID == gelieerdePersoonID
                            &&
                            t.GroepsWerkJaar.ID == groepsWerkJaarID
                    select t).FirstOrDefault();

                if (lid != null)
                {
                    int lidID = lid.ID;

                    if (lid is Kind)
                    {
                        return (
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
                        return (
                            from t in db.Lid.OfType<Leiding>()
                                .Include("GelieerdePersoon.Persoon")
                                .Include("GroepsWerkJaar")
                                .Include("AfdelingsJaar.Afdeling")
                                .Include(leid => leid.Functie)
                            where t.ID == lidID
                            select t).FirstOrDefault();
                    }
                }
                return lid;
            }
        }

        /// <summary>
        /// Haalt hoogstens <paramref name="maxAantal"/> leden op met probeerperiode die voorbij is, 
        /// inclusief persoonsgegevens, adressen, communicatie, functies, afdelingen
        /// </summary>
        /// <param name="maxAantal">max aantal leden op te halen</param>
        /// <returns>alle leden met probeerperiode die voorbij is, inclusief persoonsgegevens, voorkeursadressen,
        /// functies, afdelingen.  Communicatie niet!</returns>
        public IEnumerable<Lid> OverTeZettenOphalen(int maxAantal)
        {
            // We moeten dit apart doen voor leden en leiding, omdat de afdelingen anders geregeld zijn.

            Lid[] resultaat;

            // communicatie wordt hier niet mee opgehaald, want wanneer de info naar kipadmin gaat,
            // moet sowieso de communicatie van mogelijk andere gelieerde personen opnieuw opgezocht
            // worden.

            using (var db = new ChiroGroepEntities())
            {
                var leiding = (from l in db.Lid.OfType<Leiding>()
                            .Include(lei => lei.GroepsWerkJaar.Groep)
                                .Include(lei => lei.GelieerdePersoon.Persoon)
                        .Include(lei => lei.GelieerdePersoon.PersoonsAdres.Adres)
                                .Include(lei => lei.Functie)
                                .Include(lei => lei.AfdelingsJaar.First().OfficieleAfdeling)
                               where !l.NonActief && l.EindeInstapPeriode < DateTime.Now && !l.IsOvergezet && l.GroepsWerkJaar.WerkJaar >= Properties.Settings.Default.MinWerkJaarLidOverzetten
                               select l).Take(maxAantal);

                int resterend = maxAantal - leiding.Count();

                var kinderen = (from l in db.Lid.OfType<Kind>()
                        .Include(kin => kin.GroepsWerkJaar.Groep)
                        .Include(kin => kin.GelieerdePersoon.Persoon)
                        .Include(kin => kin.GelieerdePersoon.PersoonsAdres.Adres)
                                .Include(kin => kin.Functie)
                                .Include(kin => kin.AfdelingsJaar.OfficieleAfdeling)
                                where !l.NonActief && l.EindeInstapPeriode < DateTime.Now && !l.IsOvergezet && l.GroepsWerkJaar.WerkJaar >= Properties.Settings.Default.MinWerkJaarLidOverzetten
                                select l).Take(resterend);

                resultaat = leiding.ToArray().Union<Lid>(kinderen.ToArray()).ToArray();

                GelieerdePersonenDao.VoorkeursAdresKoppelen(db, from l in resultaat select l.GelieerdePersoon);
            }

            return Utility.DetachObjectGraph<Lid>(resultaat);
        }

        /// <summary>
        /// Converteert lidextras <paramref name="extras"/> naar lambda-expresses voor een
        /// KindDao
        /// </summary>
        /// <param name="extras">Te converteren lidextras</param>
        /// <returns>Lambda-expresses voor een KindDao</returns>
        /// <remarks>Van de adressen kunnen straten/gemeentes niet als lambda-expressie worden uitgedrukt,
        /// om wille van het verschil tussen Belgische en buitenlandse adressen.  Om adressen mee op te halen,
        /// is dus altijd extra werk nodig!</remarks>
        internal static Expression<Func<Kind, object>>[] ExtrasNaarLambdasKind(LidExtras extras)
        {
            var paths = ExtrasNaarLambdas<Kind>(extras & ~LidExtras.Afdelingen);

            if ((extras & LidExtras.Afdelingen) != 0)
            {
                paths.Add(ld => ld.AfdelingsJaar.Afdeling.WithoutUpdate());
            }

            return paths.ToArray();
        }

        /// <summary>
        /// Converteert lidextras <paramref name="extras"/> naar lambda-expresses voor een
        /// LeidingDao.
        /// </summary>
        /// <param name="extras">Te converteren lidextras</param>
        /// <returns>Lambda-expresses voor een LeidingDao</returns>
        /// <remarks>Van de adressen kunnen straten/gemeentes niet als lambda-expressie worden uitgedrukt,
        /// om wille van het verschil tussen Belgische en buitenlandse adressen.  Om adressen mee op te halen,
        /// is dus altijd extra werk nodig!</remarks>
        internal static Expression<Func<Leiding, object>>[] ExtrasNaarLambdasLeiding(LidExtras extras)
        {
            var paths = ExtrasNaarLambdas<Leiding>(extras & ~LidExtras.Afdelingen);

            if ((extras & LidExtras.Afdelingen) != 0)
            {
                paths.Add(ld => ld.AfdelingsJaar.First().Afdeling.WithoutUpdate());
            }

            return paths.ToArray();
        }

        /// <summary>
        /// Converteert LidExtra's naar lambda-expressies voor de data-access
        /// </summary>
        /// <param name="extras">Te converteren lidextra's</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Lijst lambda-expressies geschikt voor de LedenDAO</returns>
        /// <remarks>Van de adressen kunnen straten/gemeentes niet als lambda-expressie worden uitgedrukt,
        /// om wille van het verschil tussen Belgische en buitenlandse adressen.  Om adressen mee op te halen,
        /// is dus altijd extra werk nodig!</remarks>
        internal static IList<Expression<Func<T, object>>> ExtrasNaarLambdas<T>(LidExtras extras) where T : Lid
        {
            var paths = new List<Expression<Func<T, object>>> { ld => ld.GroepsWerkJaar.WithoutUpdate() };

            if ((extras & LidExtras.VoorkeurAdres) != 0)
            {
                // enkel voorkeursadres
                paths.Add(ld => ld.GelieerdePersoon.PersoonsAdres.Adres);
            }

            if ((extras & LidExtras.Adressen) != 0)
            {
                // alle adressen
                paths.Add(ld => ld.GelieerdePersoon.Persoon.PersoonsAdres.First().Adres);

                // link naar standaardadres
                paths.Add(ld => ld.GelieerdePersoon.PersoonsAdres.Adres.WithoutUpdate());
            }
            else if ((extras & LidExtras.Persoon) != 0)
            {
                paths.Add(ld => ld.GelieerdePersoon.Persoon);
            }

            if ((extras & LidExtras.Communicatie) != 0)
            {
                paths.Add(ld => ld.GelieerdePersoon.Communicatie.First().CommunicatieType.WithoutUpdate());
            }

            if ((extras & LidExtras.Groep) != 0)
            {
                paths.Add(ld => ld.GroepsWerkJaar.Groep.WithoutUpdate());
            }
            if ((extras & LidExtras.Afdelingen) != 0)
            {
                //// Onderstaande had cool geweest; dan hadden we de generieke <T> niet nodig. 
                //// Maar helaas lukt dat (nog??) niet met AttachObjectGraph:

                // paths.Add(ld => ld is Kind ? (ld as Kind).AfdelingsJaar.Afdeling : ld is Leiding ? (ld as Leiding).AfdelingsJaar.First().Afdeling : null);

                //// Dus:
                throw new NotSupportedException();
            }
            if ((extras & LidExtras.Functies) != 0)
            {
                // FIXME (#116): Hieronder zou 'WithoutUpdate' gebruikt moeten worden, maar owv #116 kan dat nog niet.
                paths.Add(ld => ld.Functie.First());
            }
            if ((extras & LidExtras.AlleAfdelingen) != 0)
            {
                paths.Add(ld => ld.GroepsWerkJaar.AfdelingsJaar.First().Afdeling.WithoutUpdate());
                paths.Add(ld => ld.GroepsWerkJaar.AfdelingsJaar.First().OfficieleAfdeling.WithoutUpdate());
            }
            if ((extras & LidExtras.Verzekeringen) != 0)
            {
                paths.Add(ld => ld.GelieerdePersoon.Persoon.PersoonsVerzekering.First().VerzekeringsType.WithoutUpdate());
            }
            return paths;
        }
    }
}
