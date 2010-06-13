// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Chiro.Gap.Workers.Exceptions
{
	/// <summary>
	/// Algemene exception voor GAP
	/// </summary>
	/// <remarks>Gebaseerd op http://blog.gurock.com/articles/creating-custom-exceptions-in-dotnet/</remarks>
	[Serializable]
	public class GapException : Exception
	{
		#region standaardconstructors

		/// <summary>
		/// De standaardconstructor
		/// </summary>
		public GapException() { }

		/// <summary>
		/// Construeer GapException met bericht <paramref name="message"/>.
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		public GapException(string message) : base(message) { }

		/// <summary>
		/// Construeer GapException met bericht <paramref name="message"/> en een inner exception
		/// <paramref name="innerException"/>
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		/// <param name="innerException">Andere exception die de deze veroorzaakt</param>
		public GapException(string message, Exception innerException) : base(message, innerException) { }

		#endregion

		#region serializatie

		/// <summary>
		/// Constructor voor deserializatie.
		/// </summary>
		/// <param name="info">De serializatie-info</param>
		/// <param name="context">De streamingcontext</param>
		protected GapException(SerializationInfo info, StreamingContext context) : base(info, context) 
		{
			if (info == null)
			{
				return;
			}
		}
		
		#endregion
	}
}
