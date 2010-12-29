using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor problemen i.v.m. ontbrekende gegevens van leden.  Per type probleem kan
	/// meegegeven worden hoe vaak het zich voor doet.
	/// </summary>
	[DataContract]
	public class LedenProbleemInfo
	{
		[DataMember]
		public LidProbleem Probleem { get; set; }

		[DataMember]
		public int Aantal { get; set; }
	}
}
