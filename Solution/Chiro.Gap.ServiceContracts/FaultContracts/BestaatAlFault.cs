// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract dat gebruikt wordt als een bestaand(e) entiteit/object verhindert
	/// dat bepaalde gegevens opgeslagen worden
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	[DataContract]
	public class BestaatAlFault<TObject> : GapFault
	{
		/// <summary>
		/// Het bestaande object
		/// </summary>
		[DataMember]
		public TObject Bestaande { get; set; }
	}
}
