// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract voor beperkte informatie over een land
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
