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
    // NOTE: If you change the interface name "IGroepenService" here, you must also update the reference to "IGroepenService" in Web.config.
    [ServiceContract]
    public interface IGroepenService
    {
        /// <summary>
        /// Persisteert een groep in de database
        /// </summary>
        /// <param name="g">Te persisteren groep</param>
        /// <param name="origineel">Het bewaren zal sneller gaan als een
        /// oorspronkelijk (ongewijzigd) object meegegeven wordt.  Zonder
        /// gaat het ook; geef dan null mee als origineel.
        /// </param>
        /// <returns>De persoon met eventueel gewijzigde informatie</returns>
        /// <remarks>FIXME: gedetailleerde exception</remarks>
        [OperationContract]
        Groep Updaten(Groep g, Groep origineel);

        /// <summary>
        /// Haalt groep op uit database
        /// </summary>
        /// <param name="groepID">ID van op te halen groep</param>
        /// <returns>het gevraagde groepsobject, of null indien niet gevonden.
        /// </returns>
        [OperationContract]
        [ServiceKnownType(typeof(Groep))]
        Groep Ophalen(int groepID);

        /// <summary>
        /// Functie om de service te testen
        /// </summary>
        /// <returns>Een teststring</returns>
        [OperationContract]
        string Hallo();
    }
}
