// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Diagnostics;
using System.Text.RegularExpressions;

using Chiro.Gap.Domain;

namespace Chiro.Gap.Validatie
{
	/// <summary>
	/// Klasse die validatieregels controleert voor Communicatievormen
	/// </summary>
	public class CommunicatieVormValidator : Validator<ICommunicatie>
	{
		/// <summary>
		/// Vergelijkt de opgegeven waarde met de regex die in de databank opgegeven is
		/// voor dat communicatieType
		/// </summary>
		/// <param name="cv">De communicatievorm (bv. telefoonnummer, mailadres, ...</param>
		/// <returns>
		/// <c>True</c> als de waarde ('Nummer') voldoet aan de opgegeven Regex voor dat communicatietype,
		/// en anders <c>false</c>
		/// </returns>
		public override bool Valideer(ICommunicatie cv)
		{
			Debug.Assert(cv != null);
			return cv.Nummer != null && Regex.IsMatch(cv.Nummer, cv.CommunicatieTypeValidatie);
		}
	}
}
