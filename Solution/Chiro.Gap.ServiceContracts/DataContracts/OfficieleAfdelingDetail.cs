using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
		/// OfficieleAfdelingID
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
