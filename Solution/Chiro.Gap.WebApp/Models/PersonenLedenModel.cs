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
	/// In het algemeen bevat het GelieerdePersonenModel informatie over slechts 1 persoon.
	/// Deze informatie zit dan in <c>HuidigePersoon</c>.
	/// <para />
	/// Wanneer dit model gebruikt wordt voor het toevoegen van een nieuwe persoon, dan
	/// bevat het ook mogelijke gelijkaardige personen (<c>GelijkaardigePersonen</c>) en
	/// een boolean <c>Forceer</c> die aangeeft of een nieuwe persoon geforceerd moet worden
	/// ondanks gevonden gelijkaardige personen.
	/// </summary>
	public class PersonenLedenModel : MasterViewModel
	{
		/// <summary>
		/// Informatie over een te tonen of te wijzigen persoon
		/// </summary>
		public PersoonLidInfo PersoonLidInfo { get; set; }

		public IEnumerable<AfdelingInfo> AlleAfdelingen { get; set; }

		/// <summary>
		/// Standaardconstructor voor GelieerdePersonenModel
		/// </summary>
		public PersonenLedenModel(): base()
		{
		}
	}
}