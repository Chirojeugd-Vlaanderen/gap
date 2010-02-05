using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Een afdelingsjaar bepaalt welke geboortejaren, geslacht en officiele afdelingen
	/// in een bepaald werkjaar overeenkomen met een gegeven afdeling.
	/// </summary>
	[MetadataType(typeof(AfdelingsJaarAttributen))]
	public partial class AfdelingsJaar : IEfBasisEntiteit
	{
		/// <summary>
		/// Geneste klasse om attributen te definieren.
		/// (Op de echte property's kan dat niet, want die zijn 
		/// automatisch gegenereerd)
		/// </summary>
		public class AfdelingsJaarAttributen
		{
			[DisplayName("Geboortejaar van")]
			[Verplicht()]
			public int GeboorteJaarVan { get; set; }

			[DisplayName("Geboortejaar tot")]
			[Verplicht()]
			public int GeboorteJaarTot { get; set; }
		}

		/// <summary>
		/// De constructor maakt een lege gemengde afdeling
		/// </summary>
		public AfdelingsJaar()
		{
			Geslacht = GeslachtsType.Gemengd;
		}

		public GeslachtsType Geslacht
		{
			get { return (GeslachtsType)this.GeslachtsInt; }
			set { this.GeslachtsInt = (int)value; }
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

		public override int GetHashCode()
		{
			return 16;
		}
	}
}
