/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model.Exceptions
{
    /// <summary>
    /// Exception die opgegooid kan worden als een bepaalde entiteit al bestaat
    /// </summary>
    /// <typeparam name="TEntiteit">
    /// De bestaande entiteit
    /// </typeparam>
    [Serializable]
    public class BestaatAlException<TEntiteit> : GapException where TEntiteit : BasisEntiteit
    {
        private TEntiteit _bestaande;

        /// <summary>
        /// Property die toegang geeft tot het reeds bestaande object
        /// </summary>
        public TEntiteit Bestaande
        {
            get { return _bestaande; }
            set { _bestaande = value; }
        }

        #region standaardconstructors

        /// <summary>
        /// De standaardconstructor
        /// </summary>
        public BestaatAlException()
            : this(null, null)
        {
        }

        /// <summary>
        /// Construeer BestaatAlException met bericht <paramref name="message"/>.
        /// </summary>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        public BestaatAlException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Construeer BestaatAlException met bericht <paramref name="message"/> en een inner exception
        /// <paramref name="innerException"/>
        /// </summary>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        /// <param name="innerException">
        /// Andere exception die de deze veroorzaakt
        /// </param>
        public BestaatAlException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion

        #region serializatie

        /// <summary>
        /// Constructor voor deserializatie.
        /// </summary>
        /// <param name="info">
        /// De serializatie-info
        /// </param>
        /// <param name="context">
        /// De streamingcontext
        /// </param>
        protected BestaatAlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
            {
                return;
            }

            _bestaande = (TEntiteit)info.GetValue(
                "bestaande",
                typeof(TEntiteit));
        }

        /// <summary>
        /// Serializatie van de exception
        /// </summary>
        /// <param name="info">
        /// Serializatie-info waarin eigenschappen van exception bewaard moeten worden
        /// </param>
        /// <param name="context">
        /// De streamingcontext
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("bestaande", _bestaande);
        }

        #endregion

        #region custom constructors

        /// <summary>
        /// Creëert een Exception omdat er al een entiteit zoals <paramref name="entiteit"/> al bestaat.
        /// </summary>
        /// <param name="entiteit">
        /// De bestaande entiteit, die de nieuwe in de weg staat.
        /// </param>
        public BestaatAlException(TEntiteit entiteit)
            : this(null, null)
        {
            _bestaande = entiteit;
        }

        #endregion
    }
}