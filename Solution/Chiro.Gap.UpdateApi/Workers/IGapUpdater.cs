/*
 * Copyright 2014-2015 the GAP developers. See the NOTICE file at the 
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
using Chiro.Gap.UpdateApi.Models;

namespace Chiro.Gap.UpdateApi.Workers
{
    public interface IGapUpdater: IDisposable
    {
        /// <summary>
        /// Stelt het AD-nummer van de persoon met Id <paramref name="persoonId"/> in. 
        /// Als er al een persoon bestaat met hetzelfde AD-nummer, dan wordt die persoon
        /// gemerged met de bestaande persoon. Als <paramref name="adNummer"/>
        /// <c>null</c> is, dan wordt het AD-nummer gewoon verwijderd.
        /// </summary>
        /// <param name="persoonId">
        /// Id van de persoon
        /// </param>
        /// <param name="adNummer">
        /// Nieuw AD-nummer
        /// </param>
        void AdNummerToekennen(int persoonId, int? adNummer);

        /// <summary>
        /// Vervangt het AD-nummer van de persoon met AD-nummer <paramref name="oudAd"/>
        /// door <paramref name="nieuwAd"/>.  Als er al een persoon bestond met AD-nummer
        /// <paramref name="nieuwAd"/>, dan worden de personen gemerged.
        /// </summary>
        /// <param name="oudAd">AD-nummer van persoon met te vervangen AD-nummer</param>
        /// <param name="nieuwAd">Nieuw AD-nummer</param>
        void AdNummerVervangen(int oudAd, int nieuwAd);

        /// <summary>
        /// Markeert een groep in GAP als gestopt. Of als terug actief.
        /// </summary>
        /// <param name="stamNr">Stamnummer te stoppen groep</param>
        /// <param name="stopDatum">Datum vanaf wanneer gestopt, <c>null</c> om de groep opnieuw te activeren.</param>
        /// <remarks>Als <paramref name="stopDatum"/> <c>null</c> is, wordt de groep opnieuw actief.</remarks>
        void GroepDesactiveren(string stamNr, DateTime? stopDatum);

        /// <summary>
        /// Verwijdert het ad-nummer van de persoon met gegeven <paramref name="adNummer"/>.
        /// </summary>
        /// <param name="adNummer">AD-nummer dat verwijderd moet worden</param>
        /// <returns>PersoonId van de persoon met het verwijderde AD-nummer.</returns>
        int AdNummerVerwijderen(int adNummer);

        /// <summary>
        /// Werkt een persoon bij op basis van gegevens in <paramref name="model"/>.
        /// Het PersoonID bepaalt welke persoon bijgwerkt moet worden.
        /// </summary>
        /// <param name="model">Gegevens bij te werken persoon.</param>
        void Bijwerken(PersoonModel model);
    }
}
