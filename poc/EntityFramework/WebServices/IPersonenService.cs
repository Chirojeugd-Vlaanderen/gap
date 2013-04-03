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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Core.Domain;

namespace WebServices
{
    // NOTE: If you change the interface name "IPersonenService" here, you must also update the reference to "IPersonenService" in Web.config.
    [ServiceContract]
    [ServiceKnownType(typeof(Persoon))]
    [ServiceKnownType(typeof(GeslachtsType))]
    [ServiceKnownType(typeof(CommunicatieVorm))]
    public interface IPersonenService
    {
        /// <summary>
        /// Haalt persoon op uit database
        /// </summary>
        /// <param name="persoonID">ID op te halen persoon</param>
        /// <returns>Het gevraagde persoonsobject, null als niets gevonden
        /// is.</returns>
        [OperationContract]
        Persoon Ophalen(int persoonID);

        /// <summary>
        /// Haalt persoon op uit database, inclusief communicatiegegevens
        /// </summary>
        /// <param name="persoonID">ID op te halen persoon</param>
        /// <returns>Persoonsobject waarbij member Communicatie de
        /// communicatievormen van de persoon bevat.  Als de persoon niet
        /// werd gevonden, is het resultaat null.</returns>
        [OperationContract]
        Persoon OphalenMetCommunicatie(int persoonID);

        /// <summary>
        /// Maakt een nieuwe persoon persistent
        /// </summary>
        /// <param name="p">Te bewaren persoon</param>
        /// <returns>toegewezen ID</returns>
        [OperationContract]
        int Bewaren(Persoon p);

        /// <summary>
        /// 'Detacht' een gepersisteerde persoon
        /// </summary>
        /// <param name="p">Persoon uit database te verwijderen</param>
        [OperationContract]
        void Verwijderen(Persoon p);

        /// <summary>
        /// Enkel om te testen
        /// </summary>
        /// <returns>Een teststring</returns>
        [OperationContract]
        string Hallo();
    }
}
