// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// In het algemeen bevat het GelieerdePersonenModel informatie over slechts 1 persoon.
	/// Deze informatie zit dan in <c>HuidigePersoon</c>.
	/// <para />
	/// Wanneer dit model gebruikt wordt voor het toevoegen van een nieuwe persoon, dan
	/// bevat het ook mogelijke gelijkaardige personen (<c>GelijkaardigePersonen</c>) en
	/// een boolean <c>Forceer</c> die aangeeft of een nieuwe persoon geforceerd moet worden
	/// ondanks gevonden gelijkaardige personen.
	/// </summary>
	public class GelieerdePersonenModel : MasterViewModel
	{
		/// <summary>
		/// Informatie over een te tonen of te wijzigen persoon
		/// </summary>
		public PersoonDetail HuidigePersoon { get; set; }

		/// <summary>
		/// Lijst met info over eventueel gelijkaardige personen
		/// </summary>
		public IEnumerable<PersoonDetail> GelijkaardigePersonen { get; set; }

		/// <summary>
		/// Geeft aan of een persoon ook toegevoegd moet worden als er al gelijkaardige
		/// personen bestaan.
		/// </summary>
		public bool Forceer { get; set; }

		/// <summary>
		/// Een eventuele ID als een broer zus waarvan de NIEUWE persoon gemaakt wordt gekend is.
		/// </summary>
		public int BroerzusID { get; set; }
	}
}