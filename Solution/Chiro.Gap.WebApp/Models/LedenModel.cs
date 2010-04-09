// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor het tonen/bewerken van lidinfo.  Vrij omslachtig; mogelijk kan dat beter (TODO)
	/// </summary>
	public class LedenModel : MasterViewModel
	{
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
		public IEnumerable<FunctieInfo> AlleFuncties { get; set; }

		/// <summary>
		/// ID's van geselecteerde functies in de 'functiecheckboxlist'
		/// </summary>
		public IEnumerable<int> FunctieIDs { get; set; }

		public LidInfo HuidigLid { get; set; }

		/// <summary>
		/// Standaardconstructor; er zijn geen afdelingen geselecteerd.
		/// </summary>
		public LedenModel() : base() 
		{
			AfdelingIDs = new List<int>();
		}
	}
}
