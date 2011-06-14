// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract voor een fout waar een foutnummer aan toegekend werd
	/// </summary>
	public class FoutNummerFault : GapFault
	{
		/// <summary>
		/// De foutcode
		/// </summary>
		[DataMember]
		public FoutNummer FoutNummer { get; set; }

		/// <summary>
		/// Meer uitleg over het probleem
		/// </summary>
		[DataMember]
		public string Bericht { get; set; }
	}
}
