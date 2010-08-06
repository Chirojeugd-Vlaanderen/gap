// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.Workers.Exceptions
{
	/// <summary>
	/// Exception die toelaat om meerdere foutboodschappen over de members van een object mee te sturen.
	/// </summary>
	/// <remarks></remarks>
	[Serializable]
	public class OngeldigObjectException : GapException
	{
		// TODO: Dit wordt blijkbaar enkel gebruikt voor adressen.  Is heel die constructie dan wel nodig?
		// Misschien is een AdresException wel even goed.

		private IDictionary<string, FoutBericht> _berichten = new Dictionary<string, FoutBericht>();

		/// <summary>
		/// Berichten bij de exception.  De key is de component van het adres waar de fout betrekking
		/// op heeft, de value is het foutbericht zelf.
		/// </summary>
		public IDictionary<string, FoutBericht> Berichten
		{
			get { return _berichten; }
			set { _berichten = value; }
		}

		#region standaardconstructors

		/// <summary>
		/// De standaardconstructor
		/// </summary>
		public OngeldigObjectException() { }

		/// <summary>
		/// Construeer OngeldigObjectException met bericht <paramref name="message"/>.
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		public OngeldigObjectException(string message) : base(message) { }

		/// <summary>
		/// Construeer OngeldigObjectException met bericht <paramref name="message"/> en een inner exception
		/// <paramref name="innerException"/>
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		/// <param name="innerException">Andere exception die de deze veroorzaakt</param>
		public OngeldigObjectException(string message, Exception innerException) : base(message, innerException) { }

		#endregion

		#region serializatie

		/// <summary>
		/// Constructor voor deserializatie.
		/// </summary>
		/// <param name="info">De serializatie-info</param>
		/// <param name="context">De streamingcontext</param>
		protected OngeldigObjectException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				_berichten = (IDictionary<string, FoutBericht>)info.GetValue("berichten", typeof(IDictionary<string, FoutBericht>));
			}
		}

		/// <summary>
		/// Serializatie van de exception
		/// </summary>
		/// <param name="info">Serializatie-info waarin eigenschappen van exception bewaard moeten worden</param>
		/// <param name="context">De streamingcontext</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("berichten", _berichten);
			}
		}

		#endregion

		#region custom constructors
		/// <summary>
		/// Construeert OngeldigObjectException met meegeleverde <paramref name="berichten"/>.
		/// </summary>
		/// <param name="berichten">Lijstje met berichten voor de exception</param>
		public OngeldigObjectException(IDictionary<string, FoutBericht> berichten)
			: this()
		{
			_berichten = berichten;
		}
		#endregion
	}
}
