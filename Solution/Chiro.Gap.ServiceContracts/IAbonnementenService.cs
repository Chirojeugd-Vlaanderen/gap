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
﻿using System.ServiceModel;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.ServiceContracts
{
    /// <summary>
    /// Functionaliteit voor abonnementen krijgt nu ook een eigen service
    /// </summary>
    [ServiceContract]
    public interface IAbonnementenService
    {
        /// <summary>
        /// Bestelt Dubbelpunt voor de persoon met GelieerdePersoonID <paramref name="gelieerdePersoonID"/>.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon van persoon die Dubbelpunt wil</param>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
        void DubbelPuntBestellen(int gelieerdePersoonID);

        /// <summary>
        /// Controleert of de gelieerde persoon met gegeven <paramref name="id"/> recht heeft op gratis Dubbelpunt.
        /// </summary>
        /// <param name="id">GelieerdePersoonID</param>
        /// <returns><c>true</c> als de persoon zeker een gratis dubbelpuntabonnement krijgt voor jouw groep. <c>false</c>
        /// als de persoon zeker geen gratis dubbelpuntabonnement krijgt voor jouw groep. En <c>null</c> als het niet
        /// duidelijk is. (In praktijk: als de persoon contactpersoon is, maar als er meerdere contactpersonen zijn.)
        /// </returns>
        [OperationContract]
        bool? HeeftGratisDubbelpunt(int id);
    }
}
