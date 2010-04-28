using System;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Minimale persoonsinformatie
	/// </summary>
	[KnownType(typeof(PersoonDetail))]	// Anders pakt de inheritance niet
	[DataContract]
	public class PersoonInfo
	{
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
		public int ChiroLeefTijd
		{
			get;
			set;
		}

		[DataMember]
		public string VoorNaam
		{
			get;
			set;
		}

		[DataMember]
		public string Naam
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? GeboorteDatum
		{
			get;
			set;
		}

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
