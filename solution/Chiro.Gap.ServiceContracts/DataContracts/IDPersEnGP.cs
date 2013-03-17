// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Klein datacontractje om zowel PersoonID als GelieerdePersoonID over de lijn te sturen
	/// </summary>
	[DataContract]
	public class IDPersEnGP
	{
		/// <summary>
		/// De ID van de geassocieerde persoon
		/// </summary>
		[DataMember]
		public int PersoonID { get; set; }

		/// <summary>
		/// De ID van de geassocieerde gelieerde persoon
		/// </summary>
		[DataMember]
		public int GelieerdePersoonID { get; set; }
	}
}
