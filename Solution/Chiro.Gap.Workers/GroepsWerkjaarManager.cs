// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. groepswerkjaren bevat
    /// </summary>
    public class GroepsWerkJaarManager : IGroepsWerkJaarManager
    {
        private readonly IGroepsWerkJaarDao _groepsWjDao;
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IAfdelingenDao _afdelingenDao;

        private readonly IVeelGebruikt _veelGebruikt;

        /// <summary>
        /// Creëert een GroepsWerkJaarManager
        /// </summary>
        /// <param name="groepsWjDao">
        /// Repository voor groepswerkjaren
        /// </param>
        /// <param name="afdelingenDao">
        /// Repository voor afdelingen
        /// </param>
        /// <param name="veelGebruikt">
        /// Object dat veel gebruikte items cachet
        /// </param>
        /// <param name="autorisatieMgr">
        /// Worker die autorisatie regelt
        /// </param>
        public GroepsWerkJaarManager(
            IGroepsWerkJaarDao groepsWjDao,
            IAfdelingenDao afdelingenDao,
            IVeelGebruikt veelGebruikt,
            IAutorisatieManager autorisatieMgr)
        {
            _groepsWjDao = groepsWjDao;
            _autorisatieMgr = autorisatieMgr;
            _afdelingenDao = afdelingenDao;
            _veelGebruikt = veelGebruikt;
        }

        /// <summary>
        /// Haalt het groepswerkjaar op bij een gegeven <paramref name="groepsWerkJaarID"/>
        /// </summary>
        /// <param name="groepsWerkJaarID">
        /// ID van het gevraagde GroepsWerkJaar
        /// </param>
        /// <param name="extras">
        /// Bepaalt op te halen gerelateerde entiteiten
        /// </param>
        /// <returns>
        /// Gevraagde groepswerkjaar
        /// </returns>
        public GroepsWerkJaar Ophalen(int groepsWerkJaarID, GroepsWerkJaarExtras extras)
        {
            if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            GroepsWerkJaar resultaat = _groepsWjDao.Ophalen(
                groepsWerkJaarID,
                ExtrasNaarLambdas(extras));

            return resultaat;
        }

        /// <summary>
        /// Haalt de afdelingen van een groep op die niet gebruikt zijn in een gegeven 
        /// groepswerkjaar, op basis van een <paramref name="groepsWerkJaarID"/>
        /// </summary>
        /// <param name="groepsWerkJaarID">
        /// ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
        /// opgezocht moeten worden.
        /// </param>
        /// <returns>
        /// De ongebruikte afdelingen van een groep in het gegeven groepswerkjaar
        /// </returns>
        public IList<Afdeling> OngebruikteAfdelingenOphalen(int groepsWerkJaarID)
        {
            return _afdelingenDao.OngebruikteOphalen(groepsWerkJaarID);
        }

        /// <summary>
        /// Haalt recentste groepswerkjaar voor een groep op.
        /// </summary>
        /// <param name="groepID">
        /// ID gevraagde groep
        /// </param>
        /// <returns>
        /// Het recentste Groepswerkjaar voor de opgegeven groep
        /// </returns>
        public GroepsWerkJaar RecentsteOphalen(int groepID)
        {
            return RecentsteOphalen(groepID, GroepsWerkJaarExtras.Geen);
        }

        /// <summary>
        /// Haalt recentste groepswerkjaar voor een groep op.
        /// </summary>
        /// <param name="groepID">
        /// ID gevraagde groep
        /// </param>
        /// <param name="extras">
        /// Bepaalt eventuele mee op te halen gekoppelde entiteiten
        /// </param>
        /// <returns>
        /// Het recentste Groepswerkjaar voor de opgegeven groep
        /// </returns>
        public GroepsWerkJaar RecentsteOphalen(int groepID, GroepsWerkJaarExtras extras)
        {
            if (!(_autorisatieMgr.IsSuperGav() || _autorisatieMgr.IsGavGroep(groepID)))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if ((extras & ~GroepsWerkJaarExtras.Groep) == 0)
            {
                // Als er niets meer wordt opgevraagd dan het recentste groepswerkjaar
                // en de groep, haal dan uit de cache ipv uit de db.
                return _veelGebruikt.GroepsWerkJaarOphalen(groepID);
            }
            else
            {
                return _groepsWjDao.RecentsteOphalen(groepID, ExtrasNaarLambdas(extras));
            }
        }

        /// <summary>
        /// Haalt recentste groepswerkjaar voor een groep op.
        /// </summary>
        /// <param name="code">
        /// Stamnummer van de gevraagde groep
        /// </param>
        /// <param name="extras">
        /// Bepaalt eventuele mee op te halen gekoppelde entiteiten
        /// </param>
        /// <returns>
        /// Het recentste Groepswerkjaar voor de opgegeven groep
        /// </returns>
        public GroepsWerkJaar RecentsteOphalen(string code, GroepsWerkJaarExtras extras)
        {
            return RecentsteOphalen(_veelGebruikt.CodeNaarGroepID(code), extras);
        }

        /// <summary>
        /// Bepaalt ID van het recentste GroepsWerkJaar gemaakt voor een
        /// gegeven groep.
        /// </summary>
        /// <param name="groepID">
        /// ID van Groep
        /// </param>
        /// <returns>
        /// ID van het recentste GroepsWerkJaar
        /// </returns>
        public int RecentsteGroepsWerkJaarIDGet(int groepID)
        {
            if (!_autorisatieMgr.IsGavGroep(groepID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            return _veelGebruikt.GroepsWerkJaarOphalen(groepID).ID;
        }

        /// <summary>
        /// Converteert de GroepsWerkJaarExtras <paramref name="extras"/> naar lambda-expressies die mee naar 
        /// de data access moeten om de extra's daadwerkelijk op te halen.
        /// </summary>
        /// <param name="extras">
        /// Te converteren groepsextra's
        /// </param>
        /// <returns>
        /// Lambda-expressies geschikt voor onze DAO's
        /// </returns>
        private static Expression<Func<GroepsWerkJaar, object>>[] ExtrasNaarLambdas(GroepsWerkJaarExtras extras)
        {
            var paths = new List<Expression<Func<GroepsWerkJaar, object>>>();

            if ((extras & GroepsWerkJaarExtras.Afdelingen) != 0)
            {
                paths.Add(gwj => gwj.AfdelingsJaar.First().Afdeling);
                paths.Add(gwj => gwj.AfdelingsJaar.First().OfficieleAfdeling.WithoutUpdate());
            }

            if ((extras & GroepsWerkJaarExtras.LidFuncties) != 0)
            {
                paths.Add(gwj => gwj.Lid.First().Functie);
                paths.Add(gwj => gwj.AfdelingsJaar.First().Kind.First().Functie);
                paths.Add(gwj => gwj.AfdelingsJaar.First().Leiding.First().Functie);
            }
            else if ((extras & GroepsWerkJaarExtras.Leden) != 0)
            {
                paths.Add(gwj => gwj.AfdelingsJaar.First().Kind);
                paths.Add(gwj => gwj.AfdelingsJaar.First().Leiding);
            }

            if ((extras & GroepsWerkJaarExtras.GroepsFuncties) != 0)
            {
                paths.Add(gwj => gwj.Groep.Functie);
            }
            else if ((extras & GroepsWerkJaarExtras.Groep) != 0)
            {
                paths.Add(gwj => gwj.Groep);
            }

            return paths.ToArray();
        }

        /// <summary>
        /// Berekent de theoretische einddatum van het gegeven groepswerkjaar.
        /// </summary>
        /// <param name="groepsWerkJaar">
        /// Groepswerkjaar, met daaraan gekoppeld een werkjaarobject
        /// </param>
        /// <returns>
        /// Einddatum van het gekoppelde werkJaar.
        /// </returns>
        public static DateTime EindDatum(GroepsWerkJaar groepsWerkJaar)
        {
            DateTime wjStart = Settings.Default.WerkjaarStartNationaal;
            return new DateTime(groepsWerkJaar.WerkJaar + 1, wjStart.Month, wjStart.Day).AddDays(-1);
        }

        /// <summary>
        /// Maakt een nieuw groepswerkjaar in het gevraagde werkJaar.
        /// Persisteert niet ;-P
        /// </summary>
        /// <param name="g">
        /// De groep waarvoor een groepswerkjaar aangemaakt moet worden
        /// </param>
        /// <returns>
        /// Het nieuwe groepswerkjaar
        /// </returns>
        /// <throws>OngeldigObjectException</throws>
        public GroepsWerkJaar VolgendGroepsWerkJaarMaken(Groep g)
        {
            if (!_autorisatieMgr.IsGavGroep(g.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            // Bereken gewenste werkJaar
            var werkJaar = NieuweWerkJaar();

            // Controle op dubbels moet gebeuren door data access.  (Zie #507)
            return new GroepsWerkJaar { Groep = g, WerkJaar = werkJaar };
        }

        /// <summary>
        /// Berekent wat het nieuwe werkJaar zal zijn als op deze moment de jaarovergang zou gebeuren.
        /// </summary>
        /// <returns>
        /// Het jaar waarin dat nieuwe werkJaar begint
        /// </returns>
        public int NieuweWerkJaar()
        {
            // Bereken gewenste werkJaar

            // Het algoritme kijkt het volgende na:
            // Stel dat de jaarovergang mogelijk wordt vanaf 1 augustus.
            // Als vandaag voor 1 augustus is, dan is het werkJaar vorig jaar begonnen => huidig jaar - 1
            // Als vandaag 1 augustus of later is, dan begint het werkJaar dit kalenderjaar => huidig jaar.
            int werkJaar;

            var startdate = new DateTime(
                DateTime.Today.Year,
                Settings.Default.BeginOvergangsPeriode.Month,
                Settings.Default.BeginOvergangsPeriode.Day);

            if (DateTime.Today < startdate)
            {
                // vroeger
                werkJaar = DateTime.Today.Year - 1;
            }
            else
            {
                werkJaar = DateTime.Today.Year;
            }

            return werkJaar;
        }

        /// <summary>
        /// Bepaalt de datum vanaf wanneer het volgende werkJaar begonnen kan worden
        /// </summary>
        /// <param name="werkJaar">
        /// Jaartal van het 'huidige' werkJaar (i.e. 2010 voor 2010-2011 enz)
        /// </param>
        /// <returns>
        /// Datum in het gegeven werkJaar vanaf wanneer het nieuwe aangemaakt mag worden
        /// </returns>
        public DateTime StartOvergang(int werkJaar)
        {
            var datum = Settings.Default.BeginOvergangsPeriode;
            return new DateTime(werkJaar + 1, datum.Month, datum.Day);
        }

        /// <summary>
        /// Persisteert een groepswerkjaar in de database
        /// </summary>
        /// <param name="gwj">
        /// Te persisteren groepswerkjaar, gekoppeld aan de groep
        /// </param>
        /// <param name="groepsWerkJaarExtras">
        /// Bepaalt welke gerelateerde entiteiten mee gepersisteerd
        /// moeten worden
        /// </param>
        /// <returns>
        /// Het gepersisteerde groepswerkjaar, met eventuele nieuwe ID's
        /// </returns>
        public GroepsWerkJaar Bewaren(GroepsWerkJaar gwj, GroepsWerkJaarExtras groepsWerkJaarExtras)
        {
            if (!_autorisatieMgr.IsGavGroep(gwj.Groep.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            try
            {
                _veelGebruikt.GroepsWerkJaarResetten(gwj.Groep.ID);
                return _groepsWjDao.Bewaren(gwj, ExtrasNaarLambdas(groepsWerkJaarExtras));
            }
            catch (DubbeleEntiteitException<GroepsWerkJaar>)
            {
                throw new BestaatAlException<GroepsWerkJaar>(gwj);
            }
        }
    }
}