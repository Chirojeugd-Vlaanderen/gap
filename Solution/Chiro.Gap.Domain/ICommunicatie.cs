using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
