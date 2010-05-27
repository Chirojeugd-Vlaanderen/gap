using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Klein datacontractje om zowel PersoonID als GelieerdePersoonID over de lijn te sturen
	/// </summary>
	[DataContract]
	public class PersoonIDs
	{
		[DataMember]
		public int PersoonID { get; set; }

		[DataMember]
		public int GelieerdePersoonID { get; set; }
	}
}
