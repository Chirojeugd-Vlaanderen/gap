// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract dat een foutcode van type <typeparamref name="T"/> bevat
	/// </summary>
	[DataContract]
	public class GapFault
	{
		/// <summary>
		/// De foutcode
		/// </summary>
		[DataMember]
		public int FoutNummer { get; set; }
	}
}
