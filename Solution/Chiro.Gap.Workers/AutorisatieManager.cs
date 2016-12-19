/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkt gebruikersbeheer Copyright 2014,2015 Chirojeugd-Vlaanderen vzw
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. autorisatie bevat
    /// </summary>
    public class AutorisatieManager : IAutorisatieManager
    {
        private readonly IAuthenticatieManager _authenticatieMgr;

        /// <summary>
        /// Creeert een nieuwe autorisatiemanager
        /// </summary>
        /// <param name="authenticatieMgr">Deze zal de gebruikersnaam van de user opleveren</param>
        public AutorisatieManager(IAuthenticatieManager authenticatieMgr)
        {
            _authenticatieMgr = authenticatieMgr;
        }
        
        /// <summary>
        /// Geeft true als de aangelogde user
        /// 'superrechten' heeft
        /// (zoals het verwijderen van leden uit vorig werkjaar, het 
        /// verwijderen van leden waarvan de probeerperiode voorbij is,...)
        /// </summary>
        /// <returns>
        /// <c>True</c> (enkel) als user supergav is
        /// </returns>
        public bool IsSuperGav()
        {
            // In het GAP zijn er (op dit moment althans) geen super-GAV's.
            // GapUpdate (de service) is super-GAV, maar die heeft zijn eigen
            // IAutorisatieManger.

            return false;
        }

        /// <summary>
        /// Geeft <c>true</c> als de momenteel aangelogde gebruiker beheerder is van de gegeven persoon <paramref name="p"/>.
        /// </summary>
        /// <param name="p">Persoon waarvoor gebruikersrecht nagekeken moet worden</param>
        /// <returns>
        /// <c>true</c> als de momenteel aangelogde gebruiker beheerder is van de gegeven persoon
        /// <paramref name="p"/>.
        /// </returns>
        public bool IsGav(Groep groep)
        {
            return PermissiesOphalen(groep, SecurityAspect.AllesVanGroep) == Permissies.Bewerken;
        }

        /// <summary>
        /// Geeft weer welke effectieve permissie de aangelogde gebruiker heeft op de gegeven
        /// <paramref name="aspecten"/> van de gegeven <paramref name="groep"/>.
        /// </summary>
        /// <param name="groep">Groep waarvoor de permissies opgehaald moeten worden.</param>
        /// <param name="aspecten">Aspecten waarvoor permissies opgehaald moeten worden.</param>
        /// <returns>Permissies die de aangelogde gebruiker heeft op de gegeven
        /// <paramref name="aspecten"/> van de gegeven <paramref name="groep"/>.</returns>
        /// <remarks>
        /// (1) Als je meerdere aspecten combineert, krijg je de bitwise and van de permissies als
        ///     resultaat. D.w.z.: de permissies die je hebt op àlle meegegeven aspecten.
        /// (2) Als je rechten hebt op iedereen van je groep, dan heb je voor je afdeling minstens
        ///     diezelfde rechten.
        /// </remarks>
        public Permissies PermissiesOphalen(Groep groep, SecurityAspect aspecten)
        {
            int? adNummer = _authenticatieMgr.AdNummerGet();
            var gebruikersRecht =
                groep.GebruikersRechtV2.FirstOrDefault(
                    gr => gr.Persoon.AdNummer == adNummer && gr.VervalDatum != null && gr.VervalDatum > DateTime.Now);

            if (gebruikersRecht == null)
            {
                return Permissies.Geen;
            }

            var permissies = new List<Permissies>();

            // Dit is een tamelijk omslachtige procedure:
            // Voeg de permissies op ieder aspect uit 'aspecten' toe aan de lijst
            // 'permissies', en lever de bitwise and op.

            if (aspecten.HasFlag(SecurityAspect.PersoonlijkeGegevens))
            {
                permissies.Add(gebruikersRecht.PersoonsPermissies);
            }
            if (aspecten.HasFlag(SecurityAspect.GroepsGegevens))
            {
                permissies.Add(gebruikersRecht.GroepsPermissies);
            }
            if (aspecten.HasFlag(SecurityAspect.PersonenInAfdeling))
            {
                permissies.Add(gebruikersRecht.AfdelingsPermissies|gebruikersRecht.GroepsPermissies);
            }
            if (aspecten.HasFlag(SecurityAspect.PersonenInGroep))
            {
                permissies.Add(gebruikersRecht.IedereenPermissies);
            }

            return permissies.Aggregate((x, y) => x & y);
        }

        public bool IsGav(Persoon p)
        {
            return p.GelieerdePersoon.Any(gp => IsGav(gp.Groep));
        }

        public bool IsGav(CommunicatieVorm communicatieVorm)
        {
            return IsGav(communicatieVorm.GelieerdePersoon.Groep);
        }
        public bool IsGav(GroepsWerkJaar groepsWerkJaar)
        {
            return IsGav(groepsWerkJaar.Groep);
        }
        public bool IsGav(GelieerdePersoon gelieerdePersoon)
        {
            return IsGav(gelieerdePersoon.Groep);
        }
        public bool IsGav(Deelnemer deelnemer)
        {
            return IsGav(deelnemer.GelieerdePersoon.Groep);
        }
        public bool IsGav(Plaats gelieerdePersoon)
        {
            return IsGav(gelieerdePersoon.Groep);
        }
        public bool IsGav(Uitstap uitstap)
        {
            return IsGav(uitstap.GroepsWerkJaar.Groep);
        }
        public bool IsGav(GebruikersRechtV2 recht)
        {
            return IsGav(recht.Groep);
        }
        public bool IsGav(Afdeling afdeling)
        {
            return IsGav(afdeling.ChiroGroep);
        }
        public bool IsGav(Lid lid)
        {
            return IsGav(lid.GelieerdePersoon.Groep);
        }
        public bool IsGav(Categorie categorie)
        {
            return IsGav(categorie.Groep);
        }

        /// <summary>
        /// Controleert of de aangelogde persoon GAV is voor alle meegegeven
        /// <paramref name="gelieerdePersonen"/>
        /// </summary>
        /// <param name="gelieerdePersonen">Gelieerde personen waarvoor gebruikersrechten
        /// nagekeken moeten worden.</param>
        /// <returns>
        /// <c>true</c> als de aangelogde persoon GAV is voor alle meegegeven
        /// <paramref name="gelieerdePersonen"/>
        /// </returns>
        public bool IsGav(IList<GelieerdePersoon> gelieerdePersonen)
        {
            return
                gelieerdePersonen.Select(gp => gp.Groep)
                    .Distinct()
                    .All(g => PermissiesOphalen(g, SecurityAspect.AllesVanGroep).HasFlag(Permissies.Bewerken));
        }

        /// <summary>
        /// Vertrekt van een lijst <paramref name="personen"/>. Van al die personen
        /// waarvoor de aangelogde gebruiker GAV is, worden nu de overeenkomstige
        /// gelieerde personen opgeleverd. (Dat kunnen dus meer gelieerde personen
        /// per persoon bij zitten.)
        /// </summary>
        /// <param name="personen">
        ///     Lijst van personen
        /// </param>
        /// <returns>
        /// Voor de <paramref name="personen"/>
        /// waarvoor de aangelogde gebruiker GAV is, de overeenkomstige
        /// gelieerde personen
        /// </returns>
        /// <remarks>
        /// Mogelijk zijn er meerdere gelieerde personen per persoon.
        /// </remarks>
        public List<GelieerdePersoon> MijnGelieerdePersonen(IList<Persoon> personen)
        {
            return (from gp in personen.SelectMany(p => p.GelieerdePersoon)
                    where IsGav(gp)
                    select gp).ToList();
        }

        /// <summary>
        /// Geeft <c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="persoonsAdressen"/>. Zo niet <c>false</c>
        /// </summary>
        /// <param name="persoonsAdressen">Een aantal persoonsadrsesen</param>
        /// <returns><c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="persoonsAdressen"/>, <c>false</c> in het andere geval</returns>
        public bool IsGav(IList<PersoonsAdres> persoonsAdressen)
        {
            return persoonsAdressen.All(
                       pa =>
                           pa.Persoon.GelieerdePersoon.Any(
                               gp =>
                                   PermissiesOphalen(gp.Groep, SecurityAspect.AllesVanGroep)
                                       .HasFlag(Permissies.Bewerken)));
        }

        /// <summary>
        /// Geeft <c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="personen"/>. Zo niet <c>false</c>
        /// </summary>
        /// <param name="personen">Een aantal personen</param>
        /// <returns><c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="personen"/>, <c>false</c> in het andere geval</returns>
        public bool IsGav(IList<Persoon> personen)
        {
            return personen.All(
                p =>
                    p.GelieerdePersoon.Any(
                        gp =>
                            PermissiesOphalen(gp.Groep, SecurityAspect.AllesVanGroep).HasFlag(Permissies.Bewerken)));

        }

        /// <summary>
        /// Geeft <c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="leden"/>. Zo niet <c>false</c>
        /// </summary>
        /// <param name="leden">Een aantal leden</param>
        /// <returns><c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="leden"/>, <c>false</c> in het andere geval</returns>
        public bool IsGav(IList<Lid> leden)
        {
            return leden.Select(ld => ld.GroepsWerkJaar.Groep)
                       .Distinct()
                       .All(g => PermissiesOphalen(g, SecurityAspect.AllesVanGroep).HasFlag(Permissies.Bewerken));
        }

        /// <summary>
        /// Geeft <c>true</c> als de aangemelde gebruiker GAV is van van het gegeven
        /// <paramref name="abonnement"/>. Zo niet <c>false</c>.
        /// </summary>
        /// <param name="abonnement">Een abonnement</param>
        /// <returns>Geeft <c>true</c> als de aangemelde gebruiker GAV is van van het gegeven
        /// <paramref name="abonnement"/>. Zo niet <c>false</c>.</returns>
        public bool IsGav(Abonnement abonnement)
        {
            return IsGav(abonnement.GelieerdePersoon) && IsGav(abonnement.GroepsWerkJaar);
        }

        /// <summary>
        /// Geeft <c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="groepen"/>. Zo niet <c>false</c>
        /// </summary>
        /// <param name="groepen">Een aantal personen</param>
        /// <returns><c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="groepen"/>, <c>false</c> in het andere geval</returns>
        public bool IsGav(IList<Groep> groepen)
        {
            return groepen.All(g => PermissiesOphalen(g, SecurityAspect.AllesVanGroep).HasFlag(Permissies.Bewerken));
        }

        /// <summary>
        /// Geeft <c>true</c> als <paramref name="ik"/> de gegevens van
        /// <paramref name="persoon2"/> mag lezen. Anders <c>false</c>.
        /// </summary> 
        /// <param name="ik">De persoon die wil lezen.</param>
        /// <param name="persoon2">De persoon die <paramref name="ik"/> wil lezen.</param>
        /// <returns><c>true</c> als <paramref name="ik"/> de gegevens van
        /// <paramref name="persoon2"/> mag lezen. Anders <c>false</c>.</returns>
        public bool MagLezen(Persoon ik, Persoon persoon2)
        {
            // Als ik mijn eigen info mag lezen, en ik ben persoon2, dan is het ok.
            if (Equals(ik, persoon2) && MagZichzelfLezen(ik))
            {
                return true;
            }

            // Als ik alles mag lezen van een groep waar persoon2 aan gelieerd is, is het ok.
            if (
                persoon2.GelieerdePersoon.Any(
                    gp => PermissiesOphalen(gp.Groep, SecurityAspect.PersonenInGroep).HasFlag(Permissies.Lezen)))
            {
                return true;
            }

            // Als persoon2 in dezelfde afdeling zit als ik, en die afdeling is van het huidige werkjaar, en
            // ik mag personen van mijn afdeling lezen, dan is het OK.


            // als ik de personen van mijn afdeling mag lezen, is het in orde als persoon2 in mijn 
            // afdeling zit.
            return (from g in ToegestaneGroepenOphalen(ik, SecurityAspect.PersonenInAfdeling)
                select g.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First()
                into huidigWerkJaar
                let mijnLid =
                    (from l in ik.GelieerdePersoon.SelectMany(gp => gp.Lid)
                        where Equals(l.GroepsWerkJaar, huidigWerkJaar)
                        select l).FirstOrDefault()
                let persoon2Lid =
                    (from l in persoon2.GelieerdePersoon.SelectMany(gp => gp.Lid)
                        where Equals(l.GroepsWerkJaar, huidigWerkJaar)
                        select l).FirstOrDefault()
                where
                    mijnLid != null && persoon2Lid != null &&
                    mijnLid.AfdelingsJaarIDs.Any(ajid => persoon2Lid.AfdelingsJaarIDs.Contains(ajid))
                select mijnLid).Any();
        }

        /// <summary>
        /// Haalt alle groepen op waarvoor <paramref name="ik"/> leesrechten heb op de gegeven
        /// <paramref name="aspecten"/>.
        /// </summary>
        /// <param name="ik">Persoon waarvoor de groepen opgehaald moeten worden.</param>
        /// <param name="aspecten">Aspecten waarvoor de persoon leesrechten moet hebben.</param>
        /// <returns>alle groepen op waarvoor <paramref name="ik"/> leesrechten heb op de gegeven
        /// <paramref name="aspecten"/>.</returns>
        public IEnumerable<Groep> ToegestaneGroepenOphalen(Persoon ik, SecurityAspect aspecten)
        {
            var gebruikersrechten =
                ik.GebruikersRechtV2.Where(gr => gr.VervalDatum != null & gr.VervalDatum > DateTime.Now);

            if (aspecten.HasFlag(SecurityAspect.PersoonlijkeGegevens))
            {
                gebruikersrechten = gebruikersrechten.Where(gr => gr.PersoonsPermissies.HasFlag(Permissies.Lezen));
            }
            if (aspecten.HasFlag(SecurityAspect.GroepsGegevens))
            {
                gebruikersrechten = gebruikersrechten.Where(gr => gr.GroepsPermissies.HasFlag(Permissies.Lezen));
            }
            if (aspecten.HasFlag(SecurityAspect.PersonenInAfdeling))
            {
                gebruikersrechten = gebruikersrechten.Where(gr => gr.AfdelingsPermissies.HasFlag(Permissies.Lezen));
            }
            if (aspecten.HasFlag(SecurityAspect.PersonenInGroep))
            {
                gebruikersrechten = gebruikersrechten.Where(gr => gr.IedereenPermissies.HasFlag(Permissies.Lezen));
            }
            return gebruikersrechten.Select(gr => gr.Groep).Distinct();
        }

        /// <summary>
        /// Geeft <c>true</c> als de gegeven <paramref name="persoon"/> rechten heeft om zijn persoonlijke
        /// informatie te lezen.
        /// </summary>
        /// <param name="persoon">Persoon waarvan de rechten gecontroleerd moeten worden.</param>
        /// <returns><c>true</c> als de gegeven <paramref name="persoon"/> rechten heeft om zijn persoonlijke
        /// informatie te lezen.</returns>
        public bool MagZichzelfLezen(Persoon persoon)
        {
            return
                persoon.GebruikersRechtV2.Any(
                    gr =>
                        gr.VervalDatum != null && gr.VervalDatum > DateTime.Now &&
                        gr.PersoonsPermissies.HasFlag(Permissies.Lezen));
        }

        /// <summary>
        /// Levert de permissies op die de aangelogde gebruiker heeft op de gegeven 
        /// <paramref name="gelieerdePersoon"/>.
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon met te checken permissies.</param>
        /// <returns>de permissies die de aangelogde gebruiker heeft op de gegeven 
        /// <paramref name="gelieerdePersoon"/>.</returns>
        public Permissies PermissiesOphalen(GelieerdePersoon gelieerdePersoon)
        {
            Permissies result = Permissies.Geen;
            int? mijnAdNummer = _authenticatieMgr.AdNummerGet();
            Debug.Assert(mijnAdNummer.HasValue);

            if (gelieerdePersoon.Persoon.AdNummer == mijnAdNummer)
            {
                result = EigenPermissies(gelieerdePersoon.Persoon);
                if (result == Permissies.Bewerken)
                {
                    return result;
                }
            }

            // Heb ik rechten op de groep van de gelieerde persoon?
            var relevantGebruikersRecht = (from gr in gelieerdePersoon.Groep.GebruikersRechtV2
                where gr.Persoon.AdNummer == mijnAdNummer && gr.VervalDatum != null && gr.VervalDatum > DateTime.Now
                select gr).FirstOrDefault();

            // Nee? Dan zijn we klaar.
            if (relevantGebruikersRecht == null)
            {
                return result;
            }

            // Ik zit in de juiste groep. Ik krijg ook de permissies die ik op iedereen
            // in de groep heb.
            result |= relevantGebruikersRecht.IedereenPermissies;

            // Als ik al alle mogelijke permissies heb, of mijn permissies op mijn afdeling kunnen niets meer
            // veranderen, dan zijn we klaar.
            if (result == Permissies.Bewerken || (result | relevantGebruikersRecht.AfdelingsPermissies) == result)
            {
                return result;
            }

            // Als gelieerdePersoon in dezelfde afdeling zit als ik, en die afdeling is van het huidige werkjaar, 
            // dan zijn ook mijn afdelingspermissies relevant.
            var mijnGelieerdePersoon = (from gp in relevantGebruikersRecht.Persoon.GelieerdePersoon
                where gp.Groep.Equals(relevantGebruikersRecht.Groep)
                select gp).FirstOrDefault();
            if (mijnGelieerdePersoon == null)
            {
                // Ik ben niet gelieerd aan de groep, dus zit ik zeker niet in de juiste afdeling.
                return result;
            }

            var huidigWerkjaar = gelieerdePersoon.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).FirstOrDefault();
            var mijnAfdelingsIds =
                mijnGelieerdePersoon.Lid.Where(l => Equals(l.GroepsWerkJaar, huidigWerkjaar)).SelectMany(l => l.AfdelingIds);
            var zijnAfdelingsIds =
                gelieerdePersoon.Lid.Where(l => Equals(l.GroepsWerkJaar, huidigWerkjaar)).SelectMany(l => l.AfdelingIds);
            if (mijnAfdelingsIds.Any(afdid => zijnAfdelingsIds.Contains(afdid)))
            {
                result |= relevantGebruikersRecht.AfdelingsPermissies;
            }
            return result;
        }

        /// <summary>
        /// Levert de permissies op die de aangelogde gebruiker heeft op het gegeven
        /// <paramref name="lid"/>.
        /// </summary>
        /// <param name="lid">Lid waarvoor de permissies gecontroleerd moeten worden.</param>
        /// <returns>De permissies op die de aangelogde gebruiker heeft op het gegeven
        /// <paramref name="lid"/>.</returns>
        public Permissies PermissiesOphalen(Lid lid)
        {
            var result = Permissies.Geen;
            int? mijnAdNummer = _authenticatieMgr.AdNummerGet();
            Debug.Assert(mijnAdNummer.HasValue);

            // Gaat het over mezelf?
            if (lid.GelieerdePersoon.Persoon.AdNummer == mijnAdNummer)
            {
                result = EigenPermissies(lid.GelieerdePersoon.Persoon);
                if (result == Permissies.Bewerken)
                {
                    return result;
                }
            }

            // Heb ik rechten op de groep van het lid?
            var relevantGebruikersRecht = (from gr in lid.GelieerdePersoon.Groep.GebruikersRechtV2
                      where gr.Persoon.AdNummer == mijnAdNummer && gr.VervalDatum != null && gr.VervalDatum > DateTime.Now
                      select gr).FirstOrDefault();

            // Nee? Klaar!
            if (relevantGebruikersRecht == null)
            {
                return result;
            }

            // Ik heb rechten op de juiste groep. Ik krijg ook de permissies die ik op iedereen
            // in de groep heb.
            result |= relevantGebruikersRecht.IedereenPermissies;

            // Als ik al alle mogelijke permissies heb, of mijn permissies op mijn afdeling kunnen niets meer
            // veranderen, dan zijn we klaar.
            if (result == Permissies.Bewerken || (result | relevantGebruikersRecht.AfdelingsPermissies) == result)
            {
                return result;
            }

            // Als het lid in dezelfde afdeling zit als ik, en die afdeling is van het huidige werkjaar, 
            // dan zijn ook mijn afdelingspermissies relevant.

            var huidigWerkjaar = lid.GelieerdePersoon.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).FirstOrDefault();

            if (!lid.GroepsWerkJaar.Equals(huidigWerkjaar))
            {
                return result;
            }
            var mijnGelieerdePersoon = (from gp in relevantGebruikersRecht.Persoon.GelieerdePersoon
                                        where gp.Groep.Equals(relevantGebruikersRecht.Groep)
                                        select gp).FirstOrDefault();
            if (mijnGelieerdePersoon == null)
            {
                // Ik ben niet gelieerd aan de groep, dus zit ik zeker niet in de juiste afdeling.
                return result;
            }

            var mijnAfdelingsIds =
                mijnGelieerdePersoon.Lid.Where(l => Equals(l.GroepsWerkJaar, huidigWerkjaar)).SelectMany(l => l.AfdelingIds);

            if (mijnAfdelingsIds.Any(afdid => lid.AfdelingIds.Contains(afdid)))
            {
                result |= relevantGebruikersRecht.AfdelingsPermissies;
            }
            return result;
        }


        /// <summary>
        /// Levert de permissies die de aangelogde gebruiker heeft op de gegeven
        /// <paramref name="functie"/>.
        /// </summary>
        /// <param name="functie">Functie waarvan de permissies te checken zijn.</param>
        /// <returns>De permissies die de aangelogde gebruiker heeft op de gegeven
        /// <paramref name="functie"/>.</returns>
        public Permissies PermissiesOphalen(Functie functie)
        {
            return functie.IsNationaal
                ? Permissies.Lezen
                : PermissiesOphalen(functie.Groep, SecurityAspect.GroepsGegevens);
        }

        /// <summary>
        /// Levert de permissies op die een <paramref name="persoon"/> op dit moment heeft op zichzelf.
        /// </summary>
        /// <param name="persoon">Een persoon</param>
        /// <returns>De permissies die de <paramref name="persoon"/> op dit moment heeft op zichzelf.</returns>
        public Permissies EigenPermissies(Persoon persoon)
        {
            return
                persoon.GebruikersRechtV2.Where(gr => gr.VervalDatum != null || gr.VervalDatum > DateTime.Now)
                    .Select(gr => gr.PersoonsPermissies)
                    .Aggregate((a, b) => a | b);
        }

        /// <summary>
        /// Levert het gebruikersrecht op dat de gelieerde persoon <paramref name="gp"/> heeft op zijn
        /// eigen groep.
        /// </summary>
        /// <param name="gp">Een gelieerde persoon.</param>
        /// <returns>Het gebruikersrecht op dat de gelieerde persoon <paramref name="gp"/> heeft op zijn
        /// eigen groep.</returns>
        public GebruikersRechtV2 GebruikersRechtOpEigenGroep(GelieerdePersoon gp)
        {
            return (from gr in gp.Persoon.GebruikersRechtV2 where Equals(gr.Groep, gp.Groep) select gr).FirstOrDefault();
        }
    }
}