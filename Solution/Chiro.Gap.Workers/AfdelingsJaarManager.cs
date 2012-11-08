// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
	/// <summary>
    /// Worker die alle businesslogica i.v.m. afdelingsjaren bevat
    /// </summary>
    public class AfdelingsJaarManager : IAfdelingsJaarManager
	{
        private readonly ILedenSync _ledenSync;

        private readonly IVeelGebruikt _veelGebruikt;
        private readonly IAutorisatieManager _autorisatieMgr;

        public AfdelingsJaarManager(
            IVeelGebruikt veelGebruikt,
            IAutorisatieManager autorisatieMgr,
            ILedenSync ledenSync)
        {
            _veelGebruikt = veelGebruikt;
            _autorisatieMgr = autorisatieMgr;
            _ledenSync = ledenSync;
        }

        /// <summary>
        /// Maakt een afdelingsjaar voor een groep en een afdeling, persisteert niet.
        /// </summary>
        /// <param name="a">
        /// Afdeling voor nieuw afdelingsjaar
        /// </param>
        /// <param name="oa">
        /// Te koppelen officiële afdeling
        /// </param>
        /// <param name="gwj">
        /// Groepswerkjaar (koppelt de afdeling aan een groep en een werkJaar)
        /// </param>
        /// <param name="geboorteJaarBegin">
        /// Geboortejaar van
        /// </param>
        /// <param name="geboorteJaarEind">
        /// Geboortejaar tot
        /// </param>
        /// <param name="geslacht">
        /// Bepaalt of de afdeling een jongensafdeling, meisjesafdeling of
        /// gemengde afdeling is.
        /// </param>
        /// <returns>
        /// Het aangemaakte afdelingsjaar
        /// </returns>
        public AfdelingsJaar Aanmaken(
            Afdeling a,
            OfficieleAfdeling oa,
            GroepsWerkJaar gwj,
            int geboorteJaarBegin,
            int geboorteJaarEind,
            GeslachtsType geslacht)
        {
            if (!_autorisatieMgr.IsGavAfdeling(a.ID) || !_autorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if (gwj.Groep.ID != a.ChiroGroep.ID)
            {
                throw new FoutNummerException(
                    FoutNummer.AfdelingNietVanGroep,
                    Resources.AfdelingNietVanGroep);
            }

            // Leden moeten minstens in het 1ste leerjaar zitten voor we ze inschrijven.
            // De maximumleeftijd is arbitrair nattevingerwerk. :-)
            if (!(gwj.WerkJaar - geboorteJaarEind >= Settings.Default.MinLidLeefTijd)
                || !(gwj.WerkJaar - geboorteJaarBegin <= Settings.Default.MaxLidLeefTijd)
                || !(geboorteJaarBegin <= geboorteJaarEind))
            {
                throw new ValidatieException(Resources.OngeldigeGeboortejarenVoorAfdeling,
                                             FoutNummer.FouteGeboortejarenVoorAfdeling);
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
	    /// De afdelingen van het gegeven lid worden aangepast van whatever momenteel zijn afdelingen zijn naar
	    /// de gegeven lijst nieuwe afdelingen.
	    /// Een kind mag maar 1 afdeling hebben, voor een leider staan daar geen constraints op.
	    /// Persisteert, want ingeval van leiding kan het zijn dat er links lid-&gt;afdelingsjaar moeten 
	    /// verdwijnen.
	    /// </summary>
	    /// <param name="l">
	    /// Lid, geladen met groepswerkjaar met afdelingsjaren
	    /// </param>
	    /// <param name="afdelingsJaren">
	    /// De afdelingsjaren waarvan het kind lid is
	    /// </param>
	    /// <returns>
	    /// Lidobject met gekoppeld(e) afdelingsja(a)r(en)
	    /// </returns>
	    public Lid Vervangen(Lid l, IEnumerable<AfdelingsJaar> afdelingsJaren)
	    {
	        throw new NotImplementedException(NIEUWEBACKEND.Info);
	    }

	    /// <summary>
        /// De gegeven <paramref name="leden"/> worden toegevoegd naar
        /// de gegeven lijst nieuwe afdelingen.  Eventuele koppelingen met bestaande afdelingen worden
        /// verwijderd.
        /// <para>
        /// </para>
        /// Een kind mag maar 1 afdeling hebben, voor een leider staan daar geen constraints op.
        /// Persisteert, want ingeval van leiding kan het zijn dat er links lid-&gt;afdelingsjaar moeten 
        /// verdwijnen.
        /// </summary>
        /// <param name="leden">
        /// Leden, geladen met groepswerkjaar met afdelingsjaren
        /// </param>
        /// <param name="afdelingsJaren">
        /// De afdelingsjaren waaraan de leden gekoppeld moeten worden
        /// </param>
        /// <returns>
        /// Lidobjecten met gekoppeld(e) afdelingsja(a)r(en)
        /// </returns>
        public IEnumerable<Lid> Vervangen(IEnumerable<Lid> leden, IEnumerable<AfdelingsJaar> afdelingsJaren)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
//            var groepsWerkJaren = (from l in leden
//                                   select l.GroepsWerkJaar).Distinct().ToArray();

//            Debug.Assert(groepsWerkJaren.Count() == 1);
//            Debug.Assert(groepsWerkJaren.First() != null);
//            Debug.Assert(groepsWerkJaren.First().Groep != null);

//            int werkJaar = groepsWerkJaren.First().WerkJaar;

//            IEnumerable<Lid> resultaat;

//            var alleLidIDs = (from l in leden select l.ID).Distinct();
//            var mijnLidIDs = _autorisatieMgr.EnkelMijnLeden(from l in leden select l.ID);

//            if (alleLidIDs.Count() > mijnLidIDs.Count())
//            {
//                throw new GeenGavException(Resources.GeenGav);
//            }
//            else if (groepsWerkJaren.First().ID !=
//                     _veelGebruikt.GroepsWerkJaarOphalen(groepsWerkJaren.First().Groep.ID).ID)
//            {
//                throw new FoutNummerException(
//                    FoutNummer.GroepsWerkJaarNietBeschikbaar,
//                    Resources.GroepsWerkJaarVoorbij);
//            }

//            var probleemgevallen = from aj in afdelingsJaren
//                                   where aj.GroepsWerkJaar.ID != groepsWerkJaren.First().ID
//                                   select aj;

//            if (probleemgevallen.FirstOrDefault() != null)
//            {
//                throw new FoutNummerException(
//                    FoutNummer.AfdelingNietVanGroep,
//                    Resources.FoutieveGroepAfdeling);
//            }

//#if KIPDORP
//            using (var tx = new TransactionScope())
//            {
//#endif

//            foreach (var kind in leden.OfType<Kind>())
//            {
//                if (afdelingsJaren.Count() != 1)
//                {
//                    throw new NotSupportedException("Slechts 1 afdeling per kind.");
//                }

//                if (kind.AfdelingsJaar.ID != afdelingsJaren.First().ID)
//                {
//                    // afdeling moet verwijderd worden.
//                    afdelingsJaren.First().Kind.Add(kind);
//                    kind.AfdelingsJaar = afdelingsJaren.First();
//                }
//            }

//            var bewaaardeKinderen = _kindDao.Bewaren(leden.OfType<Kind>(), knd => knd.AfdelingsJaar);

//            foreach (var leiding in leden.OfType<Leiding>())
//            {
//                // Verwijder ontbrekende afdelingen;
//                var teVerwijderenAfdelingen = from aj in leiding.AfdelingsJaar
//                                              where !afdelingsJaren.Any(aj2 => aj2.ID == aj.ID)
//                                              select aj;

//                foreach (var aj in teVerwijderenAfdelingen)
//                {
//                    aj.TeVerwijderen = true;
//                }

//                // Ken nieuwe afdelingen toe
//                var nieuweAfdelingen = from aj in afdelingsJaren
//                                       where !leiding.AfdelingsJaar.Any(aj2 => aj2.ID == aj.ID)
//                                       select aj;

//                foreach (var aj in nieuweAfdelingen)
//                {
//                    leiding.AfdelingsJaar.Add(aj);
//                    aj.Leiding.Add(leiding);
//                }
//            }

//            // WithoutUpdate mag niet in dit geval, omdat anders te verwijderen afdelingsjaren niet
//            // verwijderd worden.
//            // Dit is een bug in AttachObjectGraph. (#116)
//            var bewaardeLeiding = _leidingDao.Bewaren(leden.OfType<Leiding>(), ldng => ldng.AfdelingsJaar.First());

//            resultaat = bewaaardeKinderen.Union<Lid>(bewaardeLeiding);

//            foreach (var l in resultaat)
//            {
//                _ledenSync.AfdelingenUpdaten(l);
//            }

//#if KIPDORP
//                tx.Complete();
//            }
//#endif
//            return resultaat;
        }
    }
}