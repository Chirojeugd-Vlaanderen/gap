using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	[DataContract]
	public class UitstapDetail
	{
		[DisplayName(@"Omschrijving van de uitstap")]
		public string Naam { get; set; }
		[DisplayName(@"Deze uitstap is ons jaarlijks bivak")]
		public bool IsBivak { get; set; }
		[DisplayName(@"Begindatum")]
		public DateTime DatumVan { get; set; }
		[DisplayName(@"Einddatum")]
		public DateTime DatumTot { get; set; }
		[DataType(DataType.MultilineText)]
		[DisplayName(@"Opmerkingen")]
		public string Opmerkingen { get; set; }
	}
}
