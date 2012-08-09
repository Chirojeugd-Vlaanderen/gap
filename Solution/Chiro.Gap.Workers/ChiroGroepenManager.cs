// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Linq;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. Chirogroepen bevat
    /// </summary>
    public class ChiroGroepenManager
    {
        private readonly IChiroGroepenDao _dao;
        private readonly IAutorisatieManager _autorisatieMgr;

        /// <summary>
        /// Creëert een ChiroGroepenManager
        /// </summary>
        /// <param name="dao">
        /// Repository voor Chirogroepen
        /// </param>
        /// <param name="autorisatieMgr">
        /// Regelt de autorisatie
        /// </param>
        public ChiroGroepenManager(IChiroGroepenDao dao, IAutorisatieManager autorisatieMgr)
        {
            _dao = dao;
            _autorisatieMgr = autorisatieMgr;
        }

        /// <summary>
        /// Haalt een groepsobject op
        /// </summary>
        /// <param name="groepID">
        /// ID van de op te halen groep
        /// </param>
        /// <param name="extras">
        /// Geeft aan of er gekoppelde entiteiten mee opgehaald moeten worden.
        /// </param>
        /// <returns>
        /// De Chirogroep met de opgegeven ID <paramref name="groepID"/>
        /// </returns>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker geen GAV is voor de groep met de opgegeven <paramref name="groepID"/>
        /// </exception>
        public ChiroGroep Ophalen(int groepID, ChiroGroepsExtras extras)
        {
            if (!_autorisatieMgr.IsGavGroep(groepID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            return _dao.Ophalen(groepID, extras);
        }

        /// <summary>
        /// Bewaart de Chirogroep <paramref name="chiroGroep"/>, met daaraan gekoppeld de
        /// gegeven <paramref name="extras"/>.
        /// </summary>
        /// <param name="chiroGroep">
        /// Te bewaren Chirogroep
        /// </param>
        /// <param name="extras">
        /// Bepaalt de mee te bewaren gekoppelde entiteiten
        /// </param>
        /// <returns>
        /// De Chirogroep met - als alles goed ging - de bijgewerkte waarden
        /// </returns>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker geen GAV is voor de opgegeven <paramref name="chiroGroep"/>
        /// </exception>
        public ChiroGroep Bewaren(ChiroGroep chiroGroep, ChiroGroepsExtras extras)
        {
            if (!_autorisatieMgr.IsGavGroep(chiroGroep.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            return _dao.Bewaren(chiroGroep, extras);
        }

        /// <summary>
        /// Maakt een nieuwe afdeling voor een Chirogroep, zonder te persisteren
        /// </summary>
        /// <param name="groep">
        /// Chirogroep waarvoor afdeling moet worden gemaakt, met daaraan gekoppeld
        /// de bestaande afdelingen
        /// </param>
        /// <param name="naam">
        /// Naam van de afdeling
        /// </param>
        /// <param name="afkorting">
        /// Handige afkorting voor in schemaatjes
        /// </param>
        /// <returns>
        /// De toegevoegde (maar nog niet gepersisteerde) afdeling
        /// </returns>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker geen GAV is voor de opgegeven <paramref name="groep"/>
        /// </exception>
        public Afdeling AfdelingToevoegen(ChiroGroep groep, string naam, string afkorting)
        {
            if (!_autorisatieMgr.IsGavGroep(groep.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            // Controleren of de afdeling nog niet bestaat
            var bestaand = from afd in groep.Afdeling
                           where string.Compare(afd.Afkorting, afkorting, true) == 0
                                 || string.Compare(afd.Naam, naam, true) == 0
                           select afd;

            if (bestaand.FirstOrDefault() != null)
            {
                // TODO (#507): Check op bestaande afdeling door DB
                throw new BestaatAlException<Afdeling>(bestaand.FirstOrDefault());
            }

            var a = new Afdeling
                        {
                            Afkorting = afkorting, 
                            Naam = naam
                        };

            a.ChiroGroep = groep;
            groep.Afdeling.Add(a);

            return a;
        }
    }
}