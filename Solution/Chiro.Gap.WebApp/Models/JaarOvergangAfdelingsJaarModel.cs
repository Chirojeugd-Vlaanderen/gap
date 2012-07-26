// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor het vastleggen van de afdelingsjaren dit werkJaar
	/// </summary>
	public class JaarOvergangAfdelingsJaarModel : MasterViewModel
	{
		/// <summary>
		/// Afdelingsjaren voor dit werkJaar
		/// </summary>
		public AfdelingDetail[] Afdelingen { get; set; }

        /// <summary>
        /// Lijst van alle officiele afdelingen
        /// </summary>
		public OfficieleAfdelingDetail[] OfficieleAfdelingen { get; set; }

        /// <summary>
        /// Als de verdeling van toepassing is op een toekomstig werkJaar, bevat
        /// deze property dat werkJaar
        /// </summary>
		public int NieuwWerkjaar { get; set; }

        /// <summary>
        /// Is <c>true</c> als de gebruiker ervoor kiest om een voorstel te doen
        /// om de huidige leden over te zetten naar de nieuwe afdelingsjaren
        /// </summary>
		public bool LedenMeteenInschrijven { get; set; }
	}
}
