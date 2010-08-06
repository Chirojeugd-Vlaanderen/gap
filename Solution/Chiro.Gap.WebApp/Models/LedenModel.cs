// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor het tonen/bewerken van lidinfo.  Vrij omslachtig; mogelijk kan dat beter (TODO)
	/// </summary>
	public class LedenModel : MasterViewModel
	{
        /// <summary>
        /// De standaardconstructor; er zijn geen afdelingen geselecteerd.
        /// </summary>
        public LedenModel()
        {
            AlleAfdelingen = new List<AfdelingDetail>();
            AfdelingIDs = new List<int>();
            AlleFuncties = new List<FunctieDetail>();
            FunctieIDs = new List<int>();
            HuidigLid = new PersoonLidInfo();
        }

		/// <summary>
		/// Rij met alle afdelingen van het groepswerkjaar
		/// </summary>
		public IEnumerable<AfdelingDetail> AlleAfdelingen { get; set; }

		/// <summary>
		/// Bevat de huidige afdelingen van een lid, of de geselecteerde na de ui, voor leiding
		/// </summary>
		public IList<int> AfdelingIDs { get; set; }

		/// <summary>
		/// Bevat de huidige of de nieuwe gewenste afdeling voor een kind
		/// </summary>
		public int AfdelingID { get; set; }

		/// <summary>
		/// Alle functies die relevant zouden kunnen zijn voor HuidigLid. (afhankelijk van lidsoort
		/// en groepswerkjaar.)
		/// </summary>
		public IEnumerable<FunctieDetail> AlleFuncties { get; set; }

		/// <summary>
		/// ID's van geselecteerde functies in de 'functiecheckboxlist'
		/// </summary>
		public IEnumerable<int> FunctieIDs { get; set; }

		public PersoonLidInfo HuidigLid { get; set; }
	}
}
