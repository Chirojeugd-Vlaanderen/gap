using System;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Minimale persoonsinformatie
	/// </summary>
	[KnownType(typeof(PersoonDetail))]	// Anders pakt de inheritance niet
	[DataContract]
	public class PersoonInfo
	{
		[DisplayName(@"AD-nummer")]
		[DataMember]
		public int? AdNummer
		{
			get;
			set;
		}

		[DataMember]
		public int GelieerdePersoonID
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName(@"Chiroleeftijd")]
        [Verplicht]
        [Range(-8, +3, ErrorMessage = "{0} is beperkt van {1} tot {2}.")]
        [DisplayFormat(DataFormatString = "{0:+#0;-#0;#0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
		public int ChiroLeefTijd
		{
			get;
			set;
		}

		[Required]
		[DisplayName(@"Voornaam")]
		[StringLengte(60), StringMinimumLengte(2)]
		[DataMember]
		public string VoorNaam
		{
			get;
			set;
		}

		[Required(), StringLengte(160), StringMinimumLengte(2)]
		[DisplayName(@"Familienaam")]
		[DataMember]
		public string Naam
		{
			get;
			set;
		}

		// [DisplayFormat(DataFormatString="{0:d}", ApplyFormatInEditMode=true, ConvertEmptyStringToNull=true)]
		[DataType(DataType.Date)]
		[DisplayName(@"Geboortedatum")]
		[GeboorteDatumInVerleden]
		[DataMember]
		public DateTime? GeboorteDatum
		{
			get;
			set;
		}

		[Verplicht]
		[DataMember]
		public GeslachtsType Geslacht
		{
			get;
			set;
		}

		/// <summary>
		/// VersieString van de Persoon.
		/// (Die van de gelieerde persoon nemen we niet mee, aangezien bij het wijzigen van een
		/// gelieerde persoon de persoon nagenoeg altijd mee zal wijzigen.  Tenzij enkel de
		/// Chiroleeftijd wordt aangepast natuurlijk.  We zullen zien hoe erg dat is.)
		/// </summary>
		[DataMember]
		public string VersieString { get; set; }
	}
}
