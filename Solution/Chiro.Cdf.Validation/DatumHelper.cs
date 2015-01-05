/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace Chiro.Cdf.Validation
{
	/// <summary>
	/// Klasse die momenteel enkel dient om leeftijden mee te berekenen.
	/// (Ik vraag me echt af waarom dat geen standaardfunctionaiteit van DateTime is.)
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Bepaalt de huidige leeftijd in jaren van iemand met gegeven <paramref name="geboorteDatum"/>.
		/// </summary>
		/// <param name="geboorteDatum">Geboortedatum van te 'agen' persoon</param>
		/// <returns>Leeftijd in jaren</returns>
		public static int LeefTijd(this DateTime geboorteDatum)
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
		public static int LeefTijd(this DateTime geboorteDatum, DateTime referentie)
		{
			var jaren = referentie.Year - geboorteDatum.Year;

			return (referentie.Month < geboorteDatum.Month ||
					(referentie.Month == geboorteDatum.Month &&
						referentie.Day < geboorteDatum.Day)) ? --jaren : jaren;
		}
	}
}
