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
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model.Exceptions
{
    /// <summary>
    /// Exception die opgegooid kan worden als bepaalde objecten/entiteiten een operatie verhinderen.
    /// </summary>
    /// <typeparam name="TEntiteit">
    /// Het blokkerende object
    /// </typeparam>
    [Serializable]
    public class BlokkerendeObjectenException<TEntiteit> : GapException where TEntiteit : BasisEntiteit
    {
        private IList<TEntiteit> _objecten;
        private int _aantal;

        /// <summary>
        /// Property die toegang geeft tot een aantal blokkerende objecten.  Deze lijst bevat niet
        /// noodzakelijk alle blokkerende objecten (want het moet allemaal over de lijn).
        /// </summary>
        public IList<TEntiteit> Objecten
        {
            get { return _objecten; }
            set { _objecten = value; }
        }

        /// <summary>
        /// Totaal aantal blokkerende objecten
        /// </summary>
        public int Aantal
        {
            get { return _aantal; }
            set { _aantal = value; }
        }

        #region standaardconstructors

        /// <summary>
        /// De standaardconstructor
        /// </summary>
        public BlokkerendeObjectenException()
        {
        }

        /// <summary>
        /// Construeer BlokkerendeObjectenException met bericht <paramref name="message"/>.
        /// </summary>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        public BlokkerendeObjectenException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Construeer BlokkerendeObjectenException met bericht <paramref name="message"/> en een inner exception
        /// <paramref name="innerException"/>
        /// </summary>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        /// <param name="innerException">
        /// Andere exception die de deze veroorzaakt
        /// </param>
        public BlokkerendeObjectenException(string message, Exception innerException)
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
        protected BlokkerendeObjectenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
            {
                return;
            }

            _aantal = info.GetInt32("aantal");
            _objecten = (IList<TEntiteit>)info.GetValue(
                "objecten",
                typeof(IList<TEntiteit>));
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
            info.AddValue("aantal", _aantal);
            info.AddValue("objecten", _objecten);
        }

        #endregion

        #region custom constructors

        /// <summary>
        /// Construeer BlokkerendeObjectenException met alle relevante info
        /// </summary>
        /// <param name="objecten">
        /// De objecten die een operatie blokkeren (als er veel zijn, is het maar een selectie)
        /// </param>
        /// <param name="aantalTotaal">
        /// Totaal aantal blokkerende objecten
        /// </param>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        public BlokkerendeObjectenException(
            IList<TEntiteit> objecten,
            int aantalTotaal,
            string message)
            : base(message)
        {
            _objecten = objecten;
            _aantal = aantalTotaal;
        }

        /// <summary>
        /// Construeer BlokkerendeObjectenException voor precies 1 blokkerend object
        /// </summary>
        /// <param name="obj">
        /// Oject dat een operatie blokkeert
        /// </param>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        public BlokkerendeObjectenException(
            TEntiteit obj,
            string message)
            : base(message)
        {
            _objecten = new[] { obj };
            _aantal = 1;
        }

        #endregion
    }
}