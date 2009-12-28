using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace Chiro.Gap.Orm
{
	[DataContract]
	public enum GeslachtsType
	{
		[EnumMember]
		Man = 1
			,
		[EnumMember]
		Vrouw = 2
			,
		[EnumMember]
		Onbekend = 0
	}

	[MetadataType(typeof(Persoon_Validatie))]
	public partial class Persoon : IEfBasisEntiteit
	{
		/// <summary>
		/// Nested class die toelaat om validatie properties op te zetten, en die gereferenced wordt door het MetadataType attribute
		/// Dit kan niet op de echte class, want die wordt gegenereerd door de EF Designer
		/// </summary>
		public class Persoon_Validatie
		{
			[Verplicht(), StringLengte(160), StringMinimumLengte(2)]
			[DisplayName("Familienaam")]
			public string Naam { get; set; }

			[Verplicht()]
			[StringLengte(60), StringMinimumLengte(2)]
			public string VoorNaam { get; set; }

			[Verplicht()]
			public Chiro.Gap.Orm.GeslachtsType Geslacht { set; get; }

			[DisplayName("AD-Nummer")]
			public Nullable<int> AdNummer { set; get; }

			//[DisplayFormat(DataFormatString="{0:d}", ApplyFormatInEditMode=true, ConvertEmptyStringToNull=true)]
			[DataType(DataType.Date)]
			public DateTime? GeboorteDatum { get; set; }

		}

		private bool _teVerwijderen = false;

		public bool TeVerwijderen
		{
			get { return _teVerwijderen; }
			set { _teVerwijderen = value; }
		}

		public string VersieString
		{
			get { return this.VersieStringGet(); }
			set { this.VersieStringSet(value); }
		}

		#region Identity en equality

		public override int GetHashCode()
		{
			return 3;
		}

		public override bool Equals(object obj)
		{
			IEfBasisEntiteit andere = obj as Persoon;
			// Als obj geen GelieerdePersoon is, wordt andere null.

			if (andere == null)
			{
				return false;
			}
			else
			{
				return (ID != 0) && (ID == andere.ID)
				    || (ID == 0 || andere.ID == 0) && base.Equals(andere);
			}

			// Is obj geen GelieerdePersoon, dan is de vergelijking altijd vals.
			// Hebben beide objecten een ID verschillend van 0, en zijn deze
			// ID's gelijk, dan zijn de objecten ook gelijk.  Zo niet gebruiken we
			// base.Equals, wat eigenlijk niet helemaal correct is.
		}

		#endregion

		public GeslachtsType Geslacht
		{
			get { return (GeslachtsType)this.GeslachtsInt; }
			set { this.GeslachtsInt = (int)value; }
		}

		public string VolledigeNaam
		{
			get { return String.Format("{0} {1}", VoorNaam, Naam); }
		}

	}
}
