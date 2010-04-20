// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Cdf.Data;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Workers.Exceptions
{
	/// <summary>
	/// Exception die opgegooid kan worden als een bepaalde entiteit al bestaat
	/// </summary>
	[Serializable]
	public class BestaatAlException<TEntiteit>: GapException where TEntiteit: IBasisEntiteit
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
		/// Standaardconstructor
		/// </summary>
		public BestaatAlException() : this(null, null) { }

		/// <summary>
		/// Construeer BestaatAlException met bericht <paramref name="message"/>.
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		public BestaatAlException(string message) : this(message, null) { }

		/// <summary>
		/// Construeer BestaatAlException met bericht <paramref name="message"/> en een inner exception
		/// <paramref name="innerException"/>
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		/// <param name="innerException">Andere exception die de deze veroorzaakt</param>
		public BestaatAlException(string message, Exception innerException)
			: base(message, innerException)
		{
			FoutNummer = FoutNummers.BestaatAl;
		}

		#endregion

		#region serializatie

		/// <summary>
		/// Constructor voor deserializatie.
		/// </summary>
		/// <param name="info">Serializatie-info</param>
		/// <param name="context">Streamingcontext</param>
		protected BestaatAlException(SerializationInfo info, StreamingContext context)
			: base(info, context) 
		{
			if (info != null)
			{
				_bestaande = (TEntiteit)info.GetValue(
					"_bestaande", 
					typeof(TEntiteit));
			}
		}

		/// <summary>
		/// Serializatie van de exception
		/// </summary>
		/// <param name="info">Serializatie-info waarin eigenschappen van exception bewaard moeten worden</param>
		/// <param name="context"></param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("_bestaande", _bestaande);
			}
		}

		#endregion


		#region custom exceptions

		/// <summary>
		/// Creeert een Exception omdat er al een entiteit zoals <paramref name="entiteit"/> al bestaat.
		/// </summary>
		/// <param name="entiteit">De bestaande entiteit, die de nieuwe in de weg staat.</param>
		public BestaatAlException(TEntiteit entiteit)
			: this(null, null)
		{
			_bestaande = entiteit;
		}

		#endregion
	}
}
