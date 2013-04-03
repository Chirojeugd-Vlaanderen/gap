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
﻿using System.Collections.Generic;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IPersonenManager
    {
        /// <summary>
        /// Verhuist een persoon van oudAdres naar nieuwAdres.  Persisteert niet.
        /// </summary>
        /// <param name="verhuizer">
        /// Te verhuizen GelieerdePersoon
        /// </param>
        /// <param name="oudAdres">
        /// Oud adres, met personen gekoppeld
        /// </param>
        /// <param name="nieuwAdres">
        /// Nieuw adres, met personen gekoppeld
        /// </param>
        /// <param name="adresType">
        /// Adrestype voor de nieuwe koppeling persoon-adres
        /// </param>
        /// <remarks>
        /// Als de persoon niet gekoppeld is aan het oude adres,
        /// zal hij of zij ook niet verhuizen
        /// </remarks>
        void Verhuizen(Persoon verhuizer, Adres oudAdres, Adres nieuwAdres, AdresTypeEnum adresType);

        /// <summary>
        /// Verhuist een persoon van oudAdres naar nieuwAdres.  Persisteert niet.
        /// </summary>
        /// <param name="verhuizers">
        /// Te verhuizen personen
        /// </param>
        /// <param name="oudAdres">
        /// Oud adres, met personen gekoppeld
        /// </param>
        /// <param name="nieuwAdres">
        /// Nieuw adres, met personen gekoppeld
        /// </param>
        /// <param name="adresType">
        /// Adrestype voor de nieuwe koppeling persoon-adres
        /// </param>
        /// <remarks>
        /// Als de persoon niet gekoppeld is aan het oude adres,
        /// zal hij of zij ook niet verhuizen
        /// </remarks>
        void Verhuizen(IList<Persoon> verhuizers, Adres oudAdres, Adres nieuwAdres, AdresTypeEnum adresType);

        /// <summary>
        /// Verlegt alle referenties van de persoon met ID <paramref name="dubbelID"/> naar de persoon met ID
        /// <paramref name="origineelID"/>, en verwijdert vervolgens de dubbele persoon.
        /// </summary>
        /// <param name="origineelID">
        /// ID van de te behouden persoon
        /// </param>
        /// <param name="dubbelID">
        /// ID van de te verwijderen persoon, die eigenlijk gewoon dezelfde is de te
        /// behouden.
        /// </param>
        /// <remarks>
        /// Het is niet proper dit soort van logica in de data access te doen.  Anderzijds zou het een 
        /// heel gedoe zijn om dit in de businesslaag te implementeren, omdat er heel wat relaties verlegd moeten worden.
        /// Dat wil zeggen: relaties verwijderen en vervolgens nieuwe maken.  Dit zou een heel aantal 'TeVerwijderens' met zich
        /// meebrengen, wat het allemaal zeer complex zou maken.  Vandaar dat we gewoon via een stored procedure werken.
        /// <para>
        /// </para>
        /// </remarks>
        void DubbelVerwijderen(int origineelID, int dubbelID);

        /// <summary>
        /// Zoekt in de database personen met een gedeeld AD-nummer, en merget deze.
        /// </summary>
        void FixGedeeldeAds();

        /// <summary>
        /// Kent een AD-nummer toe aan een persoon, en persisteert.  Als er al een persoon bestond met
        /// het gegeven AD-nummer, worden de personen gemerged.
        /// </summary>
        /// <param name="persoon">
        /// Persoon met toe te kennen AD-nummer
        /// </param>
        /// <param name="adNummer">
        /// Toe te kennen AD-nummer
        /// </param>
        void AdNummerToekennen(Persoon persoon, int adNummer);
    }
}