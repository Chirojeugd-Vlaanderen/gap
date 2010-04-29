// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Instantieert een CommunicatieVorm-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
	public partial class CommunicatieVorm : IEfBasisEntiteit, ICommunicatie
	{
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

		#region ICommunicatie Members

		string ICommunicatie.CommunicatieTypeValidatie
		{
			get
			{
				return CommunicatieType.Validatie;
			}
			set
			{
				CommunicatieType.Validatie = value;
			}
		}

		#endregion
	}
}
