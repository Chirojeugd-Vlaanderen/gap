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
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model.Exceptions
{
    /// <summary>
    /// Exceptie voor fouten tegen validatieregels.
    /// </summary>
    public class ValidatieException : GapException
    {
        /// <summary>
        /// Enumwaarde die meer info geeft over de aard van de exceptie
        /// </summary>
        public FoutNummer FoutNummer { get; set; }

        /// <summary>
        /// Instantieert een lege ValidatieException
        /// </summary>
        public ValidatieException()
        {
        }

        /// <summary>
        /// Instantieert een ValidatieException met een opgegeven foutboodschap
        /// </summary>
        /// <param name="message">
        /// De foutboodschap die doorgegeven moet worden
        /// </param>
        public ValidatieException(string message) : base(message)
        {
        }

        /// <summary>
        /// Instantieert een ValidatieException met een opgegeven foutboodschap en een foutnummer
        /// </summary>
        /// <param name="message">
        /// De foutboodschap die doorgegeven moet worden
        /// </param>
        /// <param name="foutNummer">
        /// Het foutnummer
        /// </param>
        public ValidatieException(string message, FoutNummer foutNummer) : base(message)
        {
            FoutNummer = foutNummer;
        }

        /// <summary>
        /// Instantieert een ValidatieException met een opgegeven foutboodschap en 'inner exception'
        /// </summary>
        /// <param name="message">
        /// De foutboodschap die doorgegeven moet worden
        /// </param>
        /// <param name="inner">
        /// De 'inner exception'
        /// </param>
        public ValidatieException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Instantieert een ValidatieException met een opgegeven SerializationInfo en StreamingContext
        /// </summary>
        /// <param name="info">
        /// De SerializationInfo
        /// </param>
        /// <param name="context">
        /// De StreamingContext
        /// </param>
        public ValidatieException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}