// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. afdelingsjaren bevat
    /// </summary>
    public class AfdelingsJaarManager
    {
        private readonly IAfdelingsJarenDao _afdJarenDao;
        private readonly IAfdelingenDao _afdelingenDao;
        private readonly IGroepsWerkJaarDao _groepsWjDao;
        private readonly IKindDao _kindDao;
        private readonly ILeidingDao _leidingDao;
        private readonly ILedenSync _ledenSync;

        private readonly IVeelGebruikt _veelGebruikt;
        private readonly IAutorisatieManager _autorisatieMgr;

        /// <summary>
        /// Deze constructor laat toe om een alternatieve repository voor
        /// de groepen te gebruiken.  Nuttig voor mocking en testing.
        /// </summary>
        /// <param name="ajDao">Zorgt voor data-access ivm afdelnigsjaren</param>
        /// <param name="afdDao">Zorgt voor afdelingsgerelateerde data-access</param>
        /// <param name="gwjDao">Zorgt voor groepswerkjaargerelateerde data-access</param>
        /// <param name="kindDao">Zorgt voor kindgerelateerde data-access</param>
        /// <param name="leidingDao">Zorgt voor leidinggerelateerde data-access</param>
        /// <param name="veelGebruikt">Object om veelgebruikte zaken mee op te halen (via cache)</param>
        /// <param name="autorisatieMgr">Alternatieve autorisatiemanager</param>
        /// <param name="ledenSync">Interface voor sync van lidgegevens</param>
        public AfdelingsJaarManager(
            IAfdelingsJarenDao ajDao,
            IAfdelingenDao afdDao,
            IGroepsWerkJaarDao gwjDao,
            IKindDao kindDao,
            ILeidingDao leidingDao,
            IVeelGebruikt veelGebruikt,
            IAutorisatieManager autorisatieMgr,
            ILedenSync ledenSync)
        {
            _afdJarenDao = ajDao;
            _afdelingenDao = afdDao;
            _groepsWjDao = gwjDao;
            _kindDao = kindDao;
            _leidingDao = leidingDao;
            _veelGebruikt = veelGebruikt;
            _autorisatieMgr = autorisatieMgr;
            _ledenSync = ledenSync;
        }

        /// <summary>
        /// Maakt een afdelingsjaar voor een groep en een afdeling, persisteert niet.
        /// </summary>
        /// <param name="a">Afdeling voor nieuw afdelingsjaar</param>
        /// <param name="oa">Te koppelen officiële afdeling</param>
        /// <param name="gwj">Groepswerkjaar (koppelt de afdeling aan een groep en een werkjaar)</param>
        /// <param name="geboorteJaarBegin">Geboortejaar van</param>
        /// <param name="geboorteJaarEind">Geboortejaar tot</param>
        /// <param name="geslacht">Bepaalt of de afdeling een jongensafdeling, meisjesafdeling of
        /// gemengde afdeling is.</param>
        /// <returns>Het aangemaakte afdelingsjaar</returns>
        public AfdelingsJaar Aanmaken(
            Afdeling a,
            OfficieleAfdeling oa,
            GroepsWerkJaar gwj,
            int geboorteJaarBegin,
            int geboorteJaarEind,
            GeslachtsType geslacht)
        {
            if (!_autorisatieMgr.IsGavAfdeling(a.ID) || !_autorisatieMgr.IsGavGroepsWerkJaar((gwj.ID)))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            if (gwj.Groep.ID != a.ChiroGroep.ID)
            {
                throw new FoutNummerException(
                    FoutNummer.AfdelingNietVanGroep,
                    Properties.Resources.AfdelingNietVanGroep);
            }

            // Leden moeten minstens in het 1ste leerjaar zitten voor we ze inschrijven.
            // De maximumleeftijd is arbitrair nattevingerwerk. :-)
            if (!(gwj.WerkJaar - geboorteJaarEind >= Properties.Settings.Default.MinLidLeefTijd)
                || !(gwj.WerkJaar - geboorteJaarBegin <= Properties.Settings.Default.MaxLidLeefTijd)
                || !(geboorteJaarBegin <= geboorteJaarEind))
            {
                throw new ValidatieException(Properties.Resources.OngeldigeGeboortejarenVoorAfdeling, FoutNummer.FouteGeboortejarenVoorAfdeling);
            }

            var afdelingsJaar = new AfdelingsJaar
                                    {
                                        OfficieleAfdeling = oa,
                                        Afdeling = a,
                                        GroepsWerkJaar = gwj,
                                        GeboorteJaarVan = geboorteJaarBegin,
                                        GeboorteJaarTot = geboorteJaarEind,
                                        Geslacht = geslacht
                                    };

            // TODO check if no conflicts with existing afdelingsjaar

            a.AfdelingsJaar.Add(afdelingsJaar);
            oa.AfdelingsJaar.Add(afdelingsJaar);
            gwj.AfdelingsJaar.Add(afdelingsJaar);

            return afdelingsJaar;
        }

        /// <summary>
        /// Op basis van een ID een afdelingsjaar ophalen
        /// </summary>
        /// <param name="afdelingsJaarID">De ID van het afdelingsjaar</param>
        /// <param name="extras">Bepaalt welke gerelateerde entiteiten mee opgehaald moeten worden.</param>
        /// <returns>Het afdelingsjaar met de opgegeven ID</returns>
        public AfdelingsJaar Ophalen(int afdelingsJaarID, AfdelingsJaarExtras extras)
        {
            var paths = new List<Expression<Func<AfdelingsJaar, object>>>();

            if ((extras & AfdelingsJaarExtras.Afdeling) != 0)
            {
                paths.Add(aj => aj.Afdeling);
            }
            if ((extras & AfdelingsJaarExtras.GroepsWerkJaar) != 0)
            {
                paths.Add(aj => aj.GroepsWerkJaar);
            }
            if ((extras & AfdelingsJaarExtras.Leden) != 0)
            {
                paths.Add(aj => aj.Kind);
                paths.Add(aj => aj.Leiding);
            }
            if ((extras & AfdelingsJaarExtras.OfficieleAfdeling) != 0)
            {
                paths.Add(aj => aj.OfficieleAfdeling);
            }

            if (_autorisatieMgr.IsGavAfdelingsJaar(afdelingsJaarID))
            {
                return _afdJarenDao.Ophalen(afdelingsJaarID, paths.ToArray());
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Verwijdert AfdelingsJaar uit database
        /// </summary>
        /// <param name="afdelingsJaarID">ID van het AfdelingsJaar</param>
        /// <returns><c>True</c> on successful</returns>
        public bool Verwijderen(int afdelingsJaarID)
        {
            AfdelingsJaar aj = _afdJarenDao.Ophalen(afdelingsJaarID, a => a.Afdeling, a => a.Leiding, a => a.Kind);

            if (!_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            if (aj.Kind.Count != 0 || aj.Leiding.Count != 0)
            {
                throw new InvalidOperationException(Properties.Resources.AfdelingsJaarBevatLeden);
            }

            aj.TeVerwijderen = true;
            _afdJarenDao.Bewaren(aj);
            return true;
        }

        /// <summary>
        /// Het opgegeven afdelingsjaar opslaan
        /// </summary>
        /// <param name="aj">Het afdelingsjaar dat opgeslagen moet worden</param>
        public void Bewaren(Afdeling aj)
        {
            if (!_autorisatieMgr.IsGavAfdeling(aj.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            _afdelingenDao.Bewaren(aj);
        }

        /// <summary>
        /// Het opgegeven afdelingsjaar opslaan
        /// </summary>
        /// <param name="aj">Het afdelingsjaar dat opgeslagen moet worden</param>
        public void Bewaren(AfdelingsJaar aj)
        {
            if (_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
            {
                _afdJarenDao.Bewaren(aj);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Wijzigt de property's van <paramref name="afdelingsJaar"/>
        /// </summary>
        /// <param name="afdelingsJaar">Te wijzigen afdelingsjaar</param>
        /// <param name="officieleAfdeling">Officiele afdeling</param>
        /// <param name="geboorteJaarVan">Ondergrens geboortejaar</param>
        /// <param name="geboorteJaarTot">Bovengrens geboortejaar</param>
        /// <param name="geslachtsType">Jongensafdeling, meisjesafdeling of gemengde afdeling</param>
        /// <param name="versieString">Versiestring uit database voor concurrency controle</param>
        /// <remarks>Groepswerkjaar en afdeling kunnen niet gewijzigd worden.  Verwijder hiervoor het
        /// afdelingsjaar, en maak een nieuw.</remarks>
        public void Wijzigen(
            AfdelingsJaar afdelingsJaar,
            OfficieleAfdeling officieleAfdeling,
            int geboorteJaarVan,
            int geboorteJaarTot,
            GeslachtsType geslachtsType,
            string versieString)
        {
            if (_autorisatieMgr.IsGavAfdelingsJaar(afdelingsJaar.ID))
            {
                if (officieleAfdeling.ID != afdelingsJaar.OfficieleAfdeling.ID)
                {
                    // vervang officiele afdeling.  Een beetje gepruts om in de mate van het
                    // mogelijke de state van de entity's consistent te houden.

                    // verwijder link vorige off. afdeling - afdelingsjaar
                    afdelingsJaar.OfficieleAfdeling.AfdelingsJaar.Remove(afdelingsJaar);

                    // nieuwe link
                    afdelingsJaar.OfficieleAfdeling = officieleAfdeling;
                    officieleAfdeling.AfdelingsJaar.Add(afdelingsJaar);
                }
                afdelingsJaar.GeboorteJaarVan = geboorteJaarVan;
                afdelingsJaar.GeboorteJaarTot = geboorteJaarTot;
                afdelingsJaar.Geslacht = geslachtsType;
                afdelingsJaar.VersieString = versieString;
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Haalt een afdeling op, op basis van <paramref name="afdelingID"/>
        /// </summary>
        /// <param name="afdelingID">ID van op te halen afdeling</param>
        /// <returns>De gevraagde afdeling</returns>
        public Afdeling AfdelingOphalen(int afdelingID)
        {
            if (_autorisatieMgr.IsGavAfdeling(afdelingID))
            {
                return _afdelingenDao.Ophalen(afdelingID);
            }
            throw new GeenGavException(Properties.Resources.GeenGav);
        }

        /// <summary>
        /// Haalt lijst officiële afdelingen op.
        /// </summary>
        /// <returns>Lijst officiële afdelingen</returns>
        public IList<OfficieleAfdeling> OfficieleAfdelingenOphalen()
        {
            // Iedereen heeft het recht deze op te halen.
            return _afdelingenDao.OfficieleAfdelingenOphalen();
        }

        /// <summary>
        /// Haalt een officiele afdeling op, op basis van zijn ID
        /// </summary>
        /// <param name="officieleAfdelingID">ID van de op te halen officiele afdeling</param>
        /// <returns>Opgehaalde officiele afdeling</returns>
        public OfficieleAfdeling OfficieleAfdelingOphalen(int officieleAfdelingID)
        {
            return _afdelingenDao.OfficieleAfdelingOphalen(officieleAfdelingID);
        }

        /// <summary>
        /// De afdelingen van het gegeven lid worden aangepast van whatever momenteel zijn afdelingen zijn naar
        /// de gegeven lijst nieuwe afdelingen.
        /// Een kind mag maar 1 afdeling hebben, voor een leider staan daar geen constraints op.
        /// Persisteert, want ingeval van leiding kan het zijn dat er links lid->afdelingsjaar moeten 
        /// verdwijnen.
        /// </summary>
        /// <param name="l">Lid, geladen met groepswerkjaar met afdelingsjaren</param>
        /// <param name="afdelingsJaren">De afdelingsjaren waarvan het kind lid is</param>
        /// <returns>Lidobject met gekoppeld(e) afdelingsja(a)r(en)</returns>
        public Lid Vervangen(Lid l, IEnumerable<AfdelingsJaar> afdelingsJaren)
        {
            return Vervangen(new Lid[] { l }, afdelingsJaren).FirstOrDefault();
        }

        /// <summary>
        /// De gegeven <paramref name="leden"/> worden toegevoegd naar
        /// de gegeven lijst nieuwe afdelingen.  Eventuele koppelingen met bestaande afdelingen worden
        /// verwijderd.
        /// 
        /// Een kind mag maar 1 afdeling hebben, voor een leider staan daar geen constraints op.
        /// Persisteert, want ingeval van leiding kan het zijn dat er links lid->afdelingsjaar moeten 
        /// verdwijnen.
        /// </summary>
        /// <param name="leden">Leden, geladen met groepswerkjaar met afdelingsjaren</param>
        /// <param name="afdelingsJaren">De afdelingsjaren waaraan de leden gekoppeld moeten worden</param>
        /// <returns>Lidobjecten met gekoppeld(e) afdelingsja(a)r(en)</returns>
        public IEnumerable<Lid> Vervangen(IEnumerable<Lid> leden, IEnumerable<AfdelingsJaar> afdelingsJaren)
        {
            var groepsWerkJaren = (from l in leden
                                   select l.GroepsWerkJaar).Distinct().ToArray();

            Debug.Assert(groepsWerkJaren.Count() == 1);
            Debug.Assert(groepsWerkJaren.First() != null);
            Debug.Assert(groepsWerkJaren.First().Groep != null);

            IEnumerable<Lid> resultaat;

            var alleLidIDs = (from l in leden select l.ID).Distinct();
            var mijnLidIDs = _autorisatieMgr.EnkelMijnLeden(from l in leden select l.ID);

            if (alleLidIDs.Count() > mijnLidIDs.Count())
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
            else if (groepsWerkJaren.First().ID != _veelGebruikt.GroepsWerkJaarOphalen(groepsWerkJaren.First().Groep.ID).ID)
            {
                throw new FoutNummerException(
                    FoutNummer.GroepsWerkJaarNietBeschikbaar,
                    Properties.Resources.GroepsWerkJaarVoorbij);
            }

            var probleemgevallen = from aj in afdelingsJaren
                                   where aj.GroepsWerkJaar.ID != groepsWerkJaren.First().ID
                                   select aj;

            if (probleemgevallen.FirstOrDefault() != null)
            {
                throw new FoutNummerException(
                    FoutNummer.AfdelingNietVanGroep,
                    Properties.Resources.FoutieveGroepAfdeling);
            }
#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif

                foreach (var kind in leden.OfType<Kind>())
                {
                    if (afdelingsJaren.Count() != 1)
                    {
                        throw new NotSupportedException("Slechts 1 afdeling per kind.");
                    }

                    if (kind.AfdelingsJaar.ID != afdelingsJaren.First().ID)
                    {
                        // afdeling moet verwijderd worden.

                        afdelingsJaren.First().Kind.Add(kind);
                        kind.AfdelingsJaar = afdelingsJaren.First();
                    }
                }

                var bewaaardeKinderen = _kindDao.Bewaren(leden.OfType<Kind>(), knd => knd.AfdelingsJaar);

                foreach (var leiding in leden.OfType<Leiding>())
                {
                    // Verwijder ontbrekende afdelingen;
                    var teVerwijderenAfdelingen = from aj in leiding.AfdelingsJaar
                                                  where !afdelingsJaren.Any(aj2 => aj2.ID == aj.ID)
                                                  select aj;

                    foreach (var aj in teVerwijderenAfdelingen)
                    {
                        aj.TeVerwijderen = true;
                    }

                    // Ken nieuwe afdelingen toe
                    var nieuweAfdelingen = from aj in afdelingsJaren
                                           where !leiding.AfdelingsJaar.Any(aj2 => aj2.ID == aj.ID)
                                           select aj;

                    foreach (var aj in nieuweAfdelingen)
                    {
                        leiding.AfdelingsJaar.Add(aj);
                        aj.Leiding.Add(leiding);
                    }
                }

                // WithoutUpdate mag niet in dit geval, omdat anders te verwijderen afdelingsjaren niet
                // verwijderd worden.
                // Dit is een bug in AttachObjectGraph. (#116)

                var bewaardeLeiding = _leidingDao.Bewaren(leden.OfType<Leiding>(), ldng => ldng.AfdelingsJaar.First());

                resultaat = bewaaardeKinderen.Union<Lid>(bewaardeLeiding);

                foreach (var l in resultaat.Where(ld => ld.IsOvergezet))
                {
                    _ledenSync.AfdelingenUpdaten(l);
                }
#if KIPDORP
                tx.Complete();
            }
#endif
            return resultaat;
        }
    }
}
