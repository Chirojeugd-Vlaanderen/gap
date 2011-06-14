// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract dat gebruikt wordt als bestaande entiteiten/objecten een operatie verhinderen
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	[DataContract]
	public class BlokkerendeObjectenFault<TObject> : GapFault
	{
		/// <summary>
		/// De objecten die een operatie verhinderen
		/// </summary>
		[DataMember]
		public IEnumerable<TObject> Objecten { get; set; }

		/// <summary>
		/// Het aantal objecten dat de operatie verhindert
		/// </summary>
		[DataMember]
		public int Aantal { get; set; }
	}
}
