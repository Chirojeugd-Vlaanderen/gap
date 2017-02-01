/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkt gebruikersbeheer Copyright 2014, 2015 Chirojeugd-Vlaanderen vzw
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
using Chiro.Gap.Domain;
using System;

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

        #region Te vervangen functies

        // 'IsGav' is iets van vroeger. Toen had je wel rechten of geen rechten. Maar nu
        // heb je permissies. Deze functies moeten vervangen worden door
        // MagLezen(entity) en MagSchrijven(entity) of iets dergelijks.

        bool IsGav(Groep groep);
        bool IsGav(CommunicatieVorm communicatie);
        bool IsGav(GroepsWerkJaar g);
        bool IsGav(GelieerdePersoon gelieerdePersoon);
        bool IsGav(Deelnemer d);
        bool IsGav(Plaats p);
        bool IsGav(Uitstap u);
        bool IsGav(GebruikersRechtV2 g);
        bool IsGav(Lid l);
        bool IsGav(Afdeling a);
        bool IsGav(Categorie c);
        bool IsGav(Persoon p);

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

        /// <summary>
        /// Geeft <c>true</c> als de aangemelde gebruiker GAV is van van het gegeven
        /// <paramref name="abonnement"/>. Zo niet <c>false</c>.
        /// </summary>
        /// <param name="abonnement">Een abonnement</param>
        /// <returns>Geeft <c>true</c> als de aangemelde gebruiker GAV is van van het gegeven
        /// <paramref name="abonnement"/>. Zo niet <c>false</c>.</returns>
        bool IsGav(Abonnement abonnement);
        #endregion

        #region Nieuw gebruikersbeheer: Welke permissies heb ik op entity...
        /// <summary>
        /// Levert de permissies op die de aangelogde gebruiker heeft op de gegeven 
        /// <paramref name="gelieerdePersoon"/>.
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon met te checken permissies.</param>
        /// <returns>de permissies die de aangelogde gebruiker heeft op de gegeven 
        /// <paramref name="gelieerdePersoon"/>.</returns>
        Permissies PermissiesOphalen(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Levert de permissies op die de aangelogde gebruiker heeft op het gegeven
        /// <paramref name="lid"/>.
        /// </summary>
        /// <param name="lid">Lid waarvoor de permissies gecontroleerd moeten worden.</param>
        /// <returns>De permissies op die de aangelogde gebruiker heeft op het gegeven
        /// <paramref name="lid"/>.</returns>
        Permissies PermissiesOphalen(Lid lid);

        /// <summary>
        /// Levert de permissies die de aangelogde gebruiker heeft op de gegeven
        /// <paramref name="functie"/>.
        /// </summary>
        /// <param name="functie">Functie waarvan de permissies te checken zijn.</param>
        /// <returns>De permissies die de aangelogde gebruiker heeft op de gegeven
        /// <paramref name="functie"/>.</returns>
        Permissies PermissiesOphalen(Functie functie);
        #endregion

        #region Nieuw gebruikersbeheer: mag persoon X ...
        /// <summary>
        /// Geeft <c>true</c> als <paramref name="ik"/> de gegevens van
        /// <paramref name="persoon2"/> mag lezen. Anders <c>false</c>.
        /// </summary> 
        /// <param name="ik">De persoon die wil lezen.</param>
        /// <param name="persoon2">De persoon die <paramref name="ik"/> wil lezen.</param>
        /// <returns><c>true</c> als <paramref name="ik"/> de gegevens van
        /// <paramref name="persoon2"/> mag lezen. Anders <c>false</c>.</returns>
        bool MagLezen(Persoon ik, Persoon persoon2);

        /// <summary>
        /// Geeft <c>true</c> als de gegeven <paramref name="persoon"/> rechten heeft om zijn persoonlijke
        /// informatie te lezen.
        /// </summary>
        /// <param name="persoon">Persoon waarvan de rechten gecontroleerd moeten worden.</param>
        /// <returns><c>true</c> als de gegeven <paramref name="persoon"/> rechten heeft om zijn persoonlijke
        /// informatie te lezen.</returns>
        bool MagZichzelfLezen(Persoon persoon);
        #endregion

        #region Nog te bekijken of we deze moeten behouden in de interface.
        /// <summary>
        /// Geeft weer welke permissie de aangelogde gebruiker heeft op de gegeven
        /// <paramref name="aspecten"/> van de gegeven <paramref name="groep"/>.
        /// </summary>
        /// <param name="groep">Groep waarvoor de permissies opgehaald moeten worden.</param>
        /// <param name="aspecten">Aspecten waarvoor permissies opgehaald moeten worden.</param>
        /// <returns>Permissies die de aangelogde gebruiker heeft op de gegeven
        /// <paramref name="aspecten"/> van de gegeven <paramref name="groep"/>.</returns>
        /// <remarks>Als je meerdere aspecten combineert, krijg je de bitwise and van de permissies als
        /// resultaat. D.w.z.: de permissies die je hebt op àlle meegegeven aspecten.</remarks>
        Permissies PermissiesOphalen(Groep groep, SecurityAspect aspecten);

        /// <summary>
        /// Levert de permiessies op die een gegeven <paramref name="persoon"/> heeft op zijn
        /// eigen gegevens.
        /// </summary>
        /// <param name="persoon">Een persoon.</param>
        /// <returns>De permiessies op die de gegeven <paramref name="persoon"/> heeft op zijn
        /// eigen gegevens.</returns>
        Permissies EigenPermissies(Persoon persoon);

        /// <summary>
        /// Levert het gebruikersrecht op dat de gelieerde persoon <paramref name="gp"/> heeft op zijn
        /// eigen groep.
        /// </summary>
        /// <param name="gp">Een gelieerde persoon.</param>
        /// <returns>Het gebruikersrecht op dat de gelieerde persoon <paramref name="gp"/> heeft op zijn
        /// eigen groep.</returns>
        GebruikersRechtV2 GebruikersRechtOpEigenGroep(GelieerdePersoon gp);
        #endregion
    }
}
