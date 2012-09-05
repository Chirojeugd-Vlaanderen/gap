// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// De klasse Gav heeft eigenlijk een verkeerde naam, en zou beter Account heten. Gav wordt op termijn een rol
	/// die aan een gebruikersrecht gekoppeld kan worden.
	/// 
	/// Deze klasse stelt dus gewoon een login voor.
	/// </summary>
	public partial class Gav : IEfBasisEntiteit
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
		/// <returns>Een int waarmee we het object kunnen identificeren</returns>
		public override int GetHashCode()
		{
			return 11;
		}
	}
}
