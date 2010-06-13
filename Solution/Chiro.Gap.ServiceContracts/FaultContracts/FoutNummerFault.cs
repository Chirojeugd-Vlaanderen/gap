// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Gap.Domain;
using System.Runtime.Serialization;
namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	public class FoutNummerFault : GapFault
	{
		/// <summary>
		/// De foutcode
		/// </summary>
		[DataMember]
		public FoutNummer FoutNummer { get; set; }
	}
}
