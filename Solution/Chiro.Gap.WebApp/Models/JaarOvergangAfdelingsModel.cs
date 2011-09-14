// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor het lijstje van actieve en niet-actieve afdelingen
	/// </summary>
	public class JaarOvergangAfdelingsModel : MasterViewModel
	{
		public JaarOvergangAfdelingsModel()
		{
			Afdelingen = new List<AfdelingInfo>();
			
            // LET OP: GezokenAfdelingsIDs moet hier null zijn, en geen lege lijst,
            // omdat ik het [Verplicht] attribuut gebruik om te controleren of de groep
            // niet vergeten is afdelingen aan te klikken.
            
            GekozenAfdelingsIDs = null;
		}

		/// <summary>
		/// Afdelingen die al actief zijn dit werkjaar (met afdelingsjaar dus)
		/// </summary>
		public IEnumerable<AfdelingInfo> Afdelingen { get; set; }

        /// <summary>
        /// Gekozen afdelingen die er volgend jaar zullen zijn.  Er moet er minstens 1 gekozen
        /// zijn.  (Wel opletten dat je dit model dan niet gebruikt voor kaderploegen! Zie ook TODO #1124)
        /// </summary>
        [Verplicht]
        [DisplayName(@"Geselecteerde afdelingen")]
		public IEnumerable<int> GekozenAfdelingsIDs { get; set; }
	}
}
