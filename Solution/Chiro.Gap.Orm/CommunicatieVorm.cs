using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Cdf.Data.Entity;
using Chiro.Cdf.Data;
using System.ComponentModel.DataAnnotations;
namespace Chiro.Gap.Orm
{
	[MetadataType(typeof(CommuncatieVormValidatie))]
	public partial class CommunicatieVorm : IEfBasisEntiteit
	{
		/// <summary>
		/// De attributen op de properties van deze geneste class zorgen voor de validatie.
		/// Je kan de attributen niet rechtstreeks op de properties van CommunicatieVorm zetten,
		/// omdat Entity Framework de definitie van deze attributen telkens opnieuw genereert.
		/// (Let op het MetaData-attributt op CommunicatieVorm)
		/// </summary>
		public class CommuncatieVormValidatie
		{
			[Verplicht()]
			public string Nummer { get; set; }
			[StringLengte(320)]
			public string Nota { get; set; }
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
			return 12;
		}

	}
}
