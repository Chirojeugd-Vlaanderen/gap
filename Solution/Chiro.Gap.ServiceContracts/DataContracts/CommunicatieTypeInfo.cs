using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	[DataContract]
	public class CommunicatieTypeInfo
	{
		[DataMember]
		public int ID { get; set; }
		[DataMember]
		public string Omschrijving { get; set; }
		[DataMember]
		public string Validatie { get; set; }
		[DataMember]
		public string Voorbeeld { get; set; }
	}
}
