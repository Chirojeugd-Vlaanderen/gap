﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Chiro.Cdf.Data
{
	/// <summary>
	/// Exception die opgegooid kan worden als een bepaalde entiteit al bestaat
	/// </summary>
	/// <typeparam name="TEntiteit"></typeparam>
	[Serializable]
	public class DubbeleEntiteitException<TEntiteit> : Exception where TEntiteit : IBasisEntiteit
	{
		private TEntiteit _bestaande;

		/// <summary>
		/// Property die toegang geeft tot het reeds bestaande object
		/// </summary>
		public TEntiteit Bestaande
		{
			get
			{
				return _bestaande;
			}
			set
			{
				_bestaande = value;
			}
		}

		#region standaardconstructors

		/// <summary>
		/// De standaardconstructor
		/// </summary>
		public DubbeleEntiteitException() : this(null, null)
		{
		}

		/// <summary>
		/// Construeer DubbeleEntiteitException met bericht <paramref name="message"/>.
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		public DubbeleEntiteitException(string message) : this(message, null)
		{
		}

		/// <summary>
		/// Construeer DubbeleEntiteitException met bericht <paramref name="message"/> en een inner exception
		/// <paramref name="innerException"/>
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		/// <param name="innerException">Andere exception die de deze veroorzaakt</param>
		public DubbeleEntiteitException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		#endregion

		#region serializatie

		/// <summary>
		/// Constructor voor deserializatie.
		/// </summary>
		/// <param name="info">De serializatie-info</param>
		/// <param name="context">De streamingcontext</param>
		protected DubbeleEntiteitException(SerializationInfo info, StreamingContext context)
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
		/// <param name="info">Serializatie-info waarin eigenschappen van exception bewaard moeten worden</param>
		/// <param name="context">De streamingcontext</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("bestaande", _bestaande);
			}
		}

		#endregion
        
		#region custom constructors

		/// <summary>
		/// Creeert een Exception omdat er al een entiteit zoals <paramref name="entiteit"/> al bestaat.
		/// </summary>
		/// <param name="entiteit">De bestaande entiteit, die de nieuwe in de weg staat.</param>
		public DubbeleEntiteitException(TEntiteit entiteit)
			: this(null, null)
		{
			_bestaande = entiteit;
		}

		#endregion
	}
}