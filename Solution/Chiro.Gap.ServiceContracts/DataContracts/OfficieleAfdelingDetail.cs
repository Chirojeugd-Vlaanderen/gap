// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Datacontract voor info over officiele afdeling
	/// </summary>
	[DataContract]
	public class OfficieleAfdelingDetail
	{
		/// <summary>
		/// Naam van de officiele afdeling
		/// </summary>
		[DataMember]
		public string Naam { get; set; }

		/// <summary>
		/// De OfficieleAfdelingID
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// Standaard 'geboortejaar van' voor dit werkjaar
		/// </summary>
		[DataMember]
		public int StandaardGeboorteJaarVan { get; set; }

		/// <summary>
		/// Standaard 'geboortejaar tot' voor dit werkjaar
		/// </summary>
		[DataMember]
		public int StandaardGeboorteJaarTot { get; set; }
	}
}
