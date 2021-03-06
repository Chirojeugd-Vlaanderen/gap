/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkte authenticatie Copyright 2015 Chirojeugd-Vlaanderen vzw
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

using System.ServiceModel;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.ServiceContracts
{
    /// <summary>
    /// Interface voor de service voor gebruikersrechtenbeheer.
    /// </summary>
    /// <remarks>Er zijn methods die werken op gelieerdePersoonIDs, en er zijn methods die werken op
    /// usernames. Die laatste zijn nodig voor als de corresponderende persoon voor de gebruiker niet
    /// gekend is in GAP. Dat kan het geval zijn als iemand van Kipdorp tijdelijke toegang tot het GAP
    /// van een bepaalde groep nodig heeft.</remarks>
    [ServiceContract]
    public interface IGebruikersService
    {
        /// <summary>
        /// Als de persoon met gegeven <paramref name="gelieerdePersoonID"/> nog geen account heeft, wordt er een
        /// account voor gemaakt. Aan die account worden dan de meegegeven <paramref name="gebruikersRecht"/>
        /// gekoppeld.  Gebruikersrechten zijn standaard 14 maanden geldig.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon die rechten moet krijgen</param>
        /// <param name="gebruikersRecht">Rechten die de account moet krijgen. Mag leeg zijn. Bestaande 
        /// gebruikersrechten worden zo mogelijk verlengd als ze in <paramref name="gebruikersRecht"/> 
        /// voorkomen, eventuele bestaande rechten niet in <paramref name="gebruikersRecht"/> blijven
        /// onaangeroerd.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
        void RechtenToekennen(int gelieerdePersoonID, GebruikersRecht gebruikersRecht);

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="persoonID"/> af voor de groepen met gegeven <paramref name="groepIds"/>
        /// </summary>
        /// <param name="persoonID">Id van persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIDs">Id's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        [OperationContract]
        [FaultContract(typeof (GapFault))]
        [FaultContract(typeof (FoutNummerFault))]
        void RechtenAfnemen(int persoonID, int[] groepIDs);

        /// <summary>
        /// Levert een redirection-url op naar de site van de verzekeraar
        /// </summary>
        /// <returns>Redirection-url naar de site van de verzekeraar</returns>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
        string VerzekeringsUrlGet(int groepID);

        /// <summary>
        /// Levert de details van de persoon met gegeven <paramref name="adNummer"/>, inclusief gebruikersnaam.
        /// </summary>
        /// <param name="adNummer">AD-nummer van een persoon.</param>
        /// <param name="aanMaken">Als deze <c>true</c> is, wordt een 'stub' aangemaakt als de persoon niet wordt gevonden.</param>
        /// <returns>Details van de persoon met gegeven <paramref name="adNummer"/>.</returns>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
        GebruikersDetail DetailsOphalen(int adNummer, bool aanMaken);

        /// <summary>
        /// Indien de ingelogde gebruiker lid is voor gegeven groep in het recentste werkjaar, dan wordt de id van dat lid terug gegeven
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
        int? AangelogdeGebruikerLidIdGet(int groepID);
    }
}
