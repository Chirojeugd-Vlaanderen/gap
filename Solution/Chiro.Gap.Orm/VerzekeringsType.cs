// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// VerzekeringsType (loonverlies, beperkte periode, avontuurlijke activiteiten,...)
	/// </summary>
	public partial class VerzekeringsType : IEfBasisEntiteit
	{
		private bool _teVerwijderen;

		/// <summary>
		/// Wordt gebruikt om te verwjderen entiteiten mee te markeren
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
		/// <remarks>Verzekeringstype wordt nooit geüpdatet, dus ook nooit
		/// concurrency.  VersieString is dus niet nodig.</remarks>
		public string VersieString
		{
			get { return null; }
			set { /*Doe niets*/ }
		}

		/// <summary>
		/// De byte-representatie van Versie
		/// </summary>
		/// <remarks>Verzekeringstype wordt nooit geüpdatet, dus ook nooit
		/// concurrency.  Versie is dus niet nodig.</remarks>
		public byte[] Versie
		{
			get { return null; }
			set { /*Doe niets*/ }
		}

		/// <summary>
		/// Een waarde waarmee we het object kunnen identificeren,
		/// overgenomen van de ID
		/// </summary>
		/// <returns>Een int waarmee we het object kunnen identificeren</returns>
		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		/// <summary>
		/// Vergelijkt het huidige object met een ander om te zien of het over
		/// twee instanties van hetzelfde object gaat
		/// </summary>
		/// <param name="obj">Het object waarmee we het huidige willen vergelijken</param>
		/// <returns><c>True</c> als het schijnbaar om twee instanties van hetzelfde object gaat</returns>
		public override bool Equals(object obj)
		{
			IEfBasisEntiteit andere = obj as VerzekeringsType;
			// Als obj geen VerzekeringsType is, wordt andere null.

			return andere != null && (ID != 0) && (ID == andere.ID)
				|| (ID == 0 || andere.ID == 0) && base.Equals(andere);

			// Is obj geen Verzekeringstype, dan is de vergelijking altijd vals.
			// Hebben beide objecten een ID verschillend van 0, en zijn deze
			// ID's gelijk, dan zijn de objecten ook gelijk.  Zo niet gebruiken we
			// base.Equals, wat eigenlijk niet helemaal correct is.
		}
	}
}
