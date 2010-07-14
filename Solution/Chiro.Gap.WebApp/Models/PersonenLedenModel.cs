// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class PersonenLedenModel : MasterViewModel
	{
		public PersonenLedenModel()
		{
			AlleAfdelingen = new List<AfdelingDetail>();
			PersoonLidInfo = new PersoonLidInfo();
		}

		/// <summary>
		/// Informatie over een te tonen of te wijzigen persoon
		/// </summary>
		public PersoonLidInfo PersoonLidInfo { get; set; }

		public IEnumerable<AfdelingDetail> AlleAfdelingen { get; set; }

		/// <summary>
		/// Geef de gebruiker de optie om een verzekering te geven voor loonverlies
		/// </summary>
		public bool KanVerzekerenLoonVerlies { get; set; }

		/// <summary>
		/// De kost om iemand bij te verzekeren tegen loonverlies
		/// </summary>
		public decimal PrijsVerzekeringLoonVerlies { get; set; }
	}
}