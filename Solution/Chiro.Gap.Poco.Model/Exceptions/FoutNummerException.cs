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
using System.Collections.Generic;
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model.Exceptions
{
    /// <summary>
    /// Specifieke GapException met een foutnummer
    /// </summary>
    [Serializable]
    public class FoutNummerException : GapException
    {
        private FoutNummer _foutNummer;
        private IEnumerable<string> _items;

        #region property's

        /// <summary>
        /// Foutnummer voor de exception.
        /// </summary>
        public FoutNummer FoutNummer
        {
            get { return _foutNummer; }
            set { _foutNummer = value; }
        }

        /// <summary>
        /// Worden gebruikt voor substituties in foutberichten
        /// </summary>
        public IEnumerable<string> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        #endregion

        #region standaardconstructors

        /// <summary>
        /// De standaardconstructor
        /// </summary>
        public FoutNummerException()
        {
        }

        /// <summary>
        /// Construeer GapException met bericht <paramref name="message"/>.
        /// </summary>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        public FoutNummerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Construeer GapException met bericht <paramref name="message"/> en een inner exception
        /// <paramref name="innerException"/>
        /// </summary>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        /// <param name="innerException">
        /// Andere exception die de deze veroorzaakt
        /// </param>
        public FoutNummerException(string message, Exception innerException)
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
        protected FoutNummerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
            {
                return;
            }

            _foutNummer = (FoutNummer)info.GetInt32("foutNummer");
            _items = (IEnumerable<string>)info.GetValue("items", typeof(IEnumerable<string>));
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
            info.AddValue("foutNummer", _foutNummer);
            info.AddValue("items", _items);
        }

        #endregion

        #region custom constructors

        /// <summary>
        /// Construeer GapException met bericht <paramref name="message"/> en foutnummer
        /// <paramref name="foutNummer"/>
        /// </summary>
        /// <param name="foutNummer">
        /// Foutnummer van de fout die de exception veroorzaakte
        /// </param>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        public FoutNummerException(FoutNummer foutNummer, string message)
            : base(message)
        {
            _foutNummer = foutNummer;
        }

        #endregion
    }
}