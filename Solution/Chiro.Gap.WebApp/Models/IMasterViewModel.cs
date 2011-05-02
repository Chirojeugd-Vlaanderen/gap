// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Elk model dat door de master page gebruikt wordt, moet deze interface implementeren.
	/// </summary>
	public interface IMasterViewModel
	{
		/// <summary>
		/// Bepaalt of we op een live- of een testdatabase aan het werken zijn
		/// </summary>
		bool IsLive { get; }

		/// <summary>
		/// ID van de Chirogroep
		/// </summary>
		int GroepID { get; }

		/// <summary>
		/// ID van de Chirogroep
		/// </summary>
		string GroepsNaam { get; }

		/// <summary>
		/// Plaats van de Chirogroep
		/// </summary>
		string Plaats { get; }

		/// <summary>
		/// Het stamnummer wordt niet meer gebruikt als primary key, maar zal nog wel
		/// lang gebruikt worden als handige manier om een groep op te zoeken.
		/// </summary>
		string StamNummer { get; }

		/// <summary>
		/// Titel van de webpagina
		/// </summary>
		string Titel { get; }

		/// <summary>
		/// Kan de GAV meerdere groepen beheren?
		/// </summary>
		bool? MeerdereGroepen { get; }

		/// <summary>
		/// Mededelingen die ergens getoond moeten worden
		/// </summary>
		IList<Mededeling> Mededelingen { get; }

		/// <summary>
		/// Int die het *jaartal* van het huidige werkjaar voor de groep bepaalt.
		/// (Bijv. 2010 voor 2010-2011)
		/// </summary>
		int HuidigWerkJaar { get; }

		/// <summary>
		/// <c>true</c> indien de overgang naar het nieuwe werkjaar kan gebeuren
		/// </summary>
		bool IsInOvergangsPeriode { get; }
	}
}
