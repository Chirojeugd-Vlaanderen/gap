// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract dat gebruikt wordt als een bestaande entiteit/object een operatie verhindert
	/// </summary>
	[DataContract]
	public class BestaatAlFault<TObject> : GapFault
	{
		[DataMember]
		public TObject Bestaande { get; set; }
	}
}
