// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        private readonly IVeelGebruikt _veelGebruikt;
        private readonly IAutorisatieManager _autorisatieMgr;

        private readonly ILedenSync _ledenSync;

        public FunctiesManager(
            IVeelGebruikt veelGebruikt,
            IAutorisatieManager auMgr,
            ILedenSync ledenSync)
        {
            _veelGebruikt = veelGebruikt;
            _autorisatieMgr = auMgr;

            _ledenSync = ledenSync;
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
            throw new NotImplementedException(NIEUWEBACKEND.Info);
            //if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
            //{
            //    throw new GeenGavException(Resources.GeenGav);
            //}

            //GroepsWerkJaar gwj = _groepsWjDao.Ophalen(groepsWerkJaarID, grwj => grwj.Groep.Functie);

            //// bepaal het niveau van de nationale functies
            //Niveau niveau = gwj.Groep.Niveau;
            //if ((niveau & Niveau.Groep) != 0)
            //{
            //    if ((lidType & LidType.Leiding) == 0)
            //    {
            //        niveau &= ~Niveau.LeidingInGroep;
            //    }

            //    if ((lidType & LidType.Kind) == 0)
            //    {
            //        niveau &= ~Niveau.LidInGroep;
            //    }
            //}

            //return (from f in gwj.Groep.Functie.Union(NationaalBepaaldeFunctiesOphalen())
            //        where (f.WerkJaarVan == null || f.WerkJaarVan <= gwj.WerkJaar)
            //              && (f.WerkJaarTot == null || f.WerkJaarTot >= gwj.WerkJaar)
            //              && ((f.Niveau & niveau) != 0)
            //        select f).ToList();
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
            throw new NotImplementedException(NIEUWEBACKEND.Info);
            //Debug.Assert(lid.GroepsWerkJaar != null);
            //Debug.Assert(lid.GroepsWerkJaar.Groep != null);

            //if (!_autorisatieMgr.IsGavLid(lid.ID))
            //{
            //    throw new GeenGavException(Resources.GeenGav);
            //}

            //if (!_groepsWjDao.IsRecentste(lid.GroepsWerkJaar.ID))
            //{
            //    throw new FoutNummerException(
            //        FoutNummer.GroepsWerkJaarNietBeschikbaar,
            //        Resources.GroepsWerkJaarVoorbij);
            //}

            //// Eerst alle checks, zodat als er ergens een exceptie optreedt, er geen enkele
            //// functie wordt toegekend.

            //foreach (Functie f in functies)
            //{
            //    if (!_autorisatieMgr.IsGavFunctie(f.ID))
            //    {
            //        throw new GeenGavException(Resources.GeenGav);
            //    }

            //    if (!f.IsNationaal && f.Groep.ID != lid.GroepsWerkJaar.Groep.ID)
            //    {
            //        throw new FoutNummerException(
            //            FoutNummer.FunctieNietVanGroep,
            //            Resources.FoutieveGroepFunctie);
            //    }

            //    if (f.WerkJaarTot < lid.GroepsWerkJaar.WerkJaar // false als wjtot null
            //        || f.WerkJaarVan > lid.GroepsWerkJaar.WerkJaar)
            //    {
            //        // false als wjvan null
            //        throw new FoutNummerException(
            //            FoutNummer.FunctieNietBeschikbaar,
            //            Resources.FoutiefGroepsWerkJaarFunctie);
            //    }

            //    if ((f.Niveau & lid.Niveau) == 0)
            //    {
            //        throw new InvalidOperationException(Resources.FoutiefLidType);
            //    }

            //    // Ik test hier bewust niet of er niet te veel leden zijn met de functie;
            //    // op die manier worden inconsistenties bij het veranderen van functies toegelaten,
            //    // wat me voor de UI makkelijker lijkt.  De method 'AantallenControleren' kan
            //    // te allen tijde gebruikt worden om problemen met functieaantallen op te sporen.
            //}

            //// Alle checks goed overleefd; als we nog niet uit de method 'gethrowd' zijn, kunnen we
            //// nu de functies toekennen.

            //foreach (var f in functies)
            //{
            //    // Lokale variabele om "Access to modified closure" te vermijden [wiki:VeelVoorkomendeWaarschuwingen#Accesstomodifiedclosure]
            //    Functie f1 = f;
            //    if ((from fnc in lid.Functie where fnc.ID == f1.ID select fnc).FirstOrDefault() == null)
            //    {
            //        lid.Functie.Add(f);
            //    }

            //    if ((from ld in f.Lid where ld.ID == lid.ID select ld).FirstOrDefault() == null)
            //    {
            //        f.Lid.Add(lid);
            //    }
            //}
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
            throw new NotImplementedException(NIEUWEBACKEND.Info);
//            Lid resultaat;

//            // In deze method zitten geen checks op GAV-schap, juiste werkJaar,... dat gebeurt al in
//            // 'Toekennen' en 'Loskoppelen', dewelke door deze method worden aangeroepen.
//            IList<Functie> toeTeVoegen = (from fn in functies
//                                          where !lid.Functie.Contains(fn)
//                                          select fn).ToList();
//            IList<int> teVerwijderen = (from fn in lid.Functie
//                                        where !functies.Contains(fn)
//                                        select fn.ID).ToList();

//#if KIPDORP
//            using (var tx = new TransactionScope())
//            {
//#endif
//            Toekennen(lid, toeTeVoegen);
//            resultaat = LosKoppelen(lid, teVerwijderen); // LosKoppelen persisteert

//            _ledenSync.FunctiesUpdaten(lid);
//#if KIPDORP
//                tx.Complete();
//            }
//#endif
//            return resultaat;
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
        ///  dit werkJaar personen met de gegeven functie zijn.  Anders krijg je een exception.
        /// </param>
        /// <returns>
        /// <c>Null</c> als de functie effectief verwijderd is, anders het functie-object met
        /// aangepast 'werkjaartot'.
        /// </returns>
        /// <remarks>
        /// Als de functie geen leden meer bevat na verwijdering van die van het huidige werkJaar,
        /// dan wordt ze verwijderd.  Zo niet, wordt er een stopdatum op geplakt.
        /// </remarks>
        public Functie Verwijderen(Functie functie, bool forceren)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
            //if (!_autorisatieMgr.IsGavFunctie(functie.ID))
            //{
            //    throw new GeenGavException(Resources.GeenGav);
            //}

            //// Leden moeten gekoppeld zijn
            //// (null verschilt hier expliciet van een lege lijst)
            //Debug.Assert(functie.Lid != null);

            //int huidigGwjID = _veelGebruikt.GroepsWerkJaarOphalen(functie.Groep.ID).ID;

            //var metFunctieDitJaar = from ld in functie.Lid
            //                        where ld.GroepsWerkJaar.ID == huidigGwjID
            //                        select ld;

            //if (!forceren && metFunctieDitJaar.FirstOrDefault() != null)
            //{
            //    throw new BlokkerendeObjectenException<Lid>(
            //        metFunctieDitJaar,
            //        metFunctieDitJaar.Count(),
            //        Resources.FunctieNietLeeg);
            //}

            //foreach (var ld in metFunctieDitJaar)
            //{
            //    ld.TeVerwijderen = true; // markeer link lid->functie als te verwdrn
            //}

            //var metFunctieVroeger = from ld in functie.Lid
            //                        where ld.GroepsWerkJaar.ID != huidigGwjID
            //                        select ld;

            //if (metFunctieVroeger.FirstOrDefault() == null)
            //{
            //    functie.TeVerwijderen = true;
            //}
            //else
            //{
            //    functie.WerkJaarTot = _veelGebruikt.GroepsWerkJaarOphalen(functie.Groep.ID).WerkJaar - 1;
            //}

            //return _funDao.Bewaren(functie, fn => fn.Lid);
        }

        /// <summary>
        /// Kijkt na voor een gegeven <paramref name="groepsWerkJaar"/> of de maximum- en
        /// minimumaantallen van de functies (eigen en nationaal bepaald) niet overschreden zijn.
        /// </summary>
        /// <param name="groepsWerkJaar">
        /// Te controleren werkJaar
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
            //var eigenFuncties = from fn in groepsWerkJaar.Groep.Functie select fn;

            //return AantallenControleren(
            //    groepsWerkJaar,
            //    eigenFuncties.Union(NationaalBepaaldeFunctiesOphalen()));
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Kijkt na voor een gegeven <paramref name="groepsWerkJaar"/> of de maximum- en
        /// minimumaantallen van de functies <paramref name="functies"/> niet overschreden zijn.
        /// </summary>
        /// <param name="groepsWerkJaar">
        /// Te controleren werkJaar
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
    }
}