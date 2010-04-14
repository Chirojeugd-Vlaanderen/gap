using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

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
