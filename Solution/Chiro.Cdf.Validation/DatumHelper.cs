// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

namespace Chiro.Cdf.Validation
{
	/// <summary>
	/// Klasse die momenteel enkel dient om leeftijden mee te berekenen.
	/// (Ik vraag me echt af waarom dat geen standaardfunctionaiteit van DateTime is.)
	/// </summary>
	public static class DatumHelper
	{
		/// <summary>
		/// Bepaalt de huidige leeftijd in jaren van iemand met gegeven <paramref name="geboorteDatum"/>.
		/// </summary>
		/// <param name="geboorteDatum">Geboortedatum van te 'agen' persoon</param>
		/// <returns>Leeftijd in jaren</returns>
		public static int LeefTijd(DateTime geboorteDatum)
		{
			return LeefTijd(geboorteDatum, DateTime.Now);
		}

		/// <summary>
		/// Bepaalt de leeftijd in jaren van iemand die geboren is in <paramref name="geboorteDatum"/>
		/// op het moment bepaald door <paramref name="referentie"/>
		/// </summary>
		/// <param name="geboorteDatum">De geboortedatum</param>
		/// <param name="referentie">Datum waarop de leeftijd bepaald moet worden</param>
		/// <returns>De leeftijd in jaren</returns>
		public static int LeefTijd(DateTime geboorteDatum, DateTime referentie)
		{
			int jaren = referentie.Year - geboorteDatum.Year;

			return (referentie.Month < geboorteDatum.Month ||
					(referentie.Month == geboorteDatum.Month &&
						referentie.Day < geboorteDatum.Day)) ? --jaren : jaren;
		}
	}
}
