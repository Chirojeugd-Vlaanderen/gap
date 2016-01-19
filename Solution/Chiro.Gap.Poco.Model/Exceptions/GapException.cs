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

namespace Chiro.Gap.Poco.Model.Exceptions
{
    /// <summary>
    /// Algemene exception voor GAP
    /// </summary>
    /// <remarks>
    /// Gebaseerd op http://blog.gurock.com/articles/creating-custom-exceptions-in-dotnet/
    /// </remarks>
    [Serializable]
    public class GapException : Exception
    {
        #region standaardconstructors

        /// <summary>
        /// De standaardconstructor
        /// </summary>
        public GapException()
        {
        }

        /// <summary>
        /// Construeer GapException met bericht <paramref name="message"/>.
        /// </summary>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        public GapException(string message) : base(message)
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
        public GapException(string message, Exception innerException) : base(message, innerException)
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
        protected GapException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
            {
            }
        }

        #endregion
    }
}