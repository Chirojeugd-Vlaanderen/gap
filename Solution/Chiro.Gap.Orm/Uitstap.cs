using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Via een partial klasse de gegenereerde klasse uitstap uitbreiden tot een IEfBasisEntiteit
	/// </summary>
	public partial class Uitstap: IEfBasisEntiteit
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
			get
			{
				return this.VersieStringGet();
			}
			set
			{
				this.VersieStringSet(value);
			}
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
			IEfBasisEntiteit andere = obj as Uitstap;
			// Als obj geen uitstap is, wordt andere null.

			return andere != null && (ID != 0) && (ID == andere.ID)
				|| (ID == 0 || andere != null && andere.ID == 0) && base.Equals(andere);

			// Is obj geen Uitstap, dan is de vergelijking altijd vals.
			// Hebben beide objecten een ID verschillend van 0, en zijn deze
			// ID's gelijk, dan zijn de objecten ook gelijk.  Zo niet gebruiken we
			// base.Equals, wat eigenlijk niet helemaal correct is.
		}
	}
}
