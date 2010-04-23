// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Datacontract voor een actieve afdeling (afdeling met afdelingsjaar)
	/// </summary>
	[DataContract]
	public class ActieveAfdelingInfo: AfdelingInfo
	{
		[DataMember]
		public int AfdelingsJaarID { get; set; }
	}
}
