// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Een afdelingsjaar bepaalt welke geboortejaren, geslacht en officiële afdelingen
	/// in een bepaald werkjaar overeenkomen met een gegeven afdeling.
	/// </summary>
	public partial class AfdelingsJaar : IEfBasisEntiteit
	{
		/// <summary>
		/// Instantieert een lege gemengde afdeling
		/// </summary>
		public AfdelingsJaar()
		{
			Geslacht = GeslachtsType.Gemengd;
		}

		/// <summary>
		/// Enumwaarde voor het 'geslacht' van de afdeling
		/// </summary>
		public GeslachtsType Geslacht
		{
			get { return (GeslachtsType)GeslachtsInt; }
			set { GeslachtsInt = (int)value; }
		}

		private bool _teVerwijderen = false;

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

		// Custom Equals en GetHashCode; geeft mogelijk problemen bij deserializatie

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
			return this.ChiroEquals(obj);
		}
	}
}
