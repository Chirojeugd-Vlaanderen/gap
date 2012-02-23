// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor een actieve afdeling (afdeling met afdelingsjaar)
	/// </summary>
	[DataContract]
	public class ActieveAfdelingInfo : AfdelingInfo
	{
		/// <summary>
		/// De ID van het afdelingsjaar
		/// </summary>
		[DataMember]
		public int AfdelingsJaarID { get; set; }
	}
}
