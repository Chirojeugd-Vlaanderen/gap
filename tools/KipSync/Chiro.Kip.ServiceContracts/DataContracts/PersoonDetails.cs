using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract dat typisch gebruikt zal worden om een onbekende persoon naar kipadmin te sturen.
	/// Het bevat een hoop informatie over de persoon, in de hoop dat we op die manier kunnen uitvissen
	/// over wie in Kipadmn het gaat.
	/// </summary>
	[DataContract]
	public class PersoonDetails
	{
		[DataMember]
		public Persoon Persoon { get; set; }
		[DataMember]
		public Adres Adres { get; set; }
		[DataMember]
		public AdresTypeEnum AdresType { get; set; }
		[DataMember]
		public IEnumerable<CommunicatieMiddel> Communicatie { get; set; }
	}
}
