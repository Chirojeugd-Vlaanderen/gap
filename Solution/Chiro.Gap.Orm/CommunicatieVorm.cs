// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
		private bool _teVerwijderen;

		/// <summary>
		/// Wordt gebruikt om te verwijderen entiteiten mee te markeren
		/// </summary>
		public bool TeVerwijderen
		{
			get { return _teVerwijderen; }
			set { _teVerwijderen = value; }
		}

		/// <summary>
		/// Geeft stringrepresentatie van Versie weer (hex).
		/// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
		/// </summary>
		public string VersieString
		{
			get { return this.VersieStringGet(); }
			set { this.VersieStringSet(value); }
		}

		/// <summary>
		/// Een arbitraire waarde waarmee we het object kunnen identificeren
		/// </summary>
		/// <returns>Een int waarmee we het object kunnen herkennen</returns>
		public override int GetHashCode()
		{
			return 12;
		}

		#region ICommunicatie Members

		/// <summary>
		/// De regular expressie waar de waarde voor dit communicatietype
		/// aan moet voldoen
		/// </summary>
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
