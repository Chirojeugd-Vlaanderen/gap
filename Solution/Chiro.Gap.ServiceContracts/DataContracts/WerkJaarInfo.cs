using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	[DataContract]
	public class WerkJaarInfo
	{
		/// <summary>
		/// ID van het *groepswerkjaar*.
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// Kalenderjaar waarin het werkjaar *begint*.
		/// </summary>
		[DataMember]
		public int WerkJaar { get; set; }
                               

	}
}
