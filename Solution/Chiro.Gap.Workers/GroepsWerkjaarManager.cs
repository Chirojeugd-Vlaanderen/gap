// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
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
                extras);

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
                return _groepsWjDao.RecentsteOphalen(groepID, extras);
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
        /// Berekent de theoretische einddatum van het gegeven groepswerkjaar.
        /// </summary>
        /// <param name="groepsWerkJaar">
        /// Groepswerkjaar, met daaraan gekoppeld een werkjaarobject
        /// </param>
        /// <returns>
        /// Einddatum van het gekoppelde werkJaar.
        /// </returns>
        public DateTime EindDatum(GroepsWerkJaar groepsWerkJaar)
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

            // Bereken gewenste werkjaar
            var werkJaar = NieuweWerkJaar(g.ID);

            // Controle op dubbels moet gebeuren door data access.  (Zie #507)
            return new GroepsWerkJaar { Groep = g, WerkJaar = werkJaar };
        }

        /// <summary>
        /// Stelt afdelingsjaren voor voor de gegeven <paramref name="groep"/> en <paramref name="afdelingen"/>
        /// in het werkjaar <paramref name="nieuwWerkJaar"/> - <paramref name="nieuwWerkJaar"/>+1.
        /// </summary>
        /// <param name="groep">Groep waarvoor afdelingsjaren moeten worden voorgesteld, met daaraan gekoppeld
        /// het huidige groepswerkjaar, de huidige afdelingsjaren, en alle beschikbare afdelingen.</param>
        /// <param name="afdelingen">Afdelingen waarvoor afdelingsjaren moeten worden voorgesteld</param>
        /// <param name="nieuwWerkJaar">Bepaalt het werkjaar waarvoor de afdelingsjaren voorgesteld moeten worden.</param>
        /// <returns>Lijstje afdelingsjaren</returns>
        public IList<AfdelingsJaar> AfdelingsJarenVoorstellen(ChiroGroep groep, Afdeling[] afdelingen, int nieuwWerkJaar)
        {
            if (!_autorisatieMgr.IsGavGroep(groep.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            if (_autorisatieMgr.EnkelMijnAfdelingen(afdelingen.Select(afd => afd.ID)).Count() < afdelingen.Count())
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            if (afdelingen.FirstOrDefault(afd => afd.ChiroGroep.ID != groep.ID) != null)
            {
                throw new FoutNummerException(FoutNummer.AfdelingNietVanGroep, Properties.Resources.AfdelingNietVanGroep);
            }

            var huidigWerkJaar = groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).FirstOrDefault();

            if (huidigWerkJaar == null)
            {
                // Eigenlijk gaan we ervan uit dat elke groep al wel een afdelingsjaar heeft.  Maar
                // moest het toch niet zo zijn, dan geven we gauw een domme suggestie terug

                // Als officiële afdeling, geven we ribbels, om te vermijden dat de groepen te snel
                // 'speciaal' zouden kiezen.
                var ribbels = _afdelingenDao.OfficieleAfdelingOphalen((int) NationaleAfdeling.Ribbels);

                return (from afd in afdelingen
                        select
                            new AfdelingsJaar
                                {
                                    Afdeling = afd,
                                    OfficieleAfdeling = ribbels,
                                    Geslacht = GeslachtsType.Gemengd
                                }).ToList();
            }

            var werkJarenVerschil = nieuwWerkJaar - huidigWerkJaar.WerkJaar;

            // We halen de afdelingsjaren van het huidige (oude) werkjaar op, zodat we op basis daarvan geboortejaren
            // en geslacht voor de nieuwe afdelingsjaren in het nieuwe werkjaar kunnen voorstellen.

            var huidigeAfdelingsJaren = huidigWerkJaar.AfdelingsJaar;

            // Creeer een voorstel voor de nieuwe afdelingsjaren

            var nieuweAfdelingsJaren = new List<AfdelingsJaar>();

            foreach (var afdeling in afdelingen)
            {
                // geboortejaren en geslacht gewoon default values, passen we zo nodig
                // straks nog aan.

                var afdelingsJaar = new AfdelingsJaar
                    {Afdeling = afdeling, GeboorteJaarVan = 0, GeboorteJaarTot = 0, Geslacht = GeslachtsType.Gemengd};


                nieuweAfdelingsJaren.Add(afdelingsJaar);

                // Als de afdeling dit jaar al actief was, kunnen we de details automatisch bepalen

                var bestaandAfdelingsJaar = (from aj in huidigeAfdelingsJaren
                                             where aj.Afdeling.ID == afdeling.ID
                                             select aj).FirstOrDefault();

                if (bestaandAfdelingsJaar != null)
                {
                    afdelingsJaar.OfficieleAfdeling = bestaandAfdelingsJaar.OfficieleAfdeling;
                    afdelingsJaar.Geslacht = bestaandAfdelingsJaar.Geslacht;
                    afdelingsJaar.GeboorteJaarTot = bestaandAfdelingsJaar.GeboorteJaarTot + werkJarenVerschil;
                    afdelingsJaar.GeboorteJaarVan = bestaandAfdelingsJaar.GeboorteJaarVan + werkJarenVerschil;
                }
                else
                {
                    // Als officiële afdeling, geven we ribbels, om te vermijden dat de groepen te snel
                    // 'speciaal' zouden kiezen.
                    var ribbels = _afdelingenDao.OfficieleAfdelingOphalen((int)NationaleAfdeling.Ribbels);
                    afdelingsJaar.OfficieleAfdeling = ribbels;
                    afdelingsJaar.Geslacht = GeslachtsType.Gemengd;
                    afdelingsJaar.GeboorteJaarTot = nieuwWerkJaar - ribbels.LeefTijdVan;
                    afdelingsJaar.GeboorteJaarVan = nieuwWerkJaar - ribbels.LeefTijdTot;
                }
            }

            // Sorteer de afdelingsjaren: eerst die zonder geboortejaren, dan van jong naar oud
            var resultaat = (from a in nieuweAfdelingsJaren
                                orderby a.GeboorteJaarTot descending
                                orderby a.GeboorteJaarTot == 0 descending
                                select a).ToArray();
            return resultaat;
        }


        /// <summary>
        /// Bepaalt of in het gegeven <paramref name='werkJaar' /> op
	    /// het gegeven <paramref name='tijdstip' /> de jaarovergang al
	    /// kan doorgaan.
        /// </summary>
        /// <param name="tijdstip"> </param>
        /// <param name="werkJaar">
        /// Jaartal van het 'huidige' werkjaar (i.e. 2010 voor 2010-2011 enz)
        /// </param>
        /// <returns>
        /// Datum in het gegeven werkjaar vanaf wanneer het nieuwe aangemaakt mag worden
        /// </returns>
        public bool OvergangMogelijk(DateTime tijdstip, int werkJaar)
        {
#if JAAROVERGANGDEBUG
            return true;
#endif
            return tijdstip >= StartOvergang(werkJaar);
        }

        /// <summary>
        /// Berekent wat het nieuwe werkjaar zal zijn als op deze moment de jaarovergang zou gebeuren.
        /// </summary>
        /// <returns>
        /// Het jaar waarin dat nieuwe werkJaar begint
        /// </returns>
        /// <remarks>De paramter <paramref name="groepID"/> is er enkel voor debugging purposes</remarks>
        public int NieuweWerkJaar(int groepID)
        {
#if JAAROVERGANGDEBUG
            return RecentsteOphalen(groepID).WerkJaar+1;
#endif
            // Bereken gewenste werkjaar

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
                return _groepsWjDao.Bewaren(gwj, groepsWerkJaarExtras);
            }
            catch (DubbeleEntiteitException<GroepsWerkJaar>)
            {
                throw new BestaatAlException<GroepsWerkJaar>(gwj);
            }
        }

        /// <summary>
        /// Controleert of de datum <paramref name="dateTime"/> zich in het werkJaar <paramref name="p"/> bevindt.
        /// </summary>
        /// <param name="dateTime">
        /// Te controleren datum
        /// </param>
        /// <param name="p">
        /// Werkjaar.  (2010 voor 2010-2011 enz.)
        /// </param>
        /// <returns>
        /// <c>True</c> als <paramref name="dateTime"/> zich in het werkJaar bevindt; anders <c>false</c>.
        /// </returns>
        public bool DatumInWerkJaar(DateTime dateTime, int p)
        {
            var werkJaarStart = new DateTime(
                p,
                Settings.Default.WerkjaarStartNationaal.Month,
                Settings.Default.WerkjaarStartNationaal.Day);

            DateTime werkJaarStop = new DateTime(
                p + 1,
                Settings.Default.WerkjaarStartNationaal.Month,
                Settings.Default.WerkjaarStartNationaal.Day).AddDays(-1);

            return werkJaarStart <= dateTime && dateTime <= werkJaarStop;
        }
    }
}
