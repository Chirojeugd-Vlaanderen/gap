using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Gedetailleerde informatie over een groepswerkjaar
	/// </summary>
	[DataContract]
	public class GroepsWerkJaarDetail
	{
		[DataMember]
		public int WerkJaar { get; set; }
		[DataMember]
		public int WerkJaarID { get; set; }
		[DataMember]
		public WerkJaarStatus Status { get; set; }
		[DataMember]
		public int GroepID { get; set; }
		[DataMember]
		public string GroepNaam { get; set; }
		[DataMember]
		public string GroepPlaats { get; set; }
		[DataMember]
		public string GroepCode { get; set; }
		[DataMember]
		public Niveau GroepNiveau { get; set; }
	}
}
