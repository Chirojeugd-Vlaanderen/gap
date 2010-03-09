// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Instantieert een CommunicatieVorm-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
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
			[DataType(DataType.MultilineText)]
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
