// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Cdf.Data
{
	/// <summary>
	/// Basisentiteit.  Alle entity's moeten deze interface implementeren, onafhankelijk van het ORM-framework
	/// </summary>
	public interface IBasisEntiteit
	{
		/// <summary>
		/// Iedere entity wordt geidentificeerd door een
		/// integer.
		/// </summary>
		int ID { get; set; }

		/// <summary>
		/// Timestamp die het object in de database heeft.
		/// De bedoeling is niet dat de gebruiker hier iets
		/// mee doet, de timestamp wordt enkel gebruikt om
		/// aan concurrency control te doen.
		/// </summary>
		byte[] Versie { get; set; }

		/// <summary>
		/// Geeft stringrepresentatie van Versie weer (hex).
		/// Nodig om versie te bewaren in MVC view.
		/// </summary>
		string VersieString { get; set; }

		/// <summary>
		/// Deze property geeft aan of de entity verwijderd
		/// moet worden bij terugsturen naar de service.
		/// </summary>
		bool TeVerwijderen { get; set; }
	}
}
