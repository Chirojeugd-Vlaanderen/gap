// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Instantieert een Subgemeente-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
	/// <remarks>
	/// Een straat heeft geen versie (timestamp) in de database.
	/// Dat lijkt me ook niet direct nodig voor een klasse die
	/// bijna nooit wijzigt.
	/// <para/>
	/// Het feit dat er geen timestamp is, wil wel zeggen dat
	/// 'concurrencygewijze' de laatste altijd zal winnen.    
	/// </remarks>
	public partial class WoonPlaats : IEfBasisEntiteit
	{
		#region IBasisEntiteit Members

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
		/// <remarks> SubGemeente wordt nooit geüpdatet, dus ook nooit
		/// concurrency.  VersieString is dus niet nodig.</remarks>
		public string VersieString
		{
			get { return null; }
			set { /*Doe niets*/ }
		}

		#endregion

		/// <summary>
		/// Een arbitraire waarde waarmee we het object kunnen identificeren
		/// </summary>
		/// <returns>Een int waarmee we het object kunnen identificeren</returns>
		public override int GetHashCode()
		{
			return 18;
		}
	}
}
