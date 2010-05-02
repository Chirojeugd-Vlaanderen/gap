// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Workers.Exceptions
{
	/// <summary>
	/// Exception die opgegooid kan worden als bepaalde objecten/entiteiten een operatie verhinderen.
	/// </summary>
	/// <typeparam name="TEntiteit"></typeparam>
	[Serializable]
	public class BlokkerendeObjectenException<TEntiteit> : GapException where TEntiteit : IBasisEntiteit
	{
		private IEnumerable<TEntiteit> _objecten;
		private int _aantal;

		/// <summary>
		/// Property die toegang geeft tot een aantal blokkerende objecten.  Deze lijst bevat niet
		/// noodzakelijk alle blokkerende objecten (want het moet allemaal over de lijn).
		/// </summary>
		public IEnumerable<TEntiteit> Objecten
		{
			get
			{
				return _objecten;
			}
			set
			{
				_objecten = value;
			}
		}

		/// <summary>
		/// Totaal aantal blokkerende objecten
		/// </summary>
		public int Aantal
		{
			get
			{
				return _aantal;
			}
			set
			{
				_aantal = value;
			}
		}

		#region standaardconstructors

		/// <summary>
		/// De standaardconstructor
		/// </summary>
		public BlokkerendeObjectenException() : base()
		{
		}

		/// <summary>
		/// Construeer BlokkerendeObjectenException met bericht <paramref name="message"/>.
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		public BlokkerendeObjectenException(string message) : base(message)
		{
		}

		/// <summary>
		/// Construeer BlokkerendeObjectenException met bericht <paramref name="message"/> en een inner exception
		/// <paramref name="innerException"/>
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		/// <param name="innerException">Andere exception die de deze veroorzaakt</param>
		public BlokkerendeObjectenException(string message, Exception innerException) : base(message, innerException)
		{
		}

		#endregion

		#region serializatie

		/// <summary>
		/// Constructor voor deserializatie.
		/// </summary>
		/// <param name="info">De serializatie-info</param>
		/// <param name="context">De streamingcontext</param>
		protected BlokkerendeObjectenException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info == null)
			{
				return;
			}
			_aantal = info.GetInt32("aantal");
			_objecten = (IEnumerable<TEntiteit>)info.GetValue(
				"objecten",
				typeof(IEnumerable<TEntiteit>));
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
				info.AddValue("aantal", _aantal);
				info.AddValue("objecten", _objecten);
			}
		}

		#endregion

		#region custom constructors

		/// <summary>
		/// Construeer BlokkerendeObjectenException met alle relevante info
		/// </summary>
		/// <param name="foutNummer">Foutnummer van de fout die de exception veroorzaakt</param>
		/// <param name="objecten">De objecten die een operatie blokkeren (als er veel zijn, is het maar een selectie)</param>
		/// <param name="aantalTotaal">Totaal aantal blokkerende objecten</param>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		public BlokkerendeObjectenException(
			int foutNummer,
			IEnumerable<TEntiteit> objecten,
			int aantalTotaal,
			string message)
			: base(message)
		{
			FoutNummer = foutNummer;
			_objecten = objecten;
			_aantal = aantalTotaal;
		}

		#endregion
	}
}
