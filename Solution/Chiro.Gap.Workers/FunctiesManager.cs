// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Transactions;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Struct die gebruikt wordt om van een functie max aantal leden, min aantal leden en totaal aantal
    /// leden te stockeren.
    /// </summary>
    public struct Telling
    {
        public int ID;
        public int Aantal;
        public int? Max;
        public int Min;
    }

    /// <summary>
    /// Businesslogica ivm functies
    /// </summary>
    public class FunctiesManager
    {
        private readonly IFunctiesDao _funDao;
        private readonly ILedenDao _ledenDao;
        private readonly IGroepsWerkJaarDao _groepsWjDao;
        private readonly IVeelGebruikt _veelGebruikt;
        private readonly IAutorisatieManager _autorisatieMgr;

        private readonly ILedenSync _ledenSync;

        /// <summary>
        /// Instantieert een FunctiesManager-object
        /// </summary>
        /// <param name="funDao">
        /// Een dao voor data access mbt functies
        /// </param>
        /// <param name="ledenDao">
        /// Een dao voor data access mbt leden
        /// </param>
        /// <param name="gwjDao">
        /// Data access object voor groepswerkjaren
        /// </param>
        /// <param name="veelGebruikt">
        /// Object dat veel gebruikte items cachet
        /// </param>
        /// <param name="auMgr">
        /// Een IAutorisatieManager voor de autorisatie
        /// </param>
        /// <param name="ledenSync">
        /// Wordt gebruikt om lidinformatie te syncen naar kipadmin
        /// </param>
        public FunctiesManager(
            IFunctiesDao funDao,
            ILedenDao ledenDao,
            IGroepsWerkJaarDao gwjDao,
            IVeelGebruikt veelGebruikt,
            IAutorisatieManager auMgr,
            ILedenSync ledenSync)
        {
            _funDao = funDao;
            _ledenDao = ledenDao;
            _groepsWjDao = gwjDao;
            _veelGebruikt = veelGebruikt;
            _autorisatieMgr = auMgr;

            _ledenSync = ledenSync;
        }

        /// <summary>
        /// Persisteert de gegeven <paramref name="functie"/> in de database, samen met zijn koppeling
        /// naar groep.
        /// </summary>
        /// <param name="functie">
        /// Te persisteren functie
        /// </param>
        /// <returns>
        /// De bewaarde functie
        /// </returns>
        public Functie Bewaren(Functie functie)
        {
            if (NationaalBepaaldeFunctiesOphalen().Contains(functie)
                || functie.IsNationaal
                || !_autorisatieMgr.IsGavGroep(functie.Groep.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }
            else
            {
                return _funDao.Bewaren(functie, fn => fn.Groep.WithoutUpdate());
            }
        }

        /// <summary>
        /// Haalt 1 functie op, samen met de gekoppelde groep
        /// </summary>
        /// <param name="functieID">
        /// ID op te halen functie
        /// </param>
        /// <returns>
        /// De opgehaalde functie met de gekoppelde groep
        /// </returns>
        public Functie Ophalen(int functieID)
        {
            return Ophalen(new[] { functieID }).FirstOrDefault();
        }

        /// <summary>
        /// Een functie ophalen op basis van de ID, samen met de gekoppelde leden
        /// </summary>
        /// <param name="functieID">
        /// ID op te halen functie
        /// </param>
        /// <param name="metHannekesNest">
        /// Bij <c>true</c> worden leden, personen, groepswerkjaar en groep mee opgehaald
        /// </param>
        /// <returns>
        /// De opgehaalde functie met de gekoppelde leden
        /// </returns>
        public Functie Ophalen(int functieID, bool metHannekesNest)
        {
            if (_autorisatieMgr.IsGavFunctie(functieID))
            {
                if (metHannekesNest)
                {
                    return _funDao.Ophalen(
                        functieID,
                        fnc => fnc.Groep,
                        fie => fie.Lid.First().GroepsWerkJaar,
                        fie => fie.Lid.First().GelieerdePersoon.Persoon);
                }
                else
                {
                    return _funDao.Ophalen(functieID);
                }
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Haalt een lijstje functies op, uiteraard met gekoppelde groepen (indien van toepassing)
        /// </summary>
        /// <param name="functieIDs">
        /// ID's op te halen functies
        /// </param>
        /// <returns>
        /// Lijst opgehaalde functies, met gekoppelde groepen (indien van toepassing)
        /// </returns>
        public IList<Functie> Ophalen(IEnumerable<int> functieIDs)
        {
            var resultaat = _funDao.Ophalen(functieIDs, fn => fn.Groep);
            var groepIDs = (from fn in resultaat
                            where fn.Groep != null
                            select fn.Groep.ID).Distinct();

            if (groepIDs.Any(id => !_autorisatieMgr.IsGavGroep(id)))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            return resultaat.ToList();
        }

        /// <summary>
        /// Haalt alle functies op die mogelijk toegekend kunnen worden aan een lid uit het groepswerkjaar
        /// bepaald door <paramref name="groepsWerkJaarID"/> en van het type <paramref name="lidType"/>.
        /// </summary>
        /// <param name="groepsWerkJaarID">
        /// ID van het groepswerkjaar waarvoor de relevante functies gevraagd
        /// zijn.
        /// </param>
        /// <param name="lidType">
        /// <c>LidType.Kind</c> of <c>LidType.Leiding</c>
        /// </param>
        /// <returns>
        /// Lijst met functies die mogelijk toegekend kunnen worden aan een lid uit het groepswerkjaar
        /// bepaald door <paramref name="groepsWerkJaarID"/> en van het type <paramref name="lidType"/>.
        /// </returns>
        public IList<Functie> OphalenRelevant(int groepsWerkJaarID, LidType lidType)
        {
            if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            GroepsWerkJaar gwj = _groepsWjDao.Ophalen(groepsWerkJaarID, grwj => grwj.Groep.Functie);

            // bepaal het niveau van de nationale functies
            Niveau niveau = gwj.Groep.Niveau;
            if ((niveau & Niveau.Groep) != 0)
            {
                if ((lidType & LidType.Leiding) == 0)
                {
                    niveau &= ~Niveau.LeidingInGroep;
                }

                if ((lidType & LidType.Kind) == 0)
                {
                    niveau &= ~Niveau.LidInGroep;
                }
            }

            return (from f in gwj.Groep.Functie.Union(NationaalBepaaldeFunctiesOphalen())
                    where (f.WerkJaarVan == null || f.WerkJaarVan <= gwj.WerkJaar)
                          && (f.WerkJaarTot == null || f.WerkJaarTot >= gwj.WerkJaar)
                          && ((f.Niveau & niveau) != 0)
                    select f).ToList();
        }

        /// <summary>
        /// Kent de meegegeven <paramref name="functies"/> toe aan het gegeven <paramref name="lid"/>.
        /// Als het lid al andere functies had, blijven die behouden.  Persisteert niet.
        /// </summary>
        /// <param name="lid">
        /// Lid dat de functies moet krijgen, gekoppeld aan zijn groep
        /// </param>
        /// <param name="functies">
        /// Rij toe te kennen functies
        /// </param>
        /// <remarks>
        /// Er wordt verondersteld dat er heel wat geladen is!
        /// - lid.groepswerkjaar.groep
        /// - lid.functie
        /// - voor elke functie:
        ///  - functie.lid (voor leden van dezelfde groep)
        ///  - functie.groep
        /// </remarks>
        public void Toekennen(Lid lid, IEnumerable<Functie> functies)
        {
            Debug.Assert(lid.GroepsWerkJaar != null);
            Debug.Assert(lid.GroepsWerkJaar.Groep != null);

            if (!_autorisatieMgr.IsGavLid(lid.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if (!_groepsWjDao.IsRecentste(lid.GroepsWerkJaar.ID))
            {
                throw new FoutNummerException(
                    FoutNummer.GroepsWerkJaarNietBeschikbaar,
                    Resources.GroepsWerkJaarVoorbij);
            }

            // Eerst alle checks, zodat als er ergens een exceptie optreedt, er geen enkele
            // functie wordt toegekend.

            foreach (Functie f in functies)
            {
                if (!_autorisatieMgr.IsGavFunctie(f.ID))
                {
                    throw new GeenGavException(Resources.GeenGav);
                }

                if (!f.IsNationaal && f.Groep.ID != lid.GroepsWerkJaar.Groep.ID)
                {
                    throw new FoutNummerException(
                        FoutNummer.FunctieNietVanGroep,
                        Resources.FoutieveGroepFunctie);
                }

                if (f.WerkJaarTot < lid.GroepsWerkJaar.WerkJaar // false als wjtot null
                    || f.WerkJaarVan > lid.GroepsWerkJaar.WerkJaar)
                {
                    // false als wjvan null
                    throw new FoutNummerException(
                        FoutNummer.FunctieNietBeschikbaar,
                        Resources.FoutiefGroepsWerkJaarFunctie);
                }

                if ((f.Niveau & lid.Niveau) == 0)
                {
                    throw new InvalidOperationException(Resources.FoutiefLidType);
                }

                // Ik test hier bewust niet of er niet te veel leden zijn met de functie;
                // op die manier worden inconsistenties bij het veranderen van functies toegelaten,
                // wat me voor de UI makkelijker lijkt.  De method 'AantallenControleren' kan
                // te allen tijde gebruikt worden om problemen met functieaantallen op te sporen.
            }

            // Alle checks goed overleefd; als we nog niet uit de method 'gethrowd' zijn, kunnen we
            // nu de functies toekennen.

            foreach (var f in functies)
            {
                // Lokale variabele om "Access to modified closure" te vermijden [wiki:VeelVoorkomendeWaarschuwingen#Accesstomodifiedclosure]
                Functie f1 = f;
                if ((from fnc in lid.Functie where fnc.ID == f1.ID select fnc).FirstOrDefault() == null)
                {
                    lid.Functie.Add(f);
                }

                if ((from ld in f.Lid where ld.ID == lid.ID select ld).FirstOrDefault() == null)
                {
                    f.Lid.Add(lid);
                }
            }
        }

        /// <summary>
        /// Koppelt de functies met ID's <paramref name="functieIDs"/> los van het lid
        /// <paramref name="lid"/>.  PERSISTEERT.
        /// </summary>
        /// <param name="lid">
        /// Lid waarvan functies losgekoppeld moeten worden
        /// </param>
        /// <param name="functieIDs">
        /// ID's van de los te koppelen functies
        /// </param>
        /// <returns>
        /// Het lidobject met daaraan gekoppeld de overblijvende functies
        /// </returns>
        /// <remarks>
        /// * Functie-ID's van functies die niet aan het lid gekoppeld zijn, worden genegeerd.
        /// * Er wordt verwacht dat voor elke te verwijderen functie alle leden met groepswerkjaar geladen zijn
        /// * Er wordt niet echt losgekoppeld; de koppeling lid-functie wordt op 'te verwijderen'
        ///  gezet.  (Wat wil zeggen dat verwijderen via het lid moet gebeuren, en niet via de functie)
        /// (Dat systeem met 'teVerwijderen' is eigenlijk toch verre van ideaal.)
        /// </remarks>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Moet niet gemarkeerd worden door ReSharper/StyleCop")]
        public Lid LosKoppelen(Lid lid, IEnumerable<int> functieIDs)
        {
            if (!_autorisatieMgr.IsGavLid(lid.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            var losTeKoppelen = from fun in lid.Functie
                                where functieIDs.Contains(fun.ID)
                                select fun;

            foreach (Functie f in losTeKoppelen)
            {
                // Ik test hier bewust niet of er niet te weinig leden zijn met de functie;
                // op die manier worden inconsistenties bij het veranderen van functies toegelaten,
                // wat me voor de UI makkelijker lijkt.  De method 'AantallenControleren' kan
                // te allen tijde gebruikt worden om problemen met functieaantallen op te sporen.

                // De essentie van deze prachtige method:
                f.TeVerwijderen = true;
            }

            return _ledenDao.Bewaren(lid, ld => ld.Functie);
        }

        /// <summary>
        /// Vervangt de functies van het lid <paramref name="lid"/> door de functies in 
        /// <paramref name="functies"/>.  Persisteert.
        /// </summary>
        /// <param name="lid">
        /// Lid waarvan de functies vervangen moeten worden
        /// </param>
        /// <param name="functies">
        /// Nieuwe lijst functies
        /// </param>
        /// <returns>
        /// Het <paramref name="lid"/> met daaraan gekoppeld de nieuwe functies
        /// </returns>
        /// <remarks>
        /// Aan <paramref name="lid"/>moeten de huidige functies gekoppeld zijn
        /// </remarks>
        public Lid Vervangen(Lid lid, IEnumerable<Functie> functies)
        {
            Lid resultaat;

            // In deze method zitten geen checks op GAV-schap, juiste werkjaar,... dat gebeurt al in
            // 'Toekennen' en 'Loskoppelen', dewelke door deze method worden aangeroepen.
            IList<Functie> toeTeVoegen = (from fn in functies
                                          where !lid.Functie.Contains(fn)
                                          select fn).ToList();
            IList<int> teVerwijderen = (from fn in lid.Functie
                                        where !functies.Contains(fn)
                                        select fn.ID).ToList();

#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
            Toekennen(lid, toeTeVoegen);
            resultaat = LosKoppelen(lid, teVerwijderen); // LosKoppelen persisteert

            _ledenSync.FunctiesUpdaten(lid);
#if KIPDORP
				tx.Complete();
			}
#endif
            return resultaat;
        }

        /// <summary>
        /// Verwijdert een functie (PERSISTEERT!)
        /// </summary>
        /// <param name="functie">
        /// Te verwijderen functie, 
        ///  inclusief groep, leden en groepswerkjaar leden
        /// </param>
        /// <param name="forceren">
        /// Indien <c>true</c> wordt de functie ook verwijderd als er
        ///  dit werkjaar personen met de gegeven functie zijn.  Anders krijg je een exception.
        /// </param>
        /// <returns>
        /// <c>Null</c> als de functie effectief verwijderd is, anders het functie-object met
        /// aangepast 'werkjaartot'.
        /// </returns>
        /// <remarks>
        /// Als de functie geen leden meer bevat na verwijdering van die van het huidige werkjaar,
        /// dan wordt ze verwijderd.  Zo niet, wordt er een stopdatum op geplakt.
        /// </remarks>
        public Functie Verwijderen(Functie functie, bool forceren)
        {
            if (!_autorisatieMgr.IsGavFunctie(functie.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            // Leden moeten gekoppeld zijn
            // (null verschilt hier expliciet van een lege lijst)
            Debug.Assert(functie.Lid != null);

            int huidigGwjID = _veelGebruikt.GroepsWerkJaarOphalen(functie.Groep.ID).ID;

            var metFunctieDitJaar = from ld in functie.Lid
                                    where ld.GroepsWerkJaar.ID == huidigGwjID
                                    select ld;

            if (!forceren && metFunctieDitJaar.FirstOrDefault() != null)
            {
                throw new BlokkerendeObjectenException<Lid>(
                    metFunctieDitJaar,
                    metFunctieDitJaar.Count(),
                    Resources.FunctieNietLeeg);
            }

            foreach (var ld in metFunctieDitJaar)
            {
                ld.TeVerwijderen = true; // markeer link lid->functie als te verwdrn
            }

            var metFunctieVroeger = from ld in functie.Lid
                                    where ld.GroepsWerkJaar.ID != huidigGwjID
                                    select ld;

            if (metFunctieVroeger.FirstOrDefault() == null)
            {
                functie.TeVerwijderen = true;
            }
            else
            {
                functie.WerkJaarTot = _veelGebruikt.GroepsWerkJaarOphalen(functie.Groep.ID).WerkJaar - 1;
            }

            return _funDao.Bewaren(functie, fn => fn.Lid);
        }

        /// <summary>
        /// Kijkt na voor een gegeven <paramref name="groepsWerkJaar"/> of de maximum- en
        /// minimumaantallen van de functies (eigen en nationaal bepaald) niet overschreden zijn.
        /// </summary>
        /// <param name="groepsWerkJaar">
        /// Te controleren werkjaar
        /// </param>
        /// <returns>
        /// Een lijst met tellingsgegevens voor de functies waar de aantallen niet kloppen.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Deze functie is zich niet bewust van de aanwezigheid van een database, en verwacht
        /// dat groepsWerkJaar.Groep.Functie en groepsWerkJaar.Lid[i].Functie geladen zijn.
        /// </para>
        /// </remarks>
        public IEnumerable<Telling> AantallenControleren(GroepsWerkJaar groepsWerkJaar)
        {
            var eigenFuncties = from fn in groepsWerkJaar.Groep.Functie select fn;

            return AantallenControleren(
                groepsWerkJaar,
                eigenFuncties.Union(NationaalBepaaldeFunctiesOphalen()));
        }

        /// <summary>
        /// Kijkt na voor een gegeven <paramref name="groepsWerkJaar"/> of de maximum- en
        /// minimumaantallen van de functies <paramref name="functies"/> niet overschreden zijn.
        /// </summary>
        /// <param name="groepsWerkJaar">
        /// Te controleren werkjaar
        /// </param>
        /// <param name="functies">
        /// Functies waarop te controleren
        /// </param>
        /// <returns>
        /// Een lijst met tellingsgegevens voor de functies waar de aantallen niet kloppen.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Deze functie is zich niet bewust van de aanwezigheid van een database, en verwacht
        /// dat groepsWerkJaar.Lid[i].Functie geladen is.
        /// </para>
        /// <para>
        /// Functies in <paramref name="functies"/> waar geen groep aan gekoppeld is, worden als
        /// nationaal bepaalde functies beschouwd.
        /// </para>
        /// <para>
        /// Functies die niet geldig zijn in het gevraagde groepswerkjaar, worden genegeerd
        /// </para>
        /// </remarks>
        public IEnumerable<Telling> AantallenControleren(
            GroepsWerkJaar groepsWerkJaar,
            IEnumerable<Functie> functies)
        {
            if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaar.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }
            else
            {
                var toegekendeFuncties =
                    groepsWerkJaar.Lid.SelectMany(ld => ld.Functie)
                        .Distinct()
                        .Where(fn => functies.Contains(fn));

                var nietToegekendeFuncties = from fn in functies
                                             where !toegekendeFuncties.Contains(fn)
                                                   && (fn.IsNationaal || fn.Groep == groepsWerkJaar.Groep)
                                             // bovenstaande vermijdt groepsvreemde functies
                                             select fn;

                // toegekende functies waarvan er te veel of te weinig zijn
                var problemenToegekendeFuncties =
                    from fn in toegekendeFuncties
                    where (fn.WerkJaarVan == null || fn.WerkJaarVan <= groepsWerkJaar.WerkJaar)
                          && (fn.WerkJaarTot == null || fn.WerkJaarTot >= groepsWerkJaar.WerkJaar)
                          && (fn.Lid.Count() > fn.MaxAantal // geeft false als maxaant == null
                              || fn.Lid.Count() < fn.MinAantal)
                    // geeft false als minaant null
                    select new Telling
                               {
                                   ID = fn.ID,
                                   Aantal = fn.Lid.Count(),
                                   Max = fn.MaxAantal,
                                   Min = fn.MinAantal
                               };

                // niet-toegekende functies waarvan er te weinig zijn
                var problemenOntbrekendeFuncties =
                    from fn in nietToegekendeFuncties
                    where (fn.WerkJaarVan == null || fn.WerkJaarVan <= groepsWerkJaar.WerkJaar)
                          && (fn.WerkJaarTot == null || fn.WerkJaarTot >= groepsWerkJaar.WerkJaar)
                          && fn.MinAantal > 0
                    select new Telling
                               {
                                   ID = fn.ID,
                                   Aantal = 0,
                                   Max = fn.MaxAantal,
                                   Min = fn.MinAantal
                               };

                return problemenToegekendeFuncties.Union(problemenOntbrekendeFuncties).ToArray();
            }
        }

        /// <summary>
        /// Geeft de nationaal bepaalde functies terug
        /// </summary>
        /// <returns>
        /// De rij nationaal bepaalde functies
        /// </returns>
        public IEnumerable<Functie> NationaalBepaaldeFunctiesOphalen()
        {
            return _veelGebruikt.NationaleFunctiesOphalen();
        }
    }
}