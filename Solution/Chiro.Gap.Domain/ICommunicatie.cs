// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.Domain
{
	/// <summary>
	/// Interface voor communicatie, die zowel aan UI-kant als aan businesskant gebruikt kan worden.
	/// </summary>
	public interface ICommunicatie
	{
		/// <summary>
		/// Het telefoonnummer, e-mailadres,...
		/// </summary>
		string Nummer { get; set; }

		/// <summary>
		/// Regular expression waaraan het nummer moet voldoen
		/// </summary>
		string CommunicatieTypeValidatie { get; set; }
	}
}
