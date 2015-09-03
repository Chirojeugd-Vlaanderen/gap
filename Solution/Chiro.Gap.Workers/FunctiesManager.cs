/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private readonly IVeelGebruikt _veelGebruikt;

        /// <summary>
        /// Cache injecteren via dependency injection
        /// </summary>
        /// <param name="veelGebruikt">cache-dink</param>
        public FunctiesManager(IVeelGebruikt veelGebruikt)
        {
            _veelGebruikt = veelGebruikt;
        }

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
                        Resources.FoutiefLidTypeFunctie);
                }

                // Custom foliekes voor nationale functies

                if (f.Is(NationaleFunctie.ContactPersoon))
                {
                    // Hebben we een e-mailadres waarop we de contactpersoon mogen contacteren?

                    var mailAdressen = (from c in lid.GelieerdePersoon.Communicatie
                                        where c.CommunicatieType.ID == (int)CommunicatieTypeEnum.Email
                                            select c).ToList();

                    if (!mailAdressen.Any())
                    {
                        throw new FoutNummerException(FoutNummer.EMailVerplicht, Resources.EMailVerplicht);
                    }

                    // Schrijft de persoon zich in voor de nieuwsbrief?

                    if (!lid.GelieerdePersoon.Persoon.NieuwsBrief)
                    {
                        throw new FoutNummerException(FoutNummer.ContactMoetNieuwsBriefKrijgen, Resources.ContactMoetNieuwsBriefKrijgen);
                    }
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
                f.Lid.Add(lid);
            }

        }

        /// <summary>
        /// Verwijdert een functie
        /// </summary>
        /// <param name="functie">
        /// Te verwijderen functie
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
            // Leden moeten gekoppeld zijn
            // (null verschilt hier expliciet van een lege lijst)
            Debug.Assert(functie.Lid != null);

            if (functie.Groep == null)
            {
                throw new FoutNummerException(FoutNummer.FunctieNietBeschikbaar, Resources.NationaleFunctieVerwijderen);
            }

            var huidigGwj = functie.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First();

            var metFunctieDitJaar = (from ld in functie.Lid
                                     where ld.GroepsWerkJaar.ID == huidigGwj.ID
                                     select ld).ToList();

            if (!forceren && metFunctieDitJaar.FirstOrDefault() != null)
            {
                throw new BlokkerendeObjectenException<Lid>(
                    metFunctieDitJaar,
                    metFunctieDitJaar.Count(),
                    Resources.FunctieNietLeeg);
            }

            foreach (var ld in metFunctieDitJaar)
            {
                functie.Lid.Remove(ld);
            }

            var metFunctieVroeger = from ld in functie.Lid
                                    where ld.GroepsWerkJaar.ID != huidigGwj.ID
                                    select ld;

            if (metFunctieVroeger.FirstOrDefault() == null)
            {
                functie.Groep.Functie.Remove(functie);
                return null;
            }
            functie.WerkJaarTot = _veelGebruikt.WerkJaarOphalen(functie.Groep) - 1;
            return functie;
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
                              .Where(functies.Contains);

            // bovenstaande syntax is wat ongebruikelijker, maar wel juist. Ze is
            // equivalent met .Where(fn => functies.Contains(fn))

            var nietToegekendeFuncties = from fn in functies
                                         where !toegekendeFuncties.Contains(fn)
                                               && (fn.IsNationaal || Equals(fn.Groep, groepsWerkJaar.Groep))
                                         // bovenstaande vermijdt groepsvreemde functies
                                         select fn;

            // toegekende functies waarvan er te veel of te weinig zijn

            var problemenToegekendeFuncties = from f in toegekendeFuncties
                group f by f
                into gegroepeerd
                where (gegroepeerd.Key.WerkJaarVan == null || gegroepeerd.Key.WerkJaarVan <= groepsWerkJaar.WerkJaar)
                      && (gegroepeerd.Key.WerkJaarTot == null || gegroepeerd.Key.WerkJaarTot >= groepsWerkJaar.WerkJaar)
                      &&
                      (gegroepeerd.Count() > gegroepeerd.Key.MaxAantal ||
                       gegroepeerd.Count() < gegroepeerd.Key.MinAantal)
                select
                    new Telling
                    {
                        ID = gegroepeerd.Key.ID,
                        Aantal = gegroepeerd.Count(),
                        Max = gegroepeerd.Key.MaxAantal,
                        Min = gegroepeerd.Key.MinAantal
                    };
                                          

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