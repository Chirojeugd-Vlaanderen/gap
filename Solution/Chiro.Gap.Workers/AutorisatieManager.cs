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

using System;
using System.Collections.Generic;
using System.Linq;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

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
        /// Geeft <c>true</c> als de momenteel aangelogde gebruiker beheerder is van het gegeven <paramref name="object"/>.
        /// </summary>
        /// <param name="groep">Groep waarvoor gebruikersrecht nagekeken moet worden</param>
        /// <returns>
        /// <c>true</c> als de momenteel aangelogde gebruiker beheerder is van de gegeven
        /// <paramref name="groep"/>.
        /// </returns>
        public bool IsGav(Groep groep)
        {
            var gebruikersNaam = _authenticatieMgr.GebruikersNaamGet();
            return groep != null &&
                   groep.GebruikersRecht.Any(
                       gr =>
                       String.Compare(gr.Gav.Login, gebruikersNaam, StringComparison.OrdinalIgnoreCase) == 0 &&
                       (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now));
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
        public bool IsGav(GebruikersRecht recht)
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
            // Als er een gelieerde persoon is waarvoor alle GAV's een gebruikersnaam hebben
            // verschillend van de mijne, dan ben ik geen GAV voor alle gegeven personen.

            var gebruikersNaam = _authenticatieMgr.GebruikersNaamGet();
            return (from gp in gelieerdePersonen
                    where
                        gp.Groep.GebruikersRecht.All(
                            gr =>
                            String.Compare(gr.Gav.Login, gebruikersNaam, StringComparison.OrdinalIgnoreCase) != 0 ||
                            (gr.VervalDatum != null && gr.VervalDatum < DateTime.Now))
                    select gp).FirstOrDefault() == null;
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
            var gebruikersNaam = _authenticatieMgr.GebruikersNaamGet();

            return
                persoonsAdressen.All(
                    pa =>
                    pa.Persoon.GelieerdePersoon.Any(
                        gp =>
                        gp.Groep.GebruikersRecht.Any(
                            gr =>
                            String.Compare(gr.Gav.Login, gebruikersNaam, StringComparison.InvariantCultureIgnoreCase) ==
                            0 && (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now))));
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
            var gebruikersNaam = _authenticatieMgr.GebruikersNaamGet();

            return personen.All(
                p =>
                p.GelieerdePersoon.Any(
                    gp =>
                    gp.Groep.GebruikersRecht.Any(
                        gr =>
                        String.Compare(gr.Gav.Login, gebruikersNaam, StringComparison.InvariantCultureIgnoreCase) ==
                        0 && (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now))));

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
            var gebruikersNaam = _authenticatieMgr.GebruikersNaamGet();

            return leden.Select(ld => ld.GroepsWerkJaar.Groep).Distinct().All(g => g.GebruikersRecht.Any(
                gr =>
                String.Compare(gr.Gav.Login, gebruikersNaam, StringComparison.InvariantCultureIgnoreCase) ==
                0 && (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now)));
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
            var gebruikersNaam = _authenticatieMgr.GebruikersNaamGet();

            return groepen.All(g => g.GebruikersRecht.Any(
                gr =>
                String.Compare(gr.Gav.Login, gebruikersNaam, StringComparison.InvariantCultureIgnoreCase) ==
                0 && (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now)));
        }
    }
}