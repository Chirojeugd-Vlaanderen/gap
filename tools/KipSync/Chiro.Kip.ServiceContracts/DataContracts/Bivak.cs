using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
	[DataContract]
	public class Bivak
	{
		/// <summary>
		/// ID van de uitstap in GAP.
		/// </summary>
		[DataMember]
		public int UitstapID { get; set; }

		/// <summary>
		/// Stamnummer voor groep die op bivak gaat
		/// </summary>
		[DataMember]
		public string StamNummer { get; set; }

		/// <summary>
		/// Werkjaar van het bivak.
		/// </summary>
		[DataMember]
		public int WerkJaar { get; set; }

		/// <summary>
		/// Begindatum van het bivak
		/// </summary>
		[DataMember]
		public DateTime DatumVan { get; set; }

		/// <summary>
		/// Einddatum van het bivak
		/// </summary>
		[DataMember]
		public DateTime DatumTot { get; set; }
	}
}
