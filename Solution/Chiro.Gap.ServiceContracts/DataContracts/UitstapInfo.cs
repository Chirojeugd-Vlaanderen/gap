using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Basisinformatie over een uitstap.
	/// </summary>
	/// <remarks>Startdatum en einddatum zijn <c>DateTime?</c>, opdat we dit
	/// datacontract ook als model zouden kunnen gebruiken in de webappl.  Als
	/// Startdatum en einddatum nullable zijn, dan zijn ze bij het aanmaken
	/// van een nieuwe uitstap gewoon leeg, ipv een nietszeggende datum in het
	/// jaar 1 als ze niet nullable zijn.</remarks>
	[DataContract]
	[KnownType(typeof(UitstapDetail))]
	public class UitstapInfo
	{
		[DataMember]
		public int ID { get; set; }
		[DataMember]
		[Verplicht]
		[DisplayName(@"Omschrijving van de uitstap")]
		public string Naam { get; set; }
		[DataMember]
		[DisplayName(@"Deze uitstap is ons jaarlijks bivak")]
		public bool IsBivak { get; set; }
		[DataMember]
		[DisplayName(@"Begindatum")]
		[DataType(DataType.Date)]
		[Verplicht]
		public DateTime? DatumVan { get; set; }
		[DataMember]
		[DisplayName(@"Einddatum")]
		[DataType(DataType.Date)]
		[Verplicht]
		public DateTime? DatumTot { get; set; }
		[DataMember]
		[DataType(DataType.MultilineText)]
		[DisplayName(@"Opmerkingen")]
		public string Opmerkingen { get; set; }
		[DataMember]
		public string VersieString { get; set; }
	}
}
