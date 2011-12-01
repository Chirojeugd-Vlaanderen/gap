// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. groepen bevat (dat is breder dan 'Chirogroepen', bv. satellieten)
    /// </summary>
    public class GroepenManager
    {
        private readonly IGroepenDao _groepenDao;
        private readonly IGelieerdePersonenDao _gelPersDao;
        private readonly IVeelGebruikt _veelGebruikt;
        private readonly IAutorisatieManager _autorisatieMgr;

        /// <summary>
        /// De standaardconstructor voor GroepenManagers
        /// </summary>
        /// <param name="grpDao">Repository voor groepen</param>
        /// <param name="gelPersDao">Repository voor gelieerde personen</param>
        /// <param name="veelGebruikt">Object dat veel gebruikte items cachet</param>
        /// <param name="autorisatieMgr">Worker die autorisatie regelt</param>
        public GroepenManager(
            IGroepenDao grpDao,
            IGelieerdePersonenDao gelPersDao,
            IVeelGebruikt veelGebruikt,
            IAutorisatieManager autorisatieMgr)
        {
            _groepenDao = grpDao;
            _autorisatieMgr = autorisatieMgr;
            _gelPersDao = gelPersDao;
            _veelGebruikt = veelGebruikt;
        }

        /// <summary>
        /// Verwijdert alle gelieerde personen van de groep met ID <paramref name="groepID"/>.  Probeert ook
        /// de gekoppelde personen te verwijderen, indien <paramref name="verwijderPersonen"/> <c>true</c> is.
        /// Verwijdert ook mogelijke lidobjecten.
        /// PERSISTEERT!
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan je de gelieerde personen wilt verwijderen</param>
        /// <param name="verwijderPersonen">Indien <c>true</c>, worden ook de personen vewijderd waarvoor
        /// een GelieerdePersoon met de groep bestond.</param>
        /// <remarks>Deze functie vereist super-GAV-rechten</remarks>
        public void GelieerdePersonenVerwijderen(int groepID, bool verwijderPersonen)
        {
            if (_autorisatieMgr.IsSuperGav())
            {
                // Alle gelieerde personen van de groep ophalen
                IList<GelieerdePersoon> allePersonen = _gelPersDao.AllenOphalen(
                    groepID, PersoonSorteringsEnum.Naam,
                    PersoonsExtras.AlleLeden);

                // Alle gelieerde personen als 'te verwijderen' markeren
                foreach (GelieerdePersoon gp in allePersonen)
                {
                    gp.TeVerwijderen = true;

                    // Alle leden als 'te verwijderen' markeren
                    foreach (Lid ld in gp.Lid)
                    {
                        ld.TeVerwijderen = true;
                    }

                    // Markeer zo nodig ook de persoon
                    if (verwijderPersonen)
                    {
                        gp.Persoon.TeVerwijderen = true;
                    }
                }

                // Persisteer
                _gelPersDao.Bewaren(allePersonen, gp => gp.Lid, gp => gp.Persoon);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Persisteert groep in de database
        /// </summary>
        /// <param name="g">Te persisteren groep</param>
        /// <param name="paths">Expressies die aangeven welke dependencies mee opgehaald moeten worden</param>
        /// <returns>De bewaarde groep</returns>
        public Groep Bewaren(Groep g, params Expression<Func<Groep, object>>[] paths)
        {
            if (_autorisatieMgr.IsGavGroep(g.ID))
            {
                return _groepenDao.Bewaren(g, paths);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Haalt een groepsobject op zonder gerelateerde entiteiten
        /// </summary>
        /// <param name="groepID">ID van de op te halen groep</param>
        /// <returns>De groep met de opgegeven ID <paramref name="groepID"/></returns>
        public Groep Ophalen(int groepID)
        {
            return Ophalen(groepID, GroepsExtras.Geen);
        }

        /// <summary>
        /// Converteert GroepsExtras <paramref name="extras"/> naar lambda-expresses voor een
        /// GroepenDao
        /// </summary>
        /// <param name="extras">Te converteren GroepsExtras</param>
        /// <returns>Lambda-expresses voor een GroepenDao</returns>
        private static IEnumerable<Expression<Func<Groep, object>>> ExtrasNaarLambdas(GroepsExtras extras)
        {
            var paths = new List<Expression<Func<Groep, object>>>();

            if ((extras & GroepsExtras.GroepsWerkJaren) != 0)
            {
                paths.Add(gr => gr.GroepsWerkJaar);
            }

            if ((extras & GroepsExtras.Categorieen) != 0)
            {
                paths.Add(gr => gr.Categorie);
            }

            if ((extras & GroepsExtras.Functies) != 0)
            {
                paths.Add(gr => gr.Functie);
            }
            return paths;
        }

        /// <summary>
        /// Haalt een groepsobject op
        /// </summary>
        /// <param name="groepID">ID van de op te halen groep</param>
        /// <param name="extras">Geeft aan of er gekoppelde entiteiten mee opgehaald moeten worden.</param>
        /// <returns>De groep met de opgegeven ID <paramref name="groepID"/></returns>
        public Groep Ophalen(int groepID, GroepsExtras extras)
        {
            if (!_autorisatieMgr.IsGavGroep(groepID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            if (extras == GroepsExtras.Geen)
            {
                // Als enkel de groep nodig is, kunnen we dat uit de cache
                // halen.

                return _veelGebruikt.GroepsWerkJaarOphalen(groepID).Groep;
            }
            else
            {
                var paths = ExtrasNaarLambdas(extras);
                return _groepenDao.Ophalen(groepID, paths.ToArray());
            }
        }

        #region categorieën

        /// <summary>
        /// Maakt een nieuwe categorie, en koppelt die aan een bestaande groep (met daaraan
        /// gekoppeld zijn categorieën)
        /// </summary>
        /// <param name="g">Groep waarvoor de categorie gemaakt wordt.  Als bestaande categorieën
        /// gekoppeld zijn, wordt op dubbels gecontroleerd</param>
        /// <param name="categorieNaam">Naam voor de nieuwe categorie</param>
        /// <param name="categorieCode">Code voor de nieuwe categorie</param>
        /// <returns>De toegevoegde categorie</returns>
        public Categorie CategorieToevoegen(Groep g, String categorieNaam, String categorieCode)
        {
            if (!_autorisatieMgr.IsGavGroep(g.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
            else
            {
                // Is er al een categorie met die code?
                Categorie bestaande = (from ctg in g.Categorie
                                       where String.Compare(ctg.Code, categorieCode, true) == 0
                                    || String.Compare(ctg.Naam, categorieNaam, true) == 0
                                       select ctg).FirstOrDefault();

                if (bestaande != null)
                {
                    // TODO (#507): Check op bestaande afdeling door DB
                    // OPM: we krijgen pas een DubbeleEntiteitException op het moment dat we bewaren,
                    // maar hier doen we alleen een .Add
                    throw new BestaatAlException<Categorie>(bestaande);
                }
                else
                {
                    var c = new Categorie();
                    c.Naam = categorieNaam;
                    c.Code = categorieCode;
                    c.Groep = g;
                    g.Categorie.Add(c);
                    return c;
                }
            }
        }

        #endregion categorieën

        /// <summary>
        /// Maakt een nieuwe (groepseigen) functie voor groep <paramref name="g"/>.  Persisteert niet.
        /// </summary>
        /// <param name="g">Groep waarvoor de functie gemaakt wordt, inclusief minstens het recentste werkjaar</param>
        /// <param name="naam">Naam van de functie</param>
        /// <param name="code">Code van de functie</param>
        /// <param name="maxAantal">Maximumaantal leden in de categorie.  Onbeperkt indien null.</param>
        /// <param name="minAantal">Minimumaantal leden in de categorie.</param>
        /// <param name="lidType">LidType waarvoor de functie van toepassing is</param>
        /// <returns>De nieuwe (gekoppelde) functie</returns>
        public Functie FunctieToevoegen(
            Groep g,
            string naam,
            string code,
            int? maxAantal,
            int minAantal,
            LidType lidType)
        {
            if (!_autorisatieMgr.IsGavGroep(g.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            // Controleer op dubbele code
            var bestaande = (from fun in g.Functie
                             where String.Compare(fun.Code, code, true) == 0
                             || String.Compare(fun.Naam, naam, true) == 0
                             select fun).FirstOrDefault();

            if (bestaande != null && bestaande.TeVerwijderen)
            {
                throw new InvalidOperationException(
                    "Er bestaat al een functie met die code, gemarkeerd als TeVerwijderen");
            }
            if (bestaande != null)
            {
                // TODO (#507): Check op bestaande afdeling door DB
                // OPM: we krijgen pas een DubbeleEntiteitException op het moment dat we bewaren,
                // maar hier doen we alleen een .Add
                throw new BestaatAlException<Functie>(bestaande);
            }

            // Zonder problemen hier geraakt.  Dan kunnen we verder.

            Niveau niveau = g.Niveau;
            if ((g.Niveau & Niveau.Groep) != 0)
            {
                if ((lidType & LidType.Leiding) == 0)
                {
                    niveau &= ~(Niveau.LeidingInGroep);
                }
                if ((lidType & LidType.Kind) == 0)
                {
                    niveau &= ~(Niveau.LidInGroep);
                }
            }

            var f = new Functie
            {
                Code = code,
                Groep = g,
                MaxAantal = maxAantal,
                MinAantal = minAantal,
                Niveau = niveau,
                Naam = naam,
                WerkJaarTot = null,
                WerkJaarVan = g.GroepsWerkJaar.OrderByDescending(gwj=>gwj.WerkJaar).First().WerkJaar,
                IsNationaal = false
            };

            g.Functie.Add(f);

            return f;
        }

        /// <summary>
        /// Maakt een nieuw groepswerkjaar voor een gegeven <paramref name="groep" />
        /// </summary>
        /// <param name="groep">Groep waarvoor een groepswerkjaar gemaakt moet worden</param>
        /// <param name="werkJaar">Int die het werkjaar identificeert (bv. 2009 voor 2009-2010)</param>
        /// <returns>Het gemaakte groepswerkjaar.</returns>
        /// <remarks>Persisteert niet.</remarks>
        public GroepsWerkJaar GroepsWerkJaarMaken(Groep groep, int werkJaar)
        {
            var resultaat = new GroepsWerkJaar
            {
                Groep = groep,
                WerkJaar = werkJaar
            };
            groep.GroepsWerkJaar.Add(resultaat);
            return resultaat;
        }

        /// <summary>
        /// Haalt groep op met gegeven stamnummer, incl recentse groepswerkjaar
        /// </summary>
        /// <param name="code">Stamnummer op te halen groep</param>
        /// <returns>Groep met <paramref name="code"/> als stamnummer</returns>
        public Groep Ophalen(string code)
        {
            var resultaat = _groepenDao.Ophalen(code);

            if (_autorisatieMgr.IsSuperGav() || _autorisatieMgr.IsGavGroep(resultaat.ID))
            {
                return resultaat;
            }
            throw new GeenGavException(Properties.Resources.GeenGav);
        }
    }
}
