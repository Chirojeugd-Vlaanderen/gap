// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.Fouten.Exceptions
{
	/// <summary>
	/// Exceptie voor operatie op objecten waarvoor de
	/// gebruiker geen GAV-rechten heeft.
	/// </summary>
	/// <remarks>Deze klasse doet eigenlijk niets speciaals.</remarks>
	[Serializable]
	public class GeenGavException : FoutCodeException<GeenGavFoutCode> 
	{ 
		/// <summary>
		/// Creeert een exception met gegeven foutcode
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		/// <param name="message">String die meer informatie over de exception geeft</param>
		public GeenGavException(GeenGavFoutCode foutCode, string message) : base(foutCode, message) { }
	}
}
