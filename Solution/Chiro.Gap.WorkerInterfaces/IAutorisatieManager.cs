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
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    /// <summary>
    /// Een autorisatiemanager bepaalt de rechten van een gebruiker op entiteiten.
    /// </summary>
    public interface IAutorisatieManager
    {
        /// <summary>
        /// Geeft true als de aangelogde user
        /// 'superrechten' heeft
        /// (zoals het verwijderen van leden uit vorig werkjaar, het 
        /// verwijderen van leden waarvan de probeerperiode voorbij is,...)
        /// </summary>
        /// <returns>
        /// <c>True</c> (enkel) als user supergav is
        /// </returns>
        bool IsSuperGav();

        /// <summary>
        /// Geeft <c>true</c> als de momenteel aangelogde gebruiker beheerder is van gegeven
        /// <paramref name="groep"/>.
        /// </summary>
        /// <param name="groep">Groep waarvoor gebruikersrecht nagekeken moet worden</param>
        /// <returns>
        /// <c>true</c> als de momenteel aangelogde gebruiker beheerder is van de gegeven
        /// <paramref name="groep"/>.
        /// </returns>
        bool IsGav(Groep groep);
        bool IsGav(CommunicatieVorm communicatie);
        bool IsGav(GroepsWerkJaar g);
        bool IsGav(GelieerdePersoon gelieerdePersoon);
        bool IsGav(Deelnemer d);
        bool IsGav(Plaats p);
        bool IsGav(Uitstap u);
        bool IsGav(GebruikersRecht g);
        bool IsGav(Lid l);
        bool IsGav(Afdeling a);
        bool IsGav(Categorie c);

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
        bool IsGav(IList<GelieerdePersoon> gelieerdePersonen);

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
        List<GelieerdePersoon> MijnGelieerdePersonen(IList<Persoon> personen);

        /// <summary>
        /// Geeft <c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="persoonsAdressen"/>. Zo niet <c>false</c>
        /// </summary>
        /// <param name="persoonsAdressen">Een aantal persoonsadrsesen</param>
        /// <returns><c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="persoonsAdressen"/>, <c>false</c> in het andere geval</returns>
        bool IsGav(IList<PersoonsAdres> persoonsAdressen);

        /// <summary>
        /// Geeft <c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="personen"/>. Zo niet <c>false</c>
        /// </summary>
        /// <param name="personen">Een aantal personen</param>
        /// <returns><c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="personen"/>, <c>false</c> in het andere geval</returns>
        bool IsGav(IList<Persoon> personen);

        /// <summary>
        /// Geeft <c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="groepen"/>. Zo niet <c>false</c>
        /// </summary>
        /// <param name="groepen">Een aantal personen</param>
        /// <returns><c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="groepen"/>, <c>false</c> in het andere geval</returns>
        bool IsGav(IList<Groep> groepen);

        /// <summary>
        /// Geeft <c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="leden"/>. Zo niet <c>false</c>
        /// </summary>
        /// <param name="leden">Een aantal leden</param>
        /// <returns><c>true</c> als de aangemelde gebruiker GAV is van alle gegeven 
        /// <paramref name="leden"/>, <c>false</c> in het andere geval</returns>
        bool IsGav(IList<Lid> leden);
    }
}
