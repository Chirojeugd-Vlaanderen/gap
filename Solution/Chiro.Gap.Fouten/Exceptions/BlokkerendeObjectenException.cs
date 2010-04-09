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

		/// <summary>
		/// Creeert een exception met gegeven foutcode en 1 blokkerende object
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		/// <param name="probleemObject">object dat het probleem veroorzaakt</param>
		public BlokkerendeObjectenException(TFoutCode foutCode, TEntiteit probleemObject)
			: this(foutCode, new TEntiteit[] {probleemObject}, null, null) { }

		/// <summary>
		/// Creeert een exception met gegeven foutcode en blokkerende objecten
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		/// <param name="objecten">Rij objecten die de exception veroorzaken</param>
		public BlokkerendeObjectenException(TFoutCode foutCode, IEnumerable<TEntiteit> objecten)
			: this(foutCode, objecten, null, null) { }

		/// <summary>
		/// Creeert een exception met gegeven foutcode en blokkerende objecten
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		/// <param name="objecten">Rij objecten die de exception veroorzaken</param>
		/// <param name="message">String die meer informatie over de exception geeft</param>
		public BlokkerendeObjectenException(
			TFoutCode foutCode,
			IEnumerable<TEntiteit> objecten,
			string message)	: this(foutCode, objecten, message, null) { }


		/// <summary>
		/// Creeert een exception met gegeven foutcode en blokkerende objecten
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		/// <param name="objecten">Rij objecten die de exception veroorzaken</param>
		/// <param name="message">String die meer informatie over de exception geeft</param>
		/// <param name="inner">Inner exception</param>
		public BlokkerendeObjectenException(
			TFoutCode foutCode, 
			IEnumerable<TEntiteit> objecten, 
			string message, 
			Exception inner)
			: base(foutCode, message, inner)
		{
			_objecten = objecten;
		}

		/// <summary>
		/// Property die toegang geeft tot de blokkerende objecten
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
	}
}
