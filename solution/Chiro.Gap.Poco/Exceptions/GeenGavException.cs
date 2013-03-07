// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Chiro.Gap.Poco.Model.Exceptions
{
    /// <summary>
    /// Exceptie voor operatie op objecten waarvoor de
    /// gebruiker geen GAV-rechten heeft.
    /// </summary>
    /// <remarks>
    /// Deze klasse doet eigenlijk niets speciaals.
    /// </remarks>
    [Serializable]
    public class GeenGavException : GapException
    {
        #region standaardconstructors

        /// <summary>
        /// De standaardconstructor
        /// </summary>
        public GeenGavException() : this(null, null)
        {
        }

        /// <summary>
        /// Construeer GeenGavException met bericht <paramref name="message"/>.
        /// </summary>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        public GeenGavException(string message) : this(message, null)
        {
        }

        /// <summary>
        /// Construeer GeenGavException met bericht <paramref name="message"/> en een inner exception
        /// <paramref name="innerException"/>
        /// </summary>
        /// <param name="message">
        /// Technische info over de exception; nuttig voor developer
        /// </param>
        /// <param name="innerException">
        /// Andere exception die de deze veroorzaakt
        /// </param>
        public GeenGavException(string message, Exception innerException) : base(message, innerException)
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
        protected GeenGavException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}