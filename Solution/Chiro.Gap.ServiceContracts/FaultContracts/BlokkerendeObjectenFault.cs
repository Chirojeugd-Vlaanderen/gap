// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract dat gebruikt wordt als bestaande entiteiten/objecten een operatie verhinderen
	/// </summary>
	[DataContract]
	public class BlokkerendeObjectenFault<TObject> : GapFault
	{
		[DataMember]
		public IEnumerable<TObject> Objecten { get; set; }

		[DataMember]
		public int Aantal { get; set; }
	}
}
