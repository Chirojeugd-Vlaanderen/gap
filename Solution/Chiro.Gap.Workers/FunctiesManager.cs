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
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Businesslogica ivm functies
    /// </summary>
    public class FunctiesManager : IFunctiesManager
    {
        /// <summary>
        /// Kent de meegegeven <paramref name="functies"/> toe aan het gegeven <paramref name="lid"/>.
        /// Als het lid al andere functies had, blijven die behouden.
        /// Deze method controleert ook of alles wel volgens de business rules verloopt.
        /// </summary>
        /// <param name="lid">
        ///     Lid dat de functies moet krijgen, gekoppeld aan zijn groep
        /// </param>
        /// <param name="functies">
        ///     Rij toe te kennen functies
        /// </param>
        /// <remarks>
        /// Er wordt verondersteld dat er heel wat opgevraagd kan worden:
        /// - lid.groepswerkjaar.groep
        /// - lid.functie
        /// - voor elke functie:
        ///  - functie.lid (voor leden van dezelfde groep)
        ///  - functie.groep
        /// </remarks>
        public void Toekennen(Lid lid, IList<Functie> functies)
        {
            Debug.Assert(lid.GroepsWerkJaar != null);
            Debug.Assert(lid.GroepsWerkJaar.Groep != null);

            // Leden van een oud groepswerkjaar kunnen niet meer worden aangepast.
            var recentsteGwj = lid.GroepsWerkJaar.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First();

            if (!Equals(lid.GroepsWerkJaar, recentsteGwj))
            {
                throw new FoutNummerException(
                    FoutNummer.GroepsWerkJaarNietBeschikbaar,
                    Resources.GroepsWerkJaarVoorbij);
            }

            // Eerst alle checks, zodat als er ergens een exceptie optreedt, er geen enkele
            // functie wordt toegekend.

            foreach (var f in functies)
            {
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
                    throw new FoutNummerException(
                        FoutNummer.LidTypeVerkeerd,
                        Resources.FoutiefLidType);
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
                lid.Functie.Add(f);
            }

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

            //int huidigGwjID = _veelGebruikt.GroepsWerkJaarIDOphalen(functie.Groep.ID).ID;

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
            //    functie.WerkJaarTot = _veelGebruikt.GroepsWerkJaarIDOphalen(functie.Groep.ID).WerkJaar - 1;
            //}

            //return _funDao.Bewaren(functie, fn => fn.Lid);
        }

        /// <summary>
        /// Kijkt na voor een gegeven <paramref name="groepsWerkJaar"/> of de maximum- en
        /// minimumaantallen van de functies <paramref name="functies"/> niet overschreden zijn.
        /// </summary>
        /// <param name="groepsWerkJaar">
        ///     Te controleren werkJaar
        /// </param>
        /// <param name="functies">
        ///     Functies waarop te controleren
        /// </param>
        /// <returns>
        /// Een lijst met tellingsgegevens voor de functies waar de aantallen niet kloppen.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Deze functie is zich niet bewust van de aanwezigheid van een database, en verwacht
        /// dat groepsWerkJaar.Lid[i].Functie beschikbaar is.
        /// </para>
        /// <para>
        /// Functies in <paramref name="functies"/> waar geen groep aan gekoppeld is, worden als
        /// nationaal bepaalde functies beschouwd.
        /// </para>
        /// <para>
        /// Functies die niet geldig zijn in het gevraagde groepswerkjaar, worden genegeerd
        /// </para>
        /// </remarks>
        public List<Telling> AantallenControleren(
            GroepsWerkJaar groepsWerkJaar,
            IList<Functie> functies)
        {
            var toegekendeFuncties =
                groepsWerkJaar.Lid.SelectMany(ld => ld.Functie)
                              .Distinct()
                              .Where(functies.Contains);

            var nietToegekendeFuncties = from fn in functies
                                         where !toegekendeFuncties.Contains(fn)
                                               && (fn.IsNationaal || Equals(fn.Groep, groepsWerkJaar.Groep))
                                         // bovenstaande vermijdt groepsvreemde functies
                                         select fn;

            // toegekende functies waarvan er te veel of te weinig zijn
            var problemenToegekendeFuncties =
                toegekendeFuncties.Where(fn => (fn.WerkJaarVan == null ||
                                                fn.WerkJaarVan <= groepsWerkJaar.WerkJaar)
                                               &&
                                               (fn.WerkJaarTot == null ||
                                                fn.WerkJaarTot >= groepsWerkJaar.WerkJaar)
                                               &&
                                               (fn.Lid.Count() > fn.MaxAantal
                                                // geeft false als maxaant == null
                                                || fn.Lid.Count() < fn.MinAantal)).Select(fn => new Telling
                                                                                                       {
                                                                                                           ID = fn.ID,
                                                                                                           Aantal =
                                                                                                               fn.Lid
                                                                                                                 .Count(),
                                                                                                           Max =
                                                                                                               fn
                                                                                                               .MaxAantal,
                                                                                                           Min =
                                                                                                               fn
                                                                                                               .MinAantal
                                                                                                       });

            // niet-toegekende functies waarvan er te weinig zijn
            var problemenOntbrekendeFuncties =
                nietToegekendeFuncties.Where(fn => (fn.WerkJaarVan == null || fn.WerkJaarVan <= groepsWerkJaar.WerkJaar)
                                                   &&
                                                   (fn.WerkJaarTot == null || fn.WerkJaarTot >= groepsWerkJaar.WerkJaar)
                                                   && fn.MinAantal > 0).Select(fn => new Telling
                                                                                         {
                                                                                             ID = fn.ID,
                                                                                             Aantal = 0,
                                                                                             Max = fn.MaxAantal,
                                                                                             Min = fn.MinAantal
                                                                                         });

            return problemenToegekendeFuncties.Union(problemenOntbrekendeFuncties).ToList();
        }

        /// <summary>
        /// Vervangt de functies van het gegeven <paramref name="lid"/> door de meegegeven
        /// <paramref name="functies"/>. Deze method kijkt ook na of de functies wel aan
        /// de juiste groep gekoppeld zijn, en van het juiste niveau zijn.
        /// </summary>
        /// <param name="lid">Lid waarvan de functies vervangen moeten worden</param>
        /// <param name="functies">Nieuwe functies voor <paramref name="lid"/></param>
        public void Vervangen(Lid lid, List<Functie> functies)
        {
            // In deze method zitten geen checks op GAV-schap, juiste werkJaar,... dat gebeurt al in
            // 'Toekennen' en 'Loskoppelen', dewelke door deze method worden aangeroepen.
            var toeTeVoegen = (from fn in functies
                               where !lid.Functie.Contains(fn)
                               select fn).ToList();

            var teVerwijderen = (from fn in lid.Functie
                                 where !functies.Contains(fn)
                                 select fn).ToList();

            Toekennen(lid, toeTeVoegen);

            foreach (var f in teVerwijderen)
            {
                lid.Functie.Remove(f);
            }
        }
    }
}