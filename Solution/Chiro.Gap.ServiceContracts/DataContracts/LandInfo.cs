using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
    /// TODO (#190): documenteren
	/// </summary>
    [DataContract]
	public class LandInfo
	{
		/// <summary>
		/// Naam van het land.  Op dit moment enkel in het Nederlands :)
		/// </summary>
		[DataMember]
		public string Naam { get; set; }

		/// <summary>
		/// ID van het land; komt uit de database.
		/// </summary>
		[DataMember]
		public int ID { get; set; }
	}
}
