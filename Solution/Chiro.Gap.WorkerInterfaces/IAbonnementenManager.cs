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
using Chiro.Gap.Poco.Model.Exceptions;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IAbonnementenManager
    {
        /// <summary>
        /// Creëert een abonnement voor de gelieerde persoon <paramref name="gp"/> op publicatie
        /// <paramref name="publicatie"/> in het groepswerkjaar <paramref name="groepsWerkJaar"/>.
        /// </summary>
        /// <param name="publicatie">
        /// Publicatie waarvoor abonnement aangevraagd wordt
        /// </param>
        /// <param name="gp">
        /// Gelieerde persoon die abonnement moet krijgen
        /// </param>
        /// <param name="groepsWerkJaar">
        /// Groepswerkjaar waarvoor abonnement aangevraagd wordt
        /// </param>
        /// <returns>
        /// Het aangevraagde abonnement
        /// </returns>
        /// <exception cref="BlokkerendeObjectenException{TEntiteit}">
        /// Komt voor als de <paramref name="gp"/> voor het opgegeven <paramref name="groepsWerkJaar"/> al een
        /// abonnement heeft op die <paramref name="publicatie"/>.
        /// </exception>
        /// <exception cref="FoutNummerException">
        /// Komt voor als de publicatie niet meer uitgegeven wordt en je dus geen abonnement meer kunt aanvragen,
        /// als de bestelperiode voorbij is, of als de <paramref name="gp"/> geen adres heeft waar we de publicatie 
        /// naar kunnen opsturen.
        /// </exception>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker geen GAV is voor de groep waar het <paramref name="groepsWerkJaar"/>
        /// aan gekoppeld is.
        /// </exception>
        Abonnement Abonneren(Publicatie publicatie, GelieerdePersoon gp, GroepsWerkJaar groepsWerkJaar);

        bool KrijgtDubbelpunt(GelieerdePersoon gelieerdePersoon);
    }
}