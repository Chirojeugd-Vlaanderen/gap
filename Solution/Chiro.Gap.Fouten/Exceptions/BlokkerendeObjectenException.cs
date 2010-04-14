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

namespace Chiro.Gap.Fouten.Exceptions
{
	/// <summary>
	/// Exception die opgegooid kan worden als bepaalde objecten/entiteiten een operatie verhinderen.
	/// </summary>
	[Serializable]
	public class BlokkerendeObjectenException<TFoutCode, TEntiteit>: FoutCodeException<TFoutCode> where TEntiteit: IBasisEntiteit
	{
		private IEnumerable<TEntiteit> _objecten;
		private int _aantal;

		/// <summary>
		/// Creeert een exception met gegeven foutcode en 1 blokkerende object
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		/// <param name="probleemObject">object dat het probleem veroorzaakt</param>
		public BlokkerendeObjectenException(TFoutCode foutCode, TEntiteit probleemObject)
			: this(foutCode, new TEntiteit[] {probleemObject}, 1, null, null) { }

		/// <summary>
		/// Creeert een exception met gegeven foutcode en blokkerende objecten
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		/// <param name="objecten">(Mogelijk beperkte) rij objecten die de exception veroorzaken</param>
		/// <param name="aantal">Totaal aantal blokkerende objecten</param>
		/// <param name="message">String die meer informatie over de exception geeft</param>
		public BlokkerendeObjectenException(
			TFoutCode foutCode, 
			IEnumerable<TEntiteit> objecten,
			int aantal)
			: this(foutCode, objecten, aantal, null, null) { }

		/// <summary>
		/// Creeert een exception met gegeven foutcode en blokkerende objecten
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		/// <param name="objecten">(Mogelijk beperkte) rij objecten die de exception veroorzaken</param>
		/// <param name="aantal">Totaal aantal blokkerende objecten</param>
		/// <param name="message">String die meer informatie over de exception geeft</param>
		public BlokkerendeObjectenException(
			TFoutCode foutCode,
			IEnumerable<TEntiteit> objecten,
			int aantal,
			string message)	: this(foutCode, objecten, aantal, message, null) { }


		/// <summary>
		/// Creeert een exception met gegeven foutcode en blokkerende objecten
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		/// <param name="objecten">(Mogelijk beperkte) rij objecten die de exception veroorzaken</param>
		/// <param name="aantal">Totaal aantal blokkerende objecten</param>
		/// <param name="message">String die meer informatie over de exception geeft</param>
		/// <param name="inner">Inner exception</param>
		public BlokkerendeObjectenException(
			TFoutCode foutCode, 
			IEnumerable<TEntiteit> objecten,
			int aantal,
			string message, 
			Exception inner)
			: base(foutCode, message, inner)
		{
			_aantal = aantal;
			_objecten = objecten;
		}

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
	}
}
